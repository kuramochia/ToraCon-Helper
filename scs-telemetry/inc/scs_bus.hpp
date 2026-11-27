#ifndef SCS_BUS_HPP
#define SCS_BUS_HPP

#include "scssdk_telemetry.h"

bool initialize_bus_memory();
void close_bus_memory();
bool lock_bus_memory();
void unlock_bus_memory();
bool publish_bus_configuration(const scs_telemetry_configuration_t* info);
void clear_bus_configuration();

#endif
