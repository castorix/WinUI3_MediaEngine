using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_MediaEngine
{
    public sealed partial class CImageButton : UserControl
    {
        public CImageButton(bool bCheckLongPres = false, bool bPointLight = false)
        {
            this.InitializeComponent();

            m_bCheckLongPress = bCheckLongPres;
            m_bPointLight = bPointLight;
            if (m_bPointLight)
                CreateSpotLight(this);

            this.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(CImageButton_PointerPressed), true);
            this.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(CImageButton_PointerReleased), true);
            this.AddHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(CImageButton_PointerEntered), true);
            this.AddHandler(UIElement.PointerExitedEvent, new PointerEventHandler(CImageButton_PointerExited), true);
            this.AddHandler(UIElement.PointerCaptureLostEvent, new PointerEventHandler(CImageButton_CaptureLost), true);
            this.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(CImageButton_PointerMoved), true);
        }

        public CImageButton()
             : this(false, false)
        {
        }

        // Adapted from WinUI 3 Gallery : https://github.com/microsoft/WinUI-Gallery

        private SpringVector3NaturalMotionAnimation _springAnimation;
        private bool m_bCheckLongPress = false;
        private bool _bPressed = false;
        private bool _bInside = false;

        private Microsoft.UI.Composition.PointLight _spotLight = null;
        private bool m_bPointLight = false;

        private DispatcherTimer dTimer;
        private TimeSpan tsDuration;
        private DateTime tsEnd;

        public void StartTimer(int nMilliSeconds)
        {
            dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromMilliseconds(60);
            tsDuration = TimeSpan.FromMilliseconds(nMilliSeconds);
            tsEnd = DateTime.UtcNow + tsDuration;
            dTimer.Tick += Dt_Tick;
            dTimer.Start();
        }

        public void Dt_Tick(object sender, object e)
        {
            DateTime dtNow = DateTime.UtcNow;
            if (dtNow >= tsEnd)
            {
                if (dTimer != null)
                {                    
                    dTimer.Stop();
                    dTimer = null;
                    if (FunctionDelegate != null)
                        FunctionDelegate();
                }               
            }
        }

        public delegate void DELEGATEPROC();
        private DELEGATEPROC FunctionDelegate;

        public void SetFunctionLongPress(DELEGATEPROC fct)
        {
            FunctionDelegate = new DELEGATEPROC(fct);
        }

        private void CImageButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var img = (UserControl)sender;
            var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            if (properties.IsLeftButtonPressed)
            {
                _bPressed = true;
                bool bCapture = ((UserControl)sender).CapturePointer(e.Pointer);
                CreateOrUpdateSpringAnimation((UIElement)img.Parent, 3.0f);
                (sender as UIElement).StartAnimation(_springAnimation);

                if (m_bCheckLongPress)
                    StartTimer(5000);
            }
        }

        private void CImageButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {           
            var img = (UserControl)sender;
            _bPressed = false;
            CreateOrUpdateSpringAnimation((UIElement)img.Parent, 0.0f);
            (sender as UIElement).StartAnimation(_springAnimation);

            if (_bInside)
                CImageButton_Click(sender, e);

            if (m_bCheckLongPress)
            {
                if (dTimer != null)
                {
                    Console.Beep(6000, 10);
                    dTimer.Stop();
                    dTimer = null;
                }
            }
        }

        private void CImageButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var img = (UserControl)sender;
            _bInside = true;
            if (_bPressed)
            {
                CreateOrUpdateSpringAnimation((UIElement)img.Parent, 3.0f);
                (sender as UIElement).StartAnimation(_springAnimation);

                if (m_bCheckLongPress)
                    StartTimer(5000);
            }
            if (m_bPointLight)
            {
                _spotLight.Intensity = 1.5f;
                _spotLight.IsEnabled = true;
            }
        }

        private void CImageButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var img = (UserControl)sender;
            _bInside = false;
            if (_bPressed)
            {
                CreateOrUpdateSpringAnimation((UIElement)img.Parent, 0.0f);
                (sender as UIElement).StartAnimation(_springAnimation);

                if (m_bCheckLongPress)
                {
                    if (dTimer != null)
                    {
                        Console.Beep(6000, 10);
                        dTimer.Stop();
                        dTimer = null;
                    }
                }
            }
            if (m_bPointLight)
            {
                _spotLight.Offset = new System.Numerics.Vector3((float)this.ActualWidth / 2,
                    (float)this.ActualHeight / 2,
                    300);
                _spotLight.Intensity = 1.0f;
            }
        }

        private void CImageButton_CaptureLost(object sender, PointerRoutedEventArgs e)
        {
            var img = (UserControl)sender;
            if (_bPressed)
            {
                CreateOrUpdateSpringAnimation((UIElement)img.Parent, 0.0f);
                (sender as UIElement).StartAnimation(_springAnimation);
                _bPressed = false;
            }
            if (m_bPointLight && !_bInside)
            {
                _spotLight.Offset = new System.Numerics.Vector3((float)this.ActualWidth / 2,
                    (float)this.ActualHeight / 2,
                    300);
                _spotLight.Intensity = 1.0f;
            }
        }

        private void CImageButton_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //var properties = e.GetCurrentPoint((UIElement)sender).Properties;
            var pp = e.GetCurrentPoint((UIElement)sender);
            if (m_bPointLight && _bInside)
            {
                _spotLight.Offset = new System.Numerics.Vector3((float)pp.Position.X,
                (float)pp.Position.Y,
                100);
            }
        }

        private void CreateOrUpdateSpringAnimation(UIElement element, float finalValue)
        {
            if (_springAnimation == null)
            {
                Visual visual = ElementCompositionPreview.GetElementVisual(element);
                Compositor compositor = visual.Compositor;
                _springAnimation = compositor.CreateSpringVector3Animation();
                _springAnimation.Period = TimeSpan.FromMilliseconds(10);
                _springAnimation.Target = "Translation";
            }
            _springAnimation.FinalValue = new System.Numerics.Vector3(finalValue);
        }

        private void CreateSpotLight(UIElement element)
        {
            if (_spotLight == null)
            {
                Visual visual = ElementCompositionPreview.GetElementVisual(element);
                Compositor compositor = visual.Compositor;

                _spotLight = compositor.CreatePointLight();
                _spotLight.CoordinateSpace = visual;
                _spotLight.Color = Microsoft.UI.Colors.White;                
                _spotLight.Offset = new System.Numerics.Vector3((float)((FrameworkElement)element).ActualWidth / 2,
                  (float)((FrameworkElement)element).ActualHeight / 2,
                    300);
                _spotLight.Targets.Add(visual);
                _spotLight.Intensity = 1.0f;
                _spotLight.IsEnabled = true;
            }            
        }

        public event RoutedEventHandler CImageButtonClicked;

        private void CImageButton_Click(object sender, RoutedEventArgs e)
        {
            CImageButtonClicked?.Invoke(this, new RoutedEventArgs());
        }

        public void SetSource(string sPath, double nWidth = 0, double nHeight = 0)
        {           
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri(sPath);
            bitmapImage.DecodePixelWidth = (int)((nWidth != 0) ? nWidth : this.ActualWidth);
            bitmapImage.DecodePixelHeight = (int)((nHeight != 0) ? nHeight : this.ActualHeight);
            bitmapImage.UriSource = uri;
            this.Source = bitmapImage;
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(CImageButton), null);
    }
}
