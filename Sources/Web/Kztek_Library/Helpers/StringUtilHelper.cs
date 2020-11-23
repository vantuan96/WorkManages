using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Kztek_Library.Helpers
{
    public class StringUtilHelper
    {
        public static string TrimLength(string text, int length)
        {
            string output = text;

            if (text.Length > length)
                output = text.Substring(0, length - 1) + "...";

            return output;
        }

        public static string RemoveSpace(string text)
        {
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        public static string TruncateWords(string text, int wordCount)
        {
            text = RemoveSpace(text);
            string output = string.Empty;

            if (text.Length > 0)
            {
                try
                {
                    string[] words = text.Split(' ');

                    if (words.Length < wordCount)
                    {
                        wordCount = words.Length;
                    }

                    for (int x = 0; x < wordCount; x++)
                    {
                        output += words[x] + " ";
                    }

                    if (words.Length > wordCount)
                    {
                        output = output.Trim() + "...";
                    }
                }
                catch { /* do nothing */ }
            }

            return output;
        }

        /// <summary>
        /// remove special characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// thay the ky tu dac biet thanh ky tu mong muon
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str, string rep)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-' || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(rep);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersVn(string str)
        {
            var strNotVn = RemoveSign4VietnameseString(str);
            var sb = new StringBuilder();
            foreach (char c in strNotVn)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ' || c == '_' || c == '-')
                {
                    sb.Append(c);
                }
            }
            var text = ReplaceSpaceToPlus(sb.ToString());
            return text;
        }

        public static string StripHTML(string htmlString)
        {
            return Regex.Replace(htmlString, @"<(.|\n)*?>", string.Empty, RegexOptions.Compiled);
        }

        public static string ReplaceSpaceToPlus(string text)
        {
            return Regex.Replace(text, @"\s+", "-").Trim();
        }

        public static string UrlSeo(string text)
        {
            return RemoveSpecialCharacters(ReplaceSpaceToPlus(text));
        }

        /// <summary>
        /// remove vietnamese sign
        /// </summary>
        private static readonly string[] VietnameseSigns = new[]
                                                               {
                                                                   "aAeEoOuUiIdDyY",
                                                                   "áàạảãâấầậẩẫăắằặẳẵ",
                                                                   "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                                                                   "éèẹẻẽêếềệểễ",
                                                                   "ÉÈẸẺẼÊẾỀỆỂỄ",
                                                                   "óòọỏõôốồộổỗơớờợởỡ",
                                                                   "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                                                                   "úùụủũưứừựửữ",
                                                                   "ÚÙỤỦŨƯỨỪỰỬỮ",
                                                                   "íìịỉĩ",
                                                                   "ÍÌỊỈĨ",
                                                                   "đ",
                                                                   "Đ",
                                                                   "ýỳỵỷỹ",
                                                                   "ÝỲỴỶỸ"
                                                               };

        public static string RemoveSign4VietnameseString(string str)
        {
            //remove wildcard
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        //get text from file
        public static string GetTextFile(string path)
        {
            var reader = new StreamReader(path, Encoding.Unicode);
            string strTextFile = reader.ReadToEnd();
            reader.Close();
            return strTextFile;
        }

        public static string CalculateTimeDiff(string startDate, string endDate)
        {
            var _startDate = System.Convert.ToDateTime(startDate);
            var _endDate = System.Convert.ToDateTime(endDate);

            var timeDiff = _endDate - _startDate;      

            int hour = timeDiff.Days > 0 ? (timeDiff.Hours + timeDiff.Days * 24) : timeDiff.Hours;          

            return string.Format("{0}h{1}m{2}s", hour, timeDiff.Minutes, timeDiff.Seconds);
        }
    }
}