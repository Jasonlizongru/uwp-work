using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace Photo.Models
{
    public class ImagePath
    {
        private string fileContent;

        public ImagePath(string fileContent)
        {
            this.fileContent = fileContent;
        }

        public ImagePath()
        {
        }

        public string img { get; set; }
        public string fileName { get; set; }
        public string Path { get; internal set; }
        public BitmapImage File { get; internal set; }
        public StorageFile Storage { get; internal set; }
    }

    public class Image
    {
        public ObservableCollection<BitmapImage> Images { get; set; }
    }

    public class ImgsMangers
    {

        //public static DirectoryInfo dir = new DirectoryInfo(@"C:\\Users\\文辉\\Pictures");

        //public void setDir(DirectoryInfo dirPara)
        //{
        //    dir = dirPara;
        //}

        //public static async Task<List<Imgs>> getImgs()
        //{
        //    var img = new List<Imgs>();

        //    //FileInfo[] file = dir.GetFiles();

        //    //foreach (FileInfo tmpfile in file)
        //    //{
        //    //    if ((tmpfile.Extension == ".jpeg") || (tmpfile.Extension == ".png") || (tmpfile.Extension == ".jpg") || (tmpfile.Extension == ".gif"))
        //    //    {
        //    //        img.Add(new Imgs { img = tmpfile.ToString(), fileName = tmpfile.Name });
        //    //    }
        //    //}

        //    //StorageFolder pictureFolder = KnownFolders.PicturesLibrary;
        //    //IReadOnlyList<StorageFile> pictureFileList = await pictureFolder.GetFilesAsync();
        //    //IReadOnlyList<StorageFolder> pictureFolderList = await pictureFolder.GetFoldersAsync();

        //    //foreach (StorageFile f in pictureFileList)
        //    //{
        //    //    img.Add(new Imgs { fileName = f.Name });
        //    //}

        //    img.Add(new Imgs { img = "../Assets/1.png", fileName = "1" });
        //    img.Add(new Imgs { img = "../Assets/icon.scale-200.jpeg", fileName = "2" });

        //    img.Add(new Imgs { img = "C:/Users/文辉/Pictures/4272.jpg", fileName = "1" });

        //    return img;
        //}
        //    public static async Task<ObservableCollection<ImagePath>> getImgs()
        //    {
        //        StorageFile fileLocal = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/account/" + ImageHelper.folderStr + ".json"));
        //        if (fileLocal != null)
        //        {
        //            try
        //            {
        //                //读取本地文件内容，并且反序列化
        //                using (IRandomAccessStream readStream = await fileLocal.OpenAsync(FileAccessMode.Read))
        //                {
        //                    using (DataReader dataReader = new DataReader(readStream))
        //                    {
        //                        UInt64 size = readStream.Size;
        //                        if (size <= UInt32.MaxValue)
        //                        {
        //                            await dataReader.LoadAsync(sizeof(Int32));
        //                            Int32 stringSize = dataReader.ReadInt32();
        //                            await dataReader.LoadAsync((UInt32)stringSize);
        //                            string fileContent = dataReader.ReadString((uint)stringSize);
        //                            ImagePath imagePath = new ImagePath(fileContent);
        //                            StorageFolder folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(imagePath.Path);
        //                            //筛选图片
        //                            var queryOptions = new Windows.Storage.Search.QueryOptions();
        //                            queryOptions.FileTypeFilter.Add(".png");
        //                            queryOptions.FileTypeFilter.Add(".jpg");
        //                            queryOptions.FileTypeFilter.Add(".bmp");
        //                            var query = folder.CreateFileQueryWithOptions(queryOptions);
        //                            var files = await query.GetFilesAsync();

        //                            ImagePath img;

        //                            ObservableCollection<ImagePath>  imgList = new ObservableCollection<ImagePath>();
        //                            foreach (var item in files)
        //                            {
        //                                IRandomAccessStream irandom = await item.OpenAsync(FileAccessMode.Read);

        //                                //对图像源使用流源
        //                                BitmapImage bitmapImage = new BitmapImage();
        //                                bitmapImage.DecodePixelWidth = 160;
        //                                bitmapImage.DecodePixelHeight = 100;
        //                                await bitmapImage.SetSourceAsync(irandom);

        //                                img = new ImagePath();
        //                                img.Path = item.Path;
        //                                img.File = bitmapImage;
        //                                img.Storage = item;
        //                                imgList.Add(img);
        //                            }

        //                            return imgList;
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception exce)
        //            {
        //                await new MessageDialog(exce.ToString()).ShowAsync();
        //                throw exce;
        //            }
        //        }
        //        return null;
        //    }


        Image img_tmp = new Image();

        private Dictionary<BitmapImage, StorageFile> _cache = new Dictionary<BitmapImage, StorageFile>();
        public async Task<ObservableCollection<BitmapImage>> getImgs()
        {
            var pictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            if (pictures != null)
            {
                var folder = pictures.SaveFolder;
                StorageFileQueryResult query = folder.CreateFileQuery(Windows.Storage.Search.CommonFileQuery.OrderByDate);

                var images = await query.GetFilesAsync();
                if (images != null)
                {
                    if (images.Count > 9)  //只显示最前面的9张
                    {
                        images = images.Take(9).ToList();
                    }
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
                                (img_tmp.Images).Add(img);
                                _cache.Add(img, image);
                            }
                        }
                        catch
                        {

                        }
                    });
                }
            }
            return img_tmp.Images;
        }
    }
}
