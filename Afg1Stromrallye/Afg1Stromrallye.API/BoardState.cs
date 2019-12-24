using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Afg1Stromrallye.API
{
    public class BoardState
    {
        public Board Board { get; }
        public ImmutableDictionary<Battery, int> BatteryCharges { get; }

        public BoardState? Origin { get; }
        public IReadOnlyList<Vector2Int> PathFromOrigin { get; }

        public Battery RobotPosition { get; }
        public int RobotCharge { get; }

        public BoardState(Board board, ImmutableDictionary<Battery, int> batteryCharges, BoardState? origin, IReadOnlyList<Vector2Int> pathFromOrigin, Battery robotPosition, int robotCharge)
        {
            Board = board;
            BatteryCharges = batteryCharges;
            Origin = origin;
            PathFromOrigin = pathFromOrigin;
            RobotPosition = robotPosition;
            RobotCharge = robotCharge;
        }

        public BoardState? Solve()
        {
            var subSolutions =
                RobotPosition.ShortestPaths
                .AsParallel()
                .Select(x => MoveTo(x.Key, x.Value))
                .Where(x => x != null)
                .ToList();
        }

        private IEnumerable<Battery> ReachableBatteries => RobotPosition.ShortestPaths.Keys;

        public BoardState? MoveTo(Battery target, IReadOnlyList<Vector2Int> path)
        {
            if (target.Position != path.Last()) throw new ArgumentException(nameof(path));

            var newCharge = RobotCharge - path.Count;

            if (newCharge < 0) return null;

            var newRobotCharge = BatteryCharges[target];
            var newCharges = BatteryCharges.SetItem(target, newCharge);

            return new BoardState(Board, newCharges, this, path, target, newRobotCharge);
        }

        public BoardState? MoveTo(Battery target, int roundaboutLength)
        {
            var shortestPath = RobotPosition.ShortestPaths[target];

            if (roundaboutLength == 0) return MoveTo(target, shortestPath);

            if (!RobotPosition.Roundabouts.TryGetValue(target, out var roundaboutInfo)) return null;

            var path = new List<Vector2Int>();

            foreach (var position in shortestPath)
            {
                if (position == roundaboutInfo.ExitSpot)
                {
                    for (int i = 0; i < roundaboutLength; i++)
                    {
                        path.Add(roundaboutInfo.ExitSpot);
                        path.Add(roundaboutInfo.IntermediateSpot);
                    }
                }

                path.Add(position);
            }

            return MoveTo(target, path);
        }
    }
}
