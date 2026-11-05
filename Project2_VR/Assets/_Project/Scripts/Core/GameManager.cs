using System;
using UnityEngine;

namespace BaratieBlade.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SessionController session;
        [SerializeField] private float countdownDuration = 3f;
        [SerializeField] private float roundDuration = 60f;
        [Tooltip("Launch interval range at the start of the round.")]
        [SerializeField] private Vector2 startLaunchInterval = new Vector2(1.1f, 1.5f);
        [Tooltip("Launch interval range reached at the end of the round.")]
        [SerializeField] private Vector2 endLaunchInterval = new Vector2(0.5f, 0.75f);

        private float stateTimer;

        public event Action<GameState> StateChanged;

        public GameState State { get; private set; } = GameState.Menu;

        // Seconds left in the countdown or in the round, depending on the state.
        public float TimeRemaining => Mathf.Max(0f, stateTimer);

        public SessionController Session => session;

        private void Start()
        {
            if (session == null)
            {
                Debug.LogError($"{name}: session controller is not assigned.", this);
                enabled = false;
                return;
            }

            session.ResetSession();
            SetState(GameState.Menu);
        }

        public void StartIntro()
        {
            if (State == GameState.Menu)
            {
                SetState(GameState.Intro);
            }
        }

        public void BeginCountdown()
        {
            if (State != GameState.Intro && State != GameState.EndScreen)
            {
                return;
            }

            session.ResetSession();
            stateTimer = countdownDuration;
            SetState(GameState.Countdown);
        }

        public void Restart()
        {
            BeginCountdown();
        }

        public void ReturnToMenu()
        {
            session.ResetSession();
            SetState(GameState.Menu);
        }

        private void Update()
        {
            if (State != GameState.Countdown && State != GameState.Playing)
            {
                return;
            }

            stateTimer -= Time.deltaTime;

            if (State == GameState.Countdown)
            {
                if (stateTimer <= 0f)
                {
                    StartRound();
                }
                return;
            }

            float progress = 1f - Mathf.Clamp01(stateTimer / roundDuration);
            session.Launcher.SetLaunchInterval(Vector2.Lerp(startLaunchInterval, endLaunchInterval, progress));

            if (stateTimer <= 0f)
            {
                EndRound();
            }
        }

        private void StartRound()
        {
            stateTimer = roundDuration;
            session.Launcher.SetLaunchInterval(startLaunchInterval);
            session.Launcher.StartLaunching();
            SetState(GameState.Playing);
        }

        private void EndRound()
        {
            session.Launcher.StopLaunching();
            session.Launcher.RecycleAll();
            SetState(GameState.EndScreen);
        }

        private void SetState(GameState next)
        {
            State = next;
            StateChanged?.Invoke(next);
        }
    }
}
