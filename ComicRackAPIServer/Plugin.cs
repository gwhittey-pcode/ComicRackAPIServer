using cYo.Projects.ComicRack.Plugins.Automation;
using System;
using System.Windows;

namespace ComicRackAPIServer
{
    public static class Plugin
    {
        internal static IApplication Application;
        private static MainForm panel;
        
        
        public static bool Initialize(IApplication app)
        {
          try
          {
            Application = app;
            
            if (!BCRInstaller.Instance.Install())
              return false;

            BCR.Database.Instance.Initialize();
            
            return true;
          }
          catch (Exception e)
          {
            MessageBox.Show(e.ToString());
          }
          
          return false;
        }
        
        public static void Run(IApplication app)
        {
          if (Initialize(app))
          {
            try
            {
                if (panel == null)
                {
                    panel = new MainForm();
                    panel.Closed += new EventHandler(panel_Closed);
                }
                
                panel.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
          }
        }

        public static void RunAtStartup(IApplication app)
        {
          if (Initialize(app))
          {
            try
            {
                if (panel == null)
                {
                    panel = new MainForm();
                    panel.Closed += new EventHandler(panel_Closed);
                }
                //panel.StartService();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
          }
        }

        static void panel_Closed(object sender, EventArgs e)
        {
            panel = null;
        }

        [STAThread]
        public static void Main()
        {
            if (panel == null)
            {
                panel = new MainForm();
                panel.Closed += new EventHandler(panel_Closed);
            }
            panel.ShowDialog();
        }

    }
}
