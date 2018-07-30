﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using DICUI.Data;
using DICUI.Utilities;

namespace DICUI.UI
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

        public bool AutoScrollLogToEnd
        {
            get { return _options.AutoScrollLogToEnd; }
            set
            {
                _options.AutoScrollLogToEnd = value;
                _options.Save();
            }
        }

        public LogSnapMode LogWindowSnapMode
        {
            get { return _options.LogWindowSnapMode; }
            set
            {
                _options.LogWindowSnapMode = value;
                _options.Save();
            }

        }

        public List<LogSnapMode> LogWindowSnapModes
        {
            get
            {
                LogSnapMode[] modes = (LogSnapMode[])Enum.GetValues(typeof(LogSnapMode));
                return new List<LogSnapMode>(modes);
            }
        }

    }

    public class LoggerViewModel
    {
        private LogWindow _logWindow;

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

        public void LaunchProcessForLoggedOutput(Process process)
        {
            _logWindow.LaunchProcessForLoggedOutput(process);
        }

        public void ManageDICTermination()
        {
            _logWindow.GracefullyTerminateProcess();
        }

        public void VerboseLog(Brush color, string text)
        {
            if (ViewModels.OptionsViewModel.VerboseLogging)
                _logWindow.AppendToTextBox(text, color);
        }

        public void VerboseLogLn(LogType type, string format, params object[] args) => VerboseLog(type.Color(), string.Format(format, args) + "\n");

        public void VerboseLog(string format, params object[] args) => VerboseLog(Brushes.Yellow, string.Format(format, args));
        public void VerboseLogLn(string format, params object[] args) => VerboseLog(Brushes.Yellow, string.Format(format, args) + "\n");
    }

    public static class ViewModels
    {
        public static OptionsViewModel OptionsViewModel { get; set; }
        public static LoggerViewModel LoggerViewModel { get; set; } = new LoggerViewModel();
    }
}
