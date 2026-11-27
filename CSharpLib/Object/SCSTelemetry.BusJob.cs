using System.Collections.Generic;

namespace SCSSdkClient.Object {
    public partial class SCSTelemetry {
        public class BusJob {
            public uint? CurrentStageIndex { get; internal set; }
            public ulong? Income { get; internal set; }
            public uint? PlannedDistanceKm { get; internal set; }
            public string MarketName { get; internal set; }
            public JobMarket Market { get; internal set; }
            public float? AverageSatisfaction { get; internal set; }
            public IReadOnlyList<Stage> Stages { get; internal set; } = new List<Stage>();
            /// <summary>Non-null when the bus snapshot could not be read.</summary>
            public string Error { get; internal set; }

            internal ulong SnapshotId { get; set; }
            internal uint SnapshotSize { get; set; }
            internal uint ProcessId { get; set; }

            public class Stage {
                public string SourceType { get; internal set; }
                public string SourceId { get; internal set; }
                public string SourceName { get; internal set; }
                public string DestinationType { get; internal set; }
                public string DestinationId { get; internal set; }
                public string DestinationName { get; internal set; }
                /// <summary>Absolute in-game minutes; null when not provided by the SDK.</summary>
                public uint? PlannedDepartureTime { get; internal set; }
                public uint? ActualDepartureTime { get; internal set; }
                public uint? PlannedArrivalTime { get; internal set; }
                public uint? ActualArrivalTime { get; internal set; }
                public uint? PlannedEnteringPassengers { get; internal set; }
                public uint? PlannedLeavingPassengers { get; internal set; }
            }
        }
    }
}
