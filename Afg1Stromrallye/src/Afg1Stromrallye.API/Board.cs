using System;
using System.Collections.Generic;
using System.Text;

namespace Afg1Stromrallye.API
{
    public class Board
    {
        public int?[,] Batteries { get; }

        public Vector2Int StartingPosition { get; }
        public int StartingBattery { get; }
    }
}
