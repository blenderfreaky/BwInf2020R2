using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afg1Stromrallye.API
{
    public interface IBoard
    {
        IReadOnlyCollection<Vector2Int> Batteries { get; }
        int? GetBattery(Vector2Int pos);

        Vector2Int RobotPosition { get; }
        int RobotBattery { get; }
    }
}
