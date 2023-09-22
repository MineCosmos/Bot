using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MineCosmos.Bot.Interactive
{
    internal class CommonFunction
    {
        public static string[] GetCommand(string text)
        {
            text = text
              .Replace("\r\n", " ")
              .Replace("\r", " ")
              .Replace("\n", " ")
              .Replace("!", string.Empty)
              .Replace("！", string.Empty);
            text = Regex.Replace(text, @"\s+", " ");
           return text.Split(' ');
        }
    }
}
