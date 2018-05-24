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
using Photo.View;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Animation;


//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Photo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            //backButton.Visibility = Visibility.Collapsed;
            ContentFrame.Navigate(typeof(View.Main));
            Info.Text = "主 页";
            Main.IsSelected = true;

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Main.IsSelected)
            {
                //backButton.Visibility = Visibility.Collapsed;
                ContentFrame.Navigate(typeof(View.Main));
                Info.Text = "主 页";
            }
            else if (Camera.IsSelected)
            {
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                //backButton.Visibility = Visibility.Visible;
                
                ContentFrame.Navigate(typeof(View.Camera));
                Info.Text = "相 机";
            }
            else if(Collection.IsSelected)
            {
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                //backButton.Visibility = Visibility.Visible;
                ContentFrame.Navigate(typeof(View.favorite));
                Info.Text = "收 藏";
            }
            else if(EditPhoto.IsSelected)
            {
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                //backButton.Visibility = Visibility.Visible;
                ContentFrame.Navigate(typeof(View.edit));
                Info.Text = "编 辑";
                //editImage a = new editImage();
            }
            else if(Puzzle.IsSelected)
            {
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                //backButton.Visibility = Visibility.Visible;
                ContentFrame.Navigate(typeof(View.join));
                Info.Text = "拼 图";
            }
            else if(Settings.IsSelected)
            {
                ContentFrame.Navigate(typeof(View.set));
                Info.Text = "设 置";
            }
        }

        //private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Sidebar.IsPaneOpen = !Sidebar.IsPaneOpen;
        //}

        private void HamburgerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //if(Sidebar.Visibility == Visibility.Collapsed)
            //{
            //    //Sidebar.Visibility = Visibility.Visible;
            //    //Sidebar.Visibility = !Sidebar.Visibility;
            //    Sidebar.IsPaneOpen = true;
            //}
            //else
            //{
            //    Sidebar.IsPaneOpen = !Sidebar.IsPaneOpen;
            //}



            //if (stategroup.CurrentState == hide)
            //{
            //    //Sidebar.IsPaneOpen = (Sidebar.Visibility == Visibility.Visible) ? false : true;
            //    //Sidebar.Visibility = (Sidebar.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            //    if (Sidebar.Visibility == Visibility.Collapsed)
            //    {
            //        Sidebar.Visibility = Visibility.Visible;
            //        //Sidebar.IsPaneOpen = true;
            //    }
            //    else if (Sidebar.Visibility == Visibility.Visible)
            //    {
            //        //Sidebar.IsPaneOpen = true;
            //        Sidebar.Visibility = Visibility.Collapsed;
            //    }
            //    else if (Sidebar.IsPaneOpen)
            //    {
            //        Sidebar.Visibility = Visibility.Collapsed;
            //    }
            //}
            //else
            //{
            //    Sidebar.IsPaneOpen = !Sidebar.IsPaneOpen;
            //}

            //Sidebar.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            Sidebar.IsPaneOpen = !Sidebar.IsPaneOpen;

        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                Main.IsSelected = true;
                
            }
        }

        private void ContentFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            ContentFrame.Navigated += OnNavigated;
        }
        private bool firstmain = true;
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if(((sender as Frame).Name == "Main") && firstmain)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                firstmain = false;
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
        private bool needtwoback = false;
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            //Frame rootFrame = Window.Current.Content as Frame;
            //if (rootFrame == null)
            //    return;
            //if (rootFrame.CanGoBack && e.Handled == false)
            //{
            //    e.Handled = true;

            //}
            if(needtwoback)
            {
                ContentFrame.GoBack();
                if (ContentFrame.CanGoBack)
                {
                    ContentFrame.GoBack();
                }
                needtwoback = false;
            }
            else
            {
                ContentFrame.GoBack();
            }
            var currentFrame_back = (ContentFrame as Frame).CurrentSourcePageType.Name;
            switch(currentFrame_back)
            {
                case "Main":
                    Main.IsSelected = true;
                    needtwoback = true;
                    break;
                case "Camera":
                    Camera.IsSelected = true;
                    needtwoback = true;
                    break;
                case "edit":
                    EditPhoto.IsSelected = true;
                    needtwoback = true;
                    break;
                case "favorite":
                    Collection.IsSelected = true;
                    needtwoback = true;
                    break;
                case "join":
                    Puzzle.IsSelected = true;
                    needtwoback = true;
                    break;
                case "set":

                    break;
            }
        }
    }
}
