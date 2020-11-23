using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Kztek_Library.Extensions
{
    public static class CommonExtension
    {
        public static string PasswordHashed(this string password, string passwordSalt)
        {
            string concatSaltAndPassword = string.Concat(password, passwordSalt);

            var securityKey = Encoding.UTF8.GetBytes(concatSaltAndPassword);

            var sha1 = System.Security.Cryptography.SHA1.Create();

            var hash = sha1.ComputeHash(securityKey);

            return BitConverter.ToString(hash).Replace("-", "");
        }

        public static string GetNormalizeString(this string str)
        {
            return str.Trim().Replace(' ', '-').Replace("\"", "");
        }

        public static string GetNormalize(this string str)
        {
            return str.Trim().Replace('-', '_');
        }

        public static string RemoveSpecialCharactersVn(this string str)
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

        public static string ReplaceSpaceToPlus(string text)
        {
            return Regex.Replace(text, @"\s+", "-").Trim();
        }

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
    }
}