namespace Cau3
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
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("vi-VN");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
            Application.Run(new Form1());
        }
    }
}
