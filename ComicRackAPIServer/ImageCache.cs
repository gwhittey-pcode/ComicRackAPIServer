/*
 * Created by SharpDevelop.
 * User: jeroen
 * Date: 3/18/2013
 * Time: 10:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;

namespace BCR
{
  public sealed class ImageCache //: GlobalSettings
    {
        private const string DIRECTORY = "ComicRack BCR";
        
        private static string folder;
        private static string cache_folder;
        private static string thumbnail_folder;
        
        public int cache_size { get; set; }
        
        // Image conversion settings
        // If use_progressive_jpeg is enabled, images are recompressed to progressive jpeg format.
        // This is done, because the iPad (IOS 5 and lower) have a limit of 2 megapixels on images in normal jpeg format.
        // All normal jpeg images larger than 2 MP are subsampled to abominable quality.
        // Subsampling doesn't occur for progressive jpeg and for png file.
        // IOS 6 should have a larger limit of 5 MP, which will still be too small for HD comics.
        // As the images will be recompressed, some quality loss may occur, but it's way better than subsampling.
        public bool use_progressive_jpeg { get; set; }
        public int progressive_jpeg_quality { get; set; }
        public int progressive_jpeg_size_threshold { get; set; }
        
        // Maximum image dimensions
        public int maximum_imagesize { get; set; }
        
        public bool use_max_dimension { get; set; }
        public int max_dimension_long { get; set; }
        public int max_dimension_short { get; set; }
                
        
        
        //private static int max_image_dimension = 3072; // max ipad image size is 4096x3072 ?

        private static ImageCache instance = new ImageCache();
    
        public static ImageCache Instance 
        {
          get { return instance; }
        }
        
        private ImageCache()
        { 
          cache_size = 1024; // MB 
          // TODO: make this a user setting?
          // Maximum image dimensions for images.
          // If you never zoom in, then set this to the size of your tablet screen, e.g. 2048x1536 for ipad 3
          use_max_dimension = false;
          max_dimension_long = 4096;
          max_dimension_short = 3072; 
          

          maximum_imagesize = 5*1024*1024; // IOS5 : 5 megapixels
                    
          use_progressive_jpeg = true;
          progressive_jpeg_size_threshold = 2*1024*1024; // 2 megapixels
          progressive_jpeg_quality = 90; // 10..100 %
          
          folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DIRECTORY);
          if (!Directory.Exists(folder))
          {
        	  Directory.CreateDirectory(folder);
          }
          
          cache_folder = folder + "\\cache\\";
          if (!Directory.Exists(cache_folder))
          {
        	  Directory.CreateDirectory(cache_folder);
          }
          
          thumbnail_folder = folder + "\\cache\\thumbnails\\";
          if (!Directory.Exists(thumbnail_folder))
          {
        	  Directory.CreateDirectory(thumbnail_folder);
          }
          
        }

        
        public MemoryStream LoadFromCache(string filename, bool thumbnail)
        {
          if (cache_size <= 0)
            return null;
          
          try 
          {
            byte[] content = File.ReadAllBytes((thumbnail ? thumbnail_folder : cache_folder) + filename);
            MemoryStream stream = new MemoryStream(content);
            return stream;
          }
          catch
          {
            return null;
          }
        }
        
        public void SaveToCache(string filename, MemoryStream image, bool thumbnail)
        {
          if (cache_size <= 0)
            return;
          
          try 
          {
            CheckCache();

            using (FileStream file = new FileStream((thumbnail ? thumbnail_folder : cache_folder) + filename, FileMode.Create, FileAccess.Write))
            {
              image.WriteTo(file);
            }
          }
          catch//(Exception e)
          {
            // ignore....
          }
        }
        
        public void ClearPageCache()
        {
          DirectoryInfo d = new DirectoryInfo(cache_folder);
          
          FileInfo[] fis = d.GetFiles();
          
          foreach (FileInfo fi in fis) 
          {      
            File.Delete(fi.FullName);
          }
        }
        
        public void ClearThumbnailsCache()
        {
          DirectoryInfo d = new DirectoryInfo(thumbnail_folder);
          
          FileInfo[] fis = d.GetFiles();
          
          foreach (FileInfo fi in fis) 
          {      
            File.Delete(fi.FullName);
          }
        }
        
        private static int CompareFileDate(FileInfo x, FileInfo y)
        {
          if (x.CreationTimeUtc == y.CreationTimeUtc)
            return 0;
          return (x.CreationTimeUtc < y.CreationTimeUtc) ? -1 : 1;
        }
          
          
        public void CheckCache()
        {
          // If cache is larger than allowed, delete oldest files first
          DirectoryInfo d = new DirectoryInfo(cache_folder);
          
          long Size = 0;    
          // Add file sizes.
          FileInfo[] fis = d.GetFiles();
          
          foreach (FileInfo fi in fis) 
          {      
            Size += fi.Length;
          }
          
          long maxsize = (long)cache_size * 1024*1024;
          if (Size > maxsize)
          {
            Array.Sort(fis, CompareFileDate);
            foreach (FileInfo fi in fis) 
            {      
              File.Delete(fi.FullName);
              Size -= fi.Length;
              
              if (Size < maxsize)
                break;
            } 
          }
        }
        
        public long GetPageCacheSize()
        {
          DirectoryInfo d = new DirectoryInfo(cache_folder);
          
          long Size = 0;    
          // Add file sizes.
          FileInfo[] fis = d.GetFiles();
          
          foreach (FileInfo fi in fis) 
          {      
            Size += fi.Length;
          }
          
          return Size;
        }
        
        public long GetThumbnailsCacheSize()
        {
          DirectoryInfo d = new DirectoryInfo(thumbnail_folder);
          
          long Size = 0;    
          // Add file sizes.
          FileInfo[] fis = d.GetFiles();
          
          foreach (FileInfo fi in fis) 
          {      
            Size += fi.Length;
          }
          
          return Size;
        }
    }
}