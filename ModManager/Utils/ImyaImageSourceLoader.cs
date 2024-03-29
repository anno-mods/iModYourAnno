﻿using Imya.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imya.UI.Utils
{
    internal class ImyaImageSourceLoader
    {
        internal bool TryLoadImyaImage(ImyaImageSource source, out ImageSource resultimage)
        {
            if (source is not null && source.IsValid())
            {
                return TryLoadValidImyaImage(source, out resultimage);
            }

            resultimage= null;
            return false;
        }

        private bool TryLoadValidImyaImage(ImyaImageSource source, out ImageSource resultimage)
        {
            if (source.IsBase64ImageSource())
            {
                if (TryConvertImageFromBase64(source.GetImageData(), out var image))
                {
                    resultimage = image;
                    return true;
                }
            }

            else if (source.IsFilepathImageSource())
            {
                if (TryLoadImageFromFile(source.GetImageFilepath(), out var image))
                {
                    resultimage = image;
                    return true;
                }
            }

            resultimage = null;
            return false;
        }

        #region Base64ImageLoading

        private bool TryConvertImageFromBase64(byte[] ImageBytes, out ImageSource? image)
        {
            try
            {
                image = ConvertImageFromBase64(ImageBytes);
                return true;
            }
            catch (Exception e)
            {
                image = null;
                return false;
            }
        }

        private static BitmapImage BitmapFromStream(Stream s)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = s;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private ImageSource ConvertImageFromBase64(byte[] ImageBytes)
        {
            return BitmapFromStream(new MemoryStream(ImageBytes));
        }

        #endregion

        #region ImageFromFileLoading 

        private static bool TryLoadImageFromFile(String Filepath, out ImageSource imgsrc)
        {
            try
            {
                imgsrc = LoadImageFromFile(Filepath);
                return true;
            }
            catch (Exception e)
            {
                imgsrc = null;
                return false;
            }
        }

        public static ImageSource LoadImageFromFile(String Filepath)
        {
            var uri = new Uri(Path.Combine(Environment.CurrentDirectory, Filepath));
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            return image;
        }

        #endregion
    }
}
