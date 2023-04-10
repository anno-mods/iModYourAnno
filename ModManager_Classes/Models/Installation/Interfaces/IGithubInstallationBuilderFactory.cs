using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation.Interfaces
{
    public interface IGithubInstallationBuilderFactory
    {
        GithubInstallationBuilder Create();
    }
}
