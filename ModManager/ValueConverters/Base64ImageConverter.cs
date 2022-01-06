using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(String), typeof(ImageSource))]
    internal class Base64ImageConverter : IValueConverter
    {
        public static ImageSource DefaultImage { get; } = GetDefaultImage();

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not null)
            {
                String Base64 = (String)value;
                return TryConvertImageFromBase64(Base64, out ImageSource? Image) ? Image : DefaultImage;
            }
            return DefaultImage;
        }

        private static ImageSource GetDefaultImage()
        {
            using (var Stream = File.OpenRead(Path.Combine("resources", "modbanner_placeholder.png")))
            {
                return BitmapFromStream(Stream);
            }
        }

        private bool TryConvertImageFromBase64(String Base64, out ImageSource? image)
        {
            try
            {
                image = ConvertImageFromBase64(Base64);
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

        private ImageSource ConvertImageFromBase64(String Base64)
        { 
            var bytes = System.Convert.FromBase64String(Base64);
            return BitmapFromStream(new MemoryStream(bytes));
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}
