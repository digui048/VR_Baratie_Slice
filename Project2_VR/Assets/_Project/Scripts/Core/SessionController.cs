using System;
using BaratieBlade.Gameplay;
using UnityEngine;

namespace BaratieBlade.Core
{
    // Keeps the stats of the current round and puts everything back to a clean state between rounds.
    public class SessionController : MonoBehaviour
    {
        [SerializeField] private IngredientLauncher launcher;
        [Range(0f, 1f)]
        [SerializeField] private float headChefThreshold = 0.6f;
        [Range(0f, 1f)]
        [SerializeField] private float sanjiThreshold = 0.85f;

        public event Action StatsChanged;

        public int Score { get; private set; }
        public int SlicedCount { get; private set; }
        public int MissedCount { get; private set; }
        public int LaunchedCount { get; private set; }

        public float SliceRatio => LaunchedCount == 0 ? 0f : (float)SlicedCount / LaunchedCount;

        public IngredientLauncher Launcher => launcher;

        private void OnEnable()
        {
            if (launcher == null)
            {
                Debug.LogError($"{name}: launcher is not assigned.", this);
                return;
            }

            launcher.IngredientLaunched += HandleLaunched;
            launcher.IngredientSliced += HandleSliced;
            launcher.IngredientMissed += HandleMissed;
        }

        private void OnDisable()
        {
            if (launcher == null)
            {
                return;
            }

            launcher.IngredientLaunched -= HandleLaunched;
            launcher.IngredientSliced -= HandleSliced;
            launcher.IngredientMissed -= HandleMissed;
        }

        public void ResetSession()
        {
            if (launcher != null)
            {
                launcher.StopLaunching();
                launcher.RecycleAll();
            }

            Score = 0;
            SlicedCount = 0;
            MissedCount = 0;
            LaunchedCount = 0;
            StatsChanged?.Invoke();
        }

        public string GetRating()
        {
            float ratio = SliceRatio;
            if (ratio >= sanjiThreshold)
            {
                return "Sanji-level";
            }

            return ratio >= headChefThreshold ? "Head Chef" : "Sous Chef";
        }

        private void HandleLaunched(Ingredient ingredient)
        {
            LaunchedCount++;
            StatsChanged?.Invoke();
        }

        private void HandleSliced(Ingredient ingredient)
        {
            SlicedCount++;
            Score += ingredient.Points;
            StatsChanged?.Invoke();
        }

        private void HandleMissed(Ingredient ingredient)
        {
            MissedCount++;
            StatsChanged?.Invoke();
        }
    }
}
