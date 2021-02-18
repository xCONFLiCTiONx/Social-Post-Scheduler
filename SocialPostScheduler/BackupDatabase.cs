using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SocialPostScheduler
{
    internal static class BackupDatabase
    {
        private static readonly string sourceBackup = AppDomain.CurrentDomain.BaseDirectory + "SocialPostScheduler.mdf", databaseBackup = AppDomain.CurrentDomain.BaseDirectory + "BACKUPS\\SocialPostScheduler.mdf", databaseOLDBackup = AppDomain.CurrentDomain.BaseDirectory + "BACKUPS\\SocialPostScheduler_" + DateTime.Now.DayOfWeek + ".mdf";

        internal static bool ErrorsHaveOccurred = false;

        internal static void ExecuteBackup()
        {
            try
            {
                EasyLogger.Info("Creating a new backup of the database...");

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "sqllocaldb",
                    Arguments = "stop MSSQLLocalDB",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                ErrorsHaveOccurred = true;
                EasyLogger.Error("BackupDatabase - @ExecuteBackup(1): " + ex);

                Form.ActiveForm.Opacity = 0;
                Form.ActiveForm.TopMost = true;
                Form.ActiveForm.Show();
                Form.ActiveForm.WindowState = FormWindowState.Normal;
                Form.ActiveForm.WindowState = FormWindowState.Minimized;
                Form.ActiveForm.Opacity = 1;
                Form.ActiveForm.WindowState = FormWindowState.Normal;
                Form.ActiveForm.BringToFront();
                Form.ActiveForm.Focus();
                Form.ActiveForm.Activate();
                Form.ActiveForm.TopMost = false;

                Scheduler.tabControlOne.SelectedIndex = 4;
            }

            try
            {
                if (File.Exists(databaseBackup))
                {
                    File.Copy(databaseBackup, databaseOLDBackup, true);
                }
                if (File.Exists(sourceBackup))
                {
                    File.Copy(sourceBackup, databaseBackup, true);
                }
                if (File.Exists(databaseBackup))
                {
                    File.SetLastWriteTime(databaseBackup, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                ErrorsHaveOccurred = true;
                EasyLogger.Error("BackupDatabase - @ExecuteBackup(2): " + ex);
            }
        }
    }
}
