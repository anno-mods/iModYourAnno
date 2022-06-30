using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public interface IInstallation
    {
        /// <summary>
        /// Initiates and Starts the Setup Part of the Installation: Downloading and/or File Extracting
        /// 
        /// </summary>
        /// <returns>The Setup Process as awaitable task</returns>
        public Task<IInstallation> Setup();

        /// <summary>
        /// Finalizes the Installation by moving the files where they belong.
        /// </summary>
        /// <returns></returns>
        public Task Finalize();

        public void CleanUp();
    }
}
