using System.Runtime.InteropServices;

namespace SCSSdkClient.Input {
    /// <summary>
    /// 入力データ構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InputData {
        //public float Steering;
        //public float Aforward;
        //public float Abackward;
        //public float Clutch;
        //public bool Pause;

        /// <summary>
        /// パーキングブレーキ
        /// </summary>
        public bool ParkingBrake;
        //public bool Wipers;
        //public bool Cruiectrl;
        //public bool Cruiectrlinc;
        //public bool Cruiectrldec;
        //public bool Cruiectrlres;
        //public bool Light;
        //public bool Hblight;

        /// <summary>
        /// 左ウィンカーOn
        /// </summary>
        public bool Lblinker;

        /// <summary>
        /// 左ウィンカーOff
        /// </summary>
        public bool Lblinkerh;
        /// <summary>
        /// 右ウィンカーOn
        /// </summary>
        public bool Rblinker;

        /// <summary>
        /// 右ウィンカーOff
        /// </summary>
        public bool Rblinkerh;

        //public bool Quickpark;
        //public bool Drive;
        //public bool Reverse;
        //public bool Cycl_zoom;
        //public bool Tripreset;
        //public bool Wipersback;
        //public bool Wipers0;
        //public bool Wipers1;
        //public bool Wipers2;
        //public bool Wipers3;
        //public bool Wipers4;
        //public bool Horn;
        //public bool Airhorn;
        //public bool Lighthorn;
        //public bool Cam1;
        //public bool Cam2;
        //public bool Cam3;
        //public bool Cam4;
        //public bool Cam5;
        //public bool Cam6;
        //public bool Cam7;
        //public bool Cam8;
        //public bool Mapzoom_in;
        //public bool Mapzoom_out;
        //public bool Accmode;
        //public bool Showmirrors;
        //public bool Flasher4way;
        //public bool Activate;
        //public bool Assistact1;
        //public bool Assistact2;
        //public bool Assistact3;
        //public bool Assistact4;
        //public bool Assistact5;

        /// <summary>
        /// リターダー一段Up
        /// </summary>
        public bool RetarderUp;
        /// <summary>
        /// リターダー一段Down
        /// </summary>
        public bool RetarderDown;
        /// <summary>
        /// リターダー0
        /// </summary>
        public bool Retarder0;

        ///// <summary>
        ///// リターダー1
        ///// </summary>
        //public bool Retarder1;
        ///// <summary>
        ///// リターダー2
        ///// </summary>
        //public bool Retarder2;
        ///// <summary>
        ///// リターダー3
        ///// </summary>
        //public bool Retarder3;
        ///// <summary>
        ///// リターダー4
        ///// </summary>
        //public bool Retarder4;
        ///// <summary>
        ///// リターダー5
        ///// </summary>
        //public bool Retarder5;
    }
}
