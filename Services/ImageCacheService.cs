//using Microsoft.Extensions.Caching.Memory;
//using SixLabors.ImageSharp;
//using System.Runtime.Caching;

//namespace sarw_rp.Services
//{


//    public class ImageCacheService
//    {
//        private readonly System.Runtime.Caching.MemoryCache _cache = System.Runtime.Caching.MemoryCache.Default;

//        public Image ConvertAndCacheJpeg(byte[] imageBytes, string cacheKey)
//        {
//            if (imageBytes == null || imageBytes.Length == 0)
//                throw new ArgumentException("Image byte array is empty.");

//            using (var ms = new MemoryStream(imageBytes))
//            {
//                var image = Image.Load(ms);

//                // Ensure it's JPEG format (optional re-encoding)
//                using (var jpegStream = new MemoryStream())
//                {
//                    image.Save(SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(;
//                    var jpegImage = Image.Load(new MemoryStream(jpegStream.ToArray()));

//                    _cache.Set(cacheKey, jpegImage, DateTimeOffset.Now.AddMinutes(10));
//                    return jpegImage;
//                }
//            }
//        }

//        public Image GetCachedImage(string cacheKey)
//        {
//            return _cache.Get(cacheKey) as Image;
//        }
//    }
//}
