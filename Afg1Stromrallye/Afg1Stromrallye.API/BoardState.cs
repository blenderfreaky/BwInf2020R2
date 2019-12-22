using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Afg1Stromrallye.API
{
    public class BoardState
    {
        public readonly Board Board;
        public readonly ImmutableDictionary<Battery, int> BatteryCharges;

        public readonly Battery Robot;

        public BoardState(Board board, ImmutableDictionary<Battery, int> batteryCharges, Battery robot)
        {
            Board = board;
            BatteryCharges = batteryCharges;
            Robot = robot;
        }

        public IEnumerable<BoardState> GetNext()
        {

        }

        public BoardState MoveTo(Battery target, int roundabout)
        {
            if (roundabout % 2 != 0) throw new ArgumentException("Must be even number.", nameof(roundabout));
            if (roundabout < 0) throw new ArgumentException("Must be non-negative.", nameof(roundabout));

            var (distance, path) = Board.GetPath(Robot.Position, target);

            var next = new BoardState(board, );
        }
    }
}
