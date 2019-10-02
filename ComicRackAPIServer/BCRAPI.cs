#define USE_GDI
//#define USE_FIB
//#define USE_DIB

using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Viewer;
using Nancy;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Imazen.WebP;
using System.IO.Compression;


namespace BCR
{
    public static class BCR
    {
        private static System.Object lockThis = new System.Object();


        public static IEnumerable<Comic> GetComicsForList(BCRUser user, Guid id)
        {
            var list = Program.Database.ComicLists.FindItem(id);
            if (list == null)
            {
                return Enumerable.Empty<Comic>();
            }

            return list.GetBooks().Select(x => x.ToComic(user));
        }

        public static IEnumerable<Series> GetSeriesAndVolumes()
        {
            return ComicRackAPIServer.Plugin.Application.GetLibraryBooks().AsSeries();
        }

        public static IEnumerable<Series> GetSeries()
        {
            return ComicRackAPIServer.Plugin.Application.GetLibraryBooks().AsSeries();
        }
        /*
         * 
         */
        public static IEnumerable<Comic> GetSeries(BCRUser user, Guid id, NancyContext context)
        {
            var books = ComicRackAPIServer.Plugin.Application.GetLibraryBooks();
            var book = books.Where(x => x.Id == id).First();
            var series = books.Where(x => x.ShadowSeries == book.ShadowSeries)
                .Where(x => x.ShadowVolume == book.ShadowVolume)
                .Select(x => x.ToComic(user))
                .OrderBy(x => x.ShadowNumber).ToList();

            int totalCount = 0;
            return context.ApplyODataUriFilter(series, ref totalCount).Cast<Comic>();
        }

        // Get all comics from a specific series
        public static IEnumerable<Comic> GetComicsFromSeries(BCRUser user, Guid id)
        {
            var books = ComicRackAPIServer.Plugin.Application.GetLibraryBooks();
            var book = books.Where(x => x.Id == id).First();
            var series = books.Where(x => x.ShadowSeries == book.ShadowSeries)
                .Where(x => x.ShadowVolume == book.ShadowVolume)
                .Select(x => x.ToComic(user))
                .OrderBy(x => x.ShadowVolume)
                .ThenBy(x => x.ShadowNumber).ToList();

            return series;
        }
        // Get all comics added within x days 
        public static IEnumerable<Comic> GetMostRecentlyAdded(BCRUser user, int days)
        {
            //comicrack format = "Added": "10/15/2018 10:51:25 AM"
            //string dateformat = "MM/dd/yyyy HH':'mm':'ss ";
            var books = ComicRackAPIServer.Plugin.Application.GetLibraryBooks();
            var recent = books.Where(x => x.AddedTime >= DateTime.Today.AddDays(-(days)));
            var list = recent.Select(x => x.ToComic(user))
                .OrderBy(x => x.Added).ToList();

            return list;
        }
        // Get all volumes from a specific series
        public static IEnumerable<int> GetVolumesFromSeries(Guid id)
        {
            var books = ComicRackAPIServer.Plugin.Application.GetLibraryBooks();
            var book = books.Where(x => x.Id == id).First();
            var volumes = books.Where(x => x.ShadowSeries == book.ShadowSeries).Select(x => x.ShadowVolume).Distinct();

            return volumes;
        }

        // Get all comics from a specific series and volume
        public static IEnumerable<Comic> GetComicsFromSeriesVolume(BCRUser user, Guid id, int volume)
        {
            var books = ComicRackAPIServer.Plugin.Application.GetLibraryBooks();
            var book = books.Where(x => x.Id == id).First();
            var series = books.Where(x => x.ShadowSeries == book.ShadowSeries)
                .Where(x => x.ShadowVolume == volume)
                .Select(x => x.ToComic(user))
                .OrderBy(x => x.ShadowNumber).ToList();

            return series;
        }


