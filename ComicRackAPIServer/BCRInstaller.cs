using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Imazen.WebP;

namespace ComicRackAPIServer
{
  /// <summary>
  /// Description of BCRInstaller.
  /// </summary>
  public sealed class BCRInstaller
  {
    private const string INSTALLER_FILE = "BCRPlugin.zip";
    private const string VERSION_FILE = "BCRVersion.txt";
    private const string VERSION = "1.45.4";

    public string InstallFolder { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }

    private static BCRInstaller instance = new BCRInstaller();
    
    public static BCRInstaller Instance {
      get {
        return instance;
      }
    }
    
    private BCRInstaller()
    {
    }


    public bool Install()
    {


            ///////////////////////////////////////////////////////////
            //
            /*
            if (!InstallState.IsVC90RedistSP1Installed())
            {
              if (MessageBoxResult.Yes != MessageBox.Show("The required component 'Microsoft Visual C++ 2008 SP1 Redistributable Package' seems to be missing.\nDownload and install the correct version (32/64bit) from the Microsoft website, then restart ComicRack.\n\nDo you want to continue anyway (expect a crash...)?", "ComicRackAPIServer Plugin", MessageBoxButton.YesNo, MessageBoxImage.Error))
              {
                return false;
              }
            }
            */

            ///////////////////////////////////////////////////////////
            // Install the correct libweb.dll
            /*
             * 
      
      if (!FreeImage.IsAvailable())
      {
        System.IO.File.Copy(Path.Combine(InstallFolder, (Environment.Is64BitProcess ? "FreeImage.64bit.dll" : "FreeImage.32bit.dll")), Path.Combine(InstallFolder, "FreeImage.dll"), true);
      }
            
      if (!FreeImage.IsAvailable())
      {
        MessageBox.Show("FreeImage.dll seems to be missing. Aborting.", "ComicRackAPIServer Plugin", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }
      */
      System.IO.File.Copy(Path.Combine(InstallFolder, (Environment.Is64BitProcess ? "libwebp_x64.dll" : "libwebp_x86.dll")), Path.Combine(InstallFolder, "libwebp.dll"), true);
      ///////////////////////////////////////////////////////////
      // Install the correct SQLite.Interop.dll
      try
      {
        var connection = new SQLiteConnection();
      }
      catch (Exception e)
      {
        Trace.WriteLine(String.Format("Exception: {0}", e));
        System.IO.File.Copy(Path.Combine(InstallFolder, (Environment.Is64BitProcess ? "SQLite.Interop.64bit.dll" : "SQLite.Interop.32bit.dll")), Path.Combine(InstallFolder, "SQLite.Interop.dll"), true);
      }
      
      try 
      {
        var connection = new SQLiteConnection();
      }
      catch (System.DllNotFoundException e)
      {
        Trace.WriteLine(String.Format("Exception: {0}", e));
        MessageBox.Show("SQLite.Interop.dll seems to be missing. Aborting.", "ComicRackAPIServer Plugin", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }
      
      ///////////////////////////////////////////////////////////
      // Check version.txt in order to decide if this is an upgrade.
      var versionFilename = Path.Combine(InstallFolder, VERSION_FILE);

      if (File.Exists(versionFilename))
      {
        StreamReader streamReader = new StreamReader(versionFilename);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        
        if (text.StartsWith(VERSION))
        {
          return true;
        }
      }
      
      // Create/Update the version file.
      System.IO.File.WriteAllText(versionFilename, VERSION);

      var installerFilename = Path.Combine(InstallFolder, INSTALLER_FILE);
            
      if (!Unzip(installerFilename, InstallFolder))
      {
        MessageBox.Show("Error while installing the web viewer site. Aborting.", "ComicRackAPIServer Plugin", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }
        
      
      return true;      
    }
    
    public bool Unzip(string ZipFile, string DestinationFolder)
    {
  		if ( !File.Exists(ZipFile) ) 
  		{
  			Console.WriteLine("Cannot find file '{0}'", ZipFile);
  			return false;
  		}
  
  		using (ZipInputStream s = new ZipInputStream(File.OpenRead(ZipFile))) 
  		{
  			ZipEntry theEntry;
  			while ((theEntry = s.GetNextEntry()) != null) 
  			{
  				string directoryName = Path.GetDirectoryName(theEntry.Name);
  				string fileName      = Path.GetFileName(theEntry.Name);
  				
  				// create directory
  				if (!String.IsNullOrEmpty(directoryName)) 
  				{
  					Directory.CreateDirectory(Path.Combine(DestinationFolder, directoryName));
  				}
  				
  				if (!String.IsNullOrEmpty(fileName)) 
  				{
  					using (FileStream streamWriter = File.Create(Path.Combine(DestinationFolder, theEntry.Name))) 
  					{
  						int size = 2048;
  						byte[] data = new byte[2048];
  						while (true) 
  						{
  							size = s.Read(data, 0, data.Length);
  							if (size > 0) 
  							{
  								streamWriter.Write(data, 0, size);
  							} 
  							else
  							{
  								break;
  							}
  						}
  					}
  				}
  			}
  		}
  		return true;
  	}
    
  }
}
