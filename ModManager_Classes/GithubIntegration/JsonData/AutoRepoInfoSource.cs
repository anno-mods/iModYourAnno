
namespace Imya.GithubIntegration.JsonData
{
    public class AutoRepoInfoSource : JsonRepoInfoSource
    {
        public AutoRepoInfoSource(string fileOrUrl) : base()
        {
            if (!fileOrUrl.Contains("://"))
            {
                string? json = null;
                try
                {
                    json = File.ReadAllText(fileOrUrl);
                }
                catch { }

                if (json is not null)
                    _ = Parse(json);
            }
            else
            {
                var httpClient = new HttpClient();
                using var stream = httpClient.GetStreamAsync(fileOrUrl);
                stream.Wait();
                string json = new StreamReader(stream.Result).ReadToEnd();

                if (!string.IsNullOrWhiteSpace(json))
                    _ = Parse(json);
            }
        }
    }
}
