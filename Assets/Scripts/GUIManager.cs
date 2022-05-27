using System.Collections.Generic;
using Game.Helpers;
using System.Text.RegularExpressions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GUIManager : MonoBehaviour
    {
        public static GUIManager Instance { get; private set; }

        [SerializeField] private GameObject turnPanel;
        [SerializeField] private GameObject pointsPanel;
        [SerializeField] private GameObject rollPanel;
        [SerializeField] private GameObject gameOverPanel;

        [SerializeField] private TMP_Text turnNumberText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text winnerText;

        [SerializeField] private TMP_Text rollDescription;
        [SerializeField] private TMP_Text rollResult;
        [SerializeField] private Button rollButton;
        [SerializeField] private Button spinButton;

        internal bool isWaitingForInput;

        private GUIManager()
        {
            if (Instance != null)
            {
                Debug.LogError("There should be only one GUIManager in the scene.");
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            gameOverPanel.SetActive(false);
            rollPanel.SetActive(true);
        }

        private void OnEnable()
        {
            GameManager.Instance.TurnProgress += UpdateTurn;
            GameManager.Instance.GameEnded += OnGameOver;
        }

        private void OnDisable()
        {
            GameManager.Instance.TurnProgress -= UpdateTurn;
            GameManager.Instance.GameEnded -= OnGameOver;
        }

        private void UpdateTurn(int turn, Player currentPlayer)
        {
            turnNumberText.text = $"Turn {turn}:";
            playerNameText.color = currentPlayer.Color;
            playerNameText.text = $"{currentPlayer}'s turn";

            var pointTexts = pointsPanel.GetComponentsInChildren<TMP_Text>();
            for (int i = 0; i < pointTexts.Length; i++)
            {
                var player = GameManager.Instance.Players[i];
                pointTexts[i].color = player.Color;
                pointTexts[i].text = $"{player}: {player.Points}";
            }

            // ShowDiceRollPanel(); // THERE WAS COROUTINE HERE
        }

        // private void OnWheelSpin(WheelResult result) => ShowWheelSpinPanel(result);

        private void OnGameOver(Player winner)
        {
            GameManager.Map.movementEnabled = false;
            gameOverPanel.SetActive(true);
            turnPanel.SetActive(false);
            winnerText.color = winner.Color;
            winnerText.text = $"{winner} wins!";
        }

        public IEnumerator ShowDiceRollPanel()
        {
            GameManager.Map.movementEnabled = false;
            // yield return new WaitWhile(() => isWaitingForInput);
            isWaitingForInput = true;

            rollDescription.text = $"Roll the die to determine your range:";
            rollResult.text = string.Empty;

            rollButton.gameObject.SetActive(true);
            spinButton.gameObject.SetActive(false);
            rollPanel.SetActive(true);
            yield return new WaitForUIButtons(rollButton);
        }

        public IEnumerator ShowWheelSpinPanel(WheelResult result)
        {
            GameManager.Map.movementEnabled = false;
            // yield return new WaitWhile(() => isWaitingForInput);
            isWaitingForInput = true;

            rollDescription.text = "You spun the wheel.\n";
            rollDescription.text +=
                result == WheelResult.GainArtifact ? "Draw a card from the Artifact Pile."
                : result == WheelResult.LoseArtifact ? "Roll a die to lose an Artifact in your hand."
                : result == WheelResult.GainTreasure ? "Roll a die to determine the amount of gold you gain."
                : "Roll a die to determine the amount of gold you lose.";

            rollResult.fontSize = 56;
            rollResult.text = Regex.Replace(result.ToString(), "(\\B[A-Z])", " $1");

            rollButton.gameObject.SetActive(false);
            spinButton.gameObject.SetActive(true);
            rollPanel.SetActive(true);
            yield return new WaitWhile(() => isWaitingForInput);
        }

        public void OnDiceRollButtonClicked()
        {
            rollButton.gameObject.SetActive(false);
            GameManager.Instance.RollForMovement();
            rollResult.fontSize = 100;
            rollResult.text = $"{GameManager.Instance.TurnMovementRange}";

            isWaitingForInput = false;
            StartCoroutine(HideRollPanel());
        }

        public void OnWheelSpinButtonClicked()
        {
            spinButton.gameObject.SetActive(false);

            isWaitingForInput = false;
            // StartCoroutine(HideRollPanel());
            // HideRollPanel();
            rollPanel.SetActive(false);
            GameManager.Map.movementEnabled = true;
        }

        private IEnumerator HideRollPanel()
        {
            yield return new WaitForSeconds(1f);
            rollPanel.SetActive(false);
            GameManager.Map.movementEnabled = true;
        }
    }
}
