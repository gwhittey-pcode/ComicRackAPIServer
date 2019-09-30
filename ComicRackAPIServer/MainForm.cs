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

    using BCR;
    using cYo.Projects.ComicRack.Engine.Database;
    using cYo.Projects.ComicRack.Viewer;
    using Nancy.Hosting.Self;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Web.UI.WebControls;


    /// <summary>
    /// MainForm provides the user interface of this plugin.
    /// </summary>
    public partial class MainForm : Form
    {

        private static ManualResetEvent mre = new ManualResetEvent(false);
        private static NancyHost host;
        private int? actualPort;
        private Guid libraryGuid;
        private bool cacheSizesInitialized = false;

        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            string path = Path.Combine(BCRInstaller.Instance.InstallFolder, "about.html");

            webBrowserAbout.Url = new Uri("file://" + path);

            textBoxPort.Text = Database.Instance.GlobalSettings.webserver_port.ToString();
            actualPort = Database.Instance.GlobalSettings.webserver_port;
            textBoxUrlBase.Text = Database.Instance.GlobalSettings.url_base;

            string s = "cYo.Projects.ComicRack.Engine.Database.ComicLibraryListItem";
            ComicListItem item = Program.Database.ComicLists.GetItems<ComicListItem>(false).FirstOrDefault((ComicListItem cli) => cli.GetType().ToString() == s);
            if (item != null)
            {
                libraryGuid = item.Id;
            }

            FillComboHomeList();
            FillComboUsers();
            FillComboQuality();
            SetEnabledState();
           
            
        }

        private string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private void SetEnabledState()
        {
            if (buttonStart == null)
            {
                return;
            }
            buttonStart.Enabled = actualPort.HasValue;
            textBoxPort.Enabled = host == null;
            textBoxUrlBase.Enabled = host == null;

            if (host == null)
            {
                buttonStart.Text = "Start";
                labelStatus.Text = "The api server is not running. Right now";
            }
            else
            {
                buttonStart.Text = "Stop";
                labelStatus.Text = "The api server is running.\nUse http://" + LocalIPAddress() + ":" + textBoxPort.Text + textBoxUrlBase.Text + "\nNB in ComicRackReader server settings: don't forget to allow ComicRack in your firewall.";
            }
            System.Windows.Input.Mouse.SetCursor(null);
        }

        public void StartService()
        {
            textBoxPort.Enabled = false;
            textBoxUrlBase.Enabled = false;
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            Task.Factory.StartNew(() => LoadService());
            labelStatus.Text = "Starting";
        }

        public void LoadService()
        {
            if (host != null)
            {
                StopService();
            }



            try
            {
                int port = actualPort.Value;

                Uri uri = new Uri(String.Format("http://localhost:{0}/", port));
                HostConfiguration configuration = new HostConfiguration();
                configuration.RewriteLocalhost = true;
                host = new NancyHost(new Bootstrapper(), new Uri[] { uri });

                host.Start();
                this.BeginInvoke(new Action(SetEnabledState));
                mre.Reset();
                mre.WaitOne();

                host.Stop();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error in url binding:\n" + e.ToString());
                StopService();
                throw;
            }
            finally
            {
                host = null;
                this.BeginInvoke(new Action(SetEnabledState));
            }
        }


        private static IEnumerable<string> GetLocalIPs()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());
        }

        public void StopService()
        {
            mre.Set();
        }

        private bool IsCurrentlyRunningAsAdmin()
        {
            bool isAdmin = false;
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(currentIdentity);
                isAdmin = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                pricipal = null;
            }
            return isAdmin;
        }


        ///////////////////////////////////////////////////////////////////////
        /// WebServer TabPage
        void ButtonStartClick(object sender, System.EventArgs e)
        {
            if (host != null)
            {
                StopService();
            }
            else
            {
                if (IsCurrentlyRunningAsAdmin())
                {
                    Database.Instance.GlobalSettings.webserver_port = actualPort.HasValue ? actualPort.Value : 8080;
                    Database.Instance.GlobalSettings.url_base = textBoxUrlBase.Text;
                    Database.Instance.GlobalSettings.Save();

                    StartService();
                }
                else
                {
                    Database.Instance.GlobalSettings.webserver_port = actualPort.HasValue ? actualPort.Value : 8080;
                    Database.Instance.GlobalSettings.url_base = textBoxUrlBase.Text;
                    Database.Instance.GlobalSettings.Save();

                    StartService();
                    //  System.Windows.Forms.MessageBox.Show("Sorry!, you must be running ComicRack with administrator privileges.");
                }
            }
        }

        void TextBoxPortTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPort.Text))
            {
                return;
            }
            int x;
            if (int.TryParse(textBoxPort.Text, out x))
            {
                actualPort = x;
            }
            else
            {
                actualPort = null;
            }
            SetEnabledState();
        }


        ///////////////////////////////////////////////////////////////////////
        /// Users TabPage
        void FillComboUsers()
        {
            comboBoxUsers.Items.Clear();
            using (SQLiteDataReader reader = Database.Instance.ExecuteReader("SELECT id, username FROM user;"))
            {
                while (reader.Read())
                {
                    comboBoxUsers.Items.Add(new ComboUserItem(reader.GetString(1), reader.GetInt32(0)));
                }
            }

            if (comboBoxUsers.Items.Count > 0)
                comboBoxUsers.SelectedIndex = 0;
        }
        void FillComboQuality()
        {
            comboBoxQuality.Items.AddRange(Enumerable.Range(0, 101).Select(e => new ListItem(e.ToString())).ToArray());
            string db_quality = Database.Instance.GlobalSettings.webp_quality.ToString();
            comboBoxQuality.SelectedIndex = comboBoxQuality.FindStringExact(db_quality);
            string db_speed = Database.Instance.GlobalSettings.webp_speed.ToString();
            comboBoxSpeed.SelectedIndex = comboBoxSpeed.FindStringExact(db_speed);
        }
        void FillComboHomeList()
        {
            comboTreeHomeList.Nodes.Clear();

            var nodes = Program.Database.ComicLists.Select(c => c.ToComboTreeNode());
            comboTreeHomeList.Nodes.AddRange(nodes);
            if (comboTreeHomeList.Nodes.Count > 0)
            {
                comboTreeHomeList.SelectedNode = comboTreeHomeList.Nodes[0];
            }

            //Guid id = new Guid(string);
            //var list = Program.Database.ComicLists.FindItem(id);

        }

        void ButtonAddUserClick(object sender, EventArgs e)
        {
            InputBoxValidation validation = delegate (string val)
            {
                if (val.Length < 4)
                    return "The username must contain at least 4 characters.";

                if (UserDatabase.GetUserId(val) != -1)
                    return "The username already exists.";

                return "";
            };

            string name = "";
            var result = InputBox.Show("Add User", "Enter username (min. 4 characters):", ref name, validation);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                UserDatabase.AddUser(name, "1234567890");
                FillComboUsers();
                comboBoxUsers.SelectedIndex = comboBoxUsers.FindString(name);
                System.Windows.Forms.MessageBox.Show("User added.\nDon't forget to set a password and choose a home list.", "Add User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ButtonChangePasswordClick(null, null);
            }

        }


        void ButtonRemoveUserClick(object sender, System.EventArgs e)
        {
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            if (DialogResult.Yes == System.Windows.Forms.MessageBox.Show("Are you sure you want to remove user '" + item.Text + "'?", "Remove user", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                // remove user
                UserDatabase.RemoveUser(item.UserId);
                System.Windows.Forms.MessageBox.Show("User removed.", "Remove User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillComboUsers();
            }
        }

        void ButtonChangePasswordClick(object sender, EventArgs e)
        {
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            // Password Strength: http://xkcd.com/936/
            InputBoxValidation validation = delegate (string val)
            {
                if (val.Length < 4)
                    return "Password must contain at least 4 characters.";

                return "";
            };

            string password = "";
            if (DialogResult.OK == InputBox.Show("Change Password", "Enter a new password (min. 4 characters):", ref password, validation))
            {
                // change password
                UserDatabase.SetPassword(item.UserId, password);
            }
        }

        void ComboTreeHomeListSelectedNodeChanged(object sender, EventArgs e)
        {
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            if (comboTreeHomeList.SelectedNode != null)
            {
                string listId = comboTreeHomeList.SelectedNode.Tag.ToString();
                Database.Instance.ExecuteNonQuery("UPDATE user_settings SET home_list_id='" + listId + "' WHERE user_id=" + item.UserId + ";");
            }
        }


        void ComboBoxUsersSelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply values to previously selected user.

            // Show values for current user.
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            using (SQLiteDataReader reader = Database.Instance.ExecuteReader("SELECT id, username, fullname, home_list_id, use_comicrack_progress FROM user JOIN user_settings ON user.id=user_settings.user_id WHERE user.id = " + item.UserId + " LIMIT 1;"))
            {
                if (reader.Read())
                {
                    textBoxUsername.Text = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        textBoxFullName.Text = reader.GetString(2);
                    else
                        textBoxFullName.Text = "";

                    Guid listId;


                    if (!reader.IsDBNull(3))
                    {
                        try
                        {
                            listId = new Guid(reader.GetString(3));
                            // Check if list still exists, if not, select Library and update database
                            var list = Program.Database.ComicLists.FindItem(listId);
                            if (list == null)
                            {
                                listId = libraryGuid;
                            }
                        }
                        catch (Exception ex)
                        {
                            // invalid Guid format or reader.GetString(3) failed, just ignore and use main library list...
                            Console.WriteLine(ex.ToString());
                            listId = libraryGuid;
                        }
                    }
                    else
                    {
                        listId = libraryGuid;
                    }

                    checkBoxUseProgressFromComicRack.Checked = (reader.GetInt32(4) != 0);

                    comboTreeHomeList.SelectedNode = comboTreeHomeList.Nodes.FirstOrDefault((ComboTreeNode ctn) => ctn.Tag.Equals(listId));
                }
            }
        }


        void ButtonClearPageCacheClick(object sender, EventArgs e)
        {
            var cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            ImageCache.Instance.ClearPageCache();

            UpdateCacheSizes();

            this.Cursor = cursor;
        }


        void ButtonSetSettingsClick(object sender, EventArgs e)
        {
            if (comboBoxQuality.SelectedItem != null)
            {
                Database.Instance.GlobalSettings.webp_quality = int.Parse(comboBoxQuality.SelectedItem.ToString());
            }
            if (comboBoxSpeed.SelectedItem != null)
            {
                Database.Instance.GlobalSettings.webp_speed = int.Parse(comboBoxSpeed.SelectedItem.ToString());
            }
            Database.Instance.GlobalSettings.Save_Webp_Settings();
        }
        void TextBoxFullNameValidated(object sender, EventArgs e)
        {
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            UserDatabase.SetFullName(item.UserId, ((System.Windows.Forms.TextBox)sender).Text);
        }

        public void UpdateCacheSizes()
        {
            labelCacheSize.Text = String.Format("Used cache size: Pages {0} MB / Thumbnails {1} MB / Sync {2} MB ", (int)(ImageCache.Instance.GetPageCacheSize() / (1024 * 1024)), (int)(ImageCache.Instance.GetThumbnailsCacheSize() / (1024 * 1024)), (int)(ImageCache.Instance.GetSyncCacheSize() / (1024 * 1024)));
        }

        void ButtonClearThumbnailsCacheClick(object sender, EventArgs e)
        {
            var cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            ImageCache.Instance.ClearThumbnailsCache();

            UpdateCacheSizes();

            this.Cursor = cursor;
        }

        void ButtonClearSyncCacheClick(object sender, EventArgs e)
        {
            var cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            ImageCache.Instance.ClearSyncCache();

            UpdateCacheSizes();

            this.Cursor = cursor;
        }


        void TabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 2 && !cacheSizesInitialized)
            {
                cacheSizesInitialized = true;
                labelCacheSize.Text = "Calculating cache size....";
                var cursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;

                UpdateCacheSizes();

                this.Cursor = cursor;
            }
        }

        void CheckBoxUseProgressFromComicRackCheckedChanged(object sender, EventArgs e)
        {
            ComboUserItem item = (ComboUserItem)comboBoxUsers.SelectedItem;
            if (item == null)
            {
                return;
            }

            Database.Instance.ExecuteNonQuery("UPDATE user_settings SET use_comicrack_progress = " + (checkBoxUseProgressFromComicRack.Checked ? "1" : "0") + " WHERE user_id=" + item.UserId + ";");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void ButtonSetSettings_Click(object sender, EventArgs e)
        {

        }
    }

    // System.Resources.MissingManifestResourceException: Could not find any resources appropriate for the specified culture or the neutral culture.
    // http://stackoverflow.com/questions/2058441/could-not-find-any-resources-appropriate-for-the-specified-culture-or-the-neutra
    // To resolve this problem, move all of the other class definitions so that they appear after the form's class definition.

    public class ComboUserItem
    {
        public string Text { get; set; }
        public int UserId { get; set; }

        public ComboUserItem(string text, int userid)
        {
            Text = text;
            UserId = userid;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public static class ComboTreeNodeExtensions
    {
        public static ComboTreeNode ToComboTreeNode(this ComicListItem x)
        {
            ComboTreeNode node = new ComboTreeNode(x.Name);
            node.Tag = x.Id;

            ComicListItemFolder folderList = x as ComicListItemFolder;
            if (folderList != null)
            {
                node.Nodes.AddRange(folderList.Items.Select(c => c.ToComboTreeNode()));
            }

            return node;
        }
    }


}
