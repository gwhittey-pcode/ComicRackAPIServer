using cYo.Projects.ComicRack.Engine;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BCR
{
  public class BCRUser : IUserIdentity
  {
    // Cache the entire list of comic progress, so we only need to perform one SQL lookup instead of
    // one per comic. This speeds up the displaying of a list of comics.
    public Dictionary<Guid, ComicProgress> comicProgress = new Dictionary<Guid, ComicProgress>();

    public Guid homeListId;

    public UserSettings settings = new UserSettings();

    public IEnumerable<string> Claims { get; set; }

    public string FullName { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; }

    public ComicProgress GetComicProgress(Guid comicId)
    {
      ComicProgress progress;
      if (comicProgress.TryGetValue(comicId, out progress))
      {
        return progress;
      }

      return null;
    }

    public UserSettings GetSettings()
    {
      return settings;
    }

    public void Initialize()
    {
      settings.Load(this);

      comicProgress.Clear();
      using (SQLiteDataReader reader = Database.Instance.ExecuteReader("SELECT id, comic_id, current_page, last_page_read, date_last_read FROM comic_progress WHERE user_id = " + UserId + ";"))
      {
        while (reader.Read())
        {
          ComicProgress progress = new ComicProgress();
          progress.DatabaseId = reader.GetInt32(0);
          progress.Id = new Guid(reader.GetString(1));
          progress.CurrentPage = reader.GetInt32(2);
          progress.LastPageRead = reader.GetInt32(3);
          progress.DateLastRead = reader.GetString(4);
          comicProgress[progress.Id] = progress;
        }
      }
    }

    public void ResetComicProgress(Guid comicId)
    {
      comicProgress.Remove(comicId);
      Database.Instance.ExecuteNonQuery("DELETE FROM comic_progress WHERE comic_id = '" + comicId + "' AND user_id = " + UserId + ";");
    }

    public bool SetAccessLevel()
    {
      return false;
    }

    public bool SetPassword()
    {
      return false;
    }

    public void UpdateComicProgress(ComicBook comicBook, int currentPage)
    {
      if (comicBook == null)
        return;

      ComicProgress progress;
      if (comicProgress.TryGetValue(comicBook.Id, out progress))
      {
        if (currentPage > progress.LastPageRead)
          progress.LastPageRead = currentPage;

        progress.CurrentPage = currentPage;
        progress.DateLastRead = System.DateTime.Now.ToString("s");
        Database.Instance.ExecuteNonQuery("UPDATE comic_progress SET current_page = " + progress.CurrentPage + ", last_page_read = " + progress.LastPageRead + ", date_last_read = '" + progress.DateLastRead + "' WHERE id = " + progress.DatabaseId);
      }
      else
      {
        progress = new ComicProgress();
        progress.Id = comicBook.Id;
        progress.LastPageRead = currentPage;
        progress.CurrentPage = currentPage;
        progress.DateLastRead = System.DateTime.Now.ToString("s");

        Database.Instance.ExecuteNonQuery("INSERT INTO comic_progress (user_id, comic_id, current_page, last_page_read, date_last_read) VALUES(" + UserId + ", '" + progress.Id.ToString() + "', " + progress.CurrentPage + ", " + progress.LastPageRead + ", '" + progress.DateLastRead + "');");

        progress.DatabaseId = (int)Database.Instance.GetLastInsertRowId();
        comicProgress[comicBook.Id] = progress;
      }

      if (settings.use_comicrack_progress)
      {
        // Save the progess to the ComicRack database as well
        comicBook.CurrentPage = currentPage;
        if (comicBook.LastPageRead < currentPage)
          comicBook.LastPageRead = currentPage;

      }
    }

    public void UpdateSettings(UserSettings settings)
    {
      this.settings = settings;
      settings.Save(this);
    }
    /*
    public Guid GetHomeList()
    {
      object homeListId = Database.Instance.ExecuteScalar("SELECT home_list_id FROM user_settings WHERE user_id = " + UserId + " LIMIT 1;");
      if (homeListId != null)
      {
        Guid listId = new Guid(homeListId.ToString());

        // Check if list still exists, if not, return main library.
        var list = Program.Database.ComicLists.FindItem(listId);
        if (list == null)
        {
          // Home list no longer exists, return main library.
          string s = "cYo.Projects.ComicRack.Engine.Database.ComicLibraryListItem";
          ComicListItem item = Program.Database.ComicLists.GetItems<ComicListItem>(false).FirstOrDefault((ComicListItem cli) => cli.GetType().ToString() == s);
          if (item != null)
          {
            listId = item.Id;
          }
        }

        return listId;
      }
    }
    */
  }
}