// Example Controller Action
[HttpPost]
public JsonResult GetData(DataTablesPostModel model)
{
    string sortBy = "";
    bool sortDir = true;

    if (model.order != null)
    {
        sortBy = model.columns[model.order[0].column].data;
        sortDir = model.order[0].dir.ToLower() == "asc";
    }

    var query = db.ModelName
        .AsNoTracking()
        .Search(model.columns, model.search, out int totalResultsCount, out int? filteredResultsCount)
        .OrderBy(r => r.Id)
        .Skip(model.start)
        .Take(model.length)
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
public static IQueryable<ModelName> Search(this IQueryable<ModelName> query, List<Column> columns, Search search, out int totalResultsCount, out int? filteredResultsCount)
{
    totalResultsCount = query.Count();

    if (string.IsNullOrWhiteSpace(search.value))
    {
        filteredResultsCount = null;
        return query;
    }

    var results = query.Where(r =>
            r.Forename.ToLower().Contains(search.value.ToLower())
            || r.Surname.ToLower().Contains(search.value.ToLower())
            || r.LandlordName.ToLower().Contains(search.value.ToLower())
            // etc.....
        );

    filteredResultsCount = results.Count();

    return results;
}