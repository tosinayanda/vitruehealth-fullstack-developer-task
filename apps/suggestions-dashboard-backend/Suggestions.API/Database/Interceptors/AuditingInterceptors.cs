namespace Suggestions.API.Database.Interceptors;

public class AuditingInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted &&
                        e.Entity is Suggestion) // Only audit Suggestion entities
                        // e.Entity is not AuditLog) // Don't audit the audit logs
            .ToList();

        foreach (var entry in entries)
        {
            var auditLog = AuditLog.CreateFromEntry(entry);
            dbContext.Add(auditLog);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}