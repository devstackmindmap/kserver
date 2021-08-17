using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaThreading;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace KnightUWP.Servicecs
{
    public class ResourceManager
    {
        public static Dictionary<string, ImageSource> Images;

        public static async Task Load()
        {
            //"/ Assets / thumbs /";

            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            StorageFolder thumbs = await assets.GetFolderAsync("thumbs");
            var images = await thumbs.GetFilesAsync();

            Images = new Dictionary<string, ImageSource>();
            foreach (var image in images)
            {
                //image.DisplayName
                Images.Add(image.DisplayName, new BitmapImage(new Uri(image.Path)));
            }
        }
    }
}
