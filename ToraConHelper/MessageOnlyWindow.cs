using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToraConHelper.Helpers;
using static ToraConHelper.Helpers.RelaunchHelper;

namespace ToraConHelper;

internal class MessageOnlyWindow : Form
{

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hwndNewParent);
    
    public MessageOnlyWindow()
    {
        this.Load += MessageOnlyWindow_Load;
        this.Text = "MessageOnlyWindow";
    }

    private void MessageOnlyWindow_Load(object sender, EventArgs e)
    {
        const int HWND_MESSAGE = -1;
        SetParent(this.Handle, new IntPtr(HWND_MESSAGE));
    }

    protected override CreateParams CreateParams
    {
        get
        {
            const int WS_POPUP = unchecked((int)0x80000000);
            const int WS_EX_TOOLWINDOW = 0x80;

            CreateParams cp = base.CreateParams;
            cp.ExStyle = WS_EX_TOOLWINDOW;
            cp.Style = WS_POPUP;
            cp.Height = 0;
            cp.Width = 0;
            return cp;
        }
    }

    private const int ENDSESSION_CLOSEAPP = 0x1;
    private const int WM_QUERYENDSESSION = 0x11;
    private const int WM_ENDSESSION = 0x16;

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_QUERYENDSESSION:
                if ((m.LParam.ToInt32() & ENDSESSION_CLOSEAPP) != 0)
                {
                    RelaunchHelper.RegisterApplicationRestart(
                        null, 
                        RestartFlags.RESTART_NO_CRASH | RestartFlags.RESTART_NO_HANG | RestartFlags.RESTART_NO_REBOOT);
                }
                m.Result = new IntPtr(1);
                break;
            case WM_ENDSESSION:
                if ((m.LParam.ToInt32() & ENDSESSION_CLOSEAPP) != 0)
                {
                    App.Current.Shutdown();
                }
                m.Result = IntPtr.Zero;
                break;
        }
        base.WndProc(ref m);
    }
}
