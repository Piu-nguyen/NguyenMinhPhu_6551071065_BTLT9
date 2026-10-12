namespace Cau1
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
            var culture = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            Application.Run(new Form1());
        }
    }
}
