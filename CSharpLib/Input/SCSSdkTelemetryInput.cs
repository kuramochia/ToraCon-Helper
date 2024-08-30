using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace SCSSdkClient.Input {
    /// <summary>
    /// SCS Telemetry の入力機能を提供します
    /// </summary>
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

        /// <summary>
        /// constructor
        /// </summary>
        public SCSSdkTelemetryInput() : this(DefaultSharedMemoryMap) { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="map">Memory Mapped File name</param>
        /// <exception cref="ArgumentNullException">map name is null</exception>
        public SCSSdkTelemetryInput(string map) {
            if (string.IsNullOrEmpty(map))
                throw new ArgumentNullException(nameof(map));
            Map = map;

        }

        /// <summary>
        /// MMF に接続
        /// </summary>
        public void Connect() {
            if (Hooked)
                Disconnect();

            //connect
            try {
                _memoryMappedHandle = MemoryMappedFile.CreateOrOpen(Map, MapSize, MemoryMappedFileAccess.ReadWrite);
                _memoryMappedView = _memoryMappedHandle.CreateViewAccessor(0, MapSize);
                _memoryMappedView.Read(0, out _inputData);
                Hooked = true;
            } catch {
                Hooked = false;
            }

        }

        /// <summary>
        /// MMF から接続解除
        /// </summary>
        public void Disconnect() {
            if (Hooked) {
                Hooked = false;

                _memoryMappedView.Dispose();
                _memoryMappedHandle.Dispose();
            }
        }

        /// <summary>
        /// MMF に接続中かどうか
        /// </summary>
        public bool Hooked { get; private set; }

        /// <summary>
        /// MMF ファイル名
        /// </summary>
        public string Map { get; private set; }

        /// <summary>
        /// アンマネージメモリを解放します
        /// </summary>
        public void Dispose() {
            Disconnect();
        }

        #region Input functions

        /// <summary>
        /// パーキングブレーキ入力を実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetParkingBrake(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.ParkingBrake = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.ParkingBrake = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// 左ウィンカーレバー入力を実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetLeftBlinker(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Lblinker = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.Lblinker = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// 左ウィンカーレバー入力Offを実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetLeftBlinkerHide(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Lblinkerh = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.Lblinkerh = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// 右ウィンカーレバー入力を実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetRightBlinker(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Rblinker = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.Rblinker = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// 右ウィンカーレバー入力Offを実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetRightBlinkerHide(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Rblinkerh = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.Rblinkerh = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// リターダーUp入力を実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetRetarderUp(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.RetarderUp = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.RetarderUp = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// リターダーDown入力を実施
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetRetarderDown(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.RetarderDown = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.RetarderDown = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }

        /// <summary>
        /// リターダー段数を0にする
        /// </summary>
        /// <param name="waitMilliseconds">入力している時間</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetRetarder0(int waitMilliseconds = 100) {
            if (!Hooked)
                throw new InvalidOperationException("not Connected");

            _inputData.Retarder0 = true;

            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();

            // 待つ
            Thread.Sleep(waitMilliseconds);

            // false に戻す
            _inputData.Retarder0 = false;
            _memoryMappedView.Write(0, ref _inputData);
            _memoryMappedView.Flush();
        }
        #endregion
    }
}