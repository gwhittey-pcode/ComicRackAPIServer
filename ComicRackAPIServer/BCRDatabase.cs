using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Viewer;
using System;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace BCR
{

  
  /// <summary>
  /// Description of Database.
  /// </summary>
  public sealed class Database : IDisposable
  {
    private const int COMIC_DB_VERSION = 1;
    
    private SQLiteConnection mConnection;
    private string mFolder;
    private const string DIRECTORY = "ComicRack_ComicRackAPIServer";
    
    private static Database instance = new Database();
    private int mVersion = 0;
    private GlobalSettings _globalSettings = new GlobalSettings();
    
    private Guid libraryGuid = Guid.Empty;
    private Guid bcrGuid = Guid.Empty;
    
    public GlobalSettings GlobalSettings { get { return _globalSettings; } }
    
    
    public static Database Instance 
    {
      get { return instance; }
    }
    
    public static string ConfigurationFolder { get { return DIRECTORY; } }
    
    public Database()
    {
      mFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DIRECTORY);
      if (!Directory.Exists(mFolder))
      {
    	  Directory.CreateDirectory(mFolder);
      }
      
      string s = "cYo.Projects.ComicRack.Engine.Database.ComicLibraryListItem";
      ComicListItem item = Program.Database.ComicLists.GetItems<ComicListItem>(false).FirstOrDefault((ComicListItem cli) => cli.GetType().ToString() == s);
      if (item != null)
      {
        libraryGuid = item.Id;
      }
      
      s = "[BCR Users]";
      item = Program.Database.ComicLists.GetItems<ComicListItem>(false).FirstOrDefault((ComicListItem cli) => cli.Name == s);
      if (item == null)
      {
        // Add it
        ComicListItemFolder bcrFolder = new ComicListItemFolder(s);
        ((ComicLibrary)Program.Database).ComicLists.Add(bcrFolder);
        item = Program.Database.ComicLists.GetItems<ComicListItem>(false).FirstOrDefault((ComicListItem cli) => cli.Name == s);
      }
      
      if (item != null)
      {
        bcrGuid = item.Id;
      }
    }
    
    
    public void Initialize()
    {
      try 
      {
        mConnection = new SQLiteConnection(@"Data Source=" + mFolder + "\\bcr.s3db");
        mConnection.Open();
      }
      catch (System.DllNotFoundException e)
      {
        Trace.WriteLine(String.Format("Exception: {0}", e));
        MessageBox.Show("SQLite.Interop.dll seems to be missing. Aborting.", "ComicRackAPIServer Plugin", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      catch (SQLiteException e)
      {
        Trace.WriteLine(String.Format("Exception: {0}", e));
        // error while opening/creating database
        mConnection = null;

        Trace.WriteLine("Failed to create/open the BCR database:");
        Trace.WriteLine(e.ToString());
        return;
      }
      
      // Check if the database is initialized by checking if the settings table exists.
      object name = ExecuteScalar("SELECT name FROM sqlite_master WHERE type='table' AND name='settings';");
      if (name == null)
      {
        // No settings table.
        // Create the entire database.
        mVersion = 0;
      }
      else
      {
        // Read version so we know if we must do a database update.
        object version = ExecuteScalar("SELECT value FROM settings WHERE key='version';");
        mVersion = Convert.ToInt32(version);
      }
          
      if (mVersion < 1)
      {
        // Create the database
        using (SQLiteTransaction transaction = mConnection.BeginTransaction())
        {
          ExecuteNonQuery("CREATE TABLE settings(key TEXT PRIMARY KEY NOT NULL, value TEXT);");
          ExecuteNonQuery("INSERT INTO settings (key,value) VALUES ('version','" + COMIC_DB_VERSION + "');");
          ExecuteNonQuery("INSERT INTO settings (key,value) VALUES ('port','8080');");
          
          ExecuteNonQuery(@"CREATE TABLE user(
            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            username TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL,
            salt TEXT NOT NULL,
            activity INTEGER NOT NULL DEFAULT (CURRENT_TIMESTAMP), 
            created INTEGER NOT NULL DEFAULT (CURRENT_TIMESTAMP), 
            fullname TEXT DEFAULT ''
            );");
    
          ExecuteNonQuery(@"CREATE TABLE user_settings(
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            open_current_comic_at_launch INTEGER DEFAULT 1,
            open_next_comic INTEGER DEFAULT 1,
            page_fit_mode INTEGER DEFAULT 1,
            zoom_on_tap INTEGER DEFAULT 1,
            toggle_paging_bar INTEGER DEFAULT 2,
            use_page_turn_drag INTEGER DEFAULT 1,
            page_turn_drag_threshold INTEGER DEFAULT 75,
            use_page_change_area INTEGER DEFAULT 1,
            page_change_area_width INTEGER DEFAULT 50,
            use_comicrack_progress INTEGER DEFAULT 0,
            home_list_id TEXT DEFAULT ''
            );");
          
          
          /*
          ExecuteNonQuery(@"CREATE TABLE user_custom_settings(
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            key TEXT NOT NULL, 
            value TEXT
            );");
          */
          
          ExecuteNonQuery(@"CREATE TABLE user_apikeys(
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            apikey TEXT NOT NULL, 
            created INTEGER NOT NULL DEFAULT (CURRENT_TIMESTAMP),
            activity INTEGER NOT NULL DEFAULT (CURRENT_TIMESTAMP)
            );");
          
         
          // TODO: set hook on the deletion of a comic book from the library
          ExecuteNonQuery(@"CREATE TABLE comic_progress(
            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            comic_id TEXT NOT NULL, 
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            date_last_read TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
            current_page INTEGER DEFAULT 0,
            last_page_read INTEGER DEFAULT 0
            );");
         
          
         
          /*
          
          ExecuteNonQuery(@"CREATE TABLE comic_favorites(
            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            comic_id TEXT NOT NULL
            );");
            
          ExecuteNonQuery(@"CREATE TABLE series_favorites(
            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            series TEXT NOT NULL
            );");
            
            
          // type, favorite:
          // 0, comic guid
          // 1, series name
          // 2, writer/colorer etc name
          // 3, character name
          ExecuteNonQuery(@"CREATE TABLE favorites(
            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            user_id INTEGER NOT NULL REFERENCES user(id) ON DELETE CASCADE,
            favorite TEXT NOT NULL,
            type INTEGER
            );");  
          */
          
          
          // Automatically create a user_settings record when a user is added.
          ExecuteNonQuery("CREATE TRIGGER AddUserSettingsTrigger AFTER INSERT ON user BEGIN INSERT INTO user_settings (user_id) VALUES (NEW.id); END;");
          // Automatically invalidate all user sessions when the user changes its username or password
          ExecuteNonQuery("CREATE TRIGGER InvalidateApiKeys AFTER UPDATE ON user WHEN (NEW.username != OLD.username) OR (NEW.password != OLD.password) OR (NEW.salt != OLD.salt)  BEGIN DELETE FROM user_apikeys WHERE user_id=NEW.id; END;");
          
          // Create default user
          UserDatabase.AddUser("user", "password");
          
         
          transaction.Commit();
        }
      }
      
    
      if (mVersion < COMIC_DB_VERSION)
      {
        ExecuteNonQuery("UPDATE settings SET value='" + COMIC_DB_VERSION + "' WHERE key='version';");
      }
      
      GlobalSettings.Initialize();
      
      Validate();
    }
  
    
    /// <summary>
    /// Check if the database contains invalid references to data in the ComicRack database.
    /// Remove those references.
    /// </summary>
    private void Validate()
    {
      // Check if the lists referenced by users still exist.
      // Check if the comics referenced by users still exist.
      // TODO: provide user feedback in startup screen of BCR ?
    }
    
    
    public long GetLastInsertRowId()
    {
      return mConnection.LastInsertRowId;
    }
    
    /// <summary>
    /// Simple wrapper
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>number of affected rows</returns>
    public int ExecuteNonQuery(string sql)
    {
      using (SQLiteCommand command = mConnection.CreateCommand()) 
      {
        command.CommandText = sql;
        return command.ExecuteNonQuery();
      }
    }
    
    /// <summary>
    /// Simple wrapper
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>First column of first row of the query result.</returns>
    public object ExecuteScalar(string sql)
    {
      using (SQLiteCommand command = mConnection.CreateCommand()) 
      {
        command.CommandText = sql;
        return command.ExecuteScalar();
      }
    }
    
    
    /// <summary>
    /// Simple wrapper
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>SQLiteDataReader with the query result.</returns>
    public SQLiteDataReader ExecuteReader(string sql)
    {
      using (SQLiteCommand command = mConnection.CreateCommand()) 
      {
        command.CommandText = sql;
        return command.ExecuteReader();
      }
    }
    
    /// <summary>
    /// Executes a query and returns the first row.
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>The first row of the query result.</returns>
    public NameValueCollection QuerySingle(string sql)
    {
      using (SQLiteCommand command = mConnection.CreateCommand()) 
      {
        command.CommandText = sql;
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            return reader.GetValues();
          }
        }
      }
      
      return null;
    }
    

    public void Dispose()
    {
      if (mConnection != null)
      {
        mConnection.Dispose();
        mConnection = null;
      }
    }

    ~Database()
    {
      Dispose();
    }

  }
}
