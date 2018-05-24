using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Photo.View;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class favorite : Page
    {
        public favorite()
        {
            this.InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getFavoriteImgs();
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //this.Frame.Navigated += OnNavigated;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<BitmapImage> _images = new ObservableCollection<BitmapImage>();
        public  ObservableCollection<BitmapImage> favoriteImages
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


        public void getFavoriteImgs()
        {
            for(int i = 0; i<imageDetail.favorite.Count(); i++)
            {
                var tmp = imageDetail.favorite.ElementAt(i);
                favoriteImages.Add(tmp);
            }
            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
