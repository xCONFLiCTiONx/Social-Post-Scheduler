using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialPostScheduler
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            EnableInstagramBox.Checked = Properties.Settings.Default.EnableInstagram;

            releaseLabel.Text = "Release: " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            DateTime now = DateTime.Now;

            DateTime result = Convert.ToDateTime(Properties.Settings.Default.DBBackupTime);
            DateTime backupTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);

            dbBTimePicker.Value = backupTime;
            dbBTimePicker.Format = DateTimePickerFormat.Time;
            dbBTimePicker.ShowUpDown = true;

            result = Convert.ToDateTime(Properties.Settings.Default.PostExpireTime);
            DateTime expireTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);

            pcTimePicker.Value = expireTime;
            pcTimePicker.Format = DateTimePickerFormat.Time;
            pcTimePicker.ShowUpDown = true;

            result = Convert.ToDateTime(Properties.Settings.Default.RenewUserToken);
            DateTime renewTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);

            //RenewTimePicker.Value = renewTime;
            //RenewTimePicker.Format = DateTimePickerFormat.Time;
            //RenewTimePicker.ShowUpDown = true;

            result = Convert.ToDateTime(Properties.Settings.Default.InstagramPostTime);
            DateTime postTime = new DateTime(now.Year, now.Month, now.Day, result.Hour, result.Minute, 0);

            PostTimePicker.Value = postTime;
            PostTimePicker.Format = DateTimePickerFormat.Time;
            PostTimePicker.ShowUpDown = true;

            UserIDBox.Text = Properties.Settings.Default.InstagramUserID;
            AppIDBox.Text = Properties.Settings.Default.InstagramAppID;
            AppSecretBox.Text = Properties.Settings.Default.InstagramAppSecret;
            PageIDBox.Text = Properties.Settings.Default.InstagramPageID;
            UserIDBox.Text = Properties.Settings.Default.InstagramUserID;
            TokenBox.Text = Properties.Settings.Default.InstagramToken;
            SiteBox.Text = Properties.Settings.Default.WebsiteFeed;

            PostTimePicker.ValueChanged += PostTimePicker_ValueChanged;
            //RenewTimePicker.ValueChanged += RenewTimePicker_ValueChanged;
            dbBTimePicker.ValueChanged += DbBTimePicker_ValueChanged;
            pcTimePicker.ValueChanged += PcTimePicker_ValueChanged;

            FormClosing += Settings_FormClosing;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.InstagramUserID = UserIDBox.Text;
            Properties.Settings.Default.InstagramAppID = AppIDBox.Text;
            Properties.Settings.Default.InstagramAppSecret = AppSecretBox.Text;
            Properties.Settings.Default.InstagramPageID = PageIDBox.Text;
            Properties.Settings.Default.InstagramUserID = UserIDBox.Text;
            Properties.Settings.Default.InstagramToken = TokenBox.Text;
            Properties.Settings.Default.WebsiteFeed = SiteBox.Text;
            Properties.Settings.Default.EnableInstagram = EnableInstagramBox.Checked;

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private async void BackupButton_Click(object sender, EventArgs e)
        {
            await Task.Run(BackupDatabase.ExecuteBackup);
        }

        private void PostTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.InstagramPostTime = PostTimePicker.Value.ToString("h:mm:ss tt");
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        //private void RenewTimePicker_ValueChanged(object sender, EventArgs e)
        //{
        //    Properties.Settings.Default.RenewUserToken = RenewTimePicker.Value.ToString("h:mm:ss tt");
        //    Properties.Settings.Default.Save();
        //    Properties.Settings.Default.Reload();
        //}

        private void DbBTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DBBackupTime = dbBTimePicker.Value.ToString("h:mm:ss tt");
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void PcTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PostExpireTime = pcTimePicker.Value.ToString("h:mm:ss tt");
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
    }
}
