using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace SocialPostScheduler
{
    public partial class Scheduler : Form
    {
        #region Variables

        internal static TabControl tabControlOne = new TabControl();
        private string lastline = string.Empty;
        private static BackgroundWorker worker;
        private static Timer post;
        private static readonly int PostTimeout = 1000;
        private static NotifyIcon notifyIcon;
        private static DataGridView dataGridView;
        private static int gridviewColIndex;
        private static int gridviewRowIndex;

        // Context Menu platformMenu
        private static ContextMenu platformMenu;
        private static MenuItem platforMenuItem;

        // Notifcation Icon
        private static readonly ContextMenu trayMenu = new ContextMenu();
        private static readonly MenuItem menuItemClearLog = new MenuItem("Clear Log"), menuItemSettings = new MenuItem("Settings"), menuItemScheduler = new MenuItem("Scheduler"), menuItemClose = new MenuItem("Close");

        // ImageContextMenu
        private static readonly ContextMenu ImageContextMenu = new ContextMenu();
        private static readonly MenuItem menuItemAddImage = new MenuItem("Add"), menuItemUpdateImage = new MenuItem("Update"), menuItemRemoveImage = new MenuItem("Remove"), menuItemViewImage = new MenuItem("View"), menuItemSaveImage = new MenuItem("Save");

        #endregion Variables

        #region Entry Point

        public Scheduler()
        {
            try
            {
                InitializeComponent();
                ShowInTaskbar = false;
#if !DEBUG
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
                }

                if (Properties.Settings.Default.FirstRun)
                {
                    StartPosition = FormStartPosition.CenterScreen;

                    Properties.Settings.Default.DBBackupTime = Properties.Settings.Default.DBBackupTime;
                    Properties.Settings.Default.InstagramPostTime = Properties.Settings.Default.InstagramPostTime;
                    Properties.Settings.Default.PostExpireTime = Properties.Settings.Default.PostExpireTime;
                    Properties.Settings.Default.PublishDate = Properties.Settings.Default.PublishDate;
                    Properties.Settings.Default.RenewUserToken = Properties.Settings.Default.RenewUserToken;

                    Process.Start("https://github.com/xCONFLiCTiONx/Social-Post-Scheduler#social-post-scheduler");
                    ShowInTaskbar = true;
                    WindowState = FormWindowState.Normal;

                    CreateShortcut.Create();

                    Properties.Settings.Default.FirstRun = false;

                    DateTime now = DateTime.Now;

                    DateTime result = Convert.ToDateTime(Properties.Settings.Default.DBBackupTime);
                    DateTime backupTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);
                    Properties.Settings.Default.DBBackupTime = backupTime.ToString("h:mm:ss tt");

                    result = Convert.ToDateTime(Properties.Settings.Default.PostExpireTime);
                    DateTime expireTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);
                    Properties.Settings.Default.PostExpireTime = expireTime.ToString("h:mm:ss tt");

                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();
                }
                else
                {
                    Location = Properties.Settings.Default.windowLocation;
                    Size = Properties.Settings.Default.windowSize;

                    WindowState = FormWindowState.Minimized;
                    Hide();
                }
#endif
                Directory.CreateDirectory(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "BACKUPS\\"));
                FormClosing += Settings_FormClosing;
                Load += Scheduler_Load;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @Scheduler(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private static readonly int WM_QUERYENDSESSION = 0x11;

        private static bool systemShutdown = false;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                systemShutdown = true;
            }

            base.WndProc(ref m);
        }

        private void Worker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            while (!IsHandleCreated)
            {
                Thread.Sleep(2000);
            }

            // Background Post Timer
            post = new Timer(PostTimeout);
            post.Elapsed += PollUpdates;
            post.Start();
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= DoWork;
            worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;
        }

        #endregion Entry Point

        #region Scheduler
        private void Scheduler_Load(object sender, EventArgs e)
        {
            try
            {
#if !DEBUG
                Hide();
#endif

                VisibleChanged += Scheduler_VisibleChanged;

                try
                {
                    facebookTableAdapter.Fill(socialPostSchedulerDataSet.Facebook);
                    twitterTableAdapter.Fill(socialPostSchedulerDataSet.Twitter);
                    saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);
                    fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);
                    thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);
                    wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);
                    tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);
                    mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);
                    sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                    textEditorBox.TextChanged += TextEditorBox_TextChanged;

                    dataGridView1.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView2.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView3.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView4.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView5.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView6.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView7.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView8.UserDeletingRow += DataGridView_UserDeletingRow;
                    dataGridView9.UserDeletingRow += DataGridView_UserDeletingRow;

                    dataGridView3.CellMouseClick += DataGridView3_CellMouseClick;
                    dataGridView4.CellMouseClick += DataGridView4_CellMouseClick;
                    dataGridView5.CellMouseClick += DataGridView5_CellMouseClick;
                    dataGridView6.CellMouseClick += DataGridView6_CellMouseClick;
                    dataGridView7.CellMouseClick += DataGridView7_CellMouseClick;
                    dataGridView8.CellMouseClick += DataGridView8_CellMouseClick;
                    dataGridView9.CellMouseClick += DataGridView9_CellMouseClick;

                    dataGridView3.CellContentClick += DataGridView3_CellContentClick;
                    dataGridView4.CellContentClick += DataGridView4_CellContentClick;
                    dataGridView5.CellContentClick += DataGridView5_CellContentClick;
                    dataGridView6.CellContentClick += DataGridView6_CellContentClick;
                    dataGridView7.CellContentClick += DataGridView7_CellContentClick;
                    dataGridView8.CellContentClick += DataGridView8_CellContentClick;
                    dataGridView9.CellContentClick += DataGridView9_CellContentClick;

                    dataGridView3.DataError += DataGridView_DataError;
                    dataGridView4.DataError += DataGridView_DataError;
                    dataGridView5.DataError += DataGridView_DataError;
                    dataGridView6.DataError += DataGridView_DataError;
                    dataGridView7.DataError += DataGridView_DataError;
                    dataGridView8.DataError += DataGridView_DataError;
                    dataGridView9.DataError += DataGridView_DataError;

                    dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
                    dataGridView2.CellValueChanged += DataGridView2_CellValueChanged;
                    dataGridView3.CellValueChanged += DataGridView3_CellValueChanged;
                    dataGridView4.CellValueChanged += DataGridView4_CellValueChanged;
                    dataGridView5.CellValueChanged += DataGridView5_CellValueChanged;
                    dataGridView6.CellValueChanged += DataGridView6_CellValueChanged;
                    dataGridView7.CellValueChanged += DataGridView7_CellValueChanged;
                    dataGridView8.CellValueChanged += DataGridView8_CellValueChanged;
                    dataGridView9.CellValueChanged += DataGridView9_CellValueChanged;

                    dataGridView1.KeyUp += DataGridView1_KeyUp;
                    dataGridView2.KeyUp += DataGridView2_KeyUp;
                    dataGridView3.KeyUp += DataGridView3_KeyUp;
                    dataGridView4.KeyUp += DataGridView4_KeyUp;
                    dataGridView5.KeyUp += DataGridView5_KeyUp;
                    dataGridView6.KeyUp += DataGridView6_KeyUp;
                    dataGridView7.KeyUp += DataGridView7_KeyUp;
                    dataGridView8.KeyUp += DataGridView8_KeyUp;
                    dataGridView9.KeyUp += DataGridView9_KeyUp;
                }
                catch (Exception ex)
                {
                    EasyLogger.Error("Scheduler - @Scheduler_HandleCreated(1): " + ex);
                    if (!Visible)
                    {
                        ShowWindow();
                    }
                    tabControlOne.SelectedIndex = 4;

                    try
                    {
                        Hide();
                        notifyIcon.Visible = false;
                    }
                    catch
                    {
                        // ignore
                    }

                    Environment.Exit(0);
                }

                try
                {
                    dataGridView3.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView4.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView5.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView6.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView7.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView8.Columns[6].DefaultCellStyle.Format = "h:mm tt";
                    dataGridView9.Columns[6].DefaultCellStyle.Format = "h:mm tt";

                    dataGridView3.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView4.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView5.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView6.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView7.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView8.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridView9.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
                catch (Exception ex)
                {
                    EasyLogger.Error("Scheduler - @Scheduler_HandleCreated(2): " + ex);
                    if (!Visible)
                    {
                        ShowWindow();
                    }
                    tabControlOne.SelectedIndex = 4;
                }

                // Notifcation Icon
                notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    Icon = Properties.Resources.ICON,
                    Text = "Social Post Scheduler - " + Assembly.GetEntryAssembly().GetName().Version
                };
                notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
                menuItemClearLog.Click += MenuItemClearLog_Click;
                menuItemSettings.Click += MenuItemSettings_Click;
                menuItemScheduler.Click += MenuItemScheduler_Click;
                menuItemClose.Click += MenuItemClose_Click;

                trayMenu.MenuItems.Add(menuItemClearLog);
                trayMenu.MenuItems.Add(menuItemSettings);
                trayMenu.MenuItems.Add(menuItemScheduler);
                trayMenu.MenuItems.Add(menuItemClose);
                notifyIcon.ContextMenu = trayMenu;

                // Image Contect Menu
                menuItemAddImage.Click += MenuItemAddImage_Click;
                menuItemUpdateImage.Click += MenuItemUpdateImage_Click;
                menuItemRemoveImage.Click += MenuItemRemoveImage_Click;
                menuItemViewImage.Click += MenuItemView_Click;
                menuItemSaveImage.Click += MenuItemSave_Click;

                ImageContextMenu.MenuItems.Add(menuItemAddImage);
                ImageContextMenu.MenuItems.Add(menuItemUpdateImage);
                ImageContextMenu.MenuItems.Add(menuItemRemoveImage);
                ImageContextMenu.MenuItems.Add(menuItemViewImage);
                ImageContextMenu.MenuItems.Add(menuItemSaveImage);

                Worker();
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @Scheduler_Load(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void DataGridView9_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView8_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView7_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView6_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView5_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView4_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView3_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView2_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridView9_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView8_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView7_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;
                if (e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                {
                    dgv.EndEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int retries = 0;
        private void MenuItemClearLog_Click(object sender, EventArgs e)
        {
            try
            {
                EasyLogger.RemoveListener();
            }
            catch
            {
            }
        Retry:;
            try
            {
                retries++;
                EasyLogger.RemoveListener();
                EasyLogger.BackupLogs(EasyLogger.LogFile);
                EasyLogger.AddListener(EasyLogger.LogFile);
                EasyLogger.Info("Initializing...");
            }
            catch (Exception ex)
            {
                if (retries < 5)
                {
                    Thread.Sleep(1000);
                    goto Retry;
                }
                else
                {
                    MessageBox.Show(ex.Message, "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            retries = 0;
        }

        private void Scheduler_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (Visible)
                {
                    if (File.Exists(EasyLogger.LogFile))
                    {
                        Thread logWatcher = new Thread(LogWatcher)
                        {
                            IsBackground = true
                        };
                        logWatcher.Start();

                        menuItemClearLog.Enabled = false;
                    }
                }
                else
                {
                    menuItemClearLog.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @Scheduler_VisibleChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void LogWatcher()
        {
            try
            {
                using (FileStream fs = new FileStream(EasyLogger.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        while (Visible)
                        {
                            Invoke(new Action(() =>
                            {
                                try
                                {
                                    try
                                    {
                                        textBox1.Text += reader.ReadToEnd();

                                        if (textBox1.Text != lastline)
                                        {
                                            textBox1.SelectionStart = textBox1.Text.Length;
                                            textBox1.ScrollToCaret();
                                            textBox1.Refresh();
                                        }
                                        lastline = textBox1.Text;
                                    }
                                    catch (Exception ex)
                                    {
                                        EasyLogger.Info(ex.Message);
                                    }
                                    textBox1.Text += reader.ReadToEnd();
                                }
                                catch (Exception)
                                {
                                    if (!File.Exists(EasyLogger.LogFile))
                                    {
                                        try
                                        {
                                            EasyLogger.RemoveListener();
                                        }
                                        catch
                                        {
                                        }
                                    Retry:;
                                        try
                                        {
                                            retries++;
                                            EasyLogger.RemoveListener();
                                            EasyLogger.BackupLogs(EasyLogger.LogFile);
                                            EasyLogger.AddListener(EasyLogger.LogFile);
                                            EasyLogger.Info("BEGIN LOGGING ==>");
                                        }
                                        catch (Exception ex)
                                        {
                                            if (retries < 5)
                                            {
                                                Thread.Sleep(1000);
                                                goto Retry;
                                            }
                                            else
                                            {
                                                MessageBox.Show(ex.Message, "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        retries = 0;
                                    }
                                }
                            }));

                            Thread.Sleep(1000);
                        }
                    }
                    Invoke(new Action(() =>
                    {
                        textBox1.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Thread.Sleep(2000);

                LogWatcher();
            }
        }

        private void PollExpires()
        {
            try
            {
                while (true)
                {
                    bool PostExpired = false;

                    Invoke(new Action(() =>
                    {
                        byte[] image = null;

                        DateTime now = DateTime.Now;
                        DateTime? time = null;
                        DateTime? expire = null;

                        DataRow rowToDeleteSunday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Sunday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteSunday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteSunday != null)
                        {
                            if (rowToDeleteSunday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                sundayTableAdapter.DeleteQuery(rowToDeleteSunday.Field<int>("id"));

                                sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                                dataGridView3.Refresh();
                            }
                        }

                        DataRow rowToDeleteMonday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Monday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteMonday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteMonday != null)
                        {
                            if (rowToDeleteMonday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                mondayTableAdapter.DeleteQuery(rowToDeleteMonday.Field<int>("id"));

                                mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);

                                dataGridView4.Refresh();
                            }
                        }

                        DataRow rowToDeleteTuesday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Tuesday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteTuesday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteTuesday != null)
                        {
                            if (rowToDeleteTuesday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                tuesdayTableAdapter.DeleteQuery(rowToDeleteTuesday.Field<int>("id"));

                                tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);

                                dataGridView5.Refresh();
                            }
                        }

                        DataRow rowToDeleteWednesday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Wednesday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteWednesday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteWednesday != null)
                        {
                            if (rowToDeleteWednesday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                wednesdayTableAdapter.DeleteQuery(rowToDeleteWednesday.Field<int>("id"));

                                wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);

                                dataGridView6.Refresh();
                            }
                        }

                        DataRow rowToDeleteThursday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Thursday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteThursday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteThursday != null)
                        {
                            if (rowToDeleteThursday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                thursdayTableAdapter.DeleteQuery(rowToDeleteThursday.Field<int>("id"));

                                thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);

                                dataGridView7.Refresh();
                            }
                        }

                        DataRow rowToDeleteFriday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Friday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteFriday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteFriday != null)
                        {
                            if (rowToDeleteFriday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                fridayTableAdapter.DeleteQuery(rowToDeleteFriday.Field<int>("id"));

                                fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);

                                dataGridView8.Refresh();
                            }
                        }

                        DataRow rowToDeleteSaturday = null;
                        foreach (DataRow row in socialPostSchedulerDataSet.Saturday)
                        {
                            if (row != null)
                            {
                                if (row.RowState != DataRowState.Deleted)
                                {
                                    expire = row.Field<DateTime?>("Expire");

                                    if (row.Field<byte[]>("Image") != null)
                                    {
                                        image = row.Field<byte[]>("Image");
                                    }

                                    if (!DBNull.Value.Equals(row.Field<DateTime?>("Time")))
                                    {
                                        time = row.Field<DateTime?>("Expire");
                                    }
                                }
                            }

                            if (expire != null)
                            {
                                if (now >= expire)
                                {
                                    rowToDeleteSaturday = row;
                                    break;
                                }
                            }
                        }
                        if (rowToDeleteSaturday != null)
                        {
                            if (rowToDeleteSaturday.RowState != DataRowState.Deleted)
                            {
                                PostExpired = true;

                                saturdayTableAdapter.DeleteQuery(rowToDeleteSaturday.Field<int>("id"));

                                saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);

                                dataGridView9.Refresh();
                            }
                        }
                    }));

                    if (!PostExpired)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PollExpires(1): " + ex);
            }
        }

        private void TextEditorBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textEditorBox.Text.Length > 0)
                {
                    tabControl2.TabPages[7].Text = "Text Editor (" + textEditorBox.Text.Length + "/280)";
                }
                else
                {
                    tabControl2.TabPages[7].Text = "Text Editor";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion Scheduler

        #region Post

        private static bool GetBooleanValue(DataGridViewRow DGVR, int week)
        {
            bool value = false;
            try
            {
                if (week == 1)
                {
                    if (DGVR.Cells[7].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[7].Value);
                    }
                }
                if (week == 2)
                {
                    if (DGVR.Cells[8].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[8].Value);
                    }
                }
                if (week == 3)
                {
                    if (DGVR.Cells[9].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[9].Value);
                    }
                }
                if (week == 4)
                {
                    if (DGVR.Cells[10].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[10].Value);
                    }
                }
                if (week == 5)
                {
                    if (DGVR.Cells[11].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[11].Value);
                    }
                }
                if (week == 6)
                {
                    if (DGVR.Cells[12].Value != DBNull.Value)
                    {
                        value = Convert.ToBoolean(DGVR.Cells[12].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return value;
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            try
            {
                while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                {
                    date = date.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        private async void PollUpdates(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    Text = "Social Post Scheduler - " + Assembly.GetEntryAssembly().GetName().Version + "  |  " + DateTime.Now.ToString("h:mm:ss tt");
                }));
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PollUpdates(1): " + ex);
                if (!Visible)
                {
                    Invoke(new Action(() =>
                    {
                        ShowWindow();
                    }));
                }
                Invoke(new Action(() =>
                {
                    tabControl1.SelectTab(tabPage11);
                }));
            }

            DateTime now = DateTime.Now;
            DateTime? time = null;

            try
            {
                if (DateTime.Now.ToString("h:mm:ss tt") == Properties.Settings.Default.PostExpireTime)
                {
                    EasyLogger.Info("Cleaning up expired posts...");

                    PollExpires();
                }

                if (DateTime.Now.ToString("h:mm:ss tt") == Properties.Settings.Default.DBBackupTime)
                {
                    await Task.Run(BackupDatabase.ExecuteBackup);
                }

                if (DateTime.Now.ToString("h:mm:ss tt") == Properties.Settings.Default.InstagramPostTime)
                {
                    if (Properties.Settings.Default.EnableInstagram)
                    {
                        await Task.Run(PostFromFeed.PostToInstagram);
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PollUpdates(2): " + ex);
                if (!Visible)
                {
                    Invoke(new Action(() =>
                    {
                        ShowWindow();
                    }));
                }
                Invoke(new Action(() =>
                {
                    tabControl1.SelectTab(tabPage11);
                }));
            }
            try
            {
                if (DateTime.Now.ToString("h:mm:ss tt") == Properties.Settings.Default.RenewUserToken)
                {
                    string _CURRENT_USER_TOKEN = null;
                    string _APP_SECRET = null;
                    string _APP_ID = null;

                    DataGridViewRow dataGridViewRow = null;

                    int error = 0;
                    Invoke(new Action(() =>
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[1].Value != DBNull.Value && row.Cells[1].Value != null)
                            {
                                dataGridViewRow = row;
                                _CURRENT_USER_TOKEN = row.Cells[3].Value.ToString();
                                _APP_SECRET = row.Cells[4].Value.ToString();
                                _APP_ID = row.Cells[5].Value.ToString();

                                string token = GetToken.RenewAccessToken(_CURRENT_USER_TOKEN, _APP_SECRET, _APP_ID);
                                if (token.Contains("ERROR:"))
                                {
                                    error++;
                                    EasyLogger.Error(token);
                                }
                                else
                                {
                                    facebookTableAdapter.UpdateQuery(dataGridViewRow.Cells[1].Value.ToString(), dataGridViewRow.Cells[2].Value.ToString(), token, dataGridViewRow.Cells[4].Value.ToString(), dataGridViewRow.Cells[5].Value.ToString(), dataGridViewRow.Cells[6].Value.ToString(), (int)dataGridViewRow.Cells[0].Value);
                                    facebookTableAdapter.Fill(socialPostSchedulerDataSet.Facebook);
                                }
                            }
                        }
                    }));
                    if (error == 0)
                    {
                        EasyLogger.Info("Your Facebook user token has been renewed successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PollUpdates(3): " + ex);
                if (!Visible)
                {
                    Invoke(new Action(() =>
                    {
                        ShowWindow();
                    }));
                }
                Invoke(new Action(() =>
                {
                    tabControl1.SelectTab(tabPage11);
                }));
            }
            try
            {
                Invoke(new Action(() =>
                {
                    if (now.DayOfWeek.ToString() == "Sunday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (now.DayOfWeek.ToString() == "Monday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView4.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (now.DayOfWeek.ToString() == "Tuesday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView5.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }
                                            if (!DBNull.Value.Equals(row.Cells[1].Value) && !DBNull.Value.Equals(row.Cells[2].Value) && !DBNull.Value.Equals(row.Cells[3].Value))
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (now.DayOfWeek.ToString() == "Wednesday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView6.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (now.DayOfWeek.ToString() == "Thursday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView7.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (now.DayOfWeek.ToString() == "Friday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView8.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (now.DayOfWeek.ToString() == "Saturday")
                    {
                        // Poll Posts
                        foreach (DataGridViewRow row in dataGridView9.Rows)
                        {
                            if (row.Cells[6].Value != DBNull.Value)
                            {
                                time = null;
                                if (!DBNull.Value.Equals(row.Cells[6].Value) && row.Cells[6].Value != null)
                                {
                                    time = (DateTime)row.Cells[6].Value;
                                }

                                if (time != null)
                                {
                                    DateTime postTime = new DateTime(now.Year, now.Month, now.Day, time.Value.Hour, time.Value.Minute, 0);

                                    if (GetBooleanValue(row, GetWeekNumberOfMonth(postTime)))
                                    {
                                        if (now.ToString() == postTime.ToString())
                                        {
                                            string imagePath = null;
                                            if (!DBNull.Value.Equals(row.Cells[4].Value) && row.Cells[4].Value != null)
                                            {
                                                Image x = (Bitmap)((new ImageConverter()).ConvertFrom(row.Cells[4].Value));
                                                Guid id = Guid.NewGuid();
                                                imagePath = Path.GetTempPath() + id + ".jpg";
                                                x.Save(imagePath, ImageFormat.Jpeg);
                                                while (!File.Exists(imagePath))
                                                {
                                                    Thread.Sleep(10);
                                                }
                                            }

                                            if (row.Cells[1].Value != DBNull.Value && row.Cells[2].Value != DBNull.Value && row.Cells[3].Value != DBNull.Value)
                                            {
                                                bool isImage = false;
                                                if (row.Cells[4].Value != DBNull.Value)
                                                {
                                                    isImage = true;
                                                }

                                                Post(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString(), isImage, imagePath, row.Cells[5].Value.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PollUpdates(4): " + ex);
                if (!Visible)
                {
                    Invoke(new Action(() =>
                    {
                        ShowWindow();
                    }));
                }
                Invoke(new Action(() =>
                {
                    tabControl1.SelectTab(tabPage11);
                }));
            }
        }

        private async void Post(string platform, string PAGE_NAME, string MESSAGE_STRING, bool isImage, string IMAGE_PATH, string LINK_URL)
        {
            try
            {
                if (!InternetConnection.CheckConnection())
                {
                    EasyLogger.Warning("You are disconnected from the internet! This scheduled post will be missed...");
                    return;
                }

                if (platform == "Facebook")
                {
                    string _PAGE_ID = null;
                    string _CURRENT_USER_TOKEN = null;
                    string _APP_SECRET = null;
                    string _APP_ID = null;
                    string _USER_ID = null;

                    DataGridViewRow dataGridViewRow = null;

                    Invoke(new Action(() =>
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[1].Value != DBNull.Value && row.Cells[1].Value != null)
                            {
                                if (row.Cells[1].Value.ToString() == PAGE_NAME)
                                {
                                    dataGridViewRow = row;
                                    _PAGE_ID = row.Cells[2].Value.ToString();
                                    _CURRENT_USER_TOKEN = row.Cells[3].Value.ToString();
                                    _APP_SECRET = row.Cells[4].Value.ToString();
                                    _APP_ID = row.Cells[5].Value.ToString();
                                    _USER_ID = row.Cells[6].Value.ToString();
                                }
                            }
                        }
                    }));

                    _ = await Task.Run(() => GetToken.PostMessage(PAGE_NAME, _USER_ID, _PAGE_ID, _CURRENT_USER_TOKEN, isImage, MESSAGE_STRING, IMAGE_PATH, LINK_URL));
                }
                else if (platform == "Twitter")
                {
                    string _API_KEY = null;
                    string _API_SECRET_KEY = null;
                    string _ACCESS_TOKEN = null;
                    string _ACCESS_TOKEN_SECRET = null;

                    Invoke(new Action(() =>
                    {
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.Cells[1].Value != DBNull.Value && row.Cells[1].Value != null)
                            {
                                if (row.Cells[1].Value.ToString() == PAGE_NAME)
                                {
                                    _API_KEY = row.Cells[2].Value.ToString();
                                    _API_SECRET_KEY = row.Cells[3].Value.ToString();
                                    _ACCESS_TOKEN = row.Cells[4].Value.ToString();
                                    _ACCESS_TOKEN_SECRET = row.Cells[5].Value.ToString();
                                }
                            }
                        }
                    }));

                    string response = null;
                    if (!isImage)
                    {
                        TweetMessage twitter = new TweetMessage(_API_KEY, _ACCESS_TOKEN, _API_SECRET_KEY, _ACCESS_TOKEN_SECRET);
                        response = await Task.Run(() => twitter.PublishTweet(MESSAGE_STRING + Environment.NewLine + LINK_URL));
                    }
                    else if (isImage)
                    {
                        TweetImage twitter = new TweetImage(_API_KEY, _ACCESS_TOKEN, _API_SECRET_KEY, _ACCESS_TOKEN_SECRET);
                        response = await Task.Run(() => twitter.PublishTweet(MESSAGE_STRING, IMAGE_PATH));
                    }

                    if (response != "OK")
                    {
                        EasyLogger.Warning(response);
                        if (!Visible)
                        {
                            ShowWindow();
                        }
                        tabControlOne.SelectedIndex = 4;
                    }
                    else
                    {
                        EasyLogger.Info("Requested message posted successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @Post(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }
        #endregion Post

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                e.ThrowException = false;

                string txt = "Error with " + dataGridView.Columns[e.ColumnIndex].HeaderText + "\n\n" + e.Exception.Message;
                MessageBox.Show(txt, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                e.Cancel = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowWindow()
        {
            try
            {
                TopMost = true;
                WindowState = FormWindowState.Minimized;
                Show();
                ShowInTaskbar = true;
                if (Properties.Settings.Default.windowState != FormWindowState.Normal && Properties.Settings.Default.windowState != FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    WindowState = Properties.Settings.Default.windowState;
                }
                BringToFront();
                Focus();
                Activate();
                TopMost = false;
                if (dataGridView != null)
                {
                    dataGridView.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void EnsureVisibleRow(DataGridView view, int rowToShow)
        {
            try
            {
                view.FirstDisplayedScrollingRowIndex = rowToShow;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Add, Update, Remove, View Image

        private void MenuItemAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = openFileDialog1.FileName;
                        byte[] image = File.ReadAllBytes(fileName);

                        if (dataGridView.CurrentRow.Cells[5].Value != DBNull.Value && dataGridView.CurrentRow.Cells[5].Value != null && (string)dataGridView.CurrentRow.Cells[5].Value != "")
                        {
                            MessageBox.Show("You cannot post an image and a link in the same post.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dataGridView.CurrentRow.Cells[5].Value = "";
                        }

                        DateTime? time = null;
                        if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[6].Value))
                        {
                            time = (DateTime)dataGridView.CurrentRow.Cells[6].Value;
                        }

                        DateTime? expire = null;
                        if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[13].Value))
                        {
                            expire = (DateTime)dataGridView.CurrentRow.Cells[13].Value;
                        }

                        if (dataGridView.Parent.Text == "Sunday")
                        {
                            sundayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Monday")
                        {
                            mondayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Tuesday")
                        {
                            tuesdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Wednesday")
                        {
                            wednesdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Thursday")
                        {
                            thursdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Friday")
                        {
                            fridayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                        if (dataGridView.Parent.Text == "Saturday")
                        {
                            saturdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire);
                            saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemAddImage_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void MenuItemUpdateImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = openFileDialog1.FileName;
                        byte[] image = File.ReadAllBytes(fileName);

                        if (dataGridView.CurrentRow.Cells[5].Value != DBNull.Value && dataGridView.CurrentRow.Cells[5].Value != null && (string)dataGridView.CurrentRow.Cells[5].Value != "")
                        {
                            MessageBox.Show("You cannot post an image and a link in the same post.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dataGridView.CurrentRow.Cells[5].Value = "";
                        }

                        DateTime? time = null;
                        if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[6].Value))
                        {
                            time = (DateTime)dataGridView.CurrentRow.Cells[6].Value;
                        }

                        DateTime? expire = null;
                        if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[13].Value))
                        {
                            expire = (DateTime)dataGridView.CurrentRow.Cells[13].Value;
                        }

                        if (dataGridView.Parent.Text == "Sunday")
                        {
                            sundayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Monday")
                        {
                            mondayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Tuesday")
                        {
                            tuesdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Wednesday")
                        {
                            wednesdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Thursday")
                        {
                            thursdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Friday")
                        {
                            fridayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }

                        if (dataGridView.Parent.Text == "Saturday")
                        {
                            saturdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, (bool)dataGridView.CurrentRow.Cells[7].Value, (bool)dataGridView.CurrentRow.Cells[8].Value, (bool)dataGridView.CurrentRow.Cells[9].Value, (bool)dataGridView.CurrentRow.Cells[10].Value, (bool)dataGridView.CurrentRow.Cells[11].Value, (bool)dataGridView.CurrentRow.Cells[12].Value, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                            saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);

                            dataGridView.Rows[gridviewRowIndex].Cells[gridviewColIndex].Selected = true;

                            EnsureVisibleRow(dataGridView, dataGridView.Rows[gridviewRowIndex].Index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemUpdateImage_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void MenuItemRemoveImage_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.CurrentRow.Cells[4].Value = DBNull.Value;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemRemoveImage_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void MenuItemView_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[4].Value))
                {
                    Image x = (Bitmap)((new ImageConverter()).ConvertFrom(dataGridView.CurrentRow.Cells[4].Value));
                    string imagePath = Path.GetTempPath() + "PostImage.jpg";
                    x.Save(Path.GetTempPath() + "PostImage.jpg");

                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "ImageViewerLite.exe", imagePath);
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemView_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void MenuItemSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[4].Value))
                {
                    Image x = (Bitmap)((new ImageConverter()).ConvertFrom(dataGridView.CurrentRow.Cells[4].Value));
                    string imagePath = Path.GetTempPath() + "PostImage.jpg";

                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = "PostImage.jpg",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                    };
                    saveFileDialog.ShowDialog();

                    x.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemSave_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        #endregion Add, Update, Remove, View Image

        private void DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                dataGridView = (DataGridView)sender;

                if (dataGridView == dataGridView1)
                {
                    facebookTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView2)
                {
                    twitterTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView3)
                {
                    sundayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView4)
                {
                    mondayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView5)
                {
                    tuesdayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView6)
                {
                    wednesdayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView7)
                {
                    thursdayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView8)
                {
                    fridayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
                if (dataGridView == dataGridView9)
                {
                    saturdayTableAdapter.DeleteQuery((int)dataGridView.CurrentRow.Cells[0].Value);
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView_UserDeletingRow(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        #region Cell Value Changed

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow.Cells[0].Value != null)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Facebook WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView1.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                facebookTableAdapter.InsertQuery(dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString());
                                facebookTableAdapter.Fill(socialPostSchedulerDataSet.Facebook);
                            }
                            else
                            {
                                facebookTableAdapter.UpdateQuery(dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[3].Value.ToString(), dataGridView1.CurrentRow.Cells[4].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString(), dataGridView1.CurrentRow.Cells[6].Value.ToString(), (int)dataGridView1.CurrentRow.Cells[0].Value);
                                facebookTableAdapter.Fill(socialPostSchedulerDataSet.Facebook);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView1_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView2.CurrentRow.Cells[0].Value != null)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Twitter WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView2.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                twitterTableAdapter.InsertQuery(dataGridView2.CurrentRow.Cells[1].Value.ToString(), dataGridView2.CurrentRow.Cells[2].Value.ToString(), dataGridView2.CurrentRow.Cells[3].Value.ToString(), dataGridView2.CurrentRow.Cells[4].Value.ToString(), dataGridView2.CurrentRow.Cells[5].Value.ToString());
                                twitterTableAdapter.Fill(socialPostSchedulerDataSet.Twitter);
                            }
                            else
                            {
                                twitterTableAdapter.UpdateQuery(dataGridView2.CurrentRow.Cells[1].Value.ToString(), dataGridView2.CurrentRow.Cells[2].Value.ToString(), dataGridView2.CurrentRow.Cells[3].Value.ToString(), dataGridView2.CurrentRow.Cells[4].Value.ToString(), dataGridView2.CurrentRow.Cells[5].Value.ToString(), (int)dataGridView2.CurrentRow.Cells[0].Value);
                                twitterTableAdapter.Fill(socialPostSchedulerDataSet.Twitter);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView2_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Sunday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView[0, e.RowIndex].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {

                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView3_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Monday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView4.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView4_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView5_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Tuesday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView5.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView5_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView6_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Wednesday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView6.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView6_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView7_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Thursday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView7.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView7_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView8_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Friday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView8.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView8_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void DataGridView9_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView[0, e.RowIndex].Value != null)
                {
                    if (dataGridView.CurrentRow.Cells[1].Selected && dataGridView.CurrentRow.Cells[2].Value != DBNull.Value && dataGridView.CurrentRow.Cells[2].Value != null && (string)dataGridView.CurrentRow.Cells[2].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[2].Value = "";
                    }

                    DataGridView dgv = (DataGridView)sender;
                    using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Saturday WHERE id=@id", sqlConnection))
                        {
                            sqlConnection.Open();

                            sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView9.CurrentRow.Cells[0].Value);
                            int userCount = (int)sqlCommand.ExecuteScalar();

                            if (userCount == 0)
                            {
                                InsertQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                            else
                            {
                                UpdateQuery(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @DataGridView9_CellValueChanged(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        #endregion Cell Value Changed

        private void MenuItemSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();

            Properties.Settings.Default.Reload();
        }

        private void MenuItemScheduler_Click(object sender, EventArgs e)
        {
            try
            {
                ShowWindow();
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemScheduler_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void MenuItemClose_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();
                notifyIcon.Visible = false;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @MenuItemClose_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }

            Environment.Exit(0);
        }

        #region Cell Mouse Click

        private void DataGridView3_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView3);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView4_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView4);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView5_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView5);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView6_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView6);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView7_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView7);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView8_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView8);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridView9_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex != -1)
                    {
                        CreateContextMenu(dataGridView9);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion Cell Mouse Click

        private void CreateContextMenu(DataGridView dataView)
        {
            try
            {
                if (dataView.CurrentCell.ColumnIndex != 0)
                {
                    dataGridView = dataView;

                    string cmdText = null;
                    if (dataGridView.Parent.Text == "Sunday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Sunday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Monday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Monday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Tuesday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Tuesday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Wednesday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Wednesday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Thursday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Thursday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Friday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Friday WHERE id=@id";
                    }
                    if (dataGridView.Parent.Text == "Saturday")
                    {
                        cmdText = "SELECT COUNT(*) FROM Saturday WHERE id=@id";
                    }

                    if (dataView.CurrentCell.ColumnIndex == 1)
                    {
                        platformMenu = new ContextMenu();
                        string[] items = { "Facebook", "Twitter" };
                        foreach (string item in items)
                        {
                            platforMenuItem = new MenuItem(item);
                            platformMenu.MenuItems.Add(platforMenuItem);
                            platforMenuItem.Click += PlatforMenuItem_Click;
                        }

                        Rectangle r = dataGridView.CurrentCell.DataGridView.GetCellDisplayRectangle(dataGridView.Columns[1].Index, dataGridView.CurrentRow.Index, false);
                        platformMenu.Show(dataGridView, new Point(r.X, r.Y));
                    }
                    if (dataView.CurrentCell.ColumnIndex == 2)
                    {
                        DataGridView FBdataGrid = null;
                        DataGridView TWdataGrid = null;
                        if (dataGridView.CurrentRow.Cells[1].Value == DBNull.Value)
                        {
                            MessageBox.Show("Please choose a platform first!", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if ((string)dataGridView.CurrentRow.Cells[1].Value == "Facebook")
                        {
                            FBdataGrid = dataGridView1;
                        }
                        else if ((string)dataGridView.CurrentRow.Cells[1].Value == "Twitter")
                        {
                            TWdataGrid = dataGridView2;
                        }

                        if (FBdataGrid != null)
                        {
                            platformMenu = new ContextMenu();
                            foreach (DataGridViewRow row in FBdataGrid.Rows)
                            {
                                if (row.Cells[1].Value != null)
                                {
                                    platforMenuItem = new MenuItem(row.Cells[1].Value.ToString());
                                    platformMenu.MenuItems.Add(platforMenuItem);
                                    platforMenuItem.Click += PlatforMenuItem_Click;
                                }
                            }

                            Rectangle r = dataGridView.CurrentCell.DataGridView.GetCellDisplayRectangle(dataGridView.Columns[2].Index, dataGridView.CurrentRow.Index, false);
                            platformMenu.Show(dataGridView, new Point(r.X, r.Y));
                        }
                        else if (TWdataGrid != null)
                        {
                            platformMenu = new ContextMenu();
                            foreach (DataGridViewRow row in TWdataGrid.Rows)
                            {
                                if (row.Cells[1].Value != null)
                                {
                                    platforMenuItem = new MenuItem(row.Cells[1].Value.ToString());
                                    platformMenu.MenuItems.Add(platforMenuItem);
                                    platforMenuItem.Click += PlatforMenuItem_Click;
                                }
                            }

                            Rectangle r = dataGridView.CurrentCell.DataGridView.GetCellDisplayRectangle(dataGridView.Columns[2].Index, dataGridView.CurrentRow.Index, false);
                            platformMenu.Show(dataGridView, new Point(r.X, r.Y));
                        }
                    }
                    if (dataView.CurrentCell.ColumnIndex == 4)
                    {
                        gridviewColIndex = dataGridView.Columns[4].Index;
                        gridviewRowIndex = dataGridView.CurrentRow.Index;

                        using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.SocialPostSchedulerConnectionString))
                        {
                            using (SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection))
                            {
                                sqlConnection.Open();

                                sqlCommand.Parameters.AddWithValue("@id", (int)dataGridView.CurrentRow.Cells[0].Value);
                                int userCount = (int)sqlCommand.ExecuteScalar();

                                if (userCount == 0)
                                {
                                    menuItemAddImage.Enabled = true;
                                    menuItemUpdateImage.Enabled = false;
                                    menuItemRemoveImage.Enabled = false;
                                    menuItemViewImage.Enabled = false;

                                    Rectangle r = dataGridView.CurrentCell.DataGridView.GetCellDisplayRectangle(dataGridView.Columns[4].Index, dataGridView.CurrentRow.Index, false);
                                    ImageContextMenu.Show(dataGridView, new Point(r.X, r.Y));
                                }
                                else
                                {
                                    menuItemAddImage.Enabled = false;
                                    menuItemUpdateImage.Enabled = true;
                                    menuItemRemoveImage.Enabled = true;
                                    menuItemViewImage.Enabled = true;

                                    Rectangle r = dataGridView.CurrentCell.DataGridView.GetCellDisplayRectangle(dataGridView.Columns[4].Index, dataGridView.CurrentRow.Index, false);
                                    ImageContextMenu.Show(dataGridView, new Point(r.X, r.Y));
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                EasyLogger.Error("Scheduler - @CreateContextMenu(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();
                notifyIcon.Visible = false;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @ExitToolStripMenuItem_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }

            Environment.Exit(0);
        }

        private void AllDaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAllTabs();
        }

        private void ExportAllTabs()
        {
            try
            {
                SaveToCSV(dataGridView3, "Sunday", true);
                SaveToCSV(dataGridView4, "Monday", true);
                SaveToCSV(dataGridView5, "Tuesday", true);
                SaveToCSV(dataGridView6, "Wednesday", true);
                SaveToCSV(dataGridView7, "Thursday", true);
                SaveToCSV(dataGridView8, "Friday", true);
                SaveToCSV(dataGridView9, "Saturday", true);

                MessageBox.Show("The tables have been exported to " + Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + " successfully!", "Exported CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ActiveTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCurrentTab();
        }

        private void ExportCurrentTab()
        {
            try
            {
                if (tabControl2.SelectedTab.Text == "Sunday")
                {
                    SaveToCSV(dataGridView3, "Sunday");
                }
                else if (tabControl2.SelectedTab.Text == "Monday")
                {
                    SaveToCSV(dataGridView4, "Monday");
                }
                else if (tabControl2.SelectedTab.Text == "Tuesday")
                {
                    SaveToCSV(dataGridView5, "Tuesday");
                }
                else if (tabControl2.SelectedTab.Text == "Wednesday")
                {
                    SaveToCSV(dataGridView6, "Wednesday");
                }
                else if (tabControl2.SelectedTab.Text == "Thursday")
                {
                    SaveToCSV(dataGridView7, "Thursday");
                }
                else if (tabControl2.SelectedTab.Text == "Friday")
                {
                    SaveToCSV(dataGridView8, "Friday");
                }
                else if (tabControl2.SelectedTab.Text == "Saturday")
                {
                    SaveToCSV(dataGridView9, "Saturday");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void SaveToCSV(DataGridView DGV, string name, bool allTabs = false)
        {
            try
            {
                string file = string.Empty;
                if (!allTabs)
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        Filter = "CSV (*.csv)|*.csv",
                        FileName = name + ".csv"
                    };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        file = sfd.FileName;
                    }
                }
                if (file == string.Empty)
                {
                    file = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + name + ".csv";
                }
                int columnCount = DGV.ColumnCount;
                string columnNames = "";
                string[] output = new string[DGV.RowCount + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += DGV.Columns[i].HeaderText.ToString() + ",";
                }
                output[0] += columnNames;
                for (int i = 1; (i - 1) < DGV.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        try
                        {
                            if (DGV.Rows[i - 1].Cells[j].Value is byte[] || DGV.Rows[i - 1].Cells[j].Value is Image)
                            {
                                output[i] += ",";
                            }
                            else
                            {
                                string cellContent = DGV.Rows[i - 1].Cells[j].Value.ToString().Replace(",", "").Replace("\r\n", " ");
                                output[i] += cellContent + ",";
                            }
                        }
                        catch
                        {
                            output[i] += ",";
                            continue;
                        }
                    }
                }
                File.WriteAllLines(file, output, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wikiImage_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/xCONFLiCTiONx/Social-Post-Scheduler#social-post-scheduler");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MinimizeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                settings.ShowDialog();

                Properties.Settings.Default.Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PlatforMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MenuItem name = (MenuItem)sender;
                dataGridView.CurrentCell.Value = name.Text;
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @PlatforMenuItem_Click(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        #region Insert and Update Query

        private void InsertQuery(int col, int row)
        {
            try
            {
                byte[] image = null;

                bool cell7 = false;
                if (dataGridView.CurrentRow.Cells[7].Value != DBNull.Value)
                {
                    cell7 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[7].Value);
                }
                bool cell8 = false;
                if (dataGridView.CurrentRow.Cells[8].Value != DBNull.Value)
                {
                    cell8 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[8].Value);
                }
                bool cell9 = false;
                if (dataGridView.CurrentRow.Cells[9].Value != DBNull.Value)
                {
                    cell9 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[9].Value);
                }
                bool cell10 = false;
                if (dataGridView.CurrentRow.Cells[10].Value != DBNull.Value)
                {
                    cell10 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[10].Value);
                }
                bool cell11 = false;
                if (dataGridView.CurrentRow.Cells[11].Value != DBNull.Value)
                {
                    cell11 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[11].Value);
                }
                bool cell12 = false;
                if (dataGridView.CurrentRow.Cells[12].Value != DBNull.Value)
                {
                    cell12 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[12].Value);
                }

                if (dataGridView.CurrentRow.Cells[4].Value != DBNull.Value)
                {
                    image = (byte[])(dataGridView.CurrentRow.Cells[4].Value);

                    if (dataGridView.CurrentRow.Cells[5].Value != DBNull.Value && dataGridView.CurrentRow.Cells[5].Value != null && (string)dataGridView.CurrentRow.Cells[5].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[5].Value = "";
                        MessageBox.Show("You cannot post an image and a link in the same post.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                DateTime? time = null;
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[6].Value))
                {
                    time = (DateTime)dataGridView.CurrentRow.Cells[6].Value;
                }

                DateTime? expire = null;
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[13].Value))
                {
                    expire = (DateTime)dataGridView.CurrentRow.Cells[13].Value;
                }

                CheckLength();

                if (dataGridView.Parent.Text == "Sunday")
                {
                    sundayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Monday")
                {
                    mondayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Tuesday")
                {
                    tuesdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Wednesday")
                {
                    wednesdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Thursday")
                {
                    thursdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Friday")
                {
                    fridayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Saturday")
                {
                    saturdayTableAdapter.InsertQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire);
                    saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @InsertQuery(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        private void UpdateQuery(int col, int row)
        {
            try
            {
                byte[] image = null;

                bool cell7 = false;
                if (dataGridView.CurrentRow.Cells[7].Value != DBNull.Value)
                {
                    cell7 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[7].Value);
                }
                bool cell8 = false;
                if (dataGridView.CurrentRow.Cells[8].Value != DBNull.Value)
                {
                    cell8 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[8].Value);
                }
                bool cell9 = false;
                if (dataGridView.CurrentRow.Cells[9].Value != DBNull.Value)
                {
                    cell9 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[9].Value);
                }
                bool cell10 = false;
                if (dataGridView.CurrentRow.Cells[10].Value != DBNull.Value)
                {
                    cell10 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[10].Value);
                }
                bool cell11 = false;
                if (dataGridView.CurrentRow.Cells[11].Value != DBNull.Value)
                {
                    cell11 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[11].Value);
                }
                bool cell12 = false;
                if (dataGridView.CurrentRow.Cells[12].Value != DBNull.Value)
                {
                    cell12 = Convert.ToBoolean(dataGridView.CurrentRow.Cells[12].Value);
                }

                if (dataGridView.CurrentRow.Cells[4].Value != DBNull.Value)
                {
                    image = (byte[])(dataGridView.CurrentRow.Cells[4].Value);

                    if (dataGridView.CurrentRow.Cells[5].Value != DBNull.Value && dataGridView.CurrentRow.Cells[5].Value != null && (string)dataGridView.CurrentRow.Cells[5].Value != "")
                    {
                        dataGridView.CurrentRow.Cells[5].Value = "";
                        MessageBox.Show("You cannot post an image and a link in the same post.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                DateTime? time = null;
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[6].Value))
                {
                    time = (DateTime)dataGridView.CurrentRow.Cells[6].Value;
                }

                DateTime? expire = null;
                if (!DBNull.Value.Equals(dataGridView.CurrentRow.Cells[13].Value))
                {
                    expire = (DateTime)dataGridView.CurrentRow.Cells[13].Value;
                }

                CheckLength();

                if (dataGridView.Parent.Text == "Sunday")
                {
                    sundayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);

                    sundayTableAdapter.Fill(socialPostSchedulerDataSet.Sunday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Monday")
                {
                    mondayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    mondayTableAdapter.Fill(socialPostSchedulerDataSet.Monday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Tuesday")
                {
                    tuesdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    tuesdayTableAdapter.Fill(socialPostSchedulerDataSet.Tuesday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Wednesday")
                {
                    wednesdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    wednesdayTableAdapter.Fill(socialPostSchedulerDataSet.Wednesday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Thursday")
                {
                    thursdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    thursdayTableAdapter.Fill(socialPostSchedulerDataSet.Thursday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Friday")
                {
                    fridayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    fridayTableAdapter.Fill(socialPostSchedulerDataSet.Friday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
                if (dataGridView.Parent.Text == "Saturday")
                {
                    saturdayTableAdapter.UpdateQuery(dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), image, dataGridView.CurrentRow.Cells[5].Value.ToString(), time, cell7, cell8, cell9, cell10, cell11, cell12, expire, (int)dataGridView.CurrentRow.Cells[0].Value);
                    saturdayTableAdapter.Fill(socialPostSchedulerDataSet.Saturday);

                    dataGridView.Rows[row].Cells[col].Selected = true;

                    EnsureVisibleRow(dataGridView, row);
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @UpdateQuery(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }
        }

        #endregion Insert and Update Query

        private static void CheckLength()
        {
            try
            {
                if (dataGridView.CurrentRow.Cells[1].Value.ToString() == "Twitter")
                {
                    if (dataGridView.CurrentRow.Cells[3].Value.ToString().Length + dataGridView.CurrentRow.Cells[5].Value.ToString().Length > 283 && dataGridView.CurrentRow.Cells[3].Value.ToString().Length > 0 && dataGridView.CurrentRow.Cells[5].Value.ToString().Length > 0)
                    {
                        MessageBox.Show("There is more than 283 characters in this message plus the link which will be truncated on post. Please read the wiki on correct posting guidelines.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (dataGridView.CurrentRow.Cells[3].Value.ToString().Length > 283)
                    {
                        MessageBox.Show("There is more than 283 characters in this message which will be truncated on post. Please read the wiki on correct posting guidelines.", "Social Post Scheduler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch { }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (WindowState != FormWindowState.Minimized)
                {
                    Properties.Settings.Default.windowState = WindowState;
                }
                if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.windowLocation = Location;
                    Properties.Settings.Default.windowSize = Size;
                }
                else
                {
                    Properties.Settings.Default.windowLocation = RestoreBounds.Location;
                    Properties.Settings.Default.windowSize = RestoreBounds.Size;
                }

                Properties.Settings.Default.Save();

                e.Cancel = true;

                Hide();

                if (systemShutdown || e.CloseReason == CloseReason.TaskManagerClosing)
                {
                    notifyIcon.Visible = false;
                    e.Cancel = false;
                    Application.Exit();
                }
                else
                {
                    Hide();
                }
            }
            catch (Exception ex)
            {
                EasyLogger.Error("Scheduler - @Settings_FormClosing(1): " + ex);
                if (!Visible)
                {
                    ShowWindow();
                }
                tabControlOne.SelectedIndex = 4;
            }

            e.Cancel = true;
        }
    }
}
