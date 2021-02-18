using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SocialPostScheduler
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            EasyLogger.BackupLogs(EasyLogger.LogFile, 7);
            EasyLogger.AddListener(EasyLogger.LogFile);

            EasyLogger.Info("Initializing...");

            int tries = 60;
        TryAgain:;
            try
            {
                if (!SQLConnection.IsServerConnected(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                {
                    tries--;
                    if (tries > 0)
                    {
                        EasyLogger.Warning("SQL Server is not responding. I'll try again " + tries + " more times until I require user input...");
                        Thread.Sleep(2000);
                        goto TryAgain;
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("SQL Server is not responding. Would you like to try again?", "Social Post Scheduler", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            goto TryAgain;
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error(ex);
            }

            try
            {
                System.Reflection.Assembly assembly = typeof(Program).Assembly;
                GuidAttribute attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                string id = attribute.Value;

                Mutex mutex = new Mutex(true, id);

                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    try
                    {
                        StartApp();
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    ExitApp();
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Program - @Main(1): " + ex);
                MessageBox.Show(ex.Message, "social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void StartApp()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            try
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.DBBackupTime = Properties.Settings.Default.DBBackupTime;

                    Properties.Settings.Default.Save();

                    Properties.Settings.Default.Upgrade();
                }

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Program - @StartApp(1): " + ex);
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Scheduler());
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Program - @StartApp(2): " + ex);
                MessageBox.Show(ex.Message, "social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            EasyLogger.Error(e.Exception.Message + " Application.ThreadException");
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EasyLogger.Error(((Exception)e.ExceptionObject).Message + " AppDomain.UnhandledException");
        }

        private static void ExitApp()
        {
            Application.Exit();
        }
    }
}
