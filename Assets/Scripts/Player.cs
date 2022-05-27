using Game.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public int Points;

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }

        [SerializeField] private TMP_Text labelText;
        [SerializeField] private Vector3 gizmoOffset;

        internal bool finishedMoving;

        public HexPosition CellPosition => GameManager.Grid.LocalToCell(LocalPosition);

        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }

        private SpriteRenderer spriteRenderer;
        private void Awake() => spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        public IEnumerator Move(HexPosition targetCell, int range)
        {
            finishedMoving = false;
            var revealed = new List<HexPosition>();
            foreach (var line in GetLines(range))
            {
                if (!line.Contains(targetCell))
                    continue;

                foreach (var cell in line)
                {
                    GameManager.Revealer.RevealCell(cell);
                    revealed.Add(cell);

                    if (!GameManager.Map.IsCellNavigable(cell))
                        break;

                    LocalPosition = GameManager.Grid.CellToLocal(cell);
                    yield return new WaitWhile(() => GUIManager.Instance.isWaitingForInput);
                    yield return new WaitForSeconds(GameManager.Instance.PlayerMovementDelay);

                    if (cell == targetCell)
                        break;
                }
            }

            GameManager.Revealer.DisableCells(revealed);
            finishedMoving = true;
        }

        public bool CanMoveTo(HexPosition targetPosition, int range)
            => GetLines(range).Any(sub => sub.Contains(targetPosition));

        public IEnumerable<List<HexPosition>> GetLines(int range)
        {
            var directions = Enum.GetValues(typeof(HexDirection)).Cast<HexDirection>();
            foreach (var direction in directions)
            {
                yield return CellPosition.Line(direction, range)
                    .Where(cell => GameManager.Map.IsCellInBounds(cell))
                    .ToList();
            }
        }

        public List<HexPosition> GetNavigableCellsInLine(HexDirection direction, int range)
        {
            return CellPosition.Line(direction, range)
                .Where(cell => GameManager.Map.IsCellNavigable(cell))
                .ToList();
        }

        public override string ToString() => Name;

        private void OnValidate() => labelText.text = Name;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color;
            GetLines(GameManager.Instance.TurnMovementRange).SelectMany(line => line)
                .Select(cell => GameManager.Grid.CellToWorld(cell))
                .ToList().ForEach(pos => Gizmos.DrawSphere(pos + gizmoOffset, 0.1f));
        }
    }
}