        public static MemoryStream GetBytesFromImage(Image image/*, bool progressive, int qualitylevel*/)
        {
            var bitmap = new Bitmap(image);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);

            stream.Position = 0;
            return stream;
        }

        private static Bitmap GetPageBitmap(Guid id, int page)
        {
            try
            {
                ComicBook comic = GetComics().First(x => x.Id == id);
                var index = comic.TranslatePageToImageIndex(page);
                var provider = GetProvider(comic);
                if (provider == null)
                {
                    return null;
                }
                return provider.GetImage(index); // ComicRack returns the page converted to a jpeg image.....
            }
            catch //(Exception e)
            {
                //MessageBox.Show(e.ToString());
                return null;
            }
        }
        public static byte[] GetPageImageBytes(Guid id, int page)
        {
            try
            {
                ComicBook comic = GetComics().First(x => x.Id == id);
                // Webcomics are not (yet) supported. If I don't filter them here, ComicRack hangs.
                if (comic.IsDynamicSource)
                    return null;

                var index = comic.TranslatePageToImageIndex(page);
                var provider = GetProvider(comic);
                if (provider == null)
                {
                    return null;
                }

                return provider.GetByteImage(index); // ComicRack returns the page converted to a jpeg image.....
            }
            catch //(Exception e)
            {
                //MessageBox.Show(e.ToString());
                return null;
            }
        }

