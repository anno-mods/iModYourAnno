using Imya.Models;
using Imya.UI.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(ImyaImageSource), typeof(ImageSource))]
    internal class ImyaImageSourceConverter : IValueConverter
    {
        public static ImageSource DefaultImage { get; } = GetDefaultImage();
        ImyaImageSourceLoader ImageLoader = new ImyaImageSourceLoader();

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            ImyaImageSource source = (ImyaImageSource)value;
            return ImageLoader.TryLoadImyaImage(source, out var image) ? image : GetDefaultImage();
        }

        private static ImageSource GetDefaultImage()
        {
            return LoadImageFromFile(Path.Combine("resources", "modbanner_placeholder.png"));
        }

        public static ImageSource LoadImageFromFile(String Filepath)
        {
            var uri = new Uri(Path.Combine(Environment.CurrentDirectory, Filepath));
            return new BitmapImage(uri);
        }

        #region Dummy_ConvertBack 

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
