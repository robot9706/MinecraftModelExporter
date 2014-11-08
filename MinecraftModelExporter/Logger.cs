using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftModelExporter
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public class Logger
    {
        private string _name;
        private string _file;

        private StreamWriter _writer;

        public Logger(string name)
        {
            _name = name;

            if (Program.Logging)
            {
                _file = Path.Combine(Application.StartupPath, _name + "-0.txt");
                if (File.Exists(_file))
                {
                    string file1 = Path.Combine(Application.StartupPath, _name + "-1.txt");
                    if (File.Exists(file1))
                    {
                        string file2 = Path.Combine(Application.StartupPath, _name + "-2.txt");
                        File.Copy(file1, file2);
                    }
                    File.Copy(_file, file1);
                }

                try
                {
                    _writer = new StreamWriter(_file, false, Encoding.ASCII);
                }
                catch
                {
                    Console.WriteLine("Failed to open log file: " + _file);
                }
            }
        }

        public void Log(LogLevel level, string message)
        {
            if (_writer != null)
            {
                _writer.WriteLine("[" + level.ToString().ToUpper() + "] " + DateTime.Now.ToString("yyyy.MM.dd. HH:mm") + " : " + message);
            }
        }

        public void Close()
        {
            if (_writer != null)
            {
                _writer.Close();
            }
        }
    }
}
