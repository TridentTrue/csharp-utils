// Example Controller Action
[HttpPost]
public JsonResult GetData(DataTablesPostModel model)
{
    string sortBy = "";
    bool sortDir = true;

    if (model.order != null)
    {
        sortBy = model.columns[model.order[0].column].data;
		sortDir = string.Equals(model.order[0].dir, "asc", StringComparison.OrdinalIgnoreCase);
    }

    var query = db.ModelName
        .AsNoTracking()
        .Search(model.columns, model.search, out int totalResultsCount, out int? filteredResultsCount)
        .OrderBy(sortBy, sortDir)
        .Skip(model.start)
		.Take(model.length != -1 ? model.length : (filteredResultsCount ?? totalResultsCount))
        .AsEnumerable();

    return Json(new
    {
        draw = model.draw,
        recordsTotal = totalResultsCount,
        recordsFiltered = filteredResultsCount ?? totalResultsCount,
        data = query,
    }, JsonRequestBehavior.AllowGet);
}

// Search Extension
public static IQueryable<T> Search<T>(this IQueryable<T> query, List<Column> columns, Search search, out int totalResultsCount, out int? filteredResultsCount)
{
    totalResultsCount = query.Count();

    if (string.IsNullOrWhiteSpace(search.value))
    {
        filteredResultsCount = null;
        return query;
    }

    List<T> results = new List<T>();

    foreach (var column in columns.Where(c => c.searchable))
    {
        results.AddRange(query.ToList().Where(q => q
                    .GetType()
                    .GetProperty(column.data)
                    ?.GetValue(q, null)
                    ?.ToString()
                    ?.ToLower()
                    ?.Contains(search.value.ToLower())
                    ?? false
                ));
    }

    filteredResultsCount = results.Count;

    return results.AsQueryable();
}