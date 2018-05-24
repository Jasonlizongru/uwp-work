using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Photo.Controllers;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class join : Page
    {
        public join()
        {
            this.InitializeComponent();
        }

        private int join_mode = 1;  //1-竖直，2-水平
        private Queue<CanvasBitmap> joinimage = new Queue<CanvasBitmap>();
        //private StickManage _wall_paperUI;
        private List<CanvasBitmap> joinimage_list = new List<CanvasBitmap>();

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(join_mode ==0)
            {
                return;
            }
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            var openFile = await openPicker.PickMultipleFilesAsync();
            if (openFile != null)
            {
                CanvasDevice tmpcd = CanvasDevice.GetSharedDevice();
                if ((join_mode == 1) || (join_mode == 2))
                {
                    foreach (var a in openFile)
                    {
                        var tmpimgcanvas = await a.OpenAsync(FileAccessMode.Read);
                        var joinimage_tmp = await CanvasBitmap.LoadAsync(tmpcd, tmpimgcanvas);
                        joinimage_list.Add(joinimage_tmp);
                    }
                }
                SetCanvas();
                //imgCanvas = tmpimgcanvas;
                //editCanvas.ConvertPixelsToDips(800);
                joinCanvas.Invalidate();
                //editCanvas.CreateResources += EditCanvas_CreateResources;
                joinCanvas.Draw += joinCanvas_Draw;
            }
        }

        private List<Size> join_size = new List<Size>();

        private void SetCanvas()
        {
            if(join_mode == 1)
            {
                //var width_scale = joinimage_list[0].Bounds.Width / joinCanvas.Width;
                joinCanvas.Height = 0;
                foreach(var a in joinimage_list)
                {
                    var width_scale = a.Bounds.Width / joinCanvas.Width;
                    joinCanvas.Height += a.Bounds.Height / width_scale;
                    join_size.Add(new Size(joinCanvas.Width, a.Bounds.Height / width_scale));
                }
            }
            else if(join_mode == 2)
            {
                joinCanvas.Width = 0;
                foreach (var a in joinimage_list)
                {
                    var height_scale = a.Bounds.Height / joinCanvas.Height * 2;
                    joinCanvas.Width += a.Bounds.Width / height_scale;
                    join_size.Add(new Size(a.Bounds.Width / height_scale, joinCanvas.Height / 2));
                }
            }
        }

        private void Clear()
        {
            joinimage_list.Clear();
            joinCanvas.Width = 520;
            joinCanvas.Height = 520;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //this.Frame.Navigated += OnNavigated;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickItem = (e.ClickedItem as Image).Tag;
            bool flag = true;
            if((int.Parse(clickItem.ToString()) != join_mode) && (joinimage_list.Count > 0))
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog("您还没有保存您的更改，更换模式将不会保存您的修改，是否确定更换模式？") { Title = "提示" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { Clear(); flag = true; }));
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand => {
                    if(join_mode == 1)
                    {
                        item_ver2.IsSelected = true;
                    }
                    else if(join_mode == 2)
                    {
                        item_hor2.IsSelected = true;
                    }
                    flag = false;
                    return;
                }));
                await msgDialog.ShowAsync();
            }
            if(flag)
            {
                join_mode = int.Parse(clickItem.ToString());
            }
            
            
            //Clear();
        }

        private void joinCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var canvas_target = GetDrawing(true);
            if (canvas_target != null)
            {
                args.DrawingSession.DrawImage(canvas_target);
            }
        }
        private CanvasRenderTarget GetDrawing(bool isedit)
        {
            var w = joinCanvas.ActualWidth;
            var h = joinCanvas.ActualHeight;
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)w, (float)h, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
            {
                //while(joinimage.Count > 0)
                //{
                //    joinimage1 = joinimage.Dequeue();
                //}
                //绘制背景
                graphics.Clear(Colors.LightGray);

                //绘制底图
                DrawBackImage(graphics, 1);

            }

            return target;
        }
        
        private void DrawBackImage(CanvasDrawingSession graphics, double scale)
        {
            Rect des = new Rect();
            if(join_mode == 1)
            {
                des.X = 0;
                des.Y = 0;
            }
            else if(join_mode == 2)
            {
                des.X = 0;
                des.Y = joinCanvas.Height / 4;
            }
            //int flag = 0;
            foreach (var joinimage1 in joinimage_list)
            {
                var width_scale = joinimage1.Bounds.Width / joinCanvas.Width;
                var height_scale = joinimage1.Bounds.Height / joinCanvas.Height;
                
                if (join_mode == 1)
                {
                    des.Width = joinCanvas.Width;
                    des.Height = joinimage1.Bounds.Height / width_scale;
                }
                else if(join_mode == 2)
                {
                    des.Height = joinCanvas.Height / 2;
                    des.Width = joinimage1.Bounds.Width / (height_scale * 2);
                }
                graphics.DrawImage(joinimage1, des, joinimage1.Bounds);
                if(join_mode == 1)
                {
                    des.X = 0;
                    des.Y += des.Height;
                }
                else if(join_mode == 2)
                {
                    des.Y = joinCanvas.Height / 4;
                    des.X += des.Width;
                }
            }
        }

        private async void cancle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(joinimage_list.Count > 0)
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog("退出拼图？") { Title = "提示" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { Clear(); }));
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand => { }));
                await msgDialog.ShowAsync();
            }
        }

        private async void ok_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (joinimage_list.Count > 0)
            {
                var imageResult = GetDrawing(false);
                if (imageResult != null)
                {
                    var folder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                    StorageFile saveFile = await folder.SaveFolder.CreateFileAsync("imageJoin.png", CreationCollisionOption.GenerateUniqueName);
                    using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await imageResult.SaveAsync(fileStream, CanvasBitmapFileFormat.Png, 1f);
                        var msgDialog = new Windows.UI.Popups.MessageDialog("拼图已保存") { Title = "提示" };
                        msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { }));
                        await msgDialog.ShowAsync();
                    }
                }
            }
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

        //private BitmapImage joinImage()
        //{
        //    int width = 1920;
        //    int height = 2280;
        //    BitmapImage join = new BitmapImage();
        //    Microsoft.Graphics.Canvas g = Windows.Graphics.Imaging.
        //}
    }
}
