using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ToraConHelper.Views;

/// <summary>
/// AboutPage.xaml の相互作用ロジック
/// </summary>
public partial class AboutPage : Page
{


    public AboutPage()
    {
        InitializeComponent();

        // アイコン表示
        using(Icon ico = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName))
        {
            appIcon.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        // バージョン表示
        FileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
    }

    public string FileVersion { get; }
}
