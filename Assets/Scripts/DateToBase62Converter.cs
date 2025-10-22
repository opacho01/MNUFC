using System;
using System.Text;

/// <summary>
/// Converts date-time values to and from Base62 encoded strings for compact representation.
/// Handles date format: YYYY-MM-DD_HH-MM-SS
/// </summary>
public static class Base62DateConverter
{
    private const string BASE62_ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Converts a date string in format "YYYY-MM-DD_HH-MM-SS" to a Base62 encoded string
    /// </summary>
    /// <param name="dateString">Date string in format YYYY-MM-DD_HH-MM-SS</param>
    /// <returns>Base62 encoded string representation of the date</returns>
    /// <exception cref="ArgumentException">Thrown when input format is invalid</exception>
    /// <example>
    /// string base62Code = Base62DateConverter.ConvertDateToBase62("2025-10-21_12-46-57");
    /// Returns: "1tX8vF" (approximately 6 characters)
    /// </example>
    public static string ConvertDateToBase62(string dateString)
    {
        try
        {
            DateTime date = ParseDateString(dateString);
            long timestamp = ConvertToDateTimeOffset(date).ToUnixTimeSeconds();
            return ConvertToBase62(timestamp);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid date format: {dateString}", ex);
        }
    }

    /// <summary>
    /// Converts a DateTime object to a Base62 encoded string
    /// </summary>
    /// <param name="dateTime">DateTime object to convert</param>
    /// <returns>Base62 encoded string representation of the DateTime</returns>
    public static string ConvertDateToBase62(DateTime dateTime)
    {
        long timestamp = ConvertToDateTimeOffset(dateTime).ToUnixTimeSeconds();
        return ConvertToBase62(timestamp);
    }

    /// <summary>
    /// Converts a Base62 encoded string back to a DateTime object
    /// </summary>
    /// <param name="base62String">Base62 encoded string to convert</param>
    /// <returns>DateTime object reconstructed from the Base62 string</returns>
    /// <exception cref="ArgumentException">Thrown when Base62 string is invalid</exception>
    public static DateTime ConvertBase62ToDate(string base62String)
    {
        long timestamp = ConvertFromBase62(base62String);
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }

    /// <summary>
    /// Parses date string from format "YYYY-MM-DD_HH-MM-SS"
    /// </summary>
    /// <param name="dateString">Date string to parse</param>
    /// <returns>Parsed DateTime object</returns>
    private static DateTime ParseDateString(string dateString)
    {
        string[] parts = dateString.Replace('_', '-').Split('-');

        if (parts.Length != 6)
        {
            throw new ArgumentException("Invalid date format. Expected: YYYY-MM-DD_HH-MM-SS");
        }

        int year = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int day = int.Parse(parts[2]);
        int hour = int.Parse(parts[3]);
        int minute = int.Parse(parts[4]);
        int second = int.Parse(parts[5]);

        return new DateTime(year, month, day, hour, minute, second);
    }

    /// <summary>
    /// Converts DateTime to DateTimeOffset for Unix timestamp conversion
    /// </summary>
    /// <param name="dateTime">DateTime to convert</param>
    /// <returns>DateTimeOffset representation</returns>
    private static DateTimeOffset ConvertToDateTimeOffset(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        return new DateTimeOffset(dateTime);
    }

    /// <summary>
    /// Converts a long integer to Base62 string representation
    /// </summary>
    /// <param name="number">Number to convert</param>
    /// <returns>Base62 encoded string</returns>
    private static string ConvertToBase62(long number)
    {
        if (number == 0) return "0";

        bool isNegative = number < 0;
        long value = Math.Abs(number);
        StringBuilder result = new StringBuilder();

        while (value > 0)
        {
            long remainder = value % 62;
            result.Insert(0, BASE62_ALPHABET[(int)remainder]);
            value /= 62;
        }

        if (isNegative)
        {
            result.Insert(0, '-');
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a Base62 string back to a long integer
    /// </summary>
    /// <param name="base62String">Base62 string to convert</param>
    /// <returns>Decoded long integer</returns>
    /// <exception cref="ArgumentException">Thrown when string contains invalid Base62 characters</exception>
    private static long ConvertFromBase62(string base62String)
    {
        bool isNegative = base62String.StartsWith("-");
        string str = isNegative ? base62String.Substring(1) : base62String;

        long result = 0;
        long multiplier = 1;

        for (int i = str.Length - 1; i >= 0; i--)
        {
            char c = str[i];
            int value = BASE62_ALPHABET.IndexOf(c);

            if (value == -1)
            {
                throw new ArgumentException($"Invalid character in Base62 string: {c}");
            }

            result += value * multiplier;
            multiplier *= 62;
        }

        return isNegative ? -result : result;
    }
}