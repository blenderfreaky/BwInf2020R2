namespace Afg3Abbiegen
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    internal static class StreetParser
    {
        public static bool TryParse(
            [DisallowNull] string[] text,
            [NotNullWhen(true)][MaybeNullWhen(false)] out Vector2Int start,
            [NotNullWhen(true)][MaybeNullWhen(false)] out Vector2Int end,
            [NotNullWhen(true)][MaybeNullWhen(false)] out List<Street> streets)
        {
            start = end = default;
            streets = default!;
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

        public static bool TryParseVector2Int(
            [DisallowNull] string text,
            [NotNullWhen(true)][MaybeNullWhen(false)]out Vector2Int vector)
        {
            vector = default;

            if (text.Length <= 2) return false;
            var elements = text[1..^1].Split(',');

            if (elements.Length != 2) return false;

            if (!int.TryParse(elements[0], out var x)) return false;
            if (!int.TryParse(elements[1], out var y)) return false;

            vector = new Vector2Int(x, y);
            return true;
        }
    }
}
