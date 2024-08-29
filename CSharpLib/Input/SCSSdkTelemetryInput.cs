using System;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCSSdkClient.Input {
    public class SCSSdkTelemetryInput: IDisposable {
        private const string DefaultSharedMemoryMap = "Local\\ToraCon-SCSTelemetry-Input";

        private static readonly int MapSize = Marshal.SizeOf(typeof(InputData));

        /// <summary>
        ///     memory mapped file
        /// </summary>
        private MemoryMappedFile _memoryMappedHandle;

        /// <summary>
        ///     memory mapped view accessor
        /// </summary>
        private MemoryMappedViewAccessor _memoryMappedView;

        private InputData _inputData;

        public SCSSdkTelemetryInput() : this(DefaultSharedMemoryMap) { }
        public SCSSdkTelemetryInput(string map) {
            if (string.IsNullOrEmpty(map))
                throw new ArgumentNullException(nameof(map));
            Map = map;

        }

        public void Connect() {
            if (Hooked)
                Disconnect();

            //connect
            try {
                _memoryMappedHandle = MemoryMappedFile.CreateOrOpen(Map, MapSize, MemoryMappedFileAccess.ReadWrite);
                _memoryMappedView = _memoryMappedHandle.CreateViewAccessor(0, MapSize);
                _memoryMappedView.Read(0,out _inputData);
                Hooked = true;
            } catch {
                Hooked = false;
            }

        }

        public void Disconnect() {
            if (Hooked) {
                Hooked = false;

                _memoryMappedView.Dispose();
                _memoryMappedHandle.Dispose();
            }
        }

        public bool Hooked { get; private set; }

        public string Map { get; private set; }

        public void Dispose() {
            Disconnect();
        }

        public async Task SetParkingBrakeAsync(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.ParkingBrake = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            await Task.Delay(waitMilliseconds);

            // false に戻す
            _inputData.ParkingBrake = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        public async Task SetLeftBlinkerAsync(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Lblinker = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            await Task.Delay(waitMilliseconds);

            // false に戻す
            _inputData.Lblinker = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        public async Task SetRightBlinkerAsync(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Rblinker = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            await Task.Delay(waitMilliseconds);

            // false に戻す
            _inputData.Rblinker = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

    }
}