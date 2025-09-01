namespace sarw_rp.Utilities
{

    public static partial class Utilities
    {
        public static async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
