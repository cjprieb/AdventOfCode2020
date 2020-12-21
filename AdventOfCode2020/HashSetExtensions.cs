using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2020
{
    public static class HashSetExtensions
    {
        #region Methods...

        #region AddAll
        public static HashSet<T> AddAll<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            foreach (var x in items)
            {
                @this.Add(x);
            }
            return @this;
        }
        #endregion AddAll

        #region RemoveAll
        public static HashSet<T> RemoveAll<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            foreach (var x in items)
            {
                @this.Remove(x);
            }
            return @this;
        }
        #endregion RemoveAll

        #endregion Methods...
    }
}
