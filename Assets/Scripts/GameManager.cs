using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static HexMap Map => Instance.map;
        public static Grid Grid => Instance.grid;
        public static CellRevealManager Revealer => Instance.cellRevealManager;

        public event Action<int, Player> TurnProgress;
        public event Action<Player, int> RangeDetermined;
        public event Action<Player, HexPosition> PlayerMoving;
        public event Action<WheelResult> WheelSpun;
        // public event Action<Artifact> ArtifactCollected;
        public event Action<Player> GameEnded;

        public int PlayerCount => Players.Count;
        public Player CurrentPlayer => Players[currentPlayerIndex];
        public Queue<int> TurnQueue => turnQueue;

        [SerializeField] private HexMap map;
        [SerializeField] private Grid grid;
        [SerializeField] private CellRevealManager cellRevealManager;

        [field: SerializeField] public List<Player> Players { get; private set; }
        [field: SerializeField] public int MaxTurns { get; private set; }

        [SerializeField] private int minDiceValue = 1;
        [SerializeField] private int maxDiceValue = 3;

        [field: SerializeField] public int CurrentTurn { get; private set; }
        [field: SerializeField] public int TurnMovementRange { get; private set; }
        [field: SerializeField] public float PlayerMovementDelay { get; private set; }

        private int currentPlayerIndex;
        private Queue<int> turnQueue;
        private bool gameEnded;

        private GameManager()
        {
            if (Instance != null)
            {
                Debug.LogError("There should be only one GameManager in the scene.");
                return;
            }

            Instance = this;
        }

        private void Awake()
        {
            Assert.IsNotNull(map);
            Assert.IsNotNull(grid);
        }

        private void Start()
        {
            Assert.IsTrue(Players.Count > 0);
            Assert.IsTrue(MaxTurns > 0);

            CurrentTurn = 0;
            currentPlayerIndex = 0;
            turnQueue = new Queue<int>(Players.Count);

            for (int i = 0; i < Players.Count; i++)
                turnQueue.Enqueue(i);

            map.CellClicked += OnCellClicked;
            NextTurn();
        }

        public void NextTurn()
        {
            Assert.IsFalse(gameEnded);

            if (++CurrentTurn > MaxTurns)
            {
                EndGame();
                return;
            }

            currentPlayerIndex = turnQueue.Dequeue();
            turnQueue.Enqueue(currentPlayerIndex);

            TurnProgress?.Invoke(CurrentTurn, CurrentPlayer);
            StartCoroutine(GUIManager.Instance.ShowDiceRollPanel());
        }

        public void RollForMovement()
        {
            TurnMovementRange = Random.Range(minDiceValue, maxDiceValue + 1);
            RangeDetermined?.Invoke(CurrentPlayer, TurnMovementRange);
        }

        public Artifact RollForArtifact()
        {
            var artifact = new Artifact();
            // get random artifact from pile
            // CurrentPlayer.AddArtifact(artifact);
            // ArtifactCollected?.Invoke(artifact);
            return artifact;
        }

        public void SpinTheWheel()
        {
            var wheelResult = Random.Range(0, 4);
            StartCoroutine(GUIManager.Instance.ShowWheelSpinPanel((WheelResult)wheelResult));
            // WheelSpun?.Invoke((WheelResult)wheelResult);
        }

        public Player GetPlayerOnCell(HexPosition cell)
            => Players.Find(p => p.CellPosition == cell);

        private void OnCellClicked(HexPosition cell)
        {
            if (gameEnded || !CurrentPlayer.CanMoveTo(cell, TurnMovementRange))
                return;

            PlayerMoving?.Invoke(CurrentPlayer, cell);
            StartCoroutine(PerformTurn(cell, TurnMovementRange));
        }

        private IEnumerator PerformTurn(HexPosition cell, int range)
        {
            var player = CurrentPlayer;
            StartCoroutine(CurrentPlayer.Move(cell, range));
            yield return new WaitUntil(() => CurrentPlayer.finishedMoving);
            yield return new WaitWhile(() => GUIManager.Instance.isWaitingForInput);
            NextTurn();
        }

        public void EndGame()
        {
            gameEnded = true;
            Player winner = Players.OrderByDescending(player => player.Points).First();
            GameEnded?.Invoke(winner);
        }
    }
}
