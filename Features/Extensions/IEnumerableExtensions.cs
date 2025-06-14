namespace System.Collections.Generic
{
    internal static class IEnumerableExtensions
    {
        //It helps in some cases when the classic ForEach does not want to be called from some collections.
        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) return;
            foreach (var e in enumerable)
                action(e);
        }
    }
}
