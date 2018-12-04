using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;

namespace Uinty2glTF
{
    public static class PinYinConverter
    {
        public static string GetPinYin(string source)
        {
            string value = "";
            foreach (char c in source)
            {
                if (ChineseChar.IsValidChar(c))
                {
                    ChineseChar chineseChar = new ChineseChar(c);
                    value += Regex.Replace(chineseChar.Pinyins[0], @"\d", "").ToLower();
                }
                else
                {
                    value += c;
                }
            }
            return value;
        }
    }
}
