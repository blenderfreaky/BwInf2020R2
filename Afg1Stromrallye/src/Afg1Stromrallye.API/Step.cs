using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afg1Stromrallye.API
{

    public class Step : IBoard
    {
        public IBoard Origin { get; }

        public IReadOnlyList<Vector2Int> Path { get; }
        public Vector2Int RobotPosition { get; }
        public int RobotBattery { get; }
        public int PreviousBattery { get; }

        public IReadOnlyCollection<Vector2Int> Batteries => Origin.Batteries;

        public Step(IBoard origin, IReadOnlyList<Vector2Int> path)
        {
            Origin = origin;
            Path = path;
            RobotPosition = Path.Last();
            PreviousBattery = Origin.RobotBattery - Path.Count;
            RobotBattery = Origin.GetBattery(RobotPosition) ?? PreviousBattery;
        }

        public int? GetBattery(Vector2Int pos) =>
            pos == RobotPosition
            ? PreviousBattery
            : Origin.GetBattery(pos);
    }
}
