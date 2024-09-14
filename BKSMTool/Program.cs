// Copyright (c) 2024 Saltwatersam
// 
// This file is part of BKSMTool.
// 
// BKSMTool is licensed under the GPLv3 License:
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using BKSMTool.CLI;
using System.Windows.Forms;
using System;
using BKSMTool.Miscellaneous;

namespace BKSMTool
{
    internal static class Program
    {
        // Main entry point for the application
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            VersionChecker.AutomaticCheckAvailableUpdate().GetAwaiter().GetResult();

            try
            {
                Application.ThreadException += GlobalExceptionHandler;
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                if (args != null && args.Length > 0)
                {
                    CommandLineInterfaceHandler.RunCli(args);
                }
                else
                {
                    // No arguments provided, default to GUI
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unmanaged error at application startup: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show(@"An unexpected error occurred. The application will close.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private static void GlobalExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.Log($"UI Thread Unhandled Exception: {e.Exception.Message}\nStackTrace: {e.Exception.StackTrace}");
            MessageBox.Show(@"An unexpected error occurred in the UI thread. The application will continue, but may be unstable.", @"UI Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Log($"Unhandled Exception: {ex.Message}\nStackTrace: {ex.StackTrace}");

            if (!e.IsTerminating) return;
            MessageBox.Show(@"A critical error occurred. The application will terminate.", @"Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
}
