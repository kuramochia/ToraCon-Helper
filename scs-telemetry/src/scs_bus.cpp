#include "scs_bus.hpp"

#include <windows.h>
#include <cstring>
#include <limits>
#include <new>
#include <stdexcept>
#include <string>
#include <vector>

#include "scs-telemetry-common.hpp"

extern scsTelemetryMap_t* telem_ptr;

namespace {
HANDLE mutex_handle = nullptr;
HANDLE snapshot_handle = nullptr;
unsigned long long next_snapshot = 0;
const wchar_t* mutex_name = L"Local\\ToraCon-SCSTelemetry.Bus.Mutex";
// スナップショットごとに PID と連番を付け、MMF 名が衝突しないようにする。
constexpr wchar_t snapshot_name_prefix[] = L"Local\\ToraCon-SCSTelemetry.Bus.";

void append_bytes(std::vector<unsigned char>& output, const void* data, size_t size) {
  const auto* bytes = static_cast<const unsigned char*>(data);
  output.insert(output.end(), bytes, bytes + size);
}

void append_u32(std::vector<unsigned char>& output, unsigned int value) {
  append_bytes(output, &value, sizeof value);
}

void append_string(std::vector<unsigned char>& output, const char* value) {
  const auto length = strlen(value);
  if (length > (std::numeric_limits<unsigned int>::max)()) {
    throw std::length_error("bus string exceeds shared memory limit");
  }
  append_u32(output, static_cast<unsigned int>(length));
  append_bytes(output, value, length);
}

void set_error(unsigned int error) {
  telem_ptr->bus_extension.error = error;
  log_line(SCS_LOG_TYPE_error, "Bus telemetry shared memory error: %u", error);
}
}

bool initialize_bus_memory() {
  mutex_handle = CreateMutexW(nullptr, FALSE, mutex_name);
  if (!mutex_handle) {
    set_error(GetLastError());
    return false;
  }
  next_snapshot = GetTickCount64();
  return true;
}

void close_bus_memory() {
  if (snapshot_handle) CloseHandle(snapshot_handle);
  snapshot_handle = nullptr;
  if (mutex_handle) CloseHandle(mutex_handle);
  mutex_handle = nullptr;
}

bool lock_bus_memory() {
  if (!mutex_handle) return false;
  const auto result = WaitForSingleObject(mutex_handle, 1000);
  if (result == WAIT_OBJECT_0 || result == WAIT_ABANDONED) return true;
  set_error(result == WAIT_TIMEOUT ? ERROR_TIMEOUT : GetLastError());
  return false;
}

void unlock_bus_memory() {
  if (!ReleaseMutex(mutex_handle)) set_error(GetLastError());
}

void clear_bus_configuration() {
  telem_ptr->bus_extension.snapshotId = 0;
  telem_ptr->bus_extension.snapshotSize = 0;
  telem_ptr->bus_extension.processId = 0;
  telem_ptr->bus_extension.error = 0;
  if (snapshot_handle) CloseHandle(snapshot_handle);
  snapshot_handle = nullptr;
}

bool publish_bus_configuration(const scs_telemetry_configuration_t* info) {
  try {
    std::vector<unsigned char> data;
    unsigned int count = 0;
    append_u32(data, count);
    for (auto current = info->attributes; current->name; ++current) {
      const auto type = current->value.type;
      if (type != SCS_VALUE_TYPE_string && type != SCS_VALUE_TYPE_u32 &&
          type != SCS_VALUE_TYPE_u64) {
        log_line(SCS_LOG_TYPE_error, "Unsupported bus attribute %s (type %u)", current->name, type);
        clear_bus_configuration();
        set_error(ERROR_INVALID_DATA);
        return false;
      }
      append_string(data, current->name);
      append_u32(data, current->index);
      append_u32(data, type);
      if (type == SCS_VALUE_TYPE_string) {
        append_string(data, current->value.value_string.value);
      } else if (type == SCS_VALUE_TYPE_u32) {
        append_u32(data, current->value.value_u32.value);
      } else {
        append_bytes(data, &current->value.value_u64.value, sizeof current->value.value_u64.value);
      }
      ++count;
    }
    memcpy(data.data(), &count, sizeof count);
    if (data.size() > (std::numeric_limits<unsigned int>::max)()) {
      throw std::length_error("bus configuration exceeds shared memory limit");
    }
    const auto id = ++next_snapshot;
    const auto pid = GetCurrentProcessId();
    const auto name = std::wstring(snapshot_name_prefix) + std::to_wstring(pid) +
        L"." + std::to_wstring(id);
    const auto handle = CreateFileMappingW(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE,
                                           0, static_cast<DWORD>(data.size()), name.c_str());
    if (!handle) {
      const auto error = GetLastError();
      clear_bus_configuration();
      set_error(error);
      return false;
    }
    if (GetLastError() == ERROR_ALREADY_EXISTS) {
      CloseHandle(handle);
      clear_bus_configuration();
      set_error(ERROR_ALREADY_EXISTS);
      return false;
    }
    auto* buffer = MapViewOfFile(handle, FILE_MAP_WRITE, 0, 0, data.size());
    if (!buffer) {
      const auto error = GetLastError();
      CloseHandle(handle);
      clear_bus_configuration();
      set_error(error);
      return false;
    }
    memcpy(buffer, data.data(), data.size());
    UnmapViewOfFile(buffer);
    clear_bus_configuration();
    snapshot_handle = handle;
    telem_ptr->bus_extension.snapshotId = id;
    telem_ptr->bus_extension.snapshotSize = static_cast<unsigned int>(data.size());
    telem_ptr->bus_extension.processId = pid;
    telem_ptr->bus_extension.error = 0;
    return true;
  } catch (const std::bad_alloc&) {
    clear_bus_configuration();
    set_error(ERROR_NOT_ENOUGH_MEMORY);
  } catch (const std::length_error&) {
    clear_bus_configuration();
    set_error(ERROR_ARITHMETIC_OVERFLOW);
  }
  return false;
}
