using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JLPTWeb.Helper
{
    public class CommonHelper
    {
        public static bool checkUnicode(string strValue)
        {
            string word = strValue.Trim();
            bool containUnicode = false;
            for (int x = 0; x < word.Length; x++)
            {
                if (char.GetUnicodeCategory(word[x]) == UnicodeCategory.OtherLetter)
                {
                    containUnicode = true;
                    break;
                }
            }
            if (containUnicode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int getCountryId(HttpCookie strValue)
        {
            if(strValue == null || strValue.Value == "VN")
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        // Replace dau tieng viet
        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}