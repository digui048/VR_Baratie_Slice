using BaratieBlade.Core;
using TMPro;
using UnityEngine;

namespace BaratieBlade.UI
{
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text countdownLabel;

        [SerializeField] private float goDuration = 0.8f;

        private int shownSeconds = -1;
        private float goTimer;

        private void OnEnable()
        {
            if (gameManager == null || gameManager.Session == null)
            {
                Debug.LogError($"{name}: game manager is not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            gameManager.Session.StatsChanged += RefreshScore;
            HandleStateChanged(gameManager.State);
            RefreshScore();
        }

        private void OnDisable()
        {
            if (gameManager == null || gameManager.Session == null)
            {
                return;
            }

            gameManager.StateChanged -= HandleStateChanged;
            gameManager.Session.StatsChanged -= RefreshScore;
        }

        private void HandleStateChanged(GameState state)
        {
            bool visible = state == GameState.Countdown || state == GameState.Playing;
            if (hudPanel != null)
            {
                hudPanel.SetActive(visible);
            }

            if (countdownLabel != null)
            {
                bool showLabel = state == GameState.Countdown || state == GameState.Playing;
                countdownLabel.gameObject.SetActive(showLabel);
                if (state == GameState.Playing)
                {
                    countdownLabel.SetText("GO!");
                    goTimer = goDuration;
                }
            }

            shownSeconds = -1;
        }

        // Text only changes once per second, so strings are only rebuilt then.
        private void Update()
        {
            GameState state = gameManager.State;
            if (state != GameState.Countdown && state != GameState.Playing)
            {
                return;
            }

            if (goTimer > 0f)
            {
                goTimer -= Time.deltaTime;
                if (goTimer <= 0f && countdownLabel != null)
                {
                    countdownLabel.gameObject.SetActive(false);
                }
            }

            int seconds = Mathf.CeilToInt(gameManager.TimeRemaining);
            if (seconds == shownSeconds)
            {
                return;
            }

            shownSeconds = seconds;
            if (state == GameState.Countdown)
            {
                if (countdownLabel != null)
                {
                    countdownLabel.SetText("{0}", seconds);
                }
            }
            else if (timeLabel != null)
            {
                timeLabel.SetText("{0}s", seconds);
            }
        }

        private void RefreshScore()
        {
            if (scoreLabel != null)
            {
                scoreLabel.SetText("{0}", gameManager.Session.Score);
            }
        }
    }
}
