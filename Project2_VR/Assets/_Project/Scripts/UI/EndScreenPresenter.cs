using BaratieBlade.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaratieBlade.UI
{
    public class EndScreenPresenter : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject endPanel;
        [SerializeField] private TMP_Text slicedLabel;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text ratingLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void OnEnable()
        {
            if (gameManager == null || endPanel == null)
            {
                Debug.LogError($"{name}: end screen references are not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(gameManager.Restart);
            }
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(gameManager.ReturnToMenu);
            }

            HandleStateChanged(gameManager.State);
        }

        private void OnDisable()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.StateChanged -= HandleStateChanged;
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(gameManager.Restart);
            }
            if (menuButton != null)
            {
                menuButton.onClick.RemoveListener(gameManager.ReturnToMenu);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            bool visible = state == GameState.EndScreen;
            endPanel.SetActive(visible);
            if (!visible)
            {
                return;
            }

            SessionController session = gameManager.Session;
            if (slicedLabel != null)
            {
                slicedLabel.SetText("Sliced {0} / {1}", session.SlicedCount, session.LaunchedCount);
            }
            if (scoreLabel != null)
            {
                scoreLabel.SetText("Score {0}", session.Score);
            }
            if (ratingLabel != null)
            {
                ratingLabel.text = session.GetRating();
            }
        }
    }
}
