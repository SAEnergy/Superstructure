using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Client.Base
{
        public class GlassWindow : Window
        {
            private bool _isWin7Basic;
            private bool _isDWM;

            public GlassWindow()
            {
                this.Loaded += new RoutedEventHandler(OnLoaded);
            }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                ProcessStyle();
            }

            private void ProcessStyle()
            {
                try
                {
                    _isDWM = NonClientRegionAPI.DwmIsCompositionEnabled();
                }
                catch
                {
                    _isDWM = false;
                }
                if (_isDWM)
                {
                    try
                    {
                        // Obtain the window handle for WPF application
                        IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
                        HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                        mainWindowSrc.AddHook(WndProc);

                        // Get System Dpi
                        System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                        float DesktopDpiX = desktop.DpiX;
                        float DesktopDpiY = desktop.DpiY;

                        // Set Margins
                        NonClientRegionAPI.MARGINS margins = new NonClientRegionAPI.MARGINS();

                        // Extend glass frame into client area
                        // Note that the default desktop Dpi is 96dpi. The  margins are
                        // adjusted for the system Dpi.
                        margins.cxLeftWidth = Convert.ToInt32(0);
                        margins.cxRightWidth = Convert.ToInt32(0);
                        margins.cyTopHeight = Convert.ToInt32(65535);
                        margins.cyBottomHeight = Convert.ToInt32(0);

                        int hr = NonClientRegionAPI.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                        if (hr < 0)
                        {
                            throw new ApplicationException("DwmExtendFrameIntoClientArea failed with error code " + hr);
                        }
                        _isWin7Basic = false;
                        mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
                        this.SetResourceReference(Window.BackgroundProperty, Brushes.Transparent);
                    }
                    catch
                    {
                        _isDWM = false;
                    }
                }
                if (!_isDWM)
                {
                    try
                    {
                        _isWin7Basic = System.Windows.Forms.VisualStyles.VisualStyleInformation.DisplayName.Contains("Aero");
                    }
                    catch
                    {
                        _isWin7Basic = false;
                    }

                    if (_isWin7Basic)
                    {

                        if (this.IsActive)
                            OnActivated(null);
                        else
                            OnDeactivated(null);
                    }
                    else {
                        this.SetResourceReference(Window.BackgroundProperty, SystemColors.ControlBrushKey);
                    }
                }
            }

            protected override void OnActivated(EventArgs e)
            {
                if (_isWin7Basic)
                {
                    this.SetResourceReference(Window.BackgroundProperty, SystemColors.GradientActiveCaptionBrushKey);
                }
                base.OnActivated(e);
            }

            protected override void OnDeactivated(EventArgs e)
            {
                if (_isWin7Basic)
                {
                    this.SetResourceReference(Window.BackgroundProperty, SystemColors.GradientInactiveCaptionBrushKey);
                }
                base.OnActivated(e);
            }

            private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (
                    msg == NonClientRegionAPI.WM_DWMCOMPOSITIONCHANGED ||
                    msg == NonClientRegionAPI.WM_THEMECHANGED
                    )
                {
                    ProcessStyle();
                }
                return IntPtr.Zero;
            }
        }

        class NonClientRegionAPI
        {
            public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
            public const int WM_THEMECHANGED = 0x031A;

            [DllImport("DwmApi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(
                IntPtr hwnd,
                ref MARGINS pMarInset);

            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern bool DwmIsCompositionEnabled();

            [StructLayout(LayoutKind.Sequential)]
            public struct MARGINS
            {
                public int cxLeftWidth;      // width of left border that retains its size
                public int cxRightWidth;     // width of right border that retains its size
                public int cyTopHeight;      // height of top border that retains its size
                public int cyBottomHeight;   // height of bottom border that retains its size
            }
        }
    }
