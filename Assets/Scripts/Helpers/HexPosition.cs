using System;
using UnityEngine;

namespace Game
{
    public struct HexPosition : IEquatable<HexPosition>
    {
        public int x => position.x;
        public int y => position.y;

        private readonly Vector3Int position;

        public HexPosition(int x, int y) => position = new(x, y);

        public override string ToString() => $"({x}, {y})";

        public static HexPosition operator +(HexPosition a) => a.position;
        public static HexPosition operator -(HexPosition a) => -a.position;
        public static HexPosition operator +(HexPosition a, HexPosition b) => (HexPosition)(a.position + b.position);
        public static HexPosition operator -(HexPosition a, HexPosition b) => (HexPosition)(a.position - b.position);

        public static bool operator ==(HexPosition a, HexPosition b) => a.position == b.position;
        public static bool operator !=(HexPosition a, HexPosition b) => a.position != b.position;
        public static bool operator ==(HexPosition a, Vector3Int b) => a.position == b;
        public static bool operator !=(HexPosition a, Vector3Int b) => a.position != b;

        public static implicit operator HexPosition(Vector3Int vector) => new(vector.x, vector.y);
        public static implicit operator Vector3Int(HexPosition hex) => new(hex.x, hex.y);
        public static implicit operator HexPosition(Vector2Int vector) => new(vector.x, vector.y);

        public override bool Equals(object other) => Vector3Int.Equals(position, other);
        public override int GetHashCode() => position.GetHashCode();

        public bool Equals(HexPosition other) => position == other.position;
    }
}