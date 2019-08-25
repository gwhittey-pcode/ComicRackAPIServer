using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BCR
{

  public class GlobalSettings
  {
    public bool nancy_request_tracing { get; set; }
    public string nancy_diagnostics_password { get; set; }
    public int webserver_port { get; set; }
    public string url_base { get; set; }
            
    private Dictionary<string, string> mSettings = new Dictionary<string, string>();
    // TODO: maximum image size should be per requesting target device instead of using one global setting.

      
    public GlobalSettings()
    {
      nancy_request_tracing = false;
      nancy_diagnostics_password = "diagnostics";
      webserver_port = 8080;
      url_base = "tablet";
    }
    
    /// <summary>
    /// Read global settings.   
    /// </summary>
    public void Initialize()
    {
      mSettings.Clear();

      using (SQLiteDataReader reader = Database.Instance.ExecuteReader("SELECT key, value FROM settings;"))
      {
        while (reader.Read())
        {
          mSettings[reader["key"].ToString()] = reader["value"].ToString();
        }
      }
      
      webserver_port = GetInt32("webserver_port", 8080);
      nancy_request_tracing = GetBoolean("nancy_request_tracing", false);
      nancy_diagnostics_password = GetString("nancy_diagnostics_password", "diagnostics");
      url_base = GetString("url_base", "tablet");
    }
    
    public void Save()
    {
      // Save to SQLite
      Set("webserver_port", webserver_port.ToString());
      Set("nancy_request_tracing", nancy_request_tracing.ToString());
      Set("nancy_diagnostics_password", nancy_diagnostics_password.ToString());
      Set("url_base", url_base);
    }
    
    
    public string GetString(string key, string defaultValue)
    {
      string value;
      if (mSettings.TryGetValue(key, out value))
        return value;
      
      Set(key, defaultValue);
      return defaultValue;
    }
    
    public int GetInt32(string key, int defaultValue)
    {
      string s;
      if (mSettings.TryGetValue(key, out s))
      {
        return Convert.ToInt32(s);
      }
      
      Set(key, defaultValue.ToString());
      return defaultValue;
    }
    
    public bool GetBoolean(string key, bool defaultValue)
    {
      string s;
      if (mSettings.TryGetValue(key, out s))
      {
        return Convert.ToBoolean(s);
      }
      
      Set(key, defaultValue.ToString());
      return defaultValue;
    }
    
    public void Set(string key, string value)
    {
      string test;
      if (mSettings.TryGetValue(key, out test))
      {
        Database.Instance.ExecuteNonQuery("UPDATE settings SET value='" + value + "' WHERE key='"+key+"';");
      }
      else
      {
        Database.Instance.ExecuteNonQuery("INSERT INTO settings (key,value) VALUES ('"+key+"','"+value+"');");
      }
      
      mSettings[key] = value;
    }
    
    
  }

     
    
}
