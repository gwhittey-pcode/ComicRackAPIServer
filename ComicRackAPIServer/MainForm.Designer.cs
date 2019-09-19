/*
 * Created by SharpDevelop.
 * User: jeroen
 * Date: 03/14/2013
 * Time: 21:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ComicRackAPIServer
{
  partial class MainForm
  {
    /// <summary>
    /// Designer variable used to keep track of non-visual components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    
    /// <summary>
    /// Disposes resources used by the form.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }
    
    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageWebServer = new System.Windows.Forms.TabPage();
            this.textBoxUrlBase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.checkBoxUseProgressFromComicRack = new System.Windows.Forms.CheckBox();
            this.checkBoxAllowLibrary = new System.Windows.Forms.CheckBox();
            this.buttonChangePassword = new System.Windows.Forms.Button();
            this.textBoxFullName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboTreeHomeList = new ComboTreeBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonRemoveUser = new System.Windows.Forms.Button();
            this.buttonAddUser = new System.Windows.Forms.Button();
            this.comboBoxUsers = new System.Windows.Forms.ComboBox();
            this.tabPageMisc = new System.Windows.Forms.TabPage();
            this.buttonClearThumbnailsCache = new System.Windows.Forms.Button();
            this.labelCacheSize = new System.Windows.Forms.Label();
            this.buttonClearPageCache = new System.Windows.Forms.Button();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.webBrowserAbout = new System.Windows.Forms.WebBrowser();
            this.buttonClearSyncCache = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageWebServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPageUsers.SuspendLayout();
            this.tabPageMisc.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageWebServer);
            this.tabControl.Controls.Add(this.tabPageUsers);
            this.tabControl.Controls.Add(this.tabPageMisc);
            this.tabControl.Controls.Add(this.tabPageAbout);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(503, 298);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControlSelectedIndexChanged);
            // 
            // tabPageWebServer
            // 
            this.tabPageWebServer.Controls.Add(this.textBoxUrlBase);
            this.tabPageWebServer.Controls.Add(this.label3);
            this.tabPageWebServer.Controls.Add(this.pictureBox1);
            this.tabPageWebServer.Controls.Add(this.buttonStart);
            this.tabPageWebServer.Controls.Add(this.labelStatus);
            this.tabPageWebServer.Controls.Add(this.textBoxPort);
            this.tabPageWebServer.Controls.Add(this.label1);
            this.tabPageWebServer.Location = new System.Drawing.Point(4, 22);
            this.tabPageWebServer.Name = "tabPageWebServer";
            this.tabPageWebServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWebServer.Size = new System.Drawing.Size(495, 272);
            this.tabPageWebServer.TabIndex = 0;
            this.tabPageWebServer.Text = "Web Server";
            this.tabPageWebServer.UseVisualStyleBackColor = true;
            // 
            // textBoxUrlBase
            // 
            this.textBoxUrlBase.Location = new System.Drawing.Point(114, 46);
            this.textBoxUrlBase.Name = "textBoxUrlBase";
            this.textBoxUrlBase.Size = new System.Drawing.Size(197, 20);
            this.textBoxUrlBase.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "URL Base";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ComicRackAPIServer.Properties.Resources.ComicRackAPIServer;
            this.pictureBox1.Location = new System.Drawing.Point(158, 118);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(182, 142);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(236, 20);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(8, 69);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(479, 46);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "status text";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(114, 20);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPort.TabIndex = 1;
            this.textBoxPort.TextChanged += new System.EventHandler(this.TextBoxPortTextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port to listen on:";
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Controls.Add(this.checkBoxUseProgressFromComicRack);
            this.tabPageUsers.Controls.Add(this.checkBoxAllowLibrary);
            this.tabPageUsers.Controls.Add(this.buttonChangePassword);
            this.tabPageUsers.Controls.Add(this.textBoxFullName);
            this.tabPageUsers.Controls.Add(this.label6);
            this.tabPageUsers.Controls.Add(this.label5);
            this.tabPageUsers.Controls.Add(this.comboTreeHomeList);
            this.tabPageUsers.Controls.Add(this.label4);
            this.tabPageUsers.Controls.Add(this.textBoxUsername);
            this.tabPageUsers.Controls.Add(this.label2);
            this.tabPageUsers.Controls.Add(this.buttonRemoveUser);
            this.tabPageUsers.Controls.Add(this.buttonAddUser);
            this.tabPageUsers.Controls.Add(this.comboBoxUsers);
            this.tabPageUsers.Location = new System.Drawing.Point(4, 22);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUsers.Size = new System.Drawing.Size(495, 272);
            this.tabPageUsers.TabIndex = 1;
            this.tabPageUsers.Text = "Users";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseProgressFromComicRack
            // 
            this.checkBoxUseProgressFromComicRack.Location = new System.Drawing.Point(8, 122);
            this.checkBoxUseProgressFromComicRack.Name = "checkBoxUseProgressFromComicRack";
            this.checkBoxUseProgressFromComicRack.Size = new System.Drawing.Size(224, 24);
            this.checkBoxUseProgressFromComicRack.TabIndex = 12;
            this.checkBoxUseProgressFromComicRack.Text = "Use comic progress from ComicRack";
            this.checkBoxUseProgressFromComicRack.UseVisualStyleBackColor = true;
            this.checkBoxUseProgressFromComicRack.CheckedChanged += new System.EventHandler(this.CheckBoxUseProgressFromComicRackCheckedChanged);
            // 
            // checkBoxAllowLibrary
            // 
            this.checkBoxAllowLibrary.Location = new System.Drawing.Point(8, 180);
            this.checkBoxAllowLibrary.Name = "checkBoxAllowLibrary";
            this.checkBoxAllowLibrary.Size = new System.Drawing.Size(221, 24);
            this.checkBoxAllowLibrary.TabIndex = 11;
            this.checkBoxAllowLibrary.Text = "Allow access to complete Library";
            this.checkBoxAllowLibrary.UseVisualStyleBackColor = true;
            this.checkBoxAllowLibrary.Visible = false;
            // 
            // buttonChangePassword
            // 
            this.buttonChangePassword.Location = new System.Drawing.Point(101, 67);
            this.buttonChangePassword.Name = "buttonChangePassword";
            this.buttonChangePassword.Size = new System.Drawing.Size(75, 23);
            this.buttonChangePassword.TabIndex = 10;
            this.buttonChangePassword.Text = "Change...";
            this.buttonChangePassword.UseVisualStyleBackColor = true;
            this.buttonChangePassword.Click += new System.EventHandler(this.ButtonChangePasswordClick);
            // 
            // textBoxFullName
            // 
            this.textBoxFullName.Location = new System.Drawing.Point(101, 96);
            this.textBoxFullName.Name = "textBoxFullName";
            this.textBoxFullName.Size = new System.Drawing.Size(241, 20);
            this.textBoxFullName.TabIndex = 9;
            this.textBoxFullName.Validated += new System.EventHandler(this.TextBoxFullNameValidated);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 8;
            this.label6.Text = "Full name";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 23);
            this.label5.TabIndex = 7;
            this.label5.Text = "Home List";
            this.label5.Visible = false;
            // 
            // comboTreeHomeList
            // 
            this.comboTreeHomeList.DroppedDown = false;
            this.comboTreeHomeList.Enabled = false;
            this.comboTreeHomeList.Location = new System.Drawing.Point(101, 152);
            this.comboTreeHomeList.Name = "comboTreeHomeList";
            this.comboTreeHomeList.SelectedNode = null;
            this.comboTreeHomeList.Size = new System.Drawing.Size(241, 23);
            this.comboTreeHomeList.TabIndex = 6;
            this.comboTreeHomeList.Visible = false;
            this.comboTreeHomeList.SelectedNodeChanged += new System.EventHandler(this.ComboTreeHomeListSelectedNodeChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Enabled = false;
            this.textBoxUsername.Location = new System.Drawing.Point(101, 41);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(241, 20);
            this.textBoxUsername.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Username";
            // 
            // buttonRemoveUser
            // 
            this.buttonRemoveUser.Location = new System.Drawing.Point(267, 6);
            this.buttonRemoveUser.Name = "buttonRemoveUser";
            this.buttonRemoveUser.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveUser.TabIndex = 2;
            this.buttonRemoveUser.Text = "Remove";
            this.buttonRemoveUser.UseVisualStyleBackColor = true;
            this.buttonRemoveUser.Click += new System.EventHandler(this.ButtonRemoveUserClick);
            // 
            // buttonAddUser
            // 
            this.buttonAddUser.Location = new System.Drawing.Point(186, 6);
            this.buttonAddUser.Name = "buttonAddUser";
            this.buttonAddUser.Size = new System.Drawing.Size(75, 23);
            this.buttonAddUser.TabIndex = 1;
            this.buttonAddUser.Text = "Add";
            this.buttonAddUser.UseVisualStyleBackColor = true;
            this.buttonAddUser.Click += new System.EventHandler(this.ButtonAddUserClick);
            // 
            // comboBoxUsers
            // 
            this.comboBoxUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUsers.FormattingEnabled = true;
            this.comboBoxUsers.Location = new System.Drawing.Point(8, 6);
            this.comboBoxUsers.Name = "comboBoxUsers";
            this.comboBoxUsers.Size = new System.Drawing.Size(172, 21);
            this.comboBoxUsers.Sorted = true;
            this.comboBoxUsers.TabIndex = 0;
            this.comboBoxUsers.SelectedIndexChanged += new System.EventHandler(this.ComboBoxUsersSelectedIndexChanged);
            // 
            // tabPageMisc
            // 
            this.tabPageMisc.Controls.Add(this.buttonClearSyncCache);
            this.tabPageMisc.Controls.Add(this.buttonClearThumbnailsCache);
            this.tabPageMisc.Controls.Add(this.labelCacheSize);
            this.tabPageMisc.Controls.Add(this.buttonClearPageCache);
            this.tabPageMisc.Location = new System.Drawing.Point(4, 22);
            this.tabPageMisc.Name = "tabPageMisc";
            this.tabPageMisc.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMisc.Size = new System.Drawing.Size(495, 272);
            this.tabPageMisc.TabIndex = 2;
            this.tabPageMisc.Text = "Misc.";
            this.tabPageMisc.UseVisualStyleBackColor = true;
            // 
            // buttonClearThumbnailsCache
            // 
            this.buttonClearThumbnailsCache.Location = new System.Drawing.Point(8, 63);
            this.buttonClearThumbnailsCache.Name = "buttonClearThumbnailsCache";
            this.buttonClearThumbnailsCache.Size = new System.Drawing.Size(140, 23);
            this.buttonClearThumbnailsCache.TabIndex = 2;
            this.buttonClearThumbnailsCache.Text = "Clear Thumbnails Cache";
            this.buttonClearThumbnailsCache.UseVisualStyleBackColor = true;
            this.buttonClearThumbnailsCache.Click += new System.EventHandler(this.ButtonClearThumbnailsCacheClick);
            // 
            // labelCacheSize
            // 
            this.labelCacheSize.Location = new System.Drawing.Point(8, 8);
            this.labelCacheSize.Name = "labelCacheSize";
            this.labelCacheSize.Size = new System.Drawing.Size(370, 23);
            this.labelCacheSize.TabIndex = 1;
            this.labelCacheSize.Text = "label3";
            // 
            // buttonClearPageCache
            // 
            this.buttonClearPageCache.Location = new System.Drawing.Point(8, 34);
            this.buttonClearPageCache.Name = "buttonClearPageCache";
            this.buttonClearPageCache.Size = new System.Drawing.Size(140, 23);
            this.buttonClearPageCache.TabIndex = 0;
            this.buttonClearPageCache.Text = "Clear Page Cache";
            this.buttonClearPageCache.UseVisualStyleBackColor = true;
            this.buttonClearPageCache.Click += new System.EventHandler(this.ButtonClearPageCacheClick);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.webBrowserAbout);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(495, 272);
            this.tabPageAbout.TabIndex = 3;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // webBrowserAbout
            // 
            this.webBrowserAbout.AllowWebBrowserDrop = false;
            this.webBrowserAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserAbout.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserAbout.Location = new System.Drawing.Point(3, 3);
            this.webBrowserAbout.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserAbout.Name = "webBrowserAbout";
            this.webBrowserAbout.ScriptErrorsSuppressed = true;
            this.webBrowserAbout.Size = new System.Drawing.Size(489, 266);
            this.webBrowserAbout.TabIndex = 0;
            this.webBrowserAbout.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // buttonClearSyncCache
            // 
            this.buttonClearSyncCache.Location = new System.Drawing.Point(8, 95);
            this.buttonClearSyncCache.Name = "buttonClearSyncCache";
            this.buttonClearSyncCache.Size = new System.Drawing.Size(140, 23);
            this.buttonClearSyncCache.TabIndex = 3;
            this.buttonClearSyncCache.Text = "Clear Sync Cache";
            this.buttonClearSyncCache.UseVisualStyleBackColor = true;
            this.buttonClearSyncCache.Click += new System.EventHandler(this.ButtonClearSyncCacheClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 298);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(410, 310);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ComicRackAPIServer 1.40";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageWebServer.ResumeLayout(false);
            this.tabPageWebServer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPageUsers.ResumeLayout(false);
            this.tabPageUsers.PerformLayout();
            this.tabPageMisc.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.ResumeLayout(false);

    }
    private System.Windows.Forms.CheckBox checkBoxUseProgressFromComicRack;
    private System.Windows.Forms.CheckBox checkBoxAllowLibrary;
    private System.Windows.Forms.Button buttonClearThumbnailsCache;
    private System.Windows.Forms.Label labelCacheSize;
    private System.Windows.Forms.WebBrowser webBrowserAbout;
    private System.Windows.Forms.Button buttonClearPageCache;
    private System.Windows.Forms.TabPage tabPageAbout;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.TabPage tabPageMisc;
    private ComboTreeBox comboTreeHomeList;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox textBoxFullName;
    private System.Windows.Forms.Button buttonChangePassword;
    private System.Windows.Forms.TextBox textBoxUsername;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox comboBoxUsers;
    private System.Windows.Forms.Button buttonAddUser;
    private System.Windows.Forms.Button buttonRemoveUser;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxPort;
    private System.Windows.Forms.Label labelStatus;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.TabPage tabPageUsers;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TabPage tabPageWebServer;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TextBox textBoxUrlBase;
    private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonClearSyncCache;
    }
}
