using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class PascalCaseSplitter
    {
        public static string Split(string s)
        {
            StringBuilder toret = new StringBuilder();

            for (int x = 0; x < s.Length; x++)
            {
                if (char.IsUpper(s[x]))
                {
                    if (x - 1 > 0 && (char.IsLower(s[x - 1])))
                    {
                        toret.Append(" " + s[x]);
                        continue;
                    }
                    if (x - 1 > 0 && (char.IsUpper(s[x - 1]) && x + 1 < s.Length && char.IsLower(s[x + 1])))
                    {
                        toret.Append(" " + s[x]);
                        continue;
                    }
                }
                if (char.IsDigit(s[x]))
                {
                    if (x - 1 > 0 && (char.IsLetter(s[x - 1])))
                    {
                        toret.Append(" " + s[x]);
                        continue;
                    }
                }
                if (char.IsDigit(s[x]))
                {
                    if (x + 1 < s.Length && (char.IsLetter(s[x + 1])))
                    {
                        toret.Append(s[x] + " ");
                        continue;
                    }
                }

                toret.Append(s[x]);
            }

            for (int x = 0; x < toret.Length; x++)
            {
                if (char.IsLower(toret[x]))
                {
                    if ((x == 0) || (x > 0 && char.IsWhiteSpace(toret[x - 1])))
                    {
                        toret[x] = char.ToUpper(toret[x]);
                    }
                }
            }

            return toret.ToString();
        }
    }
}
