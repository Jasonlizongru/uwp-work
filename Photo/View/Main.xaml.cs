using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Photo.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using System.ComponentModel;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Animation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Main : Page
    {

        private ObservableCollection<BitmapImage> imgs;

        //private Dictionary<BitmapImage, IRandomAccessStream> imagesource_dict;
        //private ObservableCollection<IRandomAccessStream> imagesource_dict;
        public Main()
        {
            this.InitializeComponent();

            imgs = null;
            
        }


        //private void OnNavigated(object sender, NavigationEventArgs e)
        //{
        //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ?
        //        AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        //}
        //private void BackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    //Frame rootFrame = Window.Current.Content as Frame;
        //    //if (rootFrame == null)
        //    //    return;
        //    //if (rootFrame.CanGoBack && e.Handled == false)
        //    //{
        //    //    e.Handled = true;

        //    //}
        //    this.Frame.GoBack();
        //}

        //protected override async void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    imgs = await ImgsMangers.getImgs();
        //}

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var imageDetail = e.ClickedItem;
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //this.Frame.Navigated += OnNavigated;
            var ab = imageDetail as BitmapImage;
            int fff = 0;
            int flag = 0;
            foreach(var a in Images)
            {
                if(a == ab)
                {
                    flag = fff;
                }
                fff++;
            }
            var tmp_editsource = all;
            List<ImageList> image_list = new List<ImageList>();
            image_list.Clear();
            image_list.Add(new ImageList { detailSource = (BitmapImage)imageDetail, editSource = tmp_editsource });
            //CommonNavigationTransitionInfo commtran = new CommonNavigationTransitionInfo();
            this.Frame.Navigate(typeof(imageDetail), image_list);
            //this.Frame.Navigate(typeof(imageDetail), imageDetail);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getImgs();
           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<BitmapImage> _images = new ObservableCollection<BitmapImage>();
        public ObservableCollection<BitmapImage> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged("Images");
            }
        }

        private Dictionary<BitmapImage, StorageFile> _cache = new Dictionary<BitmapImage, StorageFile>();

        public async void getImgs()
        {
            //Images = null;
            var pictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            if (pictures != null)
            {
                var folder = pictures.SaveFolder;
                StorageFileQueryResult query = folder.CreateFileQuery(Windows.Storage.Search.CommonFileQuery.OrderByDate);

                var images = await query.GetFilesAsync();
                if (images != null)
                {
                    //if (images.Count > 9)  //只显示最前面的9张
                    //{
                    //    images = images.Take(9).ToList();
                    //}
                    images.ToList().ForEach(async (image) =>
                    {
                        try
                        {
                            BitmapImage img = new BitmapImage();
                            var f = await image.OpenAsync(FileAccessMode.Read);
                            if (f != null)
                            {
                                f.Seek(0);
                                await img.SetSourceAsync(f);
                                
                                Images.Add(img);
                                _cache.Add(img, image);
                                //imagesource_dict.Add(f);
                            }
                        }
                        catch
                        {

                        }
                    });
                }
                images = images.ToList();
            }
        }
        private IRandomAccessStream all;
        private async void openMore_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var openFile = await openPicker.PickSingleFileAsync();
            if(openFile != null)
            {
                BitmapImage openImg = new BitmapImage();

                var tmpfile = await openFile.OpenAsync(FileAccessMode.Read);
                all = tmpfile;
                tmpfile.Seek(0);
                await openImg.SetSourceAsync(tmpfile);

                List<ImageList> image_list = new List<ImageList>();
                image_list.Clear();
                image_list.Add(new ImageList { detailSource = (BitmapImage)openImg, editSource = all });

                this.Frame.Navigate(typeof(imageDetail), image_list);
            }
           
        }
    }

    public class ImageList
    {
        public BitmapImage detailSource { get; set; }
        public IRandomAccessStream editSource { get; set; }
    }
}
