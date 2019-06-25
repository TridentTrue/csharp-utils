public static LinqExt
{
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
    
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string memberName, bool asc = true)
    {
        ParameterExpression[] typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };
        System.Reflection.PropertyInfo pi = typeof(T).GetProperty(memberName);

        return (IOrderedQueryable<T>)query.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                asc ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), pi.PropertyType },
                query.Expression,
                Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
            );
    }
}
