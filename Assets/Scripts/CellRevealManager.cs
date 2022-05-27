using System.Collections.Generic;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class CellRevealManager : MonoBehaviour
    {
        [SerializeField] private Tilemap gameTilemap;
        [SerializeField] private Tilemap fogTilemap;
        [SerializeField] private Tile disabledCellTile;

        [SerializeField] private int treasureValue = 10;

        public void RevealCell(HexPosition cell)
        {
            fogTilemap.SetTile(cell, null);
            ApplyCellEffect(cell);
        }

        public void RevealCells(IEnumerable<HexPosition> cells)
        {
            foreach (var cell in cells)
            {
                fogTilemap.SetTile(cell, null);
                ApplyCellEffect(cell);
            }
        }

        public void DisableCells(IEnumerable<HexPosition> cells)
        {
            foreach (var cell in cells)
            {
                GameManager.Map.cellStateMap[cell] = CellState.RevealedDisabled;
                fogTilemap.SetTile(cell, disabledCellTile);
            }
        }

        private void ApplyCellEffect(HexPosition cell)
        {
            if (GameManager.Map.cellStateMap.TryGetValue(cell, out CellState state) && state == CellState.RevealedDisabled)
                return;

            var tile = gameTilemap.GetTile<Tile>(cell);
            var cellType = (CellType)GameManager.Map.TileTypes.IndexOf(tile);
            switch (cellType)
            {
                case CellType.Treasure:
                    RewardTreasure(GameManager.Instance.CurrentPlayer);
                    break;
                case CellType.Artifact:
                    GameManager.Instance.RollForArtifact();
                    break;
                case CellType.Wheel:
                    GameManager.Instance.SpinTheWheel();
                    break;
            }
        }

        private void RewardTreasure(Player player) => player.Points += treasureValue;
    }
}
