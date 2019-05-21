using System;
using System.IO;
using System.Windows.Media;
using DICUI.Utilities;
using DICUI.Windows;

namespace DICUI
{
    public class OptionsViewModel
    {
        private Options _options;

        public OptionsViewModel(Options options)
        {
            this._options = options;
        }

        public bool QuietMode
        {
            get { return _options.QuietMode; }
            set { _options.QuietMode = value; }
        }

        public bool ParanoidMode
        {
            get { return _options.ParanoidMode; }
            set { _options.ParanoidMode = value; }
        }

        public bool ScanForProtection
        {
            get { return _options.ScanForProtection; }
            set { _options.ScanForProtection = value; }
        }

        public bool SkipMediaTypeDetection
        {
            get { return _options.SkipMediaTypeDetection; }
            set { _options.SkipMediaTypeDetection = value; }
        }

        public string RereadAmountForC2
        {
            get { return Convert.ToString(_options.RereadAmountForC2); }
            set
            {
                if (Int32.TryParse(value, out int result))
                    _options.RereadAmountForC2 = result;
            }
        }

        public bool VerboseLogging
        {
            get { return _options.VerboseLogging; }
            set
            {
                _options.VerboseLogging = value;
                _options.Save();
            }
        }

        public bool OpenLogWindowAtStartup
        {
            get { return _options.OpenLogWindowAtStartup; }
            set
            {
                _options.OpenLogWindowAtStartup = value;
                _options.Save();
            }
        }

        public bool SaveLogToFile
        {
            get { return _options.SaveLogToFile; }
            set
            {
                _options.SaveLogToFile = value;
                _options.Save();
            }
        }
    }

    public class LoggerViewModel
    {
        private LogWindow _logWindow;
        private StreamWriter _logToFileStream;

        public void SetWindow(LogWindow logWindow) => _logWindow = logWindow;

        public bool WindowVisible
        {
            get => _logWindow != null ? _logWindow.IsVisible : false;
            set
            {
                if (value)
                {
                    _logWindow.AdjustPositionToMainWindow();
                    _logWindow.Show();
                }
                else
                    _logWindow.Hide();
            }
        }

        public void SessionStarted(DumpEnvironment env)
        {
            string outputFilename = Path.GetFileNameWithoutExtension(env.OutputFilename);
            string combinedBase = Path.Combine(env.OutputDirectory, outputFilename + ".log");
            String header = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            _logToFileStream = File.AppendText("log.txt");
            _logToFileStream.WriteLine("-- Session started at " + header + " --" + "\n");
        }

        public void SessionEnded()
        {
            if (ViewModels.OptionsViewModel.SaveLogToFile && _logToFileStream != null)
                _logToFileStream.WriteLine("-- Session ended --");
                _logToFileStream.Close();
        }

        public void LogToFile(string text)
        {
            if (ViewModels.OptionsViewModel.SaveLogToFile && _logToFileStream != null)
                _logToFileStream.Write(text);
        }

        public void VerboseLog(string text)
        {
            if (ViewModels.OptionsViewModel.VerboseLogging)
                _logWindow.AppendToTextBox(text, Brushes.Yellow);

            LogToFile(text);
        }

        public void VerboseLog(string format, params object[] args) => VerboseLog(string.Format(format, args));
        public void VerboseLogLn(string format, params object[] args) => VerboseLog(string.Format(format, args) + "\n");
    }

    public static class ViewModels
    {
        public static OptionsViewModel OptionsViewModel { get; set; }
        public static LoggerViewModel LoggerViewModel { get; set; } = new LoggerViewModel();
    }
}
