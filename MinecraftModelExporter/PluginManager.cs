using MinecraftModelExporter.API;
using MinecraftModelExporter.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MinecraftModelExporter
{
    class LoadedFileReader
    {
        public FileReaderInfoAttribute Info;
        public FileReader Reader;
    }

    class LoadedFileWriter
    {
        public FileWriterInfoAttribute Info;
        public FileWriter Writer;
    }

    class PluginManager
    {
        private string _folder;
        private Logger _pluginsLogger;

        private List<LoadedFileReader> _readers;
        public List<LoadedFileReader> Readers
        {
            get { return _readers; }
            set { _readers = value; }
        }

        private List<LoadedFileWriter> _writers;
        public List<LoadedFileWriter> Writers
        {
            get { return _writers; }
            set { _writers = value; }
        }

        public PluginManager(string folder)
        {
            _readers = new List<LoadedFileReader>();
            _writers = new List<LoadedFileWriter>();

            _folder = folder;

            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            _pluginsLogger = new Logger("Plugins");

            _pluginsLogger.Log(LogLevel.Info, "Finding plugins in " + _folder);
            foreach (string file in Directory.GetFiles(_folder))
            {
                if (file.EndsWith("dll"))
                {
                    _pluginsLogger.Log(LogLevel.Info, "Checking " + file);
                    try
                    {
                        Assembly ass = Assembly.LoadFile(file); //Dat ass

                        foreach (Type t in ass.GetTypes())
                        {
                            object[] attrs = t.GetCustomAttributes(typeof(FileReaderInfoAttribute), false);
                            if (t.BaseType == typeof(FileReader))
                            {
                                FileReaderInfoAttribute attr = null;
                                if (attrs.Length > 1)
                                {
                                    _pluginsLogger.Log(LogLevel.Warning, "Class \"" + t.FullName + "\" has multiple FileReaderInfo attributes, using first.");
                                    attr = (FileReaderInfoAttribute)attrs[0];
                                }
                                else if (attrs.Length == 1)
                                {
                                    attr = (FileReaderInfoAttribute)attrs[0];
                                }

                                if (attr != null)
                                {
                                    _pluginsLogger.Log(LogLevel.Info, "Loading \"" + t.FullName + "\".");

                                    LoadedFileReader reader = new LoadedFileReader();
                                    reader.Info = attr;
                                    reader.Reader = (FileReader)Activator.CreateInstance(t);

                                    _readers.Add(reader);
                                }
                            }
                            else if (t.BaseType == typeof(FileWriter))
                            {
                                FileWriterInfoAttribute attr = null;
                                if (attrs.Length > 1)
                                {
                                    _pluginsLogger.Log(LogLevel.Warning, "Class \"" + t.FullName + "\" has multiple FileWriterInfo attributes, using first.");
                                    attr = (FileWriterInfoAttribute)attrs[0];
                                }
                                else if (attrs.Length == 1)
                                {
                                    attr = (FileWriterInfoAttribute)attrs[0];
                                }

                                if (attr != null)
                                {
                                    _pluginsLogger.Log(LogLevel.Info, "Loading \"" + t.FullName + "\".");

                                    LoadedFileWriter writer = new LoadedFileWriter();
                                    writer.Info = attr;
                                    writer.Writer = (FileWriter)Activator.CreateInstance(t);

                                    _writers.Add(writer);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _pluginsLogger.Log(LogLevel.Error, "Failed to load " + file + ": " + ex.Message);
                    }
                }
            }

            _pluginsLogger.Log(LogLevel.Info, "Loaded " + _readers.Count.ToString() + " file readers.");
        }

        public void AddBuiltinFileReader(FileReader reader)
        {
            LoadedFileReader inf = new LoadedFileReader();
            inf.Reader = reader;
            inf.Info = (FileReaderInfoAttribute)reader.GetType().GetCustomAttributes(typeof(FileReaderInfoAttribute), false)[0];
            _readers.Add(inf);
        }

        public void AddBuiltinFileWriter(FileWriter writer)
        {
            LoadedFileWriter inf = new LoadedFileWriter();
            inf.Writer = writer;
            inf.Info = (FileWriterInfoAttribute)writer.GetType().GetCustomAttributes(typeof(FileWriterInfoAttribute), false)[0];
            _writers.Add(inf);
        }

        public void CloseLogger()
        {
            _pluginsLogger.Close();
        }
    }
}
