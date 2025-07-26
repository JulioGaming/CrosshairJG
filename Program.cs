using System;
using System.Runtime.InteropServices; 
using System.Windows.Forms;

namespace CrossHairJulio
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main()
        {
            SetProcessDPIAware(); // Evita el escalado automático de Windows

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Application.Run(new ControllerForm());
        }
    }
}
