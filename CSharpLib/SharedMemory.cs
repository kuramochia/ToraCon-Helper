using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using SCSSdkClient.Object;

namespace SCSSdkClient {

    /// <summary>
    ///     Manage the shared memory
    /// </summary>
    public class SharedMemory {

        /// <summary>
        ///     size of the shared memory in bytes
        /// </summary>
        private const uint defaultMapSize = 32 * 1024;

        /// <summary>
        ///     holds the byte to object convert class
        /// </summary>
        private readonly SCSSdkConvert _sdkconvert = new SCSSdkConvert();

        /// <summary>
        ///     memory mapped file
        /// </summary>
        private MemoryMappedFile _memoryMappedHandle;

        /// <summary>
        ///     memory mapped view accessor
        /// </summary>
        private MemoryMappedViewAccessor _memoryMappedView;
        private string _mapName;

        /// <summary>
        ///     Could we create a memory view on the memory map
        /// </summary>
        public bool Hooked { get; private set; }

        /// <summary>
        ///     if we can't create a memory view hold the exception
        /// </summary>
        public Exception HookException { get; private set; }

        /// <summary>
        ///     raw byte data of the memory map
        /// </summary>
        public byte[] RawData { get; private set; }

        /// <summary>
        ///     create/connect to a shared memory file
        /// </summary>
        /// <param name="map">Map location string</param>
        public void Connect(string map) => Connect(map, defaultMapSize);

        /// <summary>
        ///     create/connect to a shared memory file
        /// </summary>
        /// <param name="map">map location string</param>
        /// <param name="mapSize">size of the map</param>
        public void Connect(string map, uint mapSize) {
            if (Hooked) {
                Disconnect();
            }

            // Reset any errors
            HookException = null;
            _mapName = map;

            try {
                RawData = new byte[mapSize];

                // Open the map and create a "memory view" at the begin (byte 0)
                _memoryMappedHandle = MemoryMappedFile.CreateOrOpen(map, mapSize, MemoryMappedFileAccess.ReadWrite);
                _memoryMappedView = _memoryMappedHandle.CreateViewAccessor(0, mapSize);

                // Mark as a success.
                Hooked = true;
            } catch (Exception e) {
                // We were unable to hook onto the map.
                Hooked = false;
                HookException = e;
            }
        }

        /// <summary>
        ///     close the memory view and handle
        /// </summary>
        public void Disconnect() {
            Hooked = false;

            _memoryMappedView.Dispose();
            _memoryMappedHandle.Dispose();
        }

        /// <summary>
        ///     read data from memory and update object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SCSTelemetry Update<T>() {
            Update();
            if (RawData == null) return null;
            if (RawData.Length < 44 || BitConverter.ToUInt32(RawData, 40) < 14 || RawData[0] == 0)
                return ToObject<T>(RawData);

            SCSTelemetry result = null;
            try {
                using (var mutex = Mutex.OpenExisting(_mapName + ".Bus.Mutex")) {
                    var acquired = false;
                    try {
                        try {
                            acquired = mutex.WaitOne(TimeSpan.FromSeconds(1));
                        } catch (AbandonedMutexException) {
                            acquired = true;
                        }
                        if (!acquired) throw new TimeoutException("Timed out reading bus telemetry.");
                        Update();
                        result = ToObject<T>(RawData);
                        if (result != null && result.SpecialEventsValues.OnBusJob &&
                            result.BusJobValues.Error == null) {
                            var bus = result.BusJobValues;
                            if (bus.SnapshotId == 0 || bus.SnapshotSize == 0)
                                throw new InvalidDataException("Bus telemetry snapshot is unavailable.");
                            var name = _mapName + ".Bus." + bus.ProcessId + "." + bus.SnapshotId;
                            using (var map = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.Read))
                            using (var stream = map.CreateViewStream(0, bus.SnapshotSize, MemoryMappedFileAccess.Read)) {
                                BusSnapshotReader.Read(stream, bus);
                            }
                        }
                        return result;
                    } finally {
                        if (acquired) mutex.ReleaseMutex();
                    }
                }
            } catch (Exception e) when (e is IOException || e is InvalidDataException || e is UnauthorizedAccessException ||
                                        e is WaitHandleCannotBeOpenedException || e is TimeoutException ||
                                        e is ArgumentException) {
                if (result == null) result = ToObject<T>(RawData);
                if (result != null) result.BusJobValues.Error = e.Message;
                return result;
            }
        }

        /// <summary>
        ///     reread data from memory view
        /// </summary>
        public void Update() {
            if (!Hooked || _memoryMappedView == null) {
                return;
            }

            // Re-read data from the view.
            _memoryMappedView.ReadArray(0, RawData, 0, RawData.Length);
        }

        /// <summary>
        ///     Cast a set of bytes to a managed C# object.
        /// </summary>
        /// <typeparam name="T">Managed C# object type</typeparam>
        /// <param name="structureDataBytes">Bytes array</param>
        /// <returns>Managed object from given bytes</returns>
        protected SCSTelemetry ToObject<T>(byte[] structureDataBytes) => _sdkconvert.Convert(structureDataBytes);
    }
}
