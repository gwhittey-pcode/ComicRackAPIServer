/*
 * Created by SharpDevelop.
 * User: jeroen
 * Date: 3/16/2013
 * Time: 1:04 PM
 * 
 * source: http://mdb-blog.blogspot.nl/2010/11/c-check-if-vcredist-installed.html
 */
using System;
using System.Runtime.InteropServices;

namespace ComicRackAPIServer
{
  /// <summary>
  /// Description of InstallState.
  /// </summary>
  public static class InstallState
  {
   
    public enum INSTALLSTATE
    {
      INSTALLSTATE_NOTUSED = -7,  // component disabled
      INSTALLSTATE_BADCONFIG = -6,  // configuration datacorrupt
      INSTALLSTATE_INCOMPLETE = -5,  // installationsuspended or in progress
      INSTALLSTATE_SOURCEABSENT = -4,  // run from source,source is unavailable
      INSTALLSTATE_MOREDATA = -3,  // return bufferoverflow
      INSTALLSTATE_INVALIDARG = -2,  // invalid functionargument
      INSTALLSTATE_UNKNOWN = -1,  // unrecognized productor feature
      INSTALLSTATE_BROKEN = 0,  // broken
      INSTALLSTATE_ADVERTISED = 1,  // advertised feature
      INSTALLSTATE_REMOVED = 1,  // component being removed(action state, not settable)
      INSTALLSTATE_ABSENT = 2,  // uninstalled (or actionstate absent but clients remain)
      INSTALLSTATE_LOCAL = 3,  // installed on local drive
      INSTALLSTATE_SOURCE = 4,  // run from source, CD ornet
      INSTALLSTATE_DEFAULT = 5,  // use default, local orsource
    }
     
    [DllImport("msi.dll", CharSet = CharSet.Unicode)]
    private static extern INSTALLSTATE MsiQueryProductState(string product);
     
    
    // Check if Microsoft Visual C++ 2008 SP1 Redistributable Package is installed
    // 
    public static bool IsVC90RedistSP1Installed()
    {
      // There are language specific product codes....
      // http://mdb-blog.blogspot.nl/2010/11/bootstrapper-package-detect-vcredist.html
      
      string[] strCodes_x86 = new string[]
      {
        //vcredist_x86 - ProductCode
        "{9A25302D-30C0-39D9-BD6F-21E6EC160475}",
        "{86CE1746-9EFF-3C9C-8755-81EA8903AC34}",
        "{CA8A885F-E95B-3FC6-BB91-F4D9377C7686}",
        "{820B6609-4C97-3A2B-B644-573B06A0F0CC}",
        "{6AFCA4E1-9B78-3640-8F72-A7BF33448200}",
        "{F03CB3EF-DC16-35CE-B3C1-C68EA09E5E97}",
        "{402ED4A1-8F5B-387A-8688-997ABF58B8F2}",
        "{887868A2-D6DE-3255-AA92-AA0B5A59B874}",
        "{527BBE2F-1FED-3D8B-91CB-4DB0F838E69E}",
        "{57660847-B1F7-35BD-9118-F62EB863A598}"
      };
      
      string[] strCodes_x64 = new string[]
      {
        //vcredist_x64 - ProductCode
        "{8220EEFE-38CD-377E-8595-13398D740ACE}",
        "{56F27690-F6EA-3356-980A-02BA379506EE}",
        "{14297226-E0A0-3781-8911-E9D529552663}",
        "{9B3F0A88-790D-3AD9-9F96-B19CF2746452}",
        "{D285FC5F-3021-32E9-9C59-24CA325BDC5C}",
        "{092EE08C-60DE-3FE6-B113-90076EC06D0D}",
        "{A96702F7-EFC8-3EED-BE46-22C809D4EBE5}",
        "{92B8FD1F-C1AE-3750-8577-631B0AA85DF5}",
        "{2DFD8316-9EF1-3210-908C-4CB61961C1AC}",
        "{E34002C7-8CE7-3F76-B36C-09FA973BC4F6}"
      };
      
      // IA64 is not supported by this plugin....
      /*
      string[] strCodes_IA64 = new string[]
      {
        //vcredist_IA64 - ProductCode
        "{5827ECE1-AEB0-328E-B813-6FC68622C1F9}",
        "{9363B366-8370-34F7-8164-25052EBF35FD}",
        "{4EC84186-70BB-3121-9C1B-C63512D7126E}",
        "{1F7B9797-A3C8-3B98-85C4-00620F221CE8}",
        "{6BE0A7C7-3462-30EE-8B77-D21D7848D967}",
        "{BF58DC07-38AB-3887-8000-70173F9650EA}",
        "{D289009A-2728-3D0A-833E-F08E0E58934C}",
        "{9476DC14-00C3-3C36-A435-00D714CF77B8}",
        "{678835D7-D524-3C0E-9C33-1D3767FDA6BF}"
      };
      */
      
      if (Environment.Is64BitProcess)
      {
        return IsProductInstalled(strCodes_x64) /*|| IsProductInstalled(strCodes_IA64)*/;
      }
      else
      {
        return IsProductInstalled(strCodes_x86);  
      }
    }
    
    public static bool IsProductInstalled(string[] strCodes)
    {
      for (int i = 0;i < strCodes.Length; i++)
      {
        INSTALLSTATE state = MsiQueryProductState(strCodes[i]);
        if (state == INSTALLSTATE.INSTALLSTATE_LOCAL|| state == INSTALLSTATE.INSTALLSTATE_DEFAULT)
        {
          return true;
        }
      }
      
      return false;
    }
    
  }
}
