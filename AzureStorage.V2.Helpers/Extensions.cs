using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorage.V2.Helpers
{
    public static class Extensions
    {
        //https://stackoverflow.com/questions/11463734/split-a-list-into-smaller-lists-of-n-size
        public static IEnumerable<List<T>> SplitList<T>(this List<T> @locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}
