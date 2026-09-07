namespace DuckDoku.Api;

public static class EnergyPolicy
{
    public const int Maximum = 5;
    public const int EntryCost = 1;

    private static readonly TimeSpan RecoveryPeriod = TimeSpan.FromMinutes(20);

    public static (int Value, TimeSpan TimeToNext) GetCurrent(int storedValue, DateTime? updatedAt, DateTime now)
    {
        (int value, TimeSpan remainder) = Recalculate(storedValue, updatedAt, now);

        TimeSpan timeToNext = value >= Maximum ? TimeSpan.Zero : RecoveryPeriod - remainder;

        return (value, timeToNext);
    }
    
    public static bool TrySpend(
        int storedValue,
        DateTime? updatedAt,
        DateTime now,
        int cost,
        out int newValue,
        out DateTime? newUpdatedAt)
    {
        (int value, TimeSpan remainder) = Recalculate(storedValue, updatedAt, now);

        bool canAfford = value >= cost;
        newValue = canAfford ? value - cost : value;
        newUpdatedAt = newValue >= Maximum ? null : now - remainder;

        return canAfford;
    }

    private static (int Value, TimeSpan Remainder) Recalculate(int storedValue, DateTime? updatedAt, DateTime now)
    {
        if (storedValue >= Maximum || updatedAt is null)
        {
            return (Maximum, TimeSpan.Zero);
        }

        TimeSpan elapsed = now - updatedAt.Value;
        int accruedPeriods = (int)(elapsed / RecoveryPeriod);
        int value = Math.Min(Maximum, storedValue + accruedPeriods);
        
        TimeSpan remainder = value >= Maximum
            ? TimeSpan.Zero
            : elapsed - RecoveryPeriod * accruedPeriods;

        return (value, remainder);
    }
}
