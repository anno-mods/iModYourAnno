using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Validation
{
    public interface IDlcOwnershipChanged
    {
        delegate void DlcSettingChangedEventHandler();
        event DlcSettingChangedEventHandler DlcSettingChanged;
    }
}