        // Uses GDI+ for resizing.
        public static Bitmap Resize(Image img, int width, int height)
        {
            //create a new Bitmap the size of the new image
            Bitmap bmp = new Bitmap(width, height);
            //create a new graphic from the Bitmap
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //draw the newly resized image
            graphic.DrawImage(img, 0, 0, width, height);
            //dispose and free up the resources
            graphic.Dispose();
            //return the image
            return bmp;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static bool GetPageImageSize(Guid id, int page, ref int width, ref int height)
        {
            // Check if original image is in the cache.
            string filename = string.Format("{0}-p{1}.jpg", id, page);
            MemoryStream stream = ImageCache.Instance.LoadFromCache(filename, false, false);

            if (stream == null)
            {
                // Image is not in the cache, get it via ComicRack.
                var bytes = GetPageImageBytes(id, page);
                if (bytes == null)
                {
                    return false;
                }

                stream = new MemoryStream(bytes);

                // Always save the original page to the cache
                ImageCache.Instance.SaveToCache(filename, stream, false, false);
            }

            stream.Seek(0, SeekOrigin.Begin);

            using (Bitmap bitmap = new Bitmap(stream, false))
            {
                width = (int)bitmap.Width;
                height = (int)bitmap.Height;
            }

            stream.Dispose();
            return true;
        }

        public static Response GetPageImage(Guid id, int page, int width, int height, IResponseFormatter response)
        {
            // Restrict access to the FreeImage library to one thread at a time.
            lock (lockThis)
            {
                int max_width = 0;
                int max_height = 0;
                bool thumbnail = !(width == -1 && height == -1);
                bool processed = false;

                string filename = string.Format("{0}-p{1}-w{2}-h{3}.jpg", id, page, width, height);

                if (thumbnail)
                {
                    MemoryStream cachestream = ImageCache.Instance.LoadFromCache(filename, true, false);
                    // Cached thumbnails are assumed to be in the correct format and adhere to the size/format restrictions of the ipad.
                    if (cachestream != null)
                        return response.FromStream(cachestream, MimeTypes.GetMimeType(".jpg"));
                }
                else
                {
                    // Check if a processed (rescaled and/or progressive) image is cached.
                    string processed_filename = string.Format("{0}-p{1}-processed.jpg", id, page);
                    MemoryStream cachestream = ImageCache.Instance.LoadFromCache(processed_filename, false, false);
                    if (cachestream != null)
                        return response.FromStream(cachestream, MimeTypes.GetMimeType(".jpg"));
                }

                MemoryStream stream = null;

                // Check if original image is in the cache.
                string org_filename = string.Format("{0}-p{1}.jpg", id, page);
                stream = ImageCache.Instance.LoadFromCache(org_filename, false, false);

                if (stream == null)
                {
                    // Image is not in the cache, get it via ComicRack.
                    var bytes = GetPageImageBytes(id, page);
                    if (bytes == null)
                    {
                        return HttpStatusCode.NotFound;
                    }

                    stream = new MemoryStream(bytes);

                    // Always save the original page to the cache
                    ImageCache.Instance.SaveToCache(org_filename, stream, false, false);
                }

                stream.Seek(0, SeekOrigin.Begin);

                Bitmap bitmap = new Bitmap(stream, false);
                int bitmap_width = (int)bitmap.Width;
                int bitmap_height = (int)bitmap.Height;
                if (ImageCache.Instance.use_max_dimension)
                {
                    int mw, mh;

                    if (bitmap_width >= bitmap_height)
                    {
                        mw = ImageCache.Instance.max_dimension_long;
                        mh = ImageCache.Instance.max_dimension_short;
                    }
                    else
                    {
                        mw = ImageCache.Instance.max_dimension_short;
                        mh = ImageCache.Instance.max_dimension_long;
                    }

                    if (bitmap_width > mw || bitmap_height > mh)
                    {
                        double scaleW = (double)mw / (double)bitmap_width;
                        double scaleH = (double)mh / (double)bitmap_height;
                        double scale = Math.Min(scaleW, scaleH);

                        max_width = (int)Math.Floor(scale * bitmap_width);
                        max_height = (int)Math.Floor(scale * bitmap_height);
                    }
                    else
                    {
                        max_width = bitmap_width;
                        max_height = bitmap_height;
                    }
                }
                else
                // Check if the image dimensions exceeds the maximum image dimensions
                if ((bitmap_width * bitmap_height) > ImageCache.Instance.maximum_imagesize)
                {
                    max_width = (int)Math.Floor(Math.Sqrt((double)bitmap_width / (double)bitmap_height * (double)ImageCache.Instance.maximum_imagesize));
                    max_height = (int)Math.Floor((double)max_width * (double)bitmap_height / (double)bitmap_width);
                }
                else
                {
                    max_width = bitmap_width;
                    max_height = bitmap_height;
                }

                // Calculate the dimensions of the returned image.
                int result_width = width;
                int result_height = height;

                if (result_width == -1 && result_height == -1)
                {
                    result_width = max_width;
                    result_height = max_height;
                }
                else
                {
                    if (result_width == -1)
                    {
                        result_height = Math.Min(max_height, result_height);
                        double ratio = (double)result_height / (double)max_height;
                        result_width = (int)Math.Floor(((double)max_width * ratio));
                    }
                    else
                    if (result_height == -1)
                    {
                        result_width = Math.Min(max_width, result_width);
                        double ratio = (double)result_width / (double)max_width;
                        result_height = (int)Math.Floor(((double)max_height * ratio));
                    }
                }

                // TODO: do this per requesting target device instead of using one global setting.

                // Resize ?
                if (result_width != bitmap_width || result_height != bitmap_height)
                {
                    processed = true;
                    Bitmap resizedBitmap = Resize(bitmap, result_width, result_height);
                    bitmap.Dispose();
                    bitmap = resizedBitmap;
                    resizedBitmap = null;
                }


                // Check if the image must be converted to progressive jpeg(Not using this)
                if (ImageCache.Instance.use_progressive_jpeg && (result_width * result_height) >= ImageCache.Instance.progressive_jpeg_size_threshold)
                {
                    processed = true;

                    // Convert image to progressive jpeg

                    // FreeImage source code reveals that lower 7 bits of the FREE_IMAGE_SAVE_FLAGS enum are used for low-level quality control.
                    // FREE_IMAGE_SAVE_FLAGS quality = (FREE_IMAGE_SAVE_FLAGS)ImageCache.Instance.progressive_jpeg_quality;
                    //FREE_IMAGE_SAVE_FLAGS flags = FREE_IMAGE_SAVE_FLAGS.JPEG_SUBSAMPLING_444 | FREE_IMAGE_SAVE_FLAGS.JPEG_PROGRESSIVE | quality;

                    //FIBITMAP dib = FreeImage.CreateFromBitmap(bitmap);
                    //bitmap.Dispose();
                    //bitmap = null;
                    //stream.Dispose();
                    stream = new MemoryStream();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap = null;
                    //FreeImage.SaveToStream(dib, stream, FREE_IMAGE_FORMAT.FIF_JPEG, flags);
                    //FreeImage.Unload(dib);
                    // dib.SetNull();
                }
                else
                if (processed)
                {
                    // image was rescaled, make new stream with rescaled bitmap
                    stream = GetBytesFromImage(bitmap);
                    // For now, images that were resized because they exceeded the maximum dimensions are not saved to the cache.
                }

                if (bitmap != null)
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
                // Always save thumbnails to the cache
                if (thumbnail)
                {
                    ImageCache.Instance.SaveToCache(filename, stream, true, false);
                }
                else
                if (processed)
                {
                    // Store rescaled and/or progressive jpegs in the cache for now.
                    string processed_filename = string.Format("{0}-p{1}-processed.jpg", id, page);
                    ImageCache.Instance.SaveToCache(processed_filename, stream, false, false);
                }

                stream.Seek(0, SeekOrigin.Begin);
                return response.FromStream(stream, MimeTypes.GetMimeType(".jpg"));
            }
        }

        private static ImageProvider GetProvider(ComicBook comic)
        {
            var provider = comic.CreateImageProvider();
            if (provider == null)
            {
                return null;
            }
            if (provider.Status != ImageProviderStatus.Completed)
            {
                provider.Open(false);
            }
            return provider;
        }

        public static Comic GetComic(BCRUser user, Guid id)
        {
            try
            {
                var comic = GetComics().First(x => x.Id == id);
                return comic.ToComic(user);
            }
            catch//(Exception e)
            {
                //MessageBox.Show(e.ToString());
                return null;
            }
        }


        public static ComicBook GetComicBook(Guid id)
        {
            try
            {
                var comic = GetComics().First(x => x.Id == id);
                return comic;
            }
            catch//(Exception e)
            {
                //MessageBox.Show(e.ToString());
                return null;
            }
        }


        public static IQueryable<ComicBook> GetComics()
        {
            return ComicRackAPIServer.Plugin.Application.GetLibraryBooks().AsQueryable();
        }

        public static Response GetIcon(string key, IResponseFormatter response)
        {
            var image = ComicBook.PublisherIcons.GetImage(key);
            if (image == null)
            {
                return response.AsRedirect("/original/Views/spacer.png");
            }
            return response.FromStream(GetBytesFromImage(image), MimeTypes.GetMimeType(".jpg"));
        }

        public static IEnumerable<Publisher> GetPublishers()
        {
            return ComicRackAPIServer.Plugin.Application.GetLibraryBooks().GroupBy(x => x.Publisher).Select(x =>
            {
                return x.GroupBy(y => y.Imprint).Select(y => new Publisher { Name = x.Key, Imprint = y.Key });
            }).SelectMany(x => x).OrderBy(x => x.Imprint).OrderBy(x => x.Name);
        }

        public static IEnumerable<Series> GetSeries(string publisher, string imprint)
        {
            IEnumerable<ComicBook> comics;
            if (string.Compare(publisher, "no publisher", true) == 0)
            {
                comics = ComicRackAPIServer.Plugin.Application.GetLibraryBooks().Where(x => string.IsNullOrEmpty(x.Publisher));
            }
            else
            {
                comics = ComicRackAPIServer.Plugin.Application.GetLibraryBooks().Where(x => string.Compare(publisher, x.Publisher, true) == 0);
                if (string.IsNullOrEmpty(imprint))
                {
                    comics = comics.Where(x => string.IsNullOrEmpty(x.Imprint));
                }
                comics = comics.Where(x => string.Compare(imprint, x.Imprint, true) == 0);
            }
            return comics.AsSeries();
        }

        public static Response GetSyncWebp(Comic comic, Guid id, IResponseFormatter response)
        {

            string tmpPath = System.IO.Path.GetTempPath();
            var zipPath = comic.FilePath;
            string extractPath = tmpPath + "\\" + comic.Id + "\\";
            extractPath = Path.GetFullPath(extractPath);

            // Check if original image is in the cache.

            string fileName = Path.GetFileName(zipPath);
            MemoryStream cbz_stream = null;
            cbz_stream = ImageCache.Instance.LoadFromCache(fileName, false, true);

            if (cbz_stream == null)
            {

                // If directory does not exist, create it. 
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            string combined = Path.Combine(extractPath, entry.FullName);
                            entry.ExtractToFile(combined, true);


                        }
                    }
                }
                //System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                // Check if original image is in the cache.
                for (int i = 0; i <= comic.PageCount - 1; i++)
                {
                    MemoryStream stream = null;
                    string org_filename = string.Format("{0}-p{1}.jpg", id, i);
                    stream = ImageCache.Instance.LoadFromCache(org_filename, false, false);

                    if (stream == null)
                    {
                        // Image is not in the cache, get it via ComicRack.
                        var bytes = BCR.GetPageImageBytes(id, i);
                        if (bytes == null)
                        {
                            return HttpStatusCode.NotFound;
                        }

                        stream = new MemoryStream(bytes);

                        // Always save the original page to the cache
                        ImageCache.Instance.SaveToCache(org_filename, stream, false, false);
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    Bitmap image = new Bitmap(stream);
                    var result = i.ToString().PadLeft(5, '0');
                    string webpFileName = string.Format("P{0}.webp", result);
                    string combined = Path.Combine(extractPath, webpFileName);
                    Int32 webpquality = Database.Instance.GlobalSettings.webp_quality;
                    using (var saveImageStream = System.IO.File.Open(combined, FileMode.Create))
                    {
                        var encoder = new SimpleEncoder();
                        encoder.Encode(image, saveImageStream, webpquality);
                    }
                    stream.Dispose();

                }
                string zipName = tmpPath + "\\" + comic.Id + ".cbz";
                //check if zipfile exists if so delete it.

                try
                {
                    if (File.Exists(zipName))
                    {
                        File.Delete(zipName);
                    }
                    //Creates a new, blank zip file to work with - the file will be
                    //finalized when the using statement completes
                    using (ZipArchive newFile = ZipFile.Open(zipName, ZipArchiveMode.Create))
                    {
                        foreach (string file in Directory.GetFiles(extractPath))
                        {
                            newFile.CreateEntryFromFile(file, System.IO.Path.GetFileName(file));
                        }
                    }
                }
                catch (Exception)
                {
                    return HttpStatusCode.NotFound;
                }
                // Always save the original page to the cache
                var resp_stream = new MemoryStream(File.ReadAllBytes(zipName));
                ImageCache.Instance.SaveToCache(fileName, resp_stream, false, true);
                StreamResponse resp = new StreamResponse(() => resp_stream, "application/zip");
                return resp
                   .WithHeader("Content-Disposition", "attachment; filename=" + fileName)
                   .AsAttachment(fileName, "application/zip");
            }
            else
            {
                StreamResponse resp = new StreamResponse(() => cbz_stream, "application/zip");
                return resp
                   .WithHeader("Content-Disposition", "attachment; filename=" + fileName)
                   .AsAttachment(fileName, "application/zip");
            }

        }
    }

}
