import clr

clr.AddReferenceByPartialName("IronPython")
clr.AddReferenceByPartialName("Microsoft.Scripting")
clr.AddReferenceByPartialName("ComicRackAPIServer")

clr.AddReference('System')
from System import Version

clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import MessageBox, MessageBoxButtons, MessageBoxIcon

from ComicRackAPIServer import Plugin

#@Name	ComicRack API Server
#@Key	ComicRackAPIServer
#@Hook	Books, Editor
#@Image ComicRackAPIServer_icon.png
#@Description ComicRack API Server
def ComicRackAPIServer(books):
 
  if IsVersionOK():
    Plugin.Run(ComicRack.App)
  
           
#@Name ComicRack API Server (Startup)
#@Hook Startup
#@Enabled false
#@Image ComicRackAPIServer_icon.png
#@Description ComicRack API Server (Startup)
def ComicRackAPIServerStartup():
  if IsVersionOK():
    Plugin.RunAtStartup(ComicRack.App)
   
      
def IsVersionOK():
  requiredVersion = Version(0, 9, 178)
  if str(ComicRack.App.ProductVersion) != str(requiredVersion):
    MessageBox.Show( ComicRack.MainWindow, "Version check failed!\n\nThe ComicRack API Server Plugin requires a different version of ComicRack.\nComicRack version required: " + str(requiredVersion) + ".\nExiting...", "Incompatible ComicRack version", MessageBoxButtons.OK, MessageBoxIcon.Warning)
  
  return str(ComicRack.App.ProductVersion) == str(requiredVersion)
    