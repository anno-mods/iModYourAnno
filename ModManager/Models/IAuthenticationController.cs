using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Models
{
    public interface IAuthenticationController
    {
        bool IsAuthenticated { get; }
        string? AuthenticatedUser { get; }
        Uri? AvatarUri { get; }

        void Authenticate();
        void Logout();
    }
}
