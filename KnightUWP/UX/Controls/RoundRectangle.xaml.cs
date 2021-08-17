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
using Windows.UI.Text;
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
    public sealed partial class RoundRectangle : UserControl
    {
        public double XRadius
        {
            get { return (double)GetValue(XRadiusProperty); }
            set { SetValue(XRadiusProperty, value); }
        }

        public double YRadius
        {
            get { return (double)GetValue(YRadiusProperty); }
            set { SetValue(YRadiusProperty, value); }
        }
        public Color BrushColor
        {
            get { return (Color)GetValue(BrushColorProperty); }
            set { SetValue(BrushColorProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value.ToString()); }
        }

        public RoundRectangle()
        {
            this.InitializeComponent();
        }


        public static readonly DependencyProperty XRadiusProperty =
            DependencyProperty.Register(
                "XRadius",
                typeof(double),
                typeof(RoundRectangle),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty YRadiusProperty =
            DependencyProperty.Register(
                "YRadius",
                typeof(double),
                typeof(RoundRectangle),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty BrushColorProperty =
            DependencyProperty.Register(
                "BrushColor",
                typeof(Color),
                typeof(RoundRectangle),
                new PropertyMetadata(Color.FromArgb(0,255,255,255)));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(RoundRectangle),
                new PropertyMetadata(""));

    }
}
