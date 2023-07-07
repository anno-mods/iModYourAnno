using Imya.Services.Interfaces;
using Pfim;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(String), typeof(ImageSource))]
    internal class FilepathToImageConverter : IValueConverter
    {
        private IGameFilesService _gameFilesService;
        static string parameterregex = @"\b[0-9]+x[0-9]+\b";

        public FilepathToImageConverter(IGameFilesService gameFilesService)
        {
            _gameFilesService = gameFilesService; 
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as String))
                return new Image();

            var stream = _gameFilesService.OpenIcon((String)value);
            if (stream is null)
                return new Image(); 

            IImage image = Pfimage.FromStream(stream);

            if (image is null)
                return new Image();

            Point size;
            bool UseMipmaps = false;
            if (parameter is string parameter_str && Regex.IsMatch(parameter_str, parameterregex))
            {
                var desired_size = parameter_str.Split("x");
                if (long.TryParse(desired_size[0], out var x) && long.TryParse(desired_size[1], out var y))
                {
                    size = new Point(x, y);
                    UseMipmaps = true;
                }
            }
            var wpfimg = UseMipmaps ? WpfImageMipmapped(image, size) : WpfImage(image);
            return wpfimg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static ImageSource WpfImage(IImage image)
        {
            var pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            var addr = pinnedArray.AddrOfPinnedObject();
            var bsource = BitmapSource.Create(image.Width, image.Height, 96.0, 96.0,
                PixelFormat(image), null, addr, image.DataLen, image.Stride);

            return bsource;
        }

        private static ImageSource WpfImageMipmapped(IImage image, Point size)
        {
            var pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
            var addr = pinnedArray.AddrOfPinnedObject();

            var mip = image.MipMaps.Where(x => x.Height >= size.X && x.Width >= size.Y).LastOrDefault();
            if (mip is null)
                return WpfImage(image);

            var mipAddr = addr + mip.DataOffset;
            var mipSource = BitmapSource.Create(mip.Width, mip.Height, 96.0, 96.0,
                PixelFormat(image), null, mipAddr, mip.DataLen, mip.Stride);

            return mipSource;
        }

        private static PixelFormat PixelFormat(IImage image)
        {
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    return PixelFormats.Bgr24;
                case ImageFormat.Rgba32:
                    return PixelFormats.Bgra32;
                case ImageFormat.Rgb8:
                    return PixelFormats.Gray8;
                case ImageFormat.R5g5b5a1:
                case ImageFormat.R5g5b5:
                    return PixelFormats.Bgr555;
                case ImageFormat.R5g6b5:
                    return PixelFormats.Bgr565;
                default:
                    throw new Exception($"Unable to convert {image.Format} to WPF PixelFormat");
            }
        }
    }
}
