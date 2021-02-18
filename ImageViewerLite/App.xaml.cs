using System;
using System.Windows;

namespace ImageViewerLite
{
    public partial class App : Application
    {
        public static bool ForceSetAssociation = false;

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
