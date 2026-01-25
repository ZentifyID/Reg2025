using System.Globalization;

public static class CoinFormat
{
    private static readonly CultureInfo Ru = new CultureInfo("ru-RU");

    public static string Format(int coins)
    {
        if (coins < 1000)
            return coins.ToString(Ru);

        // 1 знак после запятой ВСЕГДА
        decimal k = decimal.Round(coins / 1000m, 1, System.MidpointRounding.AwayFromZero);

        // "0.0" -> всегда одна цифра после запятой
        return k.ToString("0.0", Ru) + "k";
    }
}
