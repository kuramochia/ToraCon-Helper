/**
 * @file scssdk_telemetry_job_common_channels.h
 *
 * @brief Job telemetry specific constants for channels.
 */
#ifndef SCSSDK_TELEMETRY_JOB_COMMON_CHANNELS_H
#define SCSSDK_TELEMETRY_JOB_COMMON_CHANNELS_H

#include "../scssdk.h"

SCSSDK_HEADER

/**
 * @brief The total damage of the cargo in range 0.0 to 1.0.
 *
 * Type: float
 */
#define SCS_TELEMETRY_JOB_CHANNEL_cargo_damage                   "job.cargo.damage"

/**
 * @brief Current average satisfaction of passengers during bus job.
 *
 * Defined during bus job. Not provided if no bus job is active.
 *
 * Type: float
 */
#define SCS_TELEMETRY_BUS_JOB_CHANNEL_average_satisfaction       "bus_job.average_satisfaction"

SCSSDK_FOOTER

#endif // SCSSDK_TELEMETRY_JOB_COMMON_CHANNELS_H

/* eof */
