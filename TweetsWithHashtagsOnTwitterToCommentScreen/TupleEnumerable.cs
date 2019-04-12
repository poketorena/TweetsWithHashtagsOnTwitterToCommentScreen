using System;
using System.Collections.Generic;

namespace TweetsWithHashtagsOnTwitterToCommentScreen
{
    // https://ufcpp.net/blog/2016/12/tipsindexedforeach/
    public static partial class TupleEnumerable
    {
        public static IEnumerable<(T item, int index)> Indexed<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IEnumerable<(T item, int index)> impl()
            {
                var i = 0;
                foreach (var item in source)
                {
                    yield return (item, i);
                    ++i;
                }
            }

            return impl();
        }
    }
}
