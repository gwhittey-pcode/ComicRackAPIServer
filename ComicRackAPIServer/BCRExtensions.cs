using cYo.Common.IO;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Viewer;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BCR
{
  public static class BCRExtensions
  {
    public static Response AsError(this IResponseFormatter formatter, HttpStatusCode statusCode, string message, Request request)
    {
        return new Response
            {
                StatusCode = statusCode,
                ContentType = "text/plain",
                Contents = stream => (new StreamWriter(stream) { AutoFlush = true }).Write("Request: " + request.Url + "\nError: " + message)
            };
    }
  
  
    public static List<ComicBook> GetFolderBookList(string folder, bool includeSubFolders)
    {
      List<ComicBook> list = new List<ComicBook>();
      try
      {
        IEnumerable<string> fileExtensions = Providers.Readers.GetFileExtensions();
        foreach (string file in FileUtility.GetFiles(folder, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, new string[0]))
        {
          string f = file;
          if (Enumerable.Any<string>(fileExtensions, (Func<string, bool>) (ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
          {
            ComicBook comicBook = Program.BookFactory.Create(file, CreateBookOption.AddToTemporary, list.Count > 100 ? RefreshInfoOptions.DontReadInformation : RefreshInfoOptions.None);
            if (comicBook != null)
              list.Add(comicBook);
          }
        }
      }
      catch
      {
      }
      return list;
    }
    
    public static List<string> GetFolderBookList2(string folder, bool includeSubFolders)
    {
      List<string> list = new List<string>();
      try
      {
        IEnumerable<string> fileExtensions = Providers.Readers.GetFileExtensions();
        foreach (string file in FileUtility.GetFiles(folder, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, new string[0]))
        {
          string f = file;
          if (Enumerable.Any<string>(fileExtensions, (Func<string, bool>) (ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
          {
            list.Add(file);
          }
        }
      }
      catch
      {
      }
      return list;
    }
  }
    
}
