using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace sarw_rp.Utilities
{
    public static partial class Utilities
    {
        private static Image? ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            using (var ms = new MemoryStream(byteArray))
            {
                return Image.Load(ms);
            }
        }

        public static void SaveImageToDisk(byte[] byteArray)
        {
            var image = ByteArrayToImage(byteArray);
            if (image == null)
                return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid()+"SavedImage.jpeg");

            image.Save(filePath);
        }

    }
}
