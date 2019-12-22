namespace Afg1Stromrallye.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Extensions
    {
        public static IEnumerable<Vector2Int> GetNeighbours(this Vector2Int position)
        {
            yield return position + new Vector2Int(0, +1);
            yield return position + new Vector2Int(0, -1);
            yield return position + new Vector2Int(+1, 0);
            yield return position + new Vector2Int(-1, 0);
        }
    }
}
