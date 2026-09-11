using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DuckDoku.Api;

public static class IdempotencyGuard
{
    public static async Task<IResult?> FindReplayAsync(AppDbContext database, Guid playerId, Guid requestId)
    {
        IdempotencyRecord? record = await database.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.RequestId == requestId && r.PlayerId == playerId);

        return record is null ? null : Results.Text(record.ResponseBody, "application/json");
    }

    public static bool IsConflict(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: not null
            } postgres &&
            postgres.ConstraintName.Contains("Idempotency", StringComparison.OrdinalIgnoreCase);
    }
}
