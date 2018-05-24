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
using Photo;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class imageDetail : Page
    {
        public imageDetail()
        {
            this.InitializeComponent();
        }
        private IRandomAccessStream editImageSource;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //这个e.Parameter是获取传递过来的参数，其实大家应该再次之前判断这个参数是否为null的，我偷懒了
            //var imagesource = e.Parameter;
            var imagesource = e.Parameter as List<ImageList>;
            editImageSource = imagesource[0].editSource;
            if (imagesource != null)
            {
                detailImage.Source = (ImageSource)imagesource[0].detailSource;
            }
            //MainPage.backButton.Visibility = Visibility.Visible;
            //MainPage.Current.backButton.Visibility = Visibility.Visible;
            //Microsoft.Graphics.Canvas.CanvasDevice tmpcd = Microsoft.Graphics.Canvas.CanvasDevice.GetSharedDevice();
            //await Microsoft.Graphics.Canvas.CanvasBitmap.LoadAsync(tmpcd, detailImage.Source);
            //Microsoft.Graphics.Canvas.CanvasBitmap a = new Microsoft.Graphics.Canvas.CanvasBitmap();
        }

        //public async Task<byte[]> SaveToBytesAsync(ImageSource imageSource)
        //{
        //    byte[] imageBuffer;
        //    var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        //    var file = await localFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
        //    using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
        //    {
        //        WriteableBitmap bitmap = imageSource as WriteableBitmap;
        //        var stream = bitmap.PixelBuffer.AsStream();
        //        byte[] buffer = new byte[stream.Length];
        //        await stream.ReadAsync(buffer, 0, buffer.Length);
        //        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
        //        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);
        //        await encoder.FlushAsync();

        //        var imageStream = ras.AsStream();
        //        imageStream.Seek(0, SeekOrigin.Begin);
        //        imageBuffer = new byte[imageStream.Length];
        //        var re = await imageStream.ReadAsync(imageBuffer, 0, imageBuffer.Length);

        //    }
        //    await file.DeleteAsync(StorageDeleteOption.Default);
        //    return imageBuffer;
        //}

        public static ObservableCollection<BitmapImage> favorite = new ObservableCollection<BitmapImage>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(favorite.Contains((BitmapImage)detailImage.Source))
            {
                like.Visibility = Visibility.Collapsed;
                dislike.Visibility = Visibility.Visible;
            }
            else
            {
                like.Visibility = Visibility.Visible;
                dislike.Visibility = Visibility.Collapsed;
            } 
        }

        private void favoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if(like.Visibility == Visibility.Visible)
            {
                like.Visibility = Visibility.Collapsed;
                dislike.Visibility = Visibility.Visible;
                favorite.Add((BitmapImage)detailImage.Source);
            }
            else
            {
                like.Visibility = Visibility.Visible;
                dislike.Visibility = Visibility.Collapsed;
                favorite.Remove((BitmapImage)detailImage.Source);
            }
        }

        private void delete_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void edit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //var imageSource = detailImage.Source;
            //byte[] imageBuffer;
            //var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //var file = await localFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
            //using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            //{
            //    WriteableBitmap bitmap = imageSource as WriteableBitmap;
            //    var stream = bitmap.PixelBuffer.AsStream();
            //    byte[] buffer = new byte[stream.Length];
            //    await stream.ReadAsync(buffer, 0, buffer.Length);
            //    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
            //    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);
            //    await encoder.FlushAsync();

            //    var imageStream = ras.AsStream();
            //    imageStream.Seek(0, SeekOrigin.Begin);
            //    imageBuffer = new byte[imageStream.Length];
            //    var re = await imageStream.ReadAsync(imageBuffer, 0, imageBuffer.Length);

            //}
            //await file.DeleteAsync(StorageDeleteOption.Default);
            //this.Frame.Navigate(typeof(edit), imageBuffer);
            if(editImageSource != null)
            {
                this.Frame.Navigate(typeof(edit), editImageSource);
            }
            else
            {
                this.Frame.Navigate(typeof(edit));
            }
        }
    }
}
