using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class CellHighlightManager : MonoBehaviour
    {
        [SerializeField] private Tilemap highlightTilemap;
        [SerializeField] private Tile highlightTile;

        private void OnEnable()
        {
            GameManager.Instance.RangeDetermined += OnRangeDetermined;
            GameManager.Instance.PlayerMoving += OnPlayerMoving;
        }

        private void OnDisable()
        {
            GameManager.Instance.RangeDetermined -= OnRangeDetermined;
            GameManager.Instance.PlayerMoving -= OnPlayerMoving;
        }

        public void HighlightLines(IEnumerable<HexPosition> cells)
        {
            ResetCellHighlights();

            foreach (var cell in cells)
            {
                if (GameManager.Map.IsCellInBounds(cell))
                    highlightTilemap.SetTile(cell, highlightTile);
            }
        }

        public void ResetCellHighlights()
        {
            highlightTilemap.ClearAllTiles();
            highlightTilemap.CompressBounds();
        }

        private void Start() => ResetCellHighlights();

        private void OnPlayerMoving(Player player, HexPosition targetCell)
            => ResetCellHighlights();

        private void OnRangeDetermined(Player player, int range)
        {
            var cells = player.GetLines(range).SelectMany(line => line);
            HighlightLines(cells);
        }
    }
}
