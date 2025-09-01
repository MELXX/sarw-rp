namespace sarw_rp.Utilities
{
    public static partial class Utilities
    {
        public static int SumByteArray(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return 0;

            int sum = 0;
            foreach (byte b in byteArray)
            {
                sum += b;
            }

            return sum;
        }

    }
}
