using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Camera : Page
    {
        public Camera()
        {
            this.InitializeComponent();
            
        }

        private void camera_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CameraButton.cameraChild));
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
        private void video_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CameraButton.video));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //this.Frame.Navigated += OnNavigated;
        }
    }
}
