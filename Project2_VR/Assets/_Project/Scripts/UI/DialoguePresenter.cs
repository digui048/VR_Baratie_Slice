using System;
using BaratieBlade.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaratieBlade.UI
{
    // Visual novel style box: portrait, speaker name and text that types itself out line by line.
    public class DialoguePresenter : MonoBehaviour
    {
        [Serializable]
        private struct DialogueLine
        {
            [TextArea] public string text;
            public AudioClip voiceClip;
        }

        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerLabel;
        [SerializeField] private TMP_Text bodyLabel;
        [SerializeField] private Button nextButton;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private string speakerName = "Zeff";
        [SerializeField] private DialogueLine[] lines;
        [SerializeField] private float charactersPerSecond = 40f;

        private int lineIndex;
        private float revealTimer;
        private bool running;

        private void OnEnable()
        {
            if (gameManager == null || dialoguePanel == null || bodyLabel == null)
            {
                Debug.LogError($"{name}: dialogue references are not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(Advance);
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
            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(Advance);
            }
        }

        private void HandleStateChanged(GameState state)
        {
            bool intro = state == GameState.Intro;
            dialoguePanel.SetActive(intro);

            if (intro)
            {
                lineIndex = 0;
                running = true;
                ShowLine();
            }
            else
            {
                running = false;
            }
        }

        private void Update()
        {
            if (!running)
            {
                return;
            }

            int total = bodyLabel.textInfo.characterCount;
            if (bodyLabel.maxVisibleCharacters < total)
            {
                revealTimer += Time.deltaTime;
                bodyLabel.maxVisibleCharacters = Mathf.Min(total, Mathf.FloorToInt(revealTimer * charactersPerSecond));
            }
        }

        // First press finishes the current line, the next one moves on.
        private void Advance()
        {
            if (!running)
            {
                return;
            }

            if (bodyLabel.maxVisibleCharacters < bodyLabel.textInfo.characterCount)
            {
                bodyLabel.maxVisibleCharacters = bodyLabel.textInfo.characterCount;
                return;
            }

            lineIndex++;
            if (lines == null || lineIndex >= lines.Length)
            {
                running = false;
                gameManager.BeginCountdown();
                return;
            }

            ShowLine();
        }

        private void ShowLine()
        {
            if (lines == null || lines.Length == 0)
            {
                running = false;
                gameManager.BeginCountdown();
                return;
            }

            DialogueLine line = lines[lineIndex];
            if (speakerLabel != null)
            {
                speakerLabel.text = speakerName;
            }

            bodyLabel.text = line.text;
            bodyLabel.ForceMeshUpdate();
            bodyLabel.maxVisibleCharacters = 0;
            revealTimer = 0f;

            if (voiceSource != null && line.voiceClip != null)
            {
                voiceSource.PlayOneShot(line.voiceClip);
            }
        }
    }
}
