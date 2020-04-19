namespace Afg3Abbiegen
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Parses a map from text.
    /// </summary>
    internal static class MapParser
    {
        /// <summary>
        /// Parses text as a map.
        /// Returns <c>false</c> if parsing failed.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="start">The starting point of the map.</param>
        /// <param name="end">The ending point of the map.</param>
        /// <param name="streets">The streets contained in the map.</param>
        /// <returns><c>true</c> if parsing was a success, <c>false</c> if parsing failed.</returns>
        public static bool TryParse(
            [DisallowNull] string[] text,
            [NotNullWhen(true)][MaybeNullWhen(false)] out Vector2Int start,
            [NotNullWhen(true)][MaybeNullWhen(false)] out Vector2Int end,
            [NotNullWhen(true)][MaybeNullWhen(false)] out List<Street>? streets)
        {
            // To allow early returns
            start = end = default;
            streets = default;
            if (!int.TryParse(text[0], out var count)) return false;
            if (!TryParseVector2Int(text[1], out start)) return false;
            if (!TryParseVector2Int(text[2], out end)) return false;

            streets = new List<Street>();

            for (int i = 0; i < count; i++)
            {
                var vectors = text[3 + i].Split(' ');

                if (!TryParseVector2Int(vectors[0], out var streetStart)) return false;
                if (!TryParseVector2Int(vectors[1], out var streetEnd)) return false;

                streets.Add(new Street(streetStart, streetEnd));
            }

            return true;
        }

        /// <summary>
        /// Parses parenthesized Vector2Int.
        /// Returns <c>false</c> if parsing failed.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="vector">The parsed vector.</param>
        /// <returns><c>true</c> if parsing was a success, <c>false</c> if parsing failed.</returns>
        public static bool TryParseVector2Int(
            [DisallowNull] string text,
            [NotNullWhen(true)][MaybeNullWhen(false)]out Vector2Int vector)
        {
            // To allow early returns
            vector = default;

            if (text.Length <= 2) return false;
            // Exclude parenthesis and split x and y components
            var elements = text[1..^1].Split(',');

            if (elements.Length != 2) return false;

            if (!int.TryParse(elements[0], out var x)) return false;
            if (!int.TryParse(elements[1], out var y)) return false;

            vector = new Vector2Int(x, y);
            return true;
        }
    }
}