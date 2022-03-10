using Imya;
using Imya.GithubIntegration;
using Imya.Utils;
using ModManager_Devplayground;

public class Program
{
    public static void Main(String[] args)
    {
        GameSetupManager gsm = new GameSetupManager();
        gsm.RegisterGameRootPath(@"F:\Spiele\Anno 1800");

        GithubDevTester.DownloadModloader2();
    }
}