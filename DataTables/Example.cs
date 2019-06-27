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

    var query = db.DhpRegisters
        .AsNoTracking()
        .Search(model.columns, model.search, out int totalResultsCount, out int filteredResultsCount)
        .OrderBy(sortBy, sortDir)
        .Skip(model.start)
        .Take(model.length == -1 ? filteredResultsCount : model.length)
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
public static IQueryable<T> Search<T>(this IQueryable<T> query, List<Column> columns, Search search, out int totalResultsCount, out int filteredResultsCount)
{
    totalResultsCount = query.Count();
    bool genericSearchIsRegex = search.regex == "true";
    bool hasGenericSearch = !string.IsNullOrWhiteSpace(search.value) || genericSearchIsRegex;
    int specificSearchCount = columns.Count(c => c.searchable && (!string.IsNullOrWhiteSpace(c.search.value) || c.search.regex == "true"));

    if (!hasGenericSearch && specificSearchCount == 0)
    {
        filteredResultsCount = totalResultsCount;
        return query;
    }

    List<T> results = new List<T>();

    foreach(var row in query)
    {
        bool foundGeneric = !hasGenericSearch;
        int foundSpecificCount = 0;
        foreach (var column in columns.Where(c => c.searchable))
        {
            string value = row.GetType().GetProperty(column.data)?.GetValue(row, null)?.ToString()?.ToLower();
            bool specificSearchIsRegex = column.search.regex == "true";

            if (string.IsNullOrWhiteSpace(value) && (!genericSearchIsRegex && !specificSearchIsRegex))
                continue;

            if (hasGenericSearch && !foundGeneric)
                foundGeneric = genericSearchIsRegex ? Regex.IsMatch(value ?? "", search.value, RegexOptions.IgnoreCase) : (value ?? "").Contains(search.value.ToLower());

            if ((column.search.value != null && !specificSearchIsRegex) || specificSearchIsRegex)
            {
                if (specificSearchIsRegex)
                {
                    if (Regex.IsMatch(value ?? "", column.search.value, RegexOptions.IgnoreCase))
                        foundSpecificCount++;
                }
                else
                {
                    if ((value ?? "").Contains(column.search.value.ToLower()))
                        foundSpecificCount++;
                }
            }

            if (foundGeneric && specificSearchCount == foundSpecificCount)
            {
                results.Add(row);
                break;
            }
        }
    }

    filteredResultsCount = results.Count;

    return results.AsQueryable();
}