using Imya;
using Imya.GithubIntegration;
using Imya.Utils;
using ModManager_Devplayground;

public class Program
{
    public static async Task Main(String[] args)
    {
        GameSetupManager gsm = GameSetupManager.Instance;
        gsm.SetGamePath(@"F:\Spiele\Anno 1800");
    }
}