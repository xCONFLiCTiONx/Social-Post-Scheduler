using IWshRuntimeLibrary; // Select the "COM" tab, find and select the "Windows Script Host Object Model"
using System;
using System.IO;
using System.Reflection;

internal class CreateShortcut
{
    internal static void Create()
    {
        try
        {
            WshShell lib = new WshShell();

            IWshShortcut _startMenuShortcut;

            string _programs = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string _startup = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Startup";

            Directory.CreateDirectory(_programs + "\\" + Assembly.GetExecutingAssembly().GetName().Name);

            _startMenuShortcut = (IWshShortcut)lib.CreateShortcut(_programs + "\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
            _startMenuShortcut.TargetPath = Assembly.GetEntryAssembly().Location;
            _startMenuShortcut.WorkingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _startMenuShortcut.Description = Assembly.GetExecutingAssembly().GetName().Name;
            _startMenuShortcut.Save();

            _startMenuShortcut = (IWshShortcut)lib.CreateShortcut(_startup + "\\" + Assembly.GetExecutingAssembly().GetName().Name + ".lnk");
            _startMenuShortcut.TargetPath = Assembly.GetEntryAssembly().Location;
            _startMenuShortcut.WorkingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _startMenuShortcut.Description = Assembly.GetExecutingAssembly().GetName().Name;
            _startMenuShortcut.Save();
        }
        catch (Exception ex)
        {
            EasyLogger.Error("CreateShortcut - @Create(1): " + ex);
        }
    }
}
