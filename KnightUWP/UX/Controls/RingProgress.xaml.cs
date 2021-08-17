using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 사용자 정의 컨트롤 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234236에 나와 있습니다.

namespace KnightUWP.UX.Controls
{
    public sealed partial class RingProgress : UserControl
    {
        SpriteVisual _shadowVisual;
        DropShadow _dropShadow;
        double ProgressAmount = 0;

        System.Threading.Timer TheTimer = null;

        public RingProgress()
        {
            this.Visibility = Visibility.Collapsed;
            this.InitializeComponent();
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            TheTimer = new System.Threading.Timer(HandleTimerTick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _shadowVisual = compositor.CreateSpriteVisual();
            _dropShadow = compositor.CreateDropShadow();
            _shadowVisual.Shadow = _dropShadow;
            _dropShadow.Color = Colors.Black;

            this.Unloaded += delegate
            {
                TheTimer.Dispose();
                TheTimer = null;
            };

            RegisterPropertyChangedCallback(UIElement.VisibilityProperty, delegate(DependencyObject sender, DependencyProperty dp) {
                if ( ((UIElement)sender).Visibility == Visibility.Visible)
                    TheTimer.Change(0, 50);
                else 
                    TheTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            });
        }


        public void HandleTimerTick(Object state)
        {
            SetBarLength(ProgressAmount);
            ProgressAmount += 0.06;
            if (ProgressAmount > 1.0)
            {
                ProgressAmount = 0.0;
            }
        }

        public void SizeChangeUpdate()
        {
            if (_shadowVisual != null)
            {
                _dropShadow.BlurRadius = 10f;
                _dropShadow.Opacity = 0.02f;
                _dropShadow.Mask = null;

                ElementCompositionPreview.SetElementChildVisual(this, _shadowVisual);

                Vector2 newSize = new Vector2(0, 0);
                if (Content is FrameworkElement contentFE)
                {
                    newSize = new Vector2((float)contentFE.ActualWidth, (float)contentFE.ActualHeight);
                }
                _shadowVisual.Size = newSize;
            }
        }

        public void SetBarLength(double Value)
        {
            double Angle = 2 * 3.14159265 * Value;

            double X = 24 - Math.Sin(Angle) * 24;
            double Y = 24 + Math.Cos(Angle) * 24;

            if (Value > 0 && (int)X == 24 && (int)Y == 48)
                X += 0.55; // Never make the end the same as the start!

        //    DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            IAsyncAction TheTask = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                () =>
                {
                    arcSegment.IsLargeArc = Angle >= 3.14159265;
                    arcSegment.Point = new Point(X, Y);

                });
        }

        private void gridParent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangeUpdate();

            /*
             * 
    <Grid Margin="0,0,0,0" Padding="0,0,0,0">

        <Rectangle Margin="1,1,1,1" Width="{x:Bind Width}" Height="{x:Bind Height}"
                   Stroke="Black" StrokeThickness="2"
                   RadiusX="{x:Bind XRadius}" RadiusY="{x:Bind YRadius}" 
                   HorizontalAlignment="Center" VerticalAlignment="Center" >
            <Rectangle.Fill >
                <SolidColorBrush Color="{x:Bind BrushColor}" />
            </Rectangle.Fill>
        </Rectangle>
        <TextBlock VerticalAlignment="Center" HorizontalAlignment="Center" Text="{x:Bind Title}" />
    </Grid>
    */
        }
    }
}
