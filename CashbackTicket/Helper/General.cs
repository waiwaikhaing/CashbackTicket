

using System.Text;
using ZXing.Common;
using ZXing.QrCode;
using ZXing;

namespace CashbackTicket.Helper
{
    public class General
    {
        public static string FolderPath = "Log";
     
        public static void WriteLogInTextFile(string message)
        {
            Directory.CreateDirectory(FolderPath);
            string filePath = Path.Combine(FolderPath, $"log-{DateTime.Now.ToString("yyyy-MM-dd - hh tt")}.log");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            File.AppendAllText(filePath, sb.ToString());
        }

        public static string GeneratePromoCode()
        {
            string promoCode;
            Random random = new Random();

           
                // Generate 5 random alphabetic characters
                string letters = new string(Enumerable.Range(0, 5)
                    .Select(_ => (char)random.Next('A', 'Z' + 1))
                    .ToArray());

                // Generate 6 random digits
                string digits = random.Next(100000, 1000000).ToString();

                // Combine letters and digits
                promoCode = letters + digits;


            return promoCode;
        }


        #region generate QR code

        #endregion
    }
}
