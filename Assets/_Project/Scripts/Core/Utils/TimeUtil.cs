using System;

public static class TimeUtil
{
    public static long UnixNow() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public static long AddSeconds(long unix, int seconds) => unix + seconds;
}
