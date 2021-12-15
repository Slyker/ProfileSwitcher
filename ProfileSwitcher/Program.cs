namespace ProfileSwitcher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            MessageBox.Show("This intented to be a library not a standalone project. Refer to PfTester");
            return;

        }
    }
}