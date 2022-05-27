using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Game
{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance { get; private set; }

        [SerializeField] private Tilemap gameTilemap;

        private List<HexPosition> allCells;

        private MapGenerator()
        {
            if (Instance != null)
            {
                Debug.LogError("There should be only one RandomMapManager in the scene.");
                return;
            }

            Instance = this;
        }

        private void Awake() => Assert.IsNotNull(gameTilemap);

        private void OnEnable() => RandomizeTiles();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RandomizeTiles();
        }

        private void RandomizeTiles()
        {
            var random = new System.Random();
            allCells = GameManager.Map.GetAvailableCells().OrderBy(x => random.Next()).ToList();
            var tileTypes = GameManager.Map.TileTypes;

            var position = 0;
            for (int i = 2; i < tileTypes.Count; i++)
            {
                var tile = tileTypes[i];
                var tileCount = GameManager.Map.TileTypeCounts[i];
                for (int j = 0; j < tileCount; j++, position++)
                    gameTilemap.SetTile(allCells[position], tile);
            }

            // Set the rest to plain tiles
            for (int i = position; i < allCells.Count; i++)
                gameTilemap.SetTile(allCells[i], tileTypes[1]);
        }
    }
}