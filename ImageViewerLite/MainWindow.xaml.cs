using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace ImageViewerLite
{
    public partial class MainWindow : Window, IDisposable
    {
        #region Variables

        private FrameworkElement element;
        private Point origin;
        private Point start;
        public static string[] Args = Environment.GetCommandLineArgs();
        public string CurrentImage = null;
        private BitmapSource bitmapSource;
        private FileStream mediaStream;
        private Bitmap bitmap;

        #endregion Variables

        #region Main Entry

        public MainWindow()
        {
            InitializeComponent();

            EventSubs(true);

            if (Args.Length > 1)
            {
                CurrentImage = Args[1];

                OpenImage();
            }
            else
            {
                MessageBox.Show("This is a lite version of ImageViewer. It is only meant to be used with the application associated with it.", "ImageViewer", MessageBoxButton.OK, MessageBoxImage.Warning);

                Environment.Exit(0);
            }
        }

        #endregion Main Entry

        #region Image Control

        public void OpenImage()
        {
            using (mediaStream = new FileStream(CurrentImage, FileMode.Open, FileAccess.ReadWrite))
            {
                bitmap = new Bitmap(mediaStream);

                bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                ImagePlayer.Source = bitmapSource;

                bitmap.Dispose();

                ImagePlayer.Width = ImagePlayer.Source.Width;
                ImagePlayer.Height = ImagePlayer.Source.Height;

                element = ImagePlayer;
            }
        }

        #endregion Image Control

        #region Events

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            element.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void EventSubs(bool Startup)
        {
            if (Startup)
            {
                mainWindow.MouseWheel += MainWindow_MouseWheel;
                mainWindow.MouseRightButtonDown += delegate { ResetZoom(); };

                ZoomResetButton.Click += delegate { ResetZoom(); };

                // ImagePlayer
                ImagePlayer.MouseRightButtonDown += delegate { ResetZoom(); };
                ImagePlayer.MouseLeftButtonDown += ImagePlayer_MouseLeftButtonDown;
                ImagePlayer.MouseLeftButtonUp += ImagePlayer_MouseLeftButtonUp;
                ImagePlayer.MouseMove += ImagePlayer_MouseMove;

                ImagePlayer.MaxHeight = SystemParameters.PrimaryScreenHeight * 85 / 100;
                ImagePlayer.MaxWidth = SystemParameters.PrimaryScreenWidth * 85 / 100;
            }
            else
            {
                try
                {
                    mainWindow.MouseWheel -= MainWindow_MouseWheel;

                    // ImagePlayer
                    ImagePlayer.MouseLeftButtonDown -= ImagePlayer_MouseLeftButtonDown;
                    ImagePlayer.MouseLeftButtonUp -= ImagePlayer_MouseLeftButtonUp;
                    ImagePlayer.MouseMove -= ImagePlayer_MouseMove;
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            EventSubs(false);
            Opacity = 0;
            Hide();
            ShowInTaskbar = false;
            Application.Current.Shutdown();
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bitmap img = (Bitmap)Bitmap.FromFile(CurrentImage);

                img.RotateFlip(RotateFlipType.Rotate90FlipNone);

                if (Path.GetExtension(CurrentImage).ToLower() == ".jpg")
                {
                    img.Save(CurrentImage, ImageFormat.Jpeg);
                }
                else if (Path.GetExtension(CurrentImage).ToLower() == ".png")
                {
                    img.Save(CurrentImage, ImageFormat.Png);
                }
                else if (Path.GetExtension(CurrentImage).ToLower() == ".gif")
                {
                    MessageBox.Show("This program cannot save gif files. If you want to rotate this image and save it, use a program that is capable of properly saving gif files.", "ImageViewer", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (Path.GetExtension(CurrentImage).ToLower() == ".bmp")
                {
                    img.Save(CurrentImage, ImageFormat.Bmp);
                }
                else if (Path.GetExtension(CurrentImage).ToLower() == ".ico")
                {
                    MessageBox.Show("This program cannot save icon files. If you want to rotate this image and save it, use a program that is capable of properly saving icon files.", "ImageViewer", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (Path.GetExtension(CurrentImage).ToLower() == ".tiff")
                {
                    img.Save(CurrentImage, ImageFormat.Tiff);
                }

                OpenImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Zoom
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn(true);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut(true);
        }

        private void ZoomIn(bool isButton)
        {
            if (element.TransformToAncestor(border)
                  .TransformBounds(new Rect(element.RenderSize)).Width >= 20000)
            {
                return;
            }

            Matrix m = element.RenderTransform.Value;

            if (isButton)
            {
                m.Scale(1.5, 1.5);
            }
            else
            {
                m.Scale(1.1, 1.1);
            }

            element.RenderTransform = new MatrixTransform(m);
        }

        private void ZoomOut(bool isButton)
        {
            if (element.TransformToAncestor(border)
          .TransformBounds(new Rect(element.RenderSize)).Width <= 200)
            {
                return;
            }

            Matrix m = element.RenderTransform.Value;

            if (isButton)
            {
                m.Scale(1 / 1.5, 1 / 1.5);
            }
            else
            {
                m.Scale(1 / 1.1, 1 / 1.1);
            }

            element.RenderTransform = new MatrixTransform(m);
        }

        private void ResetZoom()
        {
            Matrix m = ImagePlayer.RenderTransform.Value;
            m.SetIdentity();
            ImagePlayer.RenderTransform = new MatrixTransform(m);
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Matrix m = element.RenderTransform.Value;

            if (e.Delta > 0)
            {
                if (element.TransformToAncestor(border)
               .TransformBounds(new Rect(element.RenderSize)).Width >= 20000)
                {
                    return;
                }

                m.Scale(1.1, 1.1);
            }
            else
            {
                if (element.TransformToAncestor(border)
       .TransformBounds(new Rect(element.RenderSize)).Width <= 200)
                {
                    return;
                }

                m.Scale(1 / 1.1, 1 / 1.1);
            }

            element.RenderTransform = new MatrixTransform(m);
        }

        private void MouseLeftButtonDownElements(MouseButtonEventArgs e)
        {
            if (element.IsMouseCaptured)
            {
                return;
            }

            element.CaptureMouse();

            start = e.GetPosition(border);
            origin.X = element.RenderTransform.Value.OffsetX;
            origin.Y = element.RenderTransform.Value.OffsetY;
        }

        private void ImagePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownElements(e);
        }

        private void MouseMoveElements(MouseEventArgs e)
        {
            if (!element.IsMouseCaptured)
            {
                return;
            }

            Point p = e.MouseDevice.GetPosition(border);

            Matrix m = element.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            element.RenderTransform = new MatrixTransform(m);
        }

        private void ImagePlayer_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveElements(e);
        }

        private void ImagePlayer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            element.ReleaseMouseCapture();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            EventSubs(false);

            Environment.Exit(0);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)mediaStream).Dispose();
                    bitmap.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion



        #endregion Events
    }
}