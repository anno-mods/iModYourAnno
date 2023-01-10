using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{

    public interface IDownloadableUnpackable : IDownloadable, IUnpackable { }

    public interface IDownloadableInstallation : IDownloadable, IInstallation { }
    public interface IUnpackableInstallation : IUnpackable, IInstallation { }

    public interface IDownloadableUnpackableInstallation : IDownloadableInstallation, IUnpackableInstallation { }

}
