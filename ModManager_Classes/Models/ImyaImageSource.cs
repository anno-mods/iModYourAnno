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

        public String Data { get; private set; }

        public ImyaImageSource()
        {
            ImageType = ImyaImageSourceType.Undefined;
        }

        /// <summary>
        /// Constructs this ImageSource as an Image loaded from Base64.
        /// </summary>
        /// <param name="ImageData"></param>
        public void ConstructAsBase64Image(String ImageData)
        {
            Data = ImageData;
            ImageType = ImyaImageSourceType.Base64; 
        }

        /// <summary>
        /// Constructs this ImageSource as a Filepath Image.
        /// </summary>
        /// <param name="Filepath"></param>
        public void ConstructAsFilepathImage(String Filepath)
        {
            Data = Filepath;
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
    }
}
