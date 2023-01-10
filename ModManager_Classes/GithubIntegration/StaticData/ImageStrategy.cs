using Imya.Utils;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Imya.GithubIntegration.StaticData
{
    public class ImageStrategy : IModImageStrategy
    {
        static string img_filename = "imya_icon.png";
        public async Task<string?> GetImageUrlAsync(GithubRepoInfo repoInfo)
        {
            var client = GithubClientProvider.Client;
            if (client.IsAuthenticated())
            {
                try
                {
                    var image_content = await client.Repository.Content.GetAllContents(repoInfo.Owner, repoInfo.Name, img_filename);
                    var image_url = image_content.FirstOrDefault()?.DownloadUrl;
                    return image_url;
                }
                catch (NotFoundException e)
                {
                    return null;
                }

            }
            return null;
        }
    }
}
