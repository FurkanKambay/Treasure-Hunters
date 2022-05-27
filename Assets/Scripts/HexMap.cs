using Game.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Game
{
    [RequireComponent(typeof(Grid))]
    public class HexMap : MonoBehaviour
    {
        public event Action<HexPosition> CellHovered;
        public event Action<HexPosition> CellClicked;

        internal bool movementEnabled;

        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap tilemap;

        [field: SerializeField, Range(4, 17)] public int Size { get; private set; }
        [SerializeField] private bool isLeftLeaning;

        [field: SerializeField] public List<Tile> TileTypes { get; private set; }
        [field: SerializeField] public int[] TileTypeCounts;
        [field: SerializeField] public Vector2Int[] StartingCells { get; private set; }

        internal readonly Dictionary<HexPosition, CellState> cellStateMap;

        private bool mapIsUneven => Size % 2 == 0;
        private int GetMinX(int y) => -(Size / 2 - y / 2) + (mapIsUneven && !isLeftLeaning ? 1 : 0);
        private int GetMaxX(int y) => (Size / 2 - (y + 1) / 2) - (mapIsUneven && isLeftLeaning ? 1 : 0);

        /// <summary>
        /// Assumes the map is in triangle form and that the base is at the origin.
        /// </summary>
        public bool IsCellInBounds(HexPosition cell)
        {
            return cell.y >= 0 && cell.y < Size
                && cell.x >= GetMinX(cell.y)
                && cell.x <= GetMaxX(cell.y);
        }

        public bool IsCellNavigable(HexPosition cell)
        {
            return IsCellInBounds(cell) && GetCellType(cell) != CellType.Obstacle;
        }

        public CellType GetCellType(HexPosition cell)
        {
            var tile = tilemap.GetTile<Tile>(cell);
            return (CellType)TileTypes.IndexOf(tile);
        }

        /// <summary>
        /// Returns the cell positions on the map except for the starting cells.
        /// </summary>
        public IEnumerable<HexPosition> GetAvailableCells()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = GetMinX(y); x <= GetMaxX(y); x++)
                {
                    if (!StartingCells.Contains(new Vector2Int(x, y)))
                        yield return new HexPosition(x, y);
                }
            }
        }

        private HexMap()
        {
            cellStateMap = new Dictionary<HexPosition, CellState>();
        }

        private void Awake()
        {
            Assert.IsNotNull(tilemap);
            Assert.IsNotNull(grid);
            Assert.IsNotNull(TileTypes);
            Assert.IsNotNull(StartingCells);

            Assert.AreEqual(TileTypes.Count, TileTypeCounts.Length);
            Assert.AreEqual(StartingCells.Length, GameManager.Instance.PlayerCount);

            Assert.IsTrue(Size > 0);
        }

        private void Start() => ResetPlayerPositions();

        private void Update()
        {
            if (!movementEnabled) return;

            var cell = GetCellUnderMouse();
            if (IsCellInBounds(cell))
                CellHovered?.Invoke(cell);

            if (Input.GetMouseButtonDown(0))
                CellClicked?.Invoke(cell);
        }

        private void OnValidate() => ResetPlayerPositions();

        private void ResetPlayerPositions()
        {
            for (int i = 0; i < GameManager.Instance.PlayerCount; i++)
            {
                var player = GameManager.Instance.Players[i];
                var startPosition = (HexPosition)StartingCells[i];
                player.transform.localPosition = grid.CellToLocal(startPosition);
            }
        }

        private HexPosition GetCellUnderMouse()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return grid.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var cell in GetAvailableCells())
                Gizmos.DrawSphere(grid.CellToWorld(cell), .1f);
        }
    }
}
