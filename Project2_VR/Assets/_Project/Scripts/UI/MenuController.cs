using BaratieBlade.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BaratieBlade.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button startButton;

        private void OnEnable()
        {
            if (gameManager == null || menuPanel == null || startButton == null)
            {
                Debug.LogError($"{name}: menu references are not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            startButton.onClick.AddListener(gameManager.StartIntro);
            HandleStateChanged(gameManager.State);
        }

        private void OnDisable()
        {
            if (gameManager == null || startButton == null)
            {
                return;
            }

            gameManager.StateChanged -= HandleStateChanged;
            startButton.onClick.RemoveListener(gameManager.StartIntro);
        }

        private void HandleStateChanged(GameState state)
        {
            menuPanel.SetActive(state == GameState.Menu);
        }
    }
}
