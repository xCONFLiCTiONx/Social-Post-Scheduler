
namespace SocialPostScheduler
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.BackupButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dbBTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.pcTimePicker = new System.Windows.Forms.DateTimePicker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PostTimePicker = new System.Windows.Forms.DateTimePicker();
            this.releaseLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.UserIDBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AppIDBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.PageIDBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.TokenBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SiteBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.AppSecretBox = new System.Windows.Forms.TextBox();
            this.EnableInstagramBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // BackupButton
            // 
            this.BackupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BackupButton.Location = new System.Drawing.Point(201, 284);
            this.BackupButton.Name = "BackupButton";
            this.BackupButton.Size = new System.Drawing.Size(125, 24);
            this.BackupButton.TabIndex = 7;
            this.BackupButton.Text = "Backup Database";
            this.toolTip1.SetToolTip(this.BackupButton, "Backup the database now");
            this.BackupButton.UseVisualStyleBackColor = true;
            this.BackupButton.Click += new System.EventHandler(this.BackupButton_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Post Cleanup";
            // 
            // dbBTimePicker
            // 
            this.dbBTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbBTimePicker.Location = new System.Drawing.Point(126, 24);
            this.dbBTimePicker.Name = "dbBTimePicker";
            this.dbBTimePicker.Size = new System.Drawing.Size(201, 20);
            this.dbBTimePicker.TabIndex = 8;
            this.toolTip1.SetToolTip(this.dbBTimePicker, "Set this to a time that you will not be editing the schedule or posting. This val" +
        "ue should be different from the post cleanup time.");
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Database Backup";
            // 
            // pcTimePicker
            // 
            this.pcTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pcTimePicker.Location = new System.Drawing.Point(126, 50);
            this.pcTimePicker.Name = "pcTimePicker";
            this.pcTimePicker.Size = new System.Drawing.Size(201, 20);
            this.pcTimePicker.TabIndex = 9;
            this.toolTip1.SetToolTip(this.pcTimePicker, "Set this to a time that you will not be editing the schedule or posting. This val" +
        "ue should be different from the database backup time.");
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 100;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 20;
            // 
            // PostTimePicker
            // 
            this.PostTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PostTimePicker.Location = new System.Drawing.Point(126, 115);
            this.PostTimePicker.Name = "PostTimePicker";
            this.PostTimePicker.Size = new System.Drawing.Size(201, 20);
            this.PostTimePicker.TabIndex = 16;
            this.toolTip1.SetToolTip(this.PostTimePicker, "Set this to a time that you will not be editing the schedule or posting. This val" +
        "ue should be different from the database backup time.");
            // 
            // releaseLabel
            // 
            this.releaseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.releaseLabel.AutoSize = true;
            this.releaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.releaseLabel.Location = new System.Drawing.Point(4, 302);
            this.releaseLabel.Name = "releaseLabel";
            this.releaseLabel.Size = new System.Drawing.Size(51, 13);
            this.releaseLabel.TabIndex = 12;
            this.releaseLabel.Text = "Release: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "Instagram";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Post Time";
            // 
            // UserIDBox
            // 
            this.UserIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UserIDBox.Location = new System.Drawing.Point(126, 138);
            this.UserIDBox.Margin = new System.Windows.Forms.Padding(2);
            this.UserIDBox.Name = "UserIDBox";
            this.UserIDBox.Size = new System.Drawing.Size(201, 20);
            this.UserIDBox.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "User ID";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(78, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "App ID";
            // 
            // AppIDBox
            // 
            this.AppIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AppIDBox.Location = new System.Drawing.Point(126, 161);
            this.AppIDBox.Margin = new System.Windows.Forms.Padding(2);
            this.AppIDBox.Name = "AppIDBox";
            this.AppIDBox.Size = new System.Drawing.Size(201, 20);
            this.AppIDBox.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(75, 212);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Page ID";
            // 
            // PageIDBox
            // 
            this.PageIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PageIDBox.Location = new System.Drawing.Point(126, 209);
            this.PageIDBox.Margin = new System.Windows.Forms.Padding(2);
            this.PageIDBox.Name = "PageIDBox";
            this.PageIDBox.Size = new System.Drawing.Size(201, 20);
            this.PageIDBox.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(57, 234);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "User Token";
            // 
            // TokenBox
            // 
            this.TokenBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TokenBox.Location = new System.Drawing.Point(126, 232);
            this.TokenBox.Margin = new System.Windows.Forms.Padding(2);
            this.TokenBox.Name = "TokenBox";
            this.TokenBox.Size = new System.Drawing.Size(201, 20);
            this.TokenBox.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(47, 256);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Website Feed";
            // 
            // SiteBox
            // 
            this.SiteBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SiteBox.Location = new System.Drawing.Point(126, 254);
            this.SiteBox.Margin = new System.Windows.Forms.Padding(2);
            this.SiteBox.Name = "SiteBox";
            this.SiteBox.Size = new System.Drawing.Size(201, 20);
            this.SiteBox.TabIndex = 27;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(10, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(90, 15);
            this.label11.TabIndex = 29;
            this.label11.Text = "Maintenance";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(58, 189);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "App Secret";
            // 
            // AppSecretBox
            // 
            this.AppSecretBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AppSecretBox.Location = new System.Drawing.Point(126, 186);
            this.AppSecretBox.Margin = new System.Windows.Forms.Padding(2);
            this.AppSecretBox.Name = "AppSecretBox";
            this.AppSecretBox.Size = new System.Drawing.Size(201, 20);
            this.AppSecretBox.TabIndex = 30;
            // 
            // EnableInstagramBox
            // 
            this.EnableInstagramBox.AutoSize = true;
            this.EnableInstagramBox.Location = new System.Drawing.Point(87, 91);
            this.EnableInstagramBox.Name = "EnableInstagramBox";
            this.EnableInstagramBox.Size = new System.Drawing.Size(59, 17);
            this.EnableInstagramBox.TabIndex = 32;
            this.EnableInstagramBox.Text = "Enable";
            this.EnableInstagramBox.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 319);
            this.Controls.Add(this.EnableInstagramBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.AppSecretBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.SiteBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.TokenBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.PageIDBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AppIDBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.UserIDBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PostTimePicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.releaseLabel);
            this.Controls.Add(this.BackupButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dbBTimePicker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pcTimePicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackupButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dbBTimePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker pcTimePicker;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label releaseLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker PostTimePicker;
        private System.Windows.Forms.TextBox UserIDBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox AppIDBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox PageIDBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TokenBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox SiteBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox AppSecretBox;
        private System.Windows.Forms.CheckBox EnableInstagramBox;
    }
}