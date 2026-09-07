namespace DuckDoku.Api;

public static class StarPolicy
{
    private const long MsPerSizeUnit = 20000; 

    public static int GetStars(int size, long timeMs)
    {
        long parMs = size * MsPerSizeUnit;

        if (timeMs <= parMs)
        {
            return 3;
        }

        if (timeMs <= parMs * 3 / 2)
        {
            return 2;
        }

        return 1;
    }
}
