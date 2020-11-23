using System;
using System.Security.Cryptography;

namespace Kztek_Library.Helpers
{
    public class FunctionHelper
    {
        public static string GetRandomNumericCharacters(int length)
        {
            // Note: i, o, l, 0, and 1 have been removed to reduce 
            // chances of user typos and mis-communication of passwords.
            char[] allowedCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            // Create a byte array to hold the random bytes.
            byte[] randomNumber = new byte[length];

            // Create a new instance of the RNGCryptoServiceProvider.
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            // Fill the array with a random value.
            Gen.GetBytes(randomNumber);

            string result = "";

            foreach (byte b in randomNumber)
            {
                // Convert the byte to an integer value to make the modulus operation easier.
                int rand = Convert.ToInt32(b);

                // Return the random number mod'ed.
                // This yeilds a possible value for each character in the allowable range.
                int value = rand % allowedCharacters.Length;

                char thisChar = allowedCharacters[value];

                result += thisChar;
            }

            return result;
        }
        public static string GetRandomAlphanumericCharacters(int length)
        {
            // Note: i, o, l, 0, and 1 have been removed to reduce 
            // chances of user typos and mis-communication of passwords.
            char[] allowedCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            // Create a byte array to hold the random bytes.
            byte[] randomNumber = new byte[length];

            // Create a new instance of the RNGCryptoServiceProvider.
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            // Fill the array with a random value.
            Gen.GetBytes(randomNumber);

            string result = "";

            foreach (byte b in randomNumber)
            {
                // Convert the byte to an integer value to make the modulus operation easier.
                int rand = Convert.ToInt32(b);

                // Return the random number mod'ed.
                // This yeilds a possible value for each character in the allowable range.
                int value = rand % allowedCharacters.Length;

                char thisChar = allowedCharacters[value];

                result += thisChar;
            }

            return result;
        }

        public static string GetStatusDateByDay(DateTime date)
        {
            var time = DateTime.Now;
            var status = "";

            //var k = DatetimeHelper.ConvertString_MMDDYYYY_ToDate(date.ToString("dd/MM/yyyy"));

            if (time < date)
            {
                var newDate = date.AddDays(-7);
                if (newDate > time)
                {
                    status = string.Format("<strong>{0}</strong>", date.ToString("dd/MM/yyyy HH:mm"));
                }
                else
                {
                    status = string.Format("<span class='label label-warning label-white middle'>{0}</span>", date.ToString("dd/MM/yyyy HH:mm"));
                }
            }
            else
            {
                status = string.Format("<span class='label label-danger label-white middle'>{0}</span>", date.ToString("dd/MM/yyyy HH:mm"));
            }

            return status;
        }

        public static string GetStatusDateByDay1(DateTime date)
        {
            var time = DateTime.Now;
            var status = "";

            var k = DatetimeHelper.ConvertString_DDMMYYYY_ToDate(date.ToString("MM/dd/yyyy"));

            if (time < k)
            {
                var newDate = k.AddDays(-7);
                if (newDate > time)
                {
                    status = string.Format("<strong>{0}</strong>", date);
                }
                else
                {
                    status = string.Format("<span class='label label-warning label-white middle'>{0}</span>", date);
                }
            }
            else
            {
                status = string.Format("<span class='label label-danger label-white middle'>{0}</span>", date);
            }

            return status;
        }

        public static string GetRandomCharacters(int length)
        {
            // Note: i, o, l, 0, and 1 have been removed to reduce 
            // chances of user typos and mis-communication of passwords.
            char[] allowedCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            // Create a byte array to hold the random bytes.
            byte[] randomNumber = new byte[length];

            // Create a new instance of the RNGCryptoServiceProvider.
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            // Fill the array with a random value.
            Gen.GetBytes(randomNumber);

            string result = "";

            foreach (byte b in randomNumber)
            {
                // Convert the byte to an integer value to make the modulus operation easier.
                int rand = Convert.ToInt32(b);

                // Return the random number mod'ed.
                // This yeilds a possible value for each character in the allowable range.
                int value = rand % allowedCharacters.Length;

                char thisChar = allowedCharacters[value];

                result += thisChar;
            }

            return result;
        }
    }
}