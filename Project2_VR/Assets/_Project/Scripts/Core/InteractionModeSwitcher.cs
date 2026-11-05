using UnityEngine;

namespace BaratieBlade.Core
{
    // Menus are used with the controllers and their rays; the round is played with the knives only.
    public class InteractionModeSwitcher : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject[] menuOnlyObjects;
        [SerializeField] private GameObject[] playOnlyObjects;

        private void OnEnable()
        {
            if (gameManager == null)
            {
                Debug.LogError($"{name}: game manager is not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            HandleStateChanged(gameManager.State);
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            bool playing = state == GameState.Countdown || state == GameState.Playing;
            SetActive(menuOnlyObjects, !playing);
            SetActive(playOnlyObjects, playing);
        }

        private static void SetActive(GameObject[] objects, bool active)
        {
            if (objects == null)
            {
                return;
            }

            foreach (GameObject target in objects)
            {
                if (target != null)
                {
                    target.SetActive(active);
                }
            }
        }
    }
}
