using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace sarw_rp.Utilities
{
    public static partial class Utilities
    {
        public static async Task<byte[]> CompressIFormFileToJpegAsync(
            IFormFile file,
            int quality = 40,          // 1–100 (lower = smaller)
            int? maxWidth = 1600,
            int? maxHeight = 1600
        )
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var input = file.OpenReadStream();
            using var image = await Image.LoadAsync(input);

            if (maxWidth.HasValue || maxHeight.HasValue)
            {
                var targetW = maxWidth ?? image.Width;
                var targetH = maxHeight ?? image.Height;
                var scale = Math.Min((float)targetW / image.Width, (float)targetH / image.Height);
                if (scale < 1f)
                {
                    var newW = Math.Max(1, (int)(image.Width * scale));
                    var newH = Math.Max(1, (int)(image.Height * scale));
                    image.Mutate(x => x.Resize(newW, newH));
                }
            }

            var encoder = new JpegEncoder
            {
                Quality = quality
            };


            await using var output = new MemoryStream();
            await image.SaveAsJpegAsync(output, encoder);
            return output.ToArray();
        }

        public static async Task<byte[]> DefaultJpegCompressionAsync(IFormFile file)
        {
            return await CompressIFormFileToJpegAsync(file, quality: 90, maxWidth: 1600, maxHeight: 1600);
        }
    }
}