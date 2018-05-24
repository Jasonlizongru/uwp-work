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
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.Graphics.Canvas;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Photo.Controllers;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Shapes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Photo.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class edit : Page
    {
        public edit()
        {
            this.InitializeComponent();
            Loaded += Page_Loaded;



        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //这个e.Parameter是获取传递过来的参数，其实大家应该再次之前判断这个参数是否为null的，我偷懒了
            var imagesource = e.Parameter as IRandomAccessStream;
            if (imagesource != null)
            {
                CanvasDevice tmpcd = CanvasDevice.GetSharedDevice();
                editimage = await CanvasBitmap.LoadAsync(tmpcd, imagesource);
                editCanvas.Invalidate();
                editCanvas.Draw += editCanvas_Draw;
                //editimage = await CanvasBitmap.CreateFromBytes(tmpcd, imagesource, 1400, 900, Windows.Graphics.DirectX.DirectXPixelFormat.A8UIntNormalized);
                improveListbox.Background.Opacity = 0.5;
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

        //private CanvasDrawingSession canvasDrawingSession;
        private CanvasBitmap editimage;//底图
        private Stretch _stretch = Stretch.Uniform;
        private int imageState = 0;//当前图片编辑状态，1-旋转，2-裁剪 3-增强 4-贴纸 5-滤镜 6-涂鸦 7-水印
        private int _manipulation_type = 0; // 0表示移动剪切对象 1表示缩放剪切对象 2表示移动tag 3表示移动贴纸 4表示缩放贴纸
        private Point? _pre_manipulation_position;  //操作起始点
        private int size_mode;
        private int rotate = 0;//旋转角度
        private CutManage cutmanage;//裁剪
        private bool iscut = false;
        private bool cutok = false;
        private bool iscuted = false;
        private Rect descut = new Rect(0, 0, 0, 0);
        private float left;
        private float top;
        private bool cuted = false;

        private Color _back_color = Colors.LightGray;

        private int _filter_index = 0;  //当前使用的滤镜，0表示无，1表示黑白...以此类推

        private int _pen_size;//画笔大小
        private Color _pen_color;//画笔颜色
        private DoodleManage _current_editing_doodleUI;  //当前涂鸦对象
        Stack<DrawManage> _doodleUIs = new Stack<DrawManage>();  //涂鸦

        StickManage _wall_paperUI;//贴纸

        private bool haveWatermark = false;
        private string watermark_text = "@uwp";

        private void editCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            //if(editimage != null)
            //{

            //    //CanvasDevice device = CanvasDevice.GetSharedDevice();
            //    //CanvasRenderTarget target = new CanvasRenderTarget(device, 200, 200, 96);
            //    //target.
            //    //target.CreateDrawingSession();
            //    //args.DrawingSession.DrawImage(target);


            //    var tmp_width = editCanvas.Width;
            //    var tmp_height = editimage.SizeInPixels.Height / (editimage.SizeInPixels.Width / tmp_width);
            //    var tmp_y = (editCanvas.Height - tmp_height) / 2;
            //    args.DrawingSession.DrawImage(editimage, new Rect(0, tmp_y, tmp_width, tmp_height));
            //    //args.DrawingSession.DrawImage(editimage, new Rect(0, tmp_y, tmp_width*0.8, tmp_height*0.8));
            //    //canvasDrawingSession = args.DrawingSession;

            //    //if(iscut)
            //    //{
            //    //    cutmanage.Draw(args.DrawingSession, 1);
            //    //}
            //}

            //args.DrawingSession.DrawEllipse(155, 115, 80, 30, Colors.Black, 3);
            //args.DrawingSession.DrawText("Hello, world!", 100, 100, Colors.Yellow);

            var canvas_target = GetDrawing(true);
            if(canvas_target != null)
            {
                args.DrawingSession.DrawImage(canvas_target);
            }
        }

        private CanvasRenderTarget GetDrawing(bool isedit)
        {
            double w, h;  //画布大小
            if (isedit)  //编辑状态
            {
                w = editCanvas.ActualWidth;
                h = editCanvas.ActualHeight;
            }
            else  //最终生成图片  有一定的scale
            {
                Rect des = GetImageDrawingRect();

                w = (editimage.Size.Width / des.Width) * editCanvas.Width;
                h = (editimage.Size.Height / des.Height) * editCanvas.Height;
            }
            var scale = isedit ? 1 : w / editCanvas.Width;  //缩放比例

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)w, (float)h, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
            {
                //绘制背景
                graphics.Clear(_back_color);

                //绘制底图
                DrawBackImage(graphics, scale);

                //绘制涂鸦
                if (_doodleUIs != null && _doodleUIs.Count > 0)
                {
                    var list = _doodleUIs.ToList(); list.Reverse();
                    list.ForEach((d) => { d.Draw(graphics, (float)scale); });   //绘制历史涂鸦
                }
                if (_current_editing_doodleUI != null)
                {
                    _current_editing_doodleUI.Draw(graphics, (float)scale); //绘制当前涂鸦
                }
                //绘制贴图
                if (_wall_paperUI != null)
                {
                    _wall_paperUI.Draw(graphics, (float)scale);
                }
                //水印
                if(haveWatermark)
                {
                    graphics.DrawText(watermark_text, (float)editCanvas.Width / 2 - 2, (float)editCanvas.Height / 2, Colors.Black, new CanvasTextFormat() { FontFamily = "微软雅黑", FontSize = 15});
                }
                ////绘制Tag
                //if (_tagsUIs != null)
                //{
                //    _tagsUIs.ForEach((t) => { t.Draw(graphics, (float)scale); });
                //}
                //绘制Crop裁剪工具
                if (cutmanage != null && isedit)
                {
                    cutmanage.Draw(graphics, (float)scale);
                }
            }

            return target;
        }

        private Rect GetImageDrawingRect()
        {
            Rect des;

            var image_w = editimage.Size.Width;
            var image_h = editimage.Size.Height;

            if (_stretch == Stretch.Uniform)
            {
                var w = editCanvas.Width - 20;
                var h = editCanvas.Height - 20;
                if (image_w / image_h > w / h)
                {
                    var left = 10;

                    var width = w;
                    var height = (image_h / image_w) * width;

                    var top = (h - height) / 2 + 10;

                    des = new Rect(left, top, width, height);
                }
                else
                {
                    var top = 10;
                    var height = h;
                    var width = (image_w / image_h) * height;
                    var left = (w - width) / 2 + 10;
                    des = new Rect(left, top, width, height);
                }
            }
            else
            {
                var w = editCanvas.Width;
                var h = editCanvas.Height;
                var left = 0;
                var top = 0;
                if (image_w / image_h > w / h)
                {
                    var height = h;
                    var width = (image_w / image_h) * height;
                    des = new Rect(left, top, width, height);
                }
                else
                {
                    var width = w;
                    var height = (image_h / image_w) * width;

                    des = new Rect(left, top, width, height);
                }
            }
            return des;
        }

        private void DrawBackImage(CanvasDrawingSession graphics, double scale)
        {
            if (editimage != null)
            {
                Rect des = GetImageDrawingRect();
                des.X *= scale;
                des.Y *= scale;
                des.Width *= scale;
                des.Height *= scale;
                // 滤镜特效

                //亮度
                ICanvasImage image = GetBrightnessEffect(editimage);
                //对比度
                image = GetContrastEffect(image);
                //饱和度
                image = GetSaturationEffect(image);
                //锐化
                image = GetSharpenEffect(image);
                //模糊
                image = GetBlurEffect(image);

                ////应用滤镜模板
                image = ApplyFilterTemplate(image);

                var width_scale = editimage.Bounds.Width / des.Width;
                var height_scale = editimage.Bounds.Height / des.Height;
                if (cutok)
                {
                    left = (float)cutmanage.cutPointX;
                    top = (float)cutmanage.cutPointY;
                    descut.X = (cutmanage.cutPointX - des.X) * width_scale;
                    descut.Y = (cutmanage.cutPointY - des.Y) * height_scale;
                    descut.Width = cutmanage.cutWidth * width_scale;
                    descut.Height = cutmanage.cutHeight * height_scale;
                    cutok = false;
                    cutmanage = null;
                }
                //des.Width = 100;
                //Vector2 vec = new Vector2();


                if (iscuted)
                {
                    //graphics.DrawImage(_image, left,top,descut);

                    if (cuted)
                    {

                        //graphics.DrawImage(image, des, descut);
                        //image = new AtlasEffect { Source = image, SourceRectangle = descut };
                        image = new CropEffect { Source = image, SourceRectangle = descut };

                    }
                    else
                    {
                        //graphics.DrawImage(image, des, descut);
                        //image = new AtlasEffect { Source = image, SourceRectangle = descut };
                        image = new CropEffect { Source = image, SourceRectangle = descut };
                        cuted = true;
                    }
                }
                //else
                //{
                //    graphics.DrawImage(image, des, editimage.Bounds);
                //}
                graphics.DrawImage(image, des, editimage.Bounds);
                //if(cutok)
                //{
                //    image = new CropEffect { Source = image, SourceRectangle = new Rect(cutmanage.cutPointX, cutmanage.cutPointY, cutmanage.cutWidth, cutmanage.cutHeight) };
                //    //cutok = false;
                //    cutmanage = null;
                //}
                //graphics.DrawImage(image, des, editimage.Bounds);
            }
        }

        private ICanvasImage GetBrightnessEffect(ICanvasImage source)
        {
            var t = brightness.Value / 500 * 2;
            var exposureEffect = new ExposureEffect
            {
                Source = source,
                Exposure = (float)t
            };

            return exposureEffect;
        }

        private ICanvasImage GetContrastEffect(ICanvasImage source)
        {
            var contrastEffects = new ContrastEffect
            {
                Source = source,
                Contrast = (float)(contrast.Value * 0.01)
            };

            return contrastEffects;
        }

        private ICanvasImage GetSaturationEffect(ICanvasImage source)
        {
            var t = (saturation.Value + 100) * 0.01;
            var saturationEffect = new SaturationEffect
            {
                Source = source,
                Saturation = (float)t
                //Saturation = 2
            };

            return saturationEffect;
        }

        private ICanvasImage GetSharpenEffect(ICanvasImage source)
        {
            var sharpenEffect = new SharpenEffect
            {
                Source = source,
                Amount = (float)(sharpen.Value * 0.1)
            };
            return sharpenEffect;
        }

        private ICanvasImage GetBlurEffect(ICanvasImage source)
        {
            var t = blur.Value / 100 * 12;
            var blurEffect = new GaussianBlurEffect
            {
                Source = source,
                BlurAmount = (float)t
            };
            return blurEffect;
        }

        private ICanvasImage ApplyFilterTemplate(ICanvasImage source)
        {
            if (_filter_index == 0)  //无滤镜
            {
                return source;
            }
            else if (_filter_index == 1)  // 黑白
            {
                return new GrayscaleEffect
                {
                    Source = source
                };
            }
            else if (_filter_index == 2)  //反色
            {
                return new InvertEffect
                {
                    Source = source
                };
            }
            else if (_filter_index == 3) //冷色
            {
                var hueRotationEffect = new HueRotationEffect
                {
                    Source = source,
                    Angle = 0.5f
                };
                return hueRotationEffect;
            }
            else if (_filter_index == 4)  //美食
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = 0.6f;
                temperatureAndTintEffect.Tint = 0.6f;

                return temperatureAndTintEffect;
            }
            else if (_filter_index == 5) //冷绿
            {
                var temperatureAndTintEffect = new TemperatureAndTintEffect
                {
                    Source = source
                };
                temperatureAndTintEffect.Temperature = -0.6f;
                temperatureAndTintEffect.Tint = -0.6f;

                return temperatureAndTintEffect;
            }
            else if (_filter_index == 6) //梦幻
            {
                var vignetteEffect = new VignetteEffect
                {
                    Source = source
                };
                vignetteEffect.Color = Color.FromArgb(255, 0xFF, 0xFF, 0xFF);
                vignetteEffect.Amount = 0.6f;

                return vignetteEffect;
            }
            else if (_filter_index == 7) //浮雕
            {
                var embossEffect = new EmbossEffect
                {
                    Source = source
                };
                embossEffect.Amount = 5;
                embossEffect.Angle = 0;
                return embossEffect;
            }
            else if (_filter_index == 8) //怀旧
            {
                var sepiaEffect = new SepiaEffect
                {
                    Source = source
                };
                sepiaEffect.Intensity = 1;
                return sepiaEffect;
            }
            else if(_filter_index == 9)//运动
            {
                var directEffect = new DirectionalBlurEffect
                {
                    Source = source
                };
                directEffect.BlurAmount = 12;
                directEffect.BorderMode = EffectBorderMode.Soft;
                directEffect.Angle = 3.14F;
                return directEffect;
            }
            else
            {
                return source;
            }
        }

        private IRandomAccessStream imgCanvas;
        private async void editOpen_Click(object sender, RoutedEventArgs e)
        {
            iscuted = false;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            var openFile = await openPicker.PickSingleFileAsync();
            if (openFile != null)
            {
                var tmpimgcanvas = await openFile.OpenAsync(FileAccessMode.Read);
                CanvasDevice tmpcd = CanvasDevice.GetSharedDevice();
                editimage = await CanvasBitmap.LoadAsync(tmpcd, tmpimgcanvas);
                imgCanvas = tmpimgcanvas;
                //editCanvas.ConvertPixelsToDips(800);
                editCanvas.Invalidate();
                //editCanvas.CreateResources += EditCanvas_CreateResources;
                editCanvas.Draw += editCanvas_Draw;
            }
        }

        //private async void EditCanvas_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        //{
        //    //if(imgCanvas == null)
        //    //{

        //    //}
        //    //else
        //    //{
        //    //    await CanvasBitmap.LoadAsync(sender, imgCanvas);
        //    //    editCanvas.Invalidate();
        //    //}
        //    //var bitmapTiger = await CanvasBitmap.LoadAsync(sender, "ms-appx:///Assets/StoreLogo.png");
        //    args.TrackAsyncAction(EditCanvas_CreateResourcesAsync(sender).AsAsyncAction());
        //}

        //async Task EditCanvas_CreateResourcesAsync(CanvasControl sender)
        //{
        //    // Load bitmaps, create brushes, etc.
        //    var bitmapTiger = await CanvasBitmap.LoadAsync(sender, "ms-appx:///Assets/StoreLogo.png");
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //FileOpenPicker openPicker = new FileOpenPicker();
            //openPicker.ViewMode = PickerViewMode.Thumbnail;
            //openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //openPicker.FileTypeFilter.Add(".jpg");
            //openPicker.FileTypeFilter.Add(".jpeg");
            //openPicker.FileTypeFilter.Add(".png");
            //openPicker.FileTypeFilter.Add(".gif");
            //var openFile = await openPicker.PickSingleFileAsync();
            //if (openFile != null)
            //{
            //    var tmpimgcanvas = await openFile.OpenAsync(FileAccessMode.Read);
            //    CanvasDevice tmpcd = CanvasDevice.GetSharedDevice();
            //    var tmp = await CanvasBitmap.LoadAsync(tmpcd, tmpimgcanvas);
            //    this.imgCanvas = tmpimgcanvas;
            //    editCanvas.Invalidate();
            //    editCanvas.CreateResources += EditCanvas_CreateResources;
            //}
            improveListbox.Background.Opacity = 0.35;
            adjustListbox.Background.Opacity = 0.35;
            stickListbox.Background.Opacity = 0.35;
            filterListbox.Background.Opacity = 0.35;
            writeListbox.Background.Opacity = 0.35;
            watermarkListbox.Background.Opacity = 0.35;
            setCanvas();
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = this.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
            //this.Frame.Navigated += OnNavigated;
        }

        private void editCanvas_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (imageState == 2)
            {
                if(cutmanage != null)
                {
                    if ((cutmanage as CutManage).cutArea.Contains(e.Position)) //移动剪切对象
                    {
                        _manipulation_type = 0;
                        _pre_manipulation_position = e.Position;
                    }
                    if ((cutmanage as CutManage).RightBottomPoint.Contains(e.Position)) //缩放剪切区域
                    {
                        _manipulation_type = 1;
                        _pre_manipulation_position = e.Position;
                    }
                }
            }
            if(imageState == 4)
            {
                if (_wall_paperUI != null)
                {
                    if ((_wall_paperUI as StickManage).Region.Contains(e.Position))  //移动墙纸
                    {
                        _manipulation_type = 3;
                        _pre_manipulation_position = e.Position;
                        (_wall_paperUI as StickManage).Editing = true;
                    }
                    if ((_wall_paperUI as StickManage).RightBottomRegion.Contains(e.Position) && (_wall_paperUI as StickManage).Editing)  //缩放墙纸
                    {
                        _manipulation_type = 4;
                        _pre_manipulation_position = e.Position;
                    }
                    editCanvas.Invalidate();
                }
                return;
            }
            if(imageState == 6)
            {
                if (_current_editing_doodleUI == null)
                {
                    _current_editing_doodleUI = new DoodleManage() { DrawingColor = _pen_color, DrawingSize = _pen_size };
                }
                return;
            }

        }

        private void editCanvas_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (imageState == 2)
            {
                if (cutmanage != null)
                {
                    _pre_manipulation_position = null;
                }
                return;
            }
            if(imageState == 4)
            {
                if (_wall_paperUI != null)
                {
                    _pre_manipulation_position = null;
                }
                return;
            }
            if(imageState == 6)
            {
                if (_current_editing_doodleUI != null)
                {
                    _doodleUIs.Push(_current_editing_doodleUI);
                    _current_editing_doodleUI = null;
                    editCanvas.Invalidate();
                }
                return;
            }
        }

        private void editCanvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (imageState == 2)  //剪切
            {
                if (cutmanage != null && _pre_manipulation_position != null)
                {
                    var deltaX = e.Position.X - _pre_manipulation_position.Value.X;
                    var deltaY = e.Position.Y - _pre_manipulation_position.Value.Y;

                    if (_manipulation_type == 0) //移动
                    {
                        (cutmanage as CutManage).cutPointX += deltaX;
                        (cutmanage as CutManage).cutPointY += deltaY;
                    }
                    else if (_manipulation_type == 1) //缩放
                    {
                        (cutmanage as CutManage).cutWidth += deltaX;
                        (cutmanage as CutManage).cutHeight += deltaY;
                    }

                    _pre_manipulation_position = e.Position;
                    editCanvas.Invalidate();
                }
                return;
            }
            if(imageState == 4)
            {
                if (_wall_paperUI != null && _pre_manipulation_position != null)
                {
                    var deltaX = e.Position.X - _pre_manipulation_position.Value.X;
                    var deltaY = e.Position.Y - _pre_manipulation_position.Value.Y;
                    if (_manipulation_type == 3)  //移动
                    {
                        (_wall_paperUI as StickManage).X += deltaX;
                        (_wall_paperUI as StickManage).Y += deltaY;
                    }
                    else if (_manipulation_type == 4)  //缩放
                    {
                        (_wall_paperUI as StickManage).Width += deltaX * 2;
                        (_wall_paperUI as StickManage).SyncWH();  //只需要设置宽度  高度自动同步
                    }
                    _pre_manipulation_position = e.Position;

                    editCanvas.Invalidate();
                }
            }
            if(imageState == 6)
            {
                if (_current_editing_doodleUI != null)
                {
                    _current_editing_doodleUI.Points.Add(e.Position);
                    editCanvas.Invalidate();
                }
                return;
            }
        }

        public void setCanvas()
        {
            var w = MainWorkSpace.ActualWidth - 40;  //
            var h = MainWorkSpace.ActualHeight - 40;  //
            if (w <= 0 || h <= 0)
            {
                return;
            }
            if (size_mode == 0)  //1:1
            {
                var l = w > h ? h : w;
                editCanvas.Width = editCanvas.Height = l;
            }
            else if (size_mode == 1)  //4:3
            {
                if (w <= h)
                {
                    editCanvas.Width = w;
                    editCanvas.Height = editCanvas.Width * 3 / 4;
                }
                else
                {
                    if (w / h <= (double)4 / 3)
                    {
                        editCanvas.Width = w;
                        editCanvas.Height = editCanvas.Width * 3 / 4;
                    }
                    else
                    {
                        editCanvas.Height = h;
                        editCanvas.Width = editCanvas.Height * 4 / 3;
                    }
                }
            }
            else  //3:4
            {
                if (h <= w)
                {
                    editCanvas.Height = h;
                    editCanvas.Width = editCanvas.Height * 3 / 4;
                }
                else
                {
                    if (h / w <= (double)4 / 3)
                    {
                        editCanvas.Height = h;
                        editCanvas.Width = editCanvas.Height * 3 / 4;
                    }
                    else
                    {
                        editCanvas.Width = w;
                        editCanvas.Height = editCanvas.Width * 4 / 3;
                    }
                }
            }
            editCanvas.Invalidate();
        }

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var listItem = (sender as ListBoxItem).Name;
            if(listItem == "rotateItem")
            {
                imageState = 1;
                rotate = (rotate + 90) % 360;
                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = rotate;
                rotateTransform.CenterX = editCanvas.Width / 2;
                rotateTransform.CenterY = editCanvas.Height / 2;
                editCanvas.RenderTransform = rotateTransform;
                editCanvas.Draw += editCanvas_Draw;
            }
            else if(listItem == "cutItem")
            {
                imageState = 2;
                if(cutmanage == null)
                {
                    cutmanage = new CutManage() { cutPointX = editCanvas.Width / 4, cutPointY = editCanvas.Height / 4, cutWidth = 200, cutHeight = 200, editCanvas_Width = editCanvas.Width, editCanvas_Height = editCanvas.Height };
                    iscut = true;
                }
            }
            editCanvas.Invalidate();
            editCanvas.Draw += editCanvas_Draw;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button).Name;
            if (button == "adjustButton")
            {
                improveListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = Visibility.Collapsed;
                adjustListbox.Visibility = (adjustListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if(button == "improveButton")
            {
                imageState = 3;
                adjustListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = Visibility.Collapsed;
                improveListbox.Visibility = (improveListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (button == "stickButton")
            {
                imageState = 4;
                adjustListbox.Visibility = Visibility.Collapsed;
                improveListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = (stickListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (button == "filterButton")
            {
                imageState = 5;
                adjustListbox.Visibility = Visibility.Collapsed;
                improveListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = (filterListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (button == "writeButton")
            {
                imageState = 6;
                adjustListbox.Visibility = Visibility.Collapsed;
                improveListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = (writeListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if(button == "watermarkButton")
            {
                //imageState = 7;
                adjustListbox.Visibility = Visibility.Collapsed;
                improveListbox.Visibility = Visibility.Collapsed;
                stickListbox.Visibility = Visibility.Collapsed;
                filterListbox.Visibility = Visibility.Collapsed;
                writeListbox.Visibility = Visibility.Collapsed;
                watermarkListbox.Visibility = (watermarkListbox.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void editCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var p = e.GetPosition(editCanvas);
            if (cutmanage != null)
            {
                if ((cutmanage as CutManage).LeftTopPoint.Contains(p))  //取消
                {
                    cutmanage = null;
                    editCanvas.Invalidate();
                    return;
                }
                if ((cutmanage as CutManage).RightTopPoint.Contains(p))  //OK
                {
                    //
                    cutok = true;
                    iscuted = true;
                    editCanvas.Draw += editCanvas_Draw;
                    return;
                }
                if ((cutmanage as CutManage).cutArea.Contains(p))  //点击的是 剪切对象 区域
                {
                    return;
                }
            }
            if(_wall_paperUI != null)
            {
                if((_wall_paperUI as StickManage).RightTopRegion.Contains(p))
                {
                    _wall_paperUI = null;
                    editCanvas.Invalidate();
                    return;
                }
                if((_wall_paperUI as StickManage).Region.Contains(p))
                {
                    (_wall_paperUI as StickManage).Editing = !(_wall_paperUI as StickManage).Editing;
                    editCanvas.Invalidate();
                    return;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            editCanvas.Invalidate();
            editCanvas.Draw += editCanvas_Draw;
        }

        private void filterListbox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickItem = (e.ClickedItem as StackPanel).Name;
            switch(clickItem)
            {
                case "item0":
                    _filter_index = 0;
                    break;
                case "item1":
                    _filter_index = 1;
                    break;
                case "item2":
                    _filter_index = 2;
                    break;
                case "item3":
                    _filter_index = 3;
                    break;
                case "item4":
                    _filter_index = 4;
                    break;
                case "item5":
                    _filter_index = 5;
                    break;
                case "item6":
                    _filter_index = 6;
                    break;
                case "item7":
                    _filter_index = 7;
                    break;
                case "item8":
                    _filter_index = 8;
                    break;
                case "item9":
                    _filter_index = 9;
                    break;
            }
            editCanvas.Invalidate();
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _pen_color = ((sender as Rectangle).Fill as SolidColorBrush).Color;
            currentPen.Fill = (sender as Rectangle).Fill;
        }

        private void pen_size_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _pen_size = (int)((sender as Slider).Value);
            pen_size_textblock.Text = "画笔大小: " + _pen_size;
        }

        private void undo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_doodleUIs != null && _doodleUIs.Count > 0)
            {
                _doodleUIs.Pop();
                editCanvas.Invalidate();
            }    
        }

        private async void stickListbox_ItemClick(object sender, ItemClickEventArgs e)
        {
            var stickName = (e.ClickedItem as StackPanel).Tag.ToString();
            var stickPath = "ms-appx:///../Stick/" + stickName + ".png";
            //var bitmapTiger = await CanvasBitmap.LoadAsync(sender, "ms-appx:///Assets/StoreLogo.png");

            _wall_paperUI = new StickManage() { Editing = true, Height = 100, Width = 100, Image = null, X = 150, Y = 150 };
            editCanvas.Invalidate();

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var img = await CanvasBitmap.LoadAsync(device, stickPath);

            if (img != null)
            {
                (_wall_paperUI as StickManage).Width = img.Size.Width /4;
                (_wall_paperUI as StickManage).Height = img.Size.Height /4;
                (_wall_paperUI as StickManage).Image = img;

                editCanvas.Invalidate();
            }
        }

        private void watermarkItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            imageState = 7;
            haveWatermark = true;
            watermark_text = watemarkTextbox.Text;
            editCanvas.Draw += editCanvas_Draw;
            editCanvas.Invalidate();
        }

        private async void editcancle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (editimage != null)
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog("退出编辑？") { Title = "提示" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => {
                    editimage = null;
                    editCanvas.Invalidate();
                    haveWatermark = false;
                    _current_editing_doodleUI = null;
                    _doodleUIs.Clear();
                    _wall_paperUI = null;
                    editCanvas.Draw += editCanvas_Draw;
                    //setCanvas();
                }));
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand => { }));
                await msgDialog.ShowAsync();
            }
        }

        private async void editok_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(editimage != null)
            {
                var imageResult = GetDrawing(false);
                if (imageResult != null)
                {
                    var folder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                    StorageFile saveFile = await folder.SaveFolder.CreateFileAsync("image.png", CreationCollisionOption.GenerateUniqueName);
                    using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await imageResult.SaveAsync(fileStream, CanvasBitmapFileFormat.Png, 1f);
                        var msgDialog = new Windows.UI.Popups.MessageDialog("图片已保存") { Title = "提示" };
                        msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand => { }));
                        await msgDialog.ShowAsync();
                    }

                }
            }
        }
    }
}
