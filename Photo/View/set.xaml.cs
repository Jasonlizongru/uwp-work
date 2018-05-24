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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class set : Page
    {
        public set()
        {
            this.InitializeComponent();
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            aboutItem.IsSelected = true;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(aboutItem.IsSelected)
            {
                text.Text = "Easy Photo\n版本：1.1\n2017.04.20\n\n设备信息\nWindows.Desktop\n10.0.14393.1066\nASUS GX800VH";
            }
            else if(helpItem.IsSelected)
            {
                text.Text = "相册,拍照,摄像,美图\n剪切,贴纸,拼图,收藏\n\nJust do it!";
            }
            else if(sendItem.IsSelected)
            {
                text.Text = "Email: yangwenhui@stumail.neu.edu.cn\n\n感谢您的使用与支持！";
            }
        }
    }
}
