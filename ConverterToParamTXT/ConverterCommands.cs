using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConverterToParamTXT
{
    public class ConverterCommands
    {
        private static RoutedUICommand _clearlist;
        private static RoutedUICommand _cleartext;
        private static RoutedUICommand _clearlog;
        private static RoutedUICommand _mintotray;
        private static RoutedUICommand _addautorun;
        private static RoutedUICommand _delautorun;

        public static RoutedUICommand ClearList
        {
            get {
                return _clearlist;
            }
        }

        public static RoutedUICommand ClearText
        {
            get {
                return _cleartext;
            }
        }
        public static RoutedUICommand ClearLog
        {
            get {
                return _clearlog;
            }
        }
        public static RoutedUICommand MinToTray
        {
            get {
                return _mintotray;
            }
        }
        public static RoutedUICommand AddAutorun
        {
            get {
                return _addautorun;
            }
        }
        public static RoutedUICommand DelAutorun
        {
            get {
                return _delautorun;
            }
        }
        static ConverterCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.L, ModifierKeys.Control, "Ctrl+L"));
            inputs.Add(new KeyGesture(Key.T, ModifierKeys.Control, "Ctrl+T"));
            inputs.Add(new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G"));
            inputs.Add(new KeyGesture(Key.M, ModifierKeys.Control, "Ctrl+M"));
            inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl+A"));
            inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"));
            _clearlist = new RoutedUICommand("Clear list", "ClearList", typeof(ConverterCommands), inputs);
            _cleartext = new RoutedUICommand("Clear text field", "ClearText", typeof(ConverterCommands), inputs);
            _clearlog = new RoutedUICommand("Clear log file", "ClearLog", typeof(ConverterCommands), inputs);
            _mintotray = new RoutedUICommand("Minimize to tray", "MinToTray", typeof(ConverterCommands), inputs);
            _addautorun = new RoutedUICommand("Add to autorun", "AddAutorun", typeof(ConverterCommands), inputs);
            _delautorun = new RoutedUICommand("Delete from autorun", "DelAutorun", typeof(ConverterCommands), inputs);

        }


    }
}
