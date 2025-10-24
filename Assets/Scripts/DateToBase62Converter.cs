using System;
using System.Text;

/// <summary>
/// Converts date-time values to and from Base36 encoded strings for compact representation.
/// Handles date format: YYYY-MM-DD_HH-MM-SS
/// Uses only numbers (0-9) and uppercase letters (A-Z)
/// Always returns exactly 6 characters
/// </summary>
public static class Base62DateConverter
{
    private const string BASE36_ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int OUTPUT_LENGTH = 6;

    /// <summary>
    /// Converts a date string in format "YYYY-MM-DD_HH-MM-SS" to a Base36 encoded string
    /// </summary>
    /// <param name="dateString">Date string in format YYYY-MM-DD_HH-MM-SS</param>
    /// <returns>Base36 encoded string representation of the date (always 6 characters)</returns>
    /// <exception cref="ArgumentException">Thrown when input format is invalid</exception>
    /// <example>
    /// string base36Code = Base36DateConverter.ConvertDateToBase36("2025-10-21_12-46-57");
    /// Returns: "1A2B3C" (always 6 characters)
    /// </example>
    public static string ConvertDateToBase36(string dateString)
    {
        try
        {
            DateTime date = ParseDateString(dateString);
            long timestamp = ConvertToDateTimeOffset(date).ToUnixTimeSeconds();
            return ConvertToBase36FixedLength(timestamp);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid date format: {dateString}", ex);
        }
    }

    /// <summary>
    /// Converts a DateTime object to a Base36 encoded string
    /// </summary>
    /// <param name="dateTime">DateTime object to convert</param>
    /// <returns>Base36 encoded string representation of the DateTime (always 6 characters)</returns>
    public static string ConvertDateToBase36(DateTime dateTime)
    {
        long timestamp = ConvertToDateTimeOffset(dateTime).ToUnixTimeSeconds();
        return ConvertToBase36FixedLength(timestamp);
    }

    /// <summary>
    /// Converts a Base36 encoded string back to a DateTime object
    /// </summary>
    /// <param name="base36String">Base36 encoded string to convert (must be 6 characters)</param>
    /// <returns>DateTime object reconstructed from the Base36 string</returns>
    /// <exception cref="ArgumentException">Thrown when Base36 string is invalid or not 6 characters</exception>
    public static DateTime ConvertBase36ToDate(string base36String)
    {
        if (base36String == null || base36String.Length != OUTPUT_LENGTH)
        {
            throw new ArgumentException($"Base36 string must be exactly {OUTPUT_LENGTH} characters long");
        }

        long timestamp = ConvertFromBase36(base36String);
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
    /// Converts a long integer to Base36 string representation with fixed length of 6 characters
    /// </summary>
    /// <param name="number">Number to convert</param>
    /// <returns>Base36 encoded string (always 6 characters)</returns>
    private static string ConvertToBase36FixedLength(long number)
    {
        if (number < 0)
        {
            throw new ArgumentException("Timestamp cannot be negative for fixed-length encoding");
        }

        char[] result = new char[OUTPUT_LENGTH];

        // Fill with zeros initially
        for (int i = 0; i < OUTPUT_LENGTH; i++)
        {
            result[i] = '0';
        }

        long value = number;
        int position = OUTPUT_LENGTH - 1;

        // Convert to Base36 from right to left
        while (value > 0 && position >= 0)
        {
            long remainder = value % 36;
            result[position] = BASE36_ALPHABET[(int)remainder];
            value /= 36;
            position--;
        }

        // If the number is too large to fit in 6 characters, use modulo to fit it
        if (value > 0)
        {
            // Use modulo operation to ensure it fits in 6 characters
            // This creates a hash-like behavior for very large timestamps
            long maxValue = (long)Math.Pow(36, OUTPUT_LENGTH) - 1;
            return ConvertToBase36FixedLength(number % maxValue);
        }

        return new string(result);
    }

    /// <summary>
    /// Converts a Base36 string back to a long integer
    /// </summary>
    /// <param name="base36String">Base36 string to convert</param>
    /// <returns>Decoded long integer</returns>
    /// <exception cref="ArgumentException">Thrown when string contains invalid Base36 characters</exception>
    private static long ConvertFromBase36(string base36String)
    {
        if (string.IsNullOrEmpty(base36String))
            throw new ArgumentException("Base36 string cannot be null or empty");

        long result = 0;
        long multiplier = 1;

        for (int i = base36String.Length - 1; i >= 0; i--)
        {
            char c = base36String[i];
            int value = BASE36_ALPHABET.IndexOf(c);

            if (value == -1)
            {
                throw new ArgumentException($"Invalid character in Base36 string: {c}");
            }

            result += value * multiplier;
            multiplier *= 36;
        }

        return result;
    }

    /// <summary>
    /// Validates if a string contains only valid Base36 characters (0-9, A-Z) and is 6 characters long
    /// </summary>
    /// <param name="input">String to validate</param>
    /// <returns>True if the string is valid Base36 and 6 characters long</returns>
    public static bool IsValidBase36(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length != OUTPUT_LENGTH)
            return false;

        foreach (char c in input)
        {
            if (BASE36_ALPHABET.IndexOf(c) == -1)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the current date-time as a Base36 encoded string
    /// </summary>
    /// <returns>Base36 encoded string of current date-time (6 characters)</returns>
    public static string GetCurrentDateAsBase36()
    {
        return ConvertDateToBase36(DateTime.Now);
    }

    /// <summary>
    /// Gets the maximum date that can be represented in 6-character Base36 format
    /// </summary>
    /// <returns>Maximum representable DateTime</returns>
    public static DateTime GetMaxBase36Date()
    {
        long maxTimestamp = (long)Math.Pow(36, OUTPUT_LENGTH) - 1;
        return DateTimeOffset.FromUnixTimeSeconds(maxTimestamp).DateTime;
    }

    /// <summary>
    /// Gets the minimum date that can be represented in Base36 format
    /// </summary>
    /// <returns>Minimum representable DateTime</returns>
    public static DateTime GetMinBase36Date()
    {
        return DateTimeOffset.FromUnixTimeSeconds(0).DateTime;
    }

    /// <summary>
    /// Generates a random 6-character Base36 string for testing
    /// </summary>
    /// <returns>Random Base36 string</returns>
    public static string GenerateRandomBase36()
    {
        Random random = new Random();
        char[] result = new char[OUTPUT_LENGTH];

        for (int i = 0; i < OUTPUT_LENGTH; i++)
        {
            result[i] = BASE36_ALPHABET[random.Next(0, 36)];
        }

        return new string(result);
    }
}