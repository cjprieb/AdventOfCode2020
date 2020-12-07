using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2020
{
    public class Input
    {
        #region Methods...

        #region GetLines
        public static IEnumerable<string> GetLines(string input) => input.Split('\n').Select(s => s.Trim());
        #endregion GetLines

        #endregion Methods...
    }
}
