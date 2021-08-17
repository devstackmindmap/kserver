using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Pages
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        Timer TheTimer = null;
        object LockObject = new object();
        double ProgressAmount = 0;

        public LoadingPage()
        {
            this.InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                TimerCallback tcb = HandleTimerTick;
                TheTimer = new Timer(HandleTimerTick, null, 0, 50);
            };

            this.Unloaded += (sender, e) =>
            {
                lock (LockObject)
                {
                    if (TheTimer != null)
                        TheTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    TheTimer = null;
                }
            };

        }

        public void HandleTimerTick(Object state)
        {
            lock (LockObject)
            {
                ProgressControl.SetBarLength(ProgressAmount);
                ProgressAmount += 0.06;
                if (ProgressAmount > 1.0)
                    ProgressAmount = 0.0;
            }
        }
    }
}
