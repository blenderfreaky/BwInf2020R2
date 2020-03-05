using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afg1Stromrallye.API
{

    public class Board : IBoard
    {
        private Dictionary<Vector2Int, int> BatteryCharges { get; }

        public IReadOnlyCollection<Vector2Int> Batteries => BatteryCharges.Keys;

        public int? GetBattery(Vector2Int pos) => BatteryCharges.TryGetValue(pos, out var val) ? val : (int?)null;

        public Vector2Int RobotPosition { get; }
        public int RobotBattery { get; }

        public Board(Dictionary<Vector2Int, int> batteryCharges, Vector2Int robotPosition, int robotBattery)
        {
            BatteryCharges = batteryCharges;
            RobotPosition = robotPosition;
            RobotBattery = robotBattery;
        }
    }
}
