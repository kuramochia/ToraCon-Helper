using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SCSSdkClient.Object;

namespace SCSSdkClient {
    internal static class BusSnapshotReader {
        private sealed class Attribute {
            public string Name;
            public uint Index;
            public uint Type;
            public string StringValue;
            public uint UintValue;
            public ulong UlongValue;
        }

        private static string ReadString(BinaryReader reader) {
            var length = reader.ReadUInt32();
            if (length > int.MaxValue || length > reader.BaseStream.Length - reader.BaseStream.Position)
                throw new InvalidDataException("Invalid bus snapshot string length.");
            return new UTF8Encoding(false, true).GetString(reader.ReadBytes((int)length));
        }

        internal static void Read(Stream stream, SCSTelemetry.BusJob bus) {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true)) {
                var count = reader.ReadUInt32();
                if (count > (stream.Length - stream.Position) / 16)
                    throw new InvalidDataException("Invalid bus snapshot attribute count.");
                var attributes = new List<Attribute>((int)count);
                for (uint i = 0; i < count; ++i) {
                    var item = new Attribute {
                        Name = ReadString(reader),
                        Index = reader.ReadUInt32(),
                        Type = reader.ReadUInt32()
                    };
                    switch (item.Type) {
                        case 3: item.UintValue = reader.ReadUInt32(); break;
                        case 4: item.UlongValue = reader.ReadUInt64(); break;
                        case 12: item.StringValue = ReadString(reader); break;
                        default: throw new InvalidDataException("Unsupported bus snapshot value type.");
                    }
                    attributes.Add(item);
                }
                if (stream.Position != stream.Length)
                    throw new InvalidDataException("Unexpected trailing bus snapshot data.");

                uint? stageCount = null;
                foreach (var item in attributes) {
                    if (item.Name == "bus.stages.count") {
                        Require(item, 3, uint.MaxValue);
                        stageCount = item.UintValue;
                    }
                }
                if (!stageCount.HasValue || stageCount.Value > attributes.Count)
                    throw new InvalidDataException("Missing or invalid bus stage count.");
                var stages = new List<SCSTelemetry.BusJob.Stage>((int)stageCount.Value);
                for (uint i = 0; i < stageCount.Value; ++i) stages.Add(new SCSTelemetry.BusJob.Stage());
                foreach (var item in attributes) {
                    switch (item.Name) {
                        case "bus.stages.count": break;
                        case "bus.stages.current":
                            Require(item, 3, uint.MaxValue); bus.CurrentStageIndex = item.UintValue; break;
                        case "bus_job.market":
                            Require(item, 12, uint.MaxValue); bus.MarketName = item.StringValue;
                            bus.Market = item.StringValue.ToEnum<JobMarket>(); break;
                        case "income":
                            Require(item, 4, uint.MaxValue); bus.Income = item.UlongValue; break;
                        case "planned_distance.km":
                            Require(item, 3, uint.MaxValue); bus.PlannedDistanceKm = item.UintValue; break;
                        default:
                            if (!item.Name.StartsWith("source_stop.", StringComparison.Ordinal) &&
                                !item.Name.StartsWith("destination_stop.", StringComparison.Ordinal)) break;
                            if (item.Index >= stages.Count)
                                throw new InvalidDataException("Invalid bus stage index.");
                            var stage = stages[(int)item.Index];
                            switch (item.Name) {
                                case "source_stop.type": Require(item, 12, item.Index); stage.SourceType = item.StringValue; break;
                                case "source_stop.id": Require(item, 12, item.Index); stage.SourceId = item.StringValue; break;
                                case "source_stop.name": Require(item, 12, item.Index); stage.SourceName = item.StringValue; break;
                                case "destination_stop.type": Require(item, 12, item.Index); stage.DestinationType = item.StringValue; break;
                                case "destination_stop.id": Require(item, 12, item.Index); stage.DestinationId = item.StringValue; break;
                                case "destination_stop.name": Require(item, 12, item.Index); stage.DestinationName = item.StringValue; break;
                                case "source_stop.departure_time.planned": Require(item, 3, item.Index); stage.PlannedDepartureTime = item.UintValue; break;
                                case "source_stop.departure_time.actual": Require(item, 3, item.Index); stage.ActualDepartureTime = item.UintValue; break;
                                case "destination_stop.arrival_time.planned": Require(item, 3, item.Index); stage.PlannedArrivalTime = item.UintValue; break;
                                case "destination_stop.arrival_time.actual": Require(item, 3, item.Index); stage.ActualArrivalTime = item.UintValue; break;
                                case "source_stop.entering_passengers.planned": Require(item, 3, item.Index); stage.PlannedEnteringPassengers = item.UintValue; break;
                                case "destination_stop.leaving_passengers.planned": Require(item, 3, item.Index); stage.PlannedLeavingPassengers = item.UintValue; break;
                            }
                            break;
                    }
                }
                if (bus.CurrentStageIndex.HasValue && bus.CurrentStageIndex.Value >= stages.Count)
                    throw new InvalidDataException("Invalid current bus stage.");
                bus.Stages = stages;
            }
        }

        private static void Require(Attribute item, uint type, uint index) {
            if (item.Type != type || item.Index != index)
                throw new InvalidDataException("Invalid bus attribute: " + item.Name);
        }
    }
}
