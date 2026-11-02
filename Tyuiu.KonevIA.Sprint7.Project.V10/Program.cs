namespace Tyuiu.KonevIA.Sprint7.Project.V10;
internal static class Program_KIA
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormOrders_KIA());
    }
}
