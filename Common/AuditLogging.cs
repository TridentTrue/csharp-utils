public class EntitiesWithLogging : Entities
{
    object GetPrimaryKeyValue(DbEntityEntry entry)
    {
        var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
        return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
    }


    public override int SaveChanges()
    {
        var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();

        foreach (var change in modifiedEntities)
        {
        	var entityName = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(change.Entity.GetType()).ToString();
        	var primaryKey = GetPrimaryKeyValue(change);
        	var databaseValues = change.GetDatabaseValues();

        	foreach (var prop in change.OriginalValues.PropertyNames)
        	{
        		var originalValue = databaseValues?.GetValue<object>(prop)?.ToString();
        		var currentValue = change?.CurrentValues[prop]?.ToString();

        		if (originalValue != currentValue)
        		{
        			ChangeLog log = new ChangeLog()
        			{
        				EntityName = entityName,
        				PrimaryKeyValue = primaryKey.ToString(),
        				PropertyName = prop,
        				OldValue = originalValue,
        				NewValue = currentValue,
        				DateChanged = DateTime.UtcNow,
        				ChangedBy = HttpContext.Current.User.Identity.Name
        			};
        			ChangeLogs.Add(log);
        		}
        	}
        }

        return base.SaveChanges();
    }
}