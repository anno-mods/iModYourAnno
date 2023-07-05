using Imya.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Imya.Services.Interfaces
{
    public interface IGameFilesService
    {
        Task LoadAsync(); 
        Stream? OpenFile(String filepath);
        Stream? OpenIcon(String iconPath);

    }
}
