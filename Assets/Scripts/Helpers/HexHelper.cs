using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Helpers
{
    public static class HexHelper
    {
        public static HexPosition GetNeighbor(this HexPosition cellPosition, HexDirection direction)
        {
            bool even = cellPosition.y % 2 == 0;
            switch (direction)
            {
                case HexDirection.Right:
                    return cellPosition + new HexPosition(1, 0);
                case HexDirection.Left:
                    return cellPosition + new HexPosition(-1, 0);
                case HexDirection.UpRight:
                    return cellPosition + new HexPosition(even ? 0 : 1, 1);
                case HexDirection.DownRight:
                    return cellPosition + new HexPosition(even ? 0 : 1, -1);
                case HexDirection.UpLeft:
                    return cellPosition + new HexPosition(even ? -1 : 0, 1);
                case HexDirection.DownLeft:
                    return cellPosition + new HexPosition(even ? -1 : 0, -1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static bool IsNeighbor(this HexPosition cell, HexPosition otherCellPosition)
        {
            return Enum.GetValues(typeof(HexDirection))
                .Cast<HexDirection>()
                .Any(direction => cell.GetNeighbor(direction) == otherCellPosition);
        }

        public static HexPosition Offset(this HexPosition cell, HexDirection direction, int distance)
        {
            var finalPosition = cell;
            for (int i = 0; i < distance; i++)
                finalPosition = finalPosition.GetNeighbor(direction);
            return finalPosition;
        }

        public static List<HexPosition> Line(this HexPosition cell, HexDirection direction, int distance)
        {
            var line = new List<HexPosition>();
            var current = cell;

            for (int i = 0; i < distance; i++)
            {
                line.Add(current = current.GetNeighbor(direction));
            }

            return line;
        }
    }
}
