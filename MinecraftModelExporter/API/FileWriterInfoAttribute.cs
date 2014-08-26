using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.API
{
    public class FileWriterInfoAttribute : Attribute
    {
        private string _author;
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _outputFormat;
        public string OutputFormat
        {
            get { return _outputFormat; }
            set { _outputFormat = value; }
        }
    }
}
