using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models
{
    /// <summary>
    /// Represents an Image that can be either Base64 data or a Filepath. 
    /// </summary>
    public class ImyaImageSource
    {
        private enum ImyaImageSourceType { Undefined, Base64, Path }
        private ImyaImageSourceType ImageType;

        private String Filepath { get; set; }
        private byte[] Base64Data { get; set; }

        public ImyaImageSource()
        {
            ImageType = ImyaImageSourceType.Undefined;
        }

        /// <summary>
        /// Constructs this ImageSource as an Image loaded from Base64.
        /// </summary>
        /// <param name="ImageData"></param>
        public void ConstructAsBase64Image(String _image_data_base64)
        {
            //convert to raw bytes to save some space in memory.
            try
            {
                Base64Data = System.Convert.FromBase64String(_image_data_base64);
                ImageType = ImyaImageSourceType.Base64;
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Could not load image: invalid or corrupted Base64 data");
            }
        }

        /// <summary>
        /// Constructs this ImageSource as a Filepath Image.
        /// </summary>
        /// <param name="_filepath"></param>
        public void ConstructAsFilepathImage(String _filepath)
        {
            Filepath = _filepath;
            ImageType = ImyaImageSourceType.Path;
        }

        /// <summary>
        /// Returns whether the Image is a valid Image from either Base64 or Filepath.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return ImageType != ImyaImageSourceType.Undefined; 
        }

        /// <summary>
        /// Returns whether the Image is a Base64 Image.
        /// </summary>
        /// <returns></returns>
        public bool IsBase64ImageSource()
        { 
            return ImageType == ImyaImageSourceType.Base64;
        }

        /// <summary>
        /// Returns whether the Image is a Image loaded from a Filepath.
        /// </summary>
        /// <returns></returns>
        public bool IsFilepathImageSource()
        {
            return ImageType == ImyaImageSourceType.Path;
        }

        public byte[] GetImageData()
        {
            if (IsFilepathImageSource())
            {
                throw new InvalidOperationException("Tried to access byte data on a filepath image source");
            }
            return Base64Data;
        }

        public String GetImageFilepath()
        {
            if (IsBase64ImageSource())
            {
                throw new InvalidOperationException("Tried to access filepath on base64 image source");
            }
            return Filepath;
        }
    }
}
