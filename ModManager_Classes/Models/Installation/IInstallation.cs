using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public interface IInstallation : IDownloadable, IUnpackable
    {
        IText? HeaderText { get; }
        IText? AdditionalText { get; }

        String ID { get; }
    }
}
