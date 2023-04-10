using Imya;
using Imya.GithubIntegration;
using Imya.Services;
using ModManager_Devplayground;

public class Program
{
    public static async Task Main(String[] args)
    {
        GameSetupService gsm = GameSetupService.Instance;
        gsm.SetGamePath(@"F:\Spiele\Anno 1800");
    }
}