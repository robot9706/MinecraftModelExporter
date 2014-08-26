using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MinecraftModelExporter
{
    static class Program
    {
        public static bool Logging = false;
        
        [STAThread]
        static void Main(string[] args)
        {
            foreach (string arg in args)
                if (arg.ToLower() == "log")
                    Logging = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
