using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BCR
{
  public static class EntityExtensions
  {
    public static IEnumerable<Series> AsSeries(this IEnumerable<ComicBook> comics)
    {
      return comics.Select(x => x.ToSeries()).Distinct();
    }

    public static Comic ToComic(this ComicBook x, BCRUser user)
    {
      return new Comic(x, user);
    }

    public static ComicList ToComicList(this ComicListItem x, int depth = -1)
    {
      ComicList list = new ComicList
      {
        Name = x.Name,
        Id = x.Id,
        ListsCount = 0,
        Type = x.GetType().ToString().Split('.').LastOrDefault()
      };

      ComicListItemFolder folderList = x as ComicListItemFolder;
      if (folderList != null)
      {
        list.ListsCount = folderList.Items.Count;
        // recurse ?
        if (depth != 0)
        {
          list.Lists = folderList.Items.Select(c => c.ToComicList(depth - 1));
        }
      }

      return list;
    }
    public static Series ToSeries(this ComicBook x)
    {
      return new Series
              {
                Title = x.ShadowSeries,
                Volume = x.ShadowVolume,
                Id = x.Id,
              };
    }
  }

  /// <summary>
  /// Wraps ComicBook so only desired properties are visible for querying and user specific info is
  /// available.
  ///
  /// </summary>
  public class Comic
  {
    #region Fields

    // using Lazy<> is almost twice as slow as using a string directly when sorting all comics on
    // the Caption field.
    private string _caption = null;

    private ComicBook book;
    private ComicProgress progress;
    private bool useComicrackProgress = false;

    // Cache expensive properties
    // Drawback: any updates to the original ComicBook will not be propagated.
    // However, this Comic class is not meant to be used for a long time. It is used for providing a
    // json serializable class for returning search results.

    //private readonly Lazy<string> _caption = null;
    //public string Caption { get { return _caption.Value; } }
    #endregion Fields

    #region Constructors

    public Comic(ComicBook source, BCRUser user)
    {
      book = source;
      useComicrackProgress = user.settings.use_comicrack_progress;
      progress = useComicrackProgress ? null : user.GetComicProgress(source.Id);

      //_caption = new Lazy<string>(() => { return book.Caption; });
    }

        #endregion Constructors

        #region Wrapped ComicBook properties



        public string Added { get { return book.AddedTimeAsText; } }
        public int AlternateCount { get { return book.AlternateCount; } }

    public string AlternateSeries { get { return book.AlternateSeries; } }

    public string Caption
    {
      get
      {
        if (_caption == null)
          _caption = book.Caption;

        return _caption;
      }
    }
    public string Manga { get { return book.MangaAsText; } }

    public string Characters { get { return book.Characters; } }

    public string Colorist { get { return book.Colorist; } }

    public int Count { get { return book.Count; } }

    public string CoverArtist { get { return book.CoverArtist; } }

    public int CurrentPage { get { return book.CurrentPage; } }

    public string Editor { get { return book.Editor; } }

    public string FilePath { get { return book.FilePath; } }
    public string Format { get { return book.Format; } }

    public string Genre { get { return book.Genre; } }

    public Guid Id { get { return book.Id; } }

    public string Imprint { get { return book.Imprint; } }

    public string Inker { get { return book.Inker; } }

    public int LastPageRead { get { return book.LastPageRead; } }

    public string Letterer { get { return book.Letterer; } }

    public string Locations { get { return book.Locations; } }

    public int Month { get { return book.Month; } }

    public string Notes { get { return book.Notes; } }

    public string Number { get { return book.Number; } }

    public DateTime OpenedTime { get { return book.OpenedTime; } }

    public string OpenedTimeAsText { get { return book.OpenedTimeAsText; } }

    public int PageCount { get { return book.PageCount; } }

    public string Penciller { get { return book.Penciller; } }

    public DateTime Published { get { return book.Published; } }

    public string PublishedAsText { get { return book.Published.ToString("s"); } }

    public string Publisher { get { return book.Publisher; } }

    public float Rating { get { return book.Rating; } }

    public string ScanInformation { get { return book.ScanInformation; } }

    public string Series { get { return book.Series; } }

    public int ShadowCount { get { return book.ShadowCount; } }

    public string ShadowFormat { get { return book.ShadowFormat; } }

    public string ShadowNumber { get { return book.ShadowNumber; } }

    // Shadow properties: These are values taken from the filename of the eComic (these are
    // displayed in light gray in ComicRack)
    public string ShadowSeries { get { return book.ShadowSeries; } }
        public string Pages { get { return book.PagesAsText; } }
        public string ShadowTitle { get { return book.ShadowTitle; } }

    public int ShadowVolume { get { return book.ShadowVolume; } }

    public int ShadowYear { get { return book.ShadowYear; } }

    public string Summary { get { return book.Summary; } }

    public string Tags { get { return book.Tags; } }

    public string Teams { get { return book.Teams; } }

    public string Title { get { return book.Title; } }

    public int Volume { get { return book.Volume; } }
    public string Web { get { return book.Web; } }

    public string Writer { get { return book.Writer; } }

    public int Year { get { return book.Year; } }
    #endregion Wrapped ComicBook properties

    #region User specific properties

    public int UserCurrentPage { get { return useComicrackProgress ? book.CurrentPage : (progress == null ? 0 : progress.CurrentPage); } }

    public int UserLastPageRead { get { return useComicrackProgress ? book.LastPageRead : (progress == null ? 0 : progress.LastPageRead); } }

    public string UserOpenedTimeAsText { get { return useComicrackProgress ? (book.OpenedTimeAsText == "never" ? "" : book.OpenedTimeAsText) : (progress == null ? "" : progress.DateLastRead); } }
    #endregion User specific properties
  }

  /// <summary>
  /// Allows natural sorting on all string members of Comic.
  /// </summary>
  public class ComicComparer : IComparer<Comic>
  {
    private readonly bool ascending;
    private readonly IComparer comparer;
    private readonly string field;
    private readonly PropertyInfo fieldPropertyInfo;
    private readonly Type fieldType;
    public ComicComparer(string field, bool ascending)
    {
      this.field = field;
      this.ascending = ascending;

      fieldPropertyInfo = typeof(Comic).GetProperty(field);
      fieldType = fieldPropertyInfo.PropertyType;

      if (fieldType == typeof(String))
      {
        //comparer = new LogicalStringComparer(true);
        comparer = new NaturalSortComparer(true);
      }
      else
      {
        comparer = Comparer<object>.Default;
      }
    }

    public int Compare(Comic x, Comic y)
    {
      int result = comparer.Compare(fieldPropertyInfo.GetValue(x, null), fieldPropertyInfo.GetValue(y, null));
      return ascending ? result : -result;
    }
  }

  // (Smart/Folder/Item) list
  public class ComicList
  {
    public Guid Id { get; set; }

    public IEnumerable<ComicList> Lists { get; set; }

    public int ListsCount { get; set; }

    public string Name { get; set; }
    public string Type { get; set; }
  }

  public class ComicProgress
  {
    public int CurrentPage { get; set; }

    public int DatabaseId { get; set; }

    public string DateLastRead { get; set; }

    public Guid Id { get; set; }
    public int LastPageRead { get; set; }
  }

  public class Publisher
  {
    public string Imprint { get; set; }

    public string Name { get; set; }

    public string Count { get; set; }
    }

  public class Series : IEquatable<Series>
  {
    public int Count { get; set; }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public int Volume { get; set; }
    public bool Equals(Series other)
    {
      return Title.Equals(other.Title) && (Volume.Equals(other.Volume));
    }

    public override bool Equals(object obj)
    {
      var series = obj as Series;
      if (series == null)
      {
        return false;
      }
      return Equals(series);
    }

    public override int GetHashCode()
    {
      return Title.GetHashCode() ^ Volume.GetHashCode() * 29;
    }
  }

  public class SeriesVolume : IEquatable<SeriesVolume>
  {
    public int Count { get; set; }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public int Volume { get; set; }
    public bool Equals(SeriesVolume other)
    {
      return Title.Equals(other.Title) && (Volume.Equals(other.Volume));
    }

    public override bool Equals(object obj)
    {
      var series = obj as SeriesVolume;
      if (series == null)
      {
        return false;
      }
      return Equals(series);
    }

    public override int GetHashCode()
    {
      return Title.GetHashCode() ^ Volume.GetHashCode() * 29;
    }
  }
  public class SortSettings
  {
    public bool Direction1 { get; set; }

    public bool Direction2 { get; set; }

    public string OrderBy1 { get; set; }
    public string OrderBy2 { get; set; }
  }
}