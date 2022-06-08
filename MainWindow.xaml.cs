using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using GlobalStructures;
using DXGI;
using System.Threading.Tasks;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_MediaEngine
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IntPtr hWnd = IntPtr.Zero;
        private Microsoft.UI.Windowing.AppWindow _apw;
        private Microsoft.UI.Windowing.OverlappedPresenter _presenter;

        CMediaEngine mediaEngine;

        private System.Collections.ObjectModel.ObservableCollection<string> listURLs = new System.Collections.ObjectModel.ObservableCollection<string>();
        private string sURLListFile = "URL.xaml";
        private string sSubtitlesURL = string.Empty;

        // private string sVideoURL = "E:\\big_buck_bunny.asf";

        // Streams Radios FR
        // "https://streaming.radio.funradio.fr/fun-1-44-128"
        // "http://streamingp.shoutcast.com/NostalgiePremium-mp3"
        // "http://cdn.nrjaudio.fm/audio1/fr/30001/mp3_128.mp3?origine=fluxradios"
        // "http://streaming.radio.rtl.fr/rtl-1-44-128?listen=webCwsBCggNCQgLDQUGBAcGBg"

        //// http://bbb3d.renderfarming.net/download.html
        // "http://distribution.bbb3d.renderfarming.net/video/mp4/bbb_sunflower_1080p_60fps_normal.mp4"

        private string sVideoURL = "http://amssamples.streaming.mediaservices.windows.net/49b57c87-f5f3-48b3-ba22-c55cfdffa9cb/Sintel.ism/manifest(format=m3u8-aapl)";

        // 0xC00D36C4 MF_E_UNSUPPORTED_BYTESTREAM_TYPE
        // 0x8001011F
        // "https://www.youtube.com/watch?v=aqz-KE-bpKQ"

        // "https://www.youtube.com/watch?v=36YnV9STBqc"
        // "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov"

        // "https://mediaplatstorage1.blob.core.windows.net/windows-universal-samples-media/multiangle-right.mp4"
        // "https://mediaplatstorage1.blob.core.windows.net/windows-universal-samples-media/elephantsdream-clip-h264_sd-aac_eng-aac_spa-aac_eng_commentary-srt_eng-srt_por-srt_swe.mkv"

        // 0xC00D36C4 MF_E_UNSUPPORTED_BYTESTREAM_TYPE
        // http://47.51.131.147/-wvhttp-01-/GetOneShot?image_size=1280x720&frame_count=1000000000
        // http://camera.butovo.com/mjpg/video.mjpg
        // http://77.222.181.11:8080/mjpg/video.mjpg


        public MainWindow()
        {
            this.InitializeComponent();

            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            _apw.Closing += _apw_Closing;
            _presenter = _apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            //_presenter.IsResizable = false;
            //_presenter.SetBorderAndTitleBar(false, false);
            Application.Current.Resources["ButtonBackgroundPressed"] = new SolidColorBrush(Microsoft.UI.Colors.RoyalBlue);
            Application.Current.Resources["ToggleSwitchFillOn"] = new SolidColorBrush(Microsoft.UI.Colors.LightBlue);
            Application.Current.Resources["ToggleSwitchFillOnPointerOver"] = new SolidColorBrush(Microsoft.UI.Colors.LightBlue);
            Application.Current.Resources["ToggleSwitchFillOnPressed"] = new SolidColorBrush(Microsoft.UI.Colors.LightBlue);           

            HRESULT hr = HRESULT.S_OK;
            mediaEngine = new CMediaEngine();
           // hr = mediaEngine.Initialize(hWnd, CMediaEngine.ME_MODE.MODE_AUDIO, ctrlVideo, Microsoft.UI.Colors.Black);            
            hr = mediaEngine.Initialize(hWnd, CMediaEngine.ME_MODE.MODE_FRAME_SERVER, ctrlVideo, Microsoft.UI.Colors.Black);

            //FrameworkElement fe = (FrameworkElement)this.Content;
            //fe.RequestedTheme = ElementTheme.Default;
            //Microsoft.UI.Xaml.Application.Current.RequestedTheme = ApplicationTheme.Light;
        }

        private void _apw_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            var popups = VisualTreeHelper.GetOpenPopupsForXamlRoot(this.Content.XamlRoot);
            foreach (var popup in popups)
            {
                if (popup.Name == "SuggestionsPopup")
                {
                    popup.IsOpen = false;
                }
            }
            if (mediaEngine != null)
                mediaEngine.Dispose();
        }

        bool bOpen = false;
        private async void btnLoadURL_Click(object sender, RoutedEventArgs e)
        {
            if (!bOpen)
            {               
                await LoadURLListAsync();               
                OpenURL();              
            }
        }

        private async void OpenURL()
        {
            bOpen = true;
            mediaEngine.SetOpacity(10);

            StackPanel sp = new StackPanel();
            AutoSuggestBox asb = new AutoSuggestBox()
            {
                Width = 900,
                MaxSuggestionListHeight = 300,
                ItemsSource = listURLs,
            };
            ScrollViewer sv = new ScrollViewer()
            {
                Width = 400,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };            
            sv.Content = asb;
            asb.Loaded += Asb_Loaded;
            asb.TextChanged += Asb_TextChanged;
            sp.Children.Add(sv);
            ContentDialog cd = new ContentDialog()
            {
                Title = "Network URL",
                Content = sp,
                PrimaryButtonText = "Load",
                CloseButtonText = "Cancel"
            };
            cd.XamlRoot = this.Content.XamlRoot;
            var cdResult = await cd.ShowAsync();
            mediaEngine.SetOpacity(100);
            bOpen = false;
            if (cdResult == ContentDialogResult.Primary)
            {
                if (!listURLs.Contains(asb.Text))
                {
                    listURLs.Add(asb.Text);
                    await SaveURLListAsync();
                }
                HRESULT hr = HRESULT.S_OK;
                hr = mediaEngine.LoadURL(asb.Text);               
            }
        }

        // Copied from WinUI 3 Gallery
        private void Asb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var url in listURLs)
                {
                    var found = splitText.All((key) =>
                    {
                        return url.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(url);
                    }
                }
                if (suitableItems.Count == 0)
                {
                    //suitableItems.Add("No results found");
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void Asb_Loaded(object sender, RoutedEventArgs e)
        {
            ((AutoSuggestBox)sender).IsSuggestionListOpen = true;
        }

        //https://docs.microsoft.com/en-us/windows/win32/medfound/supported-media-formats-in-media-foundation

        private async Task<string> OpenMediaDialog()
        {
            var fop = new Windows.Storage.Pickers.FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(fop, hWnd);
            fop.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;

            fop.FileTypeFilter.Add(".3gp");
            fop.FileTypeFilter.Add(".3g2");
            fop.FileTypeFilter.Add(".asf");
            fop.FileTypeFilter.Add(".avi");
            fop.FileTypeFilter.Add(".m4v");
            fop.FileTypeFilter.Add(".mkv");
            fop.FileTypeFilter.Add(".mov"); // 0xC00D5212 MF_MEDIA_ENGINE_ERR_SRC_NOT_SUPPORTED 
            fop.FileTypeFilter.Add(".mp4");
            fop.FileTypeFilter.Add(".wmv");

            //fop.FileTypeFilter.Add(".mts"); // 0xC004F011 SL_E_LICENSE_FILE_NOT_INSTALLED            
            //fop.FileTypeFilter.Add(".flv"); // 0xC00D36C4 MF_E_UNSUPPORTED_BYTESTREAM_TYPE
            //fop.FileTypeFilter.Add(".vob"); // 0xC004F011 SL_E_LICENSE_FILE_NOT_INSTALLED
            //fop.FileTypeFilter.Add(".mpeg"); // 0xC004F011 SL_E_LICENSE_FILE_NOT_INSTALLED
            //fop.FileTypeFilter.Add(".mpg"); // 0xC004F011 SL_E_LICENSE_FILE_NOT_INSTALLED

            fop.FileTypeFilter.Add(".aac");
            fop.FileTypeFilter.Add(".flac");
            fop.FileTypeFilter.Add(".m4a");
            fop.FileTypeFilter.Add(".mp3");
            fop.FileTypeFilter.Add(".ogg");
            fop.FileTypeFilter.Add(".wav");
            fop.FileTypeFilter.Add(".wma");

            var file = await fop.PickSingleFileAsync();
            return (file != null ? file.Path : string.Empty);
        }

        private async void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            if (!bOpen)
            {
                bOpen = true;
                string sVideoFile = await OpenMediaDialog();
                if (sVideoFile != string.Empty)
                {
                    hr = mediaEngine.LoadURL(sVideoFile);
                }
                bOpen = false;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = mediaEngine.Stop(true);
        }

        private void btnOverlay_Click(object sender, RoutedEventArgs e)
        {
            //if (mediaEngine.HasVideo())
            {
                string sDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string sImageFile = sDirectory + @"Assets\Butterfly.png";
                if (!mediaEngine.GetImageOverlay())
                {
                    mediaEngine.SetImageOverlay(true, sImageFile);
                    btnOverlay.Content = "Remove Overlay";
                }
                else
                {
                    mediaEngine.SetImageOverlay(false, null);
                    btnOverlay.Content = "Test Overlay";
                }
            }
            //else
            //{
            //    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("The media must have Video", "Information");
            //    WinRT.Interop.InitializeWithWindow.Initialize(md, hWnd);
            //    _ = await md.ShowAsync();
            //}
        }

        private async Task LoadURLListAsync()
        {
            string sDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = sDirectory + sURLListFile;

            Windows.Storage.StorageFile file = null;
            bool bFileExists = true;
            try
            {
                //Windows.Storage.StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(sFile);
                file = await Windows.Storage.StorageFile.GetFileFromPathAsync(sFile);
            }
            catch (System.IO.FileNotFoundException ex1)
            {
                bFileExists = false;
            }
            catch (Exception ex2)
            {
                //...
            }
            if (bFileExists)
            {
                using (Windows.Storage.Streams.IInputStream inputStream = await file.OpenSequentialReadAsync())
                {
                    System.Runtime.Serialization.DataContractSerializer serializer =
                            new System.Runtime.Serialization.DataContractSerializer(typeof(System.Collections.ObjectModel.ObservableCollection<string>));
                    listURLs = (System.Collections.ObjectModel.ObservableCollection<string>)serializer.ReadObject(inputStream.AsStreamForRead());
                }
            }
            else
            {
                listURLs.Add(sVideoURL);
                await SaveURLListAsync();
            }
        }

        private async Task SaveURLListAsync()
        {
            string sDirectory = AppDomain.CurrentDomain.BaseDirectory;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.DataContractSerializer serializer = new
                        System.Runtime.Serialization.DataContractSerializer(typeof(System.Collections.ObjectModel.ObservableCollection<string>));
            serializer.WriteObject(ms, listURLs);

            Windows.Storage.StorageFolder folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(sDirectory);
            var destFile = await folder.CreateFileAsync(sURLListFile, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            using (System.IO.Stream fileStream = await destFile.OpenStreamForWriteAsync())
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                await ms.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
        }

        private void tsMirror_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            if (mediaEngine != null)
                mediaEngine.EnableHorizontalMirrorMode(ts.IsOn);
        }

        private void tsSubtitles_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            btnSubtitles.Visibility = ts.IsOn ? Visibility.Visible : Visibility.Collapsed;
            if (mediaEngine != null)
                mediaEngine.SetSubtitles(ts.IsOn ? true : false, ts.IsOn ? sSubtitlesURL:"");
        }

        private string sSubtitlesURLOld = string.Empty;       
        private void btnSubtitles_Click(object sender, RoutedEventArgs e)
        {
            sSubtitlesURLOld = sSubtitlesURL;
            SubTitlesDialog();
        }

        private async void SubTitlesDialog()
        {
            StackPanel sp = new StackPanel();
            Button btnAddSubtitle = new Button()
            {
                Content = "Add subtitles file",
                Width = 140,
                Height = 40,
                Margin = new Thickness(0, 10, 0, 0)
            };
            ScrollViewer sv = new ScrollViewer()
            {
                Width = 400,
                Height = 30,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            TextBlock tb = new TextBlock()
            {
                IsTextSelectionEnabled = true
            };
            tb.Text = sSubtitlesURL;
            sv.Content = tb;

            btnAddSubtitle.Click += async (sender, e) =>
            {
                string sSubtitleFile = await OpenSubtitleDialog();
                if (sSubtitleFile != string.Empty)
                {
                    tb.Text = sSubtitleFile;
                }
            };

            sp.Children.Add(btnAddSubtitle);
            sp.Children.Add(sv);
            ContentDialog cd = new ContentDialog()
            {
                Title = "Subtitles",
                Content = sp,
                PrimaryButtonText = "Ok",
                CloseButtonText = "Cancel"
            };
            cd.XamlRoot = this.Content.XamlRoot;
            var cdResult = await cd.ShowAsync();
            if (cdResult == ContentDialogResult.Primary)
            {
                if (tb.Text != string.Empty)
                {
                    sSubtitlesURL = tb.Text;
                    mediaEngine.SetSubtitles(true, sSubtitlesURL);
                    btnSubtitles.Background = new SolidColorBrush(Microsoft.UI.Colors.ForestGreen);

                    if (mediaEngine.IsPlaying() && mediaEngine.HasVideo())
                    {
                        Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("You must reload the media to apply new subtitles file", "Information");
                        WinRT.Interop.InitializeWithWindow.Initialize(md, hWnd);
                        _ = await md.ShowAsync();
                    }
                }
                else
                {
                    btnSubtitles.Background = new SolidColorBrush(Microsoft.UI.Colors.IndianRed);
                }
            }
            else
            {
                sSubtitlesURL = sSubtitlesURLOld;
            }

        }

        //private async void SubTitlesDialog()
        //{
        //    StackPanel sp = new StackPanel();           
        //    ListBox lbSubtitles = new ListBox()
        //    {
        //        Name = "lbSubtitles",
        //        Width = 500,
        //        Height = 400,      
        //        HorizontalContentAlignment = HorizontalAlignment.Stretch,
        //        ItemsSource = listSubtitles                
        //    };
        //    ScrollViewer.SetHorizontalScrollMode(lbSubtitles, ScrollMode.Enabled);
        //    ScrollViewer.SetVerticalScrollMode(lbSubtitles, ScrollMode.Enabled);
        //    ScrollViewer.SetHorizontalScrollBarVisibility(lbSubtitles, ScrollBarVisibility.Auto);
        //    ScrollViewer.SetVerticalScrollBarVisibility(lbSubtitles, ScrollBarVisibility.Auto);
        //    Button btnAddSubtitle = new Button()
        //    {
        //        Content = "Add subtitle file",
        //        Width = 140,
        //        Height = 40,
        //        Margin = new Thickness(0, 10, 0, 0)
        //    };

        //    StackPanel sp2 = new StackPanel();
        //    sp2.Orientation = Orientation.Horizontal;
        //    btnAddSubtitle.Click += async (sender, e) =>
        //    {
        //        string sSubtitleFile = await OpenSubtitleDialog();
        //        if (sSubtitleFile != string.Empty)
        //        {
        //            if (!listSubtitles.Contains(sSubtitleFile))
        //            {
        //                listSubtitles.Add(sSubtitleFile);                     
        //            }
        //            else
        //            {
        //                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Subtitle file already in the list !", "Information");
        //                WinRT.Interop.InitializeWithWindow.Initialize(md, hWnd);
        //                _ = await md.ShowAsync();
        //            }
        //        }
        //    };

        //    Button btnRemoveSubtitle = new Button()
        //    {
        //        Content = "Remove subtitle",
        //        Width = 140,
        //        Height = 40,
        //        Margin = new Thickness(30, 10, 0, 0)
        //    };

        //    lbSubtitles.SelectionChanged += (sender, e) =>
        //    {
        //        if (lbSubtitles.SelectedItems.Count > 0)
        //            btnRemoveSubtitle.IsEnabled = true;
        //        else
        //            btnRemoveSubtitle.IsEnabled = false;
        //    };

        //    lbSubtitles.Loaded += (sender, e) =>
        //    {
        //        if (lbSubtitles.SelectedItems.Count > 0)
        //            btnRemoveSubtitle.IsEnabled = true;
        //        else
        //            btnRemoveSubtitle.IsEnabled = false;
        //    };

        //    btnRemoveSubtitle.Click += (sender, e) =>
        //    {
        //        if (lbSubtitles.SelectedItem != null)
        //        {
        //            listSubtitles.Remove(lbSubtitles.SelectedItem.ToString());
        //        }                
        //    };

        //    // IsEnabled = "{Binding ElementName=lbSubtitles, Path=SelectedItems.Count}"
        //    //Binding bind = new Binding();
        //    //bind.Mode = BindingMode.OneWay;
        //    //bind.ElementName = "lbSubtitles";
        //    ////bind.Source = lbSubtitles;
        //    //bind.Path = new PropertyPath("SelectedItems.Count");
        //    //bind.Converter = new IntToBoolConverter();
        //    //btnRemoveSubtitle.SetBinding(Button.IsEnabledProperty, bind);

        //    sp.Children.Add(lbSubtitles);
        //    sp2.Children.Add(btnAddSubtitle);
        //    sp2.Children.Add(btnRemoveSubtitle);
        //    sp.Children.Add(sp2);
        //    ContentDialog cd = new ContentDialog()
        //    {
        //        Title = "Subtitles",
        //        Content = sp,
        //        PrimaryButtonText = "Ok",
        //        CloseButtonText = "Cancel"
        //    };
        //    cd.XamlRoot = this.Content.XamlRoot;
        //    var cdResult = await cd.ShowAsync();
        //    if (cdResult == ContentDialogResult.Primary)
        //    {
        //        HRESULT hr = HRESULT.S_OK;
        //        foreach (var item in lbSubtitles.Items)
        //        {
        //            int nTrackId = mediaEngine.FindTrackFromLabel(item.ToString());
        //            if (nTrackId == -1)
        //            {
        //                int nNewTrackId = mediaEngine.AddSubtitle(item.ToString());
        //            }
        //        }
        //        mediaEngine.UpdateTracksFromCollection(listSubtitles);
        //    }
        //    else
        //    {
        //        listSubtitles = new System.Collections.ObjectModel.ObservableCollection<string>(listSubtitlesOld);
        //    }
        //}

        //public class IntToBoolConverter : IValueConverter
        //{  
        //    public object Convert(object value, Type targetType, object parameter, string language)
        //    {
        //        if ((int)value > 0) 
        //            return true; 
        //        else
        //            return false;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, string language)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        private async Task<string> OpenSubtitleDialog()
        {
            var fop = new Windows.Storage.Pickers.FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(fop, hWnd);
            fop.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;

            fop.FileTypeFilter.Add(".srt");
            fop.FileTypeFilter.Add(".vtt");         

            var file = await fop.PickSingleFileAsync();
            return (file != null ? file.Path : string.Empty);
        }

        private void tsControls_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            if (mediaEngine != null)
                mediaEngine.ShowControls(ts.IsOn);  
        }

        private void tsEffects_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            if (mediaEngine != null)
            {
                mediaEngine.SetEffects(ts.IsOn);
            }            
        }

        private void cbInvert_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect |= CMediaEngine.EFFECT.INVERT);
            }
        }

        private void cbInvert_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect & ~CMediaEngine.EFFECT.INVERT);
            }
        }

        private void cbGrayscale_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect |= CMediaEngine.EFFECT.GRAYSCALE);
            }
        }

        private void cbGrayscale_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect & ~CMediaEngine.EFFECT.GRAYSCALE);
            }
        }

        private void cbRGB_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect |= CMediaEngine.EFFECT.RGB);
            }
        }

        private void cbRGB_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                CMediaEngine.EFFECT nEffect = mediaEngine.GetEffects();
                mediaEngine.UpdateEffects(nEffect & ~CMediaEngine.EFFECT.RGB);
            }
        }

        private void sliderR_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mediaEngine != null)
            {
                Slider s = sender as Slider;
                mediaEngine.EFFECT_RGB_INTENSITY_RED = (byte)s.Value;
            }
        }

        private void sliderG_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mediaEngine != null)
            {
                Slider s = sender as Slider;
                mediaEngine.EFFECT_RGB_INTENSITY_GREEN = (byte)s.Value;
            }
        }

        private void sliderB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mediaEngine != null)
            {
                Slider s = sender as Slider;
                mediaEngine.EFFECT_RGB_INTENSITY_BLUE = (byte)s.Value;
            }
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {
                if (mediaEngine.CanCapture())
                    mediaEngine.Capture();
                else
                {
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("A video stream must be running !", "Information");
                    WinRT.Interop.InitializeWithWindow.Initialize(md, hWnd);
                    _ = await md.ShowAsync();
                }
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (mediaEngine != null)
            {               
                var wb = mediaEngine.GetCaptureWriteableBitmapImage();
                if (wb != null)
                {                   
                    SaveFrameDialog(wb);                   
                }
                else
                {
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No captured frame found !", "Information");
                    WinRT.Interop.InitializeWithWindow.Initialize(md, hWnd);
                    _ = await md.ShowAsync();
                }
            }
        }  

        private async void SaveFrameDialog(Microsoft.UI.Xaml.Media.Imaging.WriteableBitmap wb)
        {
            mediaEngine.SetOpacity(10);
            StackPanel sp = new StackPanel();
            Image img = new Image();
            img.Source = wb;
            sp.Children.Add(img);
            ContentDialog cd = new ContentDialog()
            {
                Title = "Save Frame",
                Content = sp,
                PrimaryButtonText = "Save as",
                CloseButtonText = "Cancel"
            };
            cd.PrimaryButtonClick += async (sender, args) =>
            {
                var clickDeferral = args.GetDeferral();
                bool bRet = await SaveFrameDialogPicker();
                args.Cancel = !bRet;
                clickDeferral.Complete();
            };

            cd.XamlRoot = this.Content.XamlRoot;
            var cdResult = await cd.ShowAsync();
            mediaEngine.SetOpacity(100);
            if (cdResult == ContentDialogResult.Primary)
            {
                
            }
        }      

        private async Task<bool> SaveFrameDialogPicker()
        {
            var fsp = new Windows.Storage.Pickers.FileSavePicker();
            WinRT.Interop.InitializeWithWindow.Initialize(fsp, hWnd);
            fsp.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            fsp.SuggestedFileName = "New Document";

            fsp.FileTypeChoices.Add("JPG (*.jpg)", new List<string>() { ".jpg" });
            fsp.FileTypeChoices.Add("PNG Portable Network Graphics (*.png)", new List<string>() { ".png" });
            fsp.FileTypeChoices.Add("GIF Graphics Interchange Format (*.gif)", new List<string>() { ".gif" });
            fsp.FileTypeChoices.Add("BMP Windows Bitmap (*.bmp)", new List<string>() { ".bmp" });
            fsp.FileTypeChoices.Add("TIF Tagged Image File Format (*.tif)", new List<string>() { ".tif" });            

            Windows.Storage.StorageFile file = await fsp.PickSaveFileAsync();
            if (file != null)
            {
                mediaEngine.SaveCapture(file);
                return true;               
            }
            else
                return false;
        }
    }
}
