namespace demopasckage
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = Tao_Xuat_Report_Chap9.AppConfig.Vi;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = Tao_Xuat_Report_Chap9.AppConfig.Vi;
            Application.Run(new FormMain());
        }
    }
}
