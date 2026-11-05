using BaratieBlade.Core;
using BaratieBlade.Gameplay;
using UnityEngine;

namespace BaratieBlade.Audio
{
    // Ambience and music are flat 2D sources; gameplay sounds are 3D and play where they happen.
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private IngredientLauncher launcher;

        [Header("Sources")]
        [SerializeField] private AudioSource kitchenAmbienceSource;
        [SerializeField] private AudioSource shipAmbienceSource;
        [SerializeField] private AudioSource menuMusicSource;
        [SerializeField] private AudioSource roundMusicSource;
        [Tooltip("Pool of 3D sources reused for slices and misses.")]
        [SerializeField] private AudioSource[] effectSources;
        [Tooltip("Separate 3D pool for launches, which happen further away and need more gain.")]
        [SerializeField] private AudioSource[] launchSources;
        [Tooltip("2D source for countdown and round cues.")]
        [SerializeField] private AudioSource cueSource;

        [Header("Ambience and music")]
        [SerializeField] private AudioClip kitchenAmbience;
        [SerializeField] private AudioClip shipAmbience;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip roundMusic;
        [SerializeField] private float menuMusicVolume = 0.2f;
        [SerializeField] private float roundMusicVolume = 0.5f;
        [SerializeField] private float musicFadeTime = 1.2f;
        [Tooltip("Ambience is turned down to this fraction during a round so music and slices stand out.")]
        [Range(0f, 1f)]
        [SerializeField] private float ambienceRoundScale = 0.35f;

        [Header("Effects")]
        [SerializeField] private AudioClip[] sliceClips;
        [SerializeField] private AudioClip[] launchClips;
        [SerializeField] private AudioClip missClip;
        [SerializeField] private AudioClip countdownClip;
        [SerializeField] private AudioClip roundEndClip;

        private int nextEffectSource;
        private int nextLaunchSource;
        private bool roundMusicActive;
        private float kitchenBaseVolume;
        private float shipBaseVolume;

        private void OnEnable()
        {
            if (gameManager == null || launcher == null)
            {
                Debug.LogError($"{name}: game manager or launcher is not assigned.", this);
                return;
            }

            gameManager.StateChanged += HandleStateChanged;
            launcher.IngredientLaunched += HandleLaunched;
            launcher.IngredientSliced += HandleSliced;
            launcher.IngredientMissed += HandleMissed;
        }

        private void OnDisable()
        {
            if (gameManager == null || launcher == null)
            {
                return;
            }

            gameManager.StateChanged -= HandleStateChanged;
            launcher.IngredientLaunched -= HandleLaunched;
            launcher.IngredientSliced -= HandleSliced;
            launcher.IngredientMissed -= HandleMissed;
        }

        private void Start()
        {
            kitchenBaseVolume = kitchenAmbienceSource != null ? kitchenAmbienceSource.volume : 0f;
            shipBaseVolume = shipAmbienceSource != null ? shipAmbienceSource.volume : 0f;

            StartLoop(kitchenAmbienceSource, kitchenAmbience, kitchenBaseVolume);
            StartLoop(shipAmbienceSource, shipAmbience, shipBaseVolume);
            StartLoop(menuMusicSource, menuMusic, roundMusicActive ? 0f : menuMusicVolume);
            StartLoop(roundMusicSource, roundMusic, roundMusicActive ? roundMusicVolume : 0f);
        }

        // Crossfade between the menu track and the round track, and duck the ambience while playing.
        private void Update()
        {
            float step = musicFadeTime > 0f ? Time.deltaTime / musicFadeTime : 1f;
            FadeTowards(menuMusicSource, roundMusicActive ? 0f : menuMusicVolume, step * menuMusicVolume);
            FadeTowards(roundMusicSource, roundMusicActive ? roundMusicVolume : 0f, step * roundMusicVolume);

            float ambienceScale = roundMusicActive ? ambienceRoundScale : 1f;
            FadeTowards(kitchenAmbienceSource, kitchenBaseVolume * ambienceScale, step * kitchenBaseVolume);
            FadeTowards(shipAmbienceSource, shipBaseVolume * ambienceScale, step * shipBaseVolume);
        }

        private void HandleStateChanged(GameState state)
        {
            bool wasRoundMusic = roundMusicActive;
            roundMusicActive = state == GameState.Countdown || state == GameState.Playing;
            if (roundMusicActive && !wasRoundMusic && roundMusicSource != null && roundMusicSource.clip != null)
            {
                // Each round starts the upbeat track from the beginning.
                roundMusicSource.time = 0f;
            }

            if (state == GameState.Countdown)
            {
                PlayCue(countdownClip);
            }
            else if (state == GameState.EndScreen)
            {
                PlayCue(roundEndClip);
            }
        }

        private void HandleLaunched(Ingredient ingredient)
        {
            PlayAt(PickRandom(launchClips), ingredient.transform.position, launchSources, ref nextLaunchSource);
        }

        private void HandleSliced(Ingredient ingredient)
        {
            PlayAt(PickRandom(sliceClips), ingredient.transform.position, effectSources, ref nextEffectSource);
        }

        private void HandleMissed(Ingredient ingredient)
        {
            PlayAt(missClip, ingredient.transform.position, effectSources, ref nextEffectSource);
        }

        private void PlayCue(AudioClip clip)
        {
            if (cueSource != null && clip != null)
            {
                cueSource.PlayOneShot(clip);
            }
        }

        private static void PlayAt(AudioClip clip, Vector3 position, AudioSource[] pool, ref int nextIndex)
        {
            if (clip == null || pool == null || pool.Length == 0)
            {
                return;
            }

            // Round robin over the pool so overlapping sounds don't cut each other off.
            AudioSource source = pool[nextIndex];
            nextIndex = (nextIndex + 1) % pool.Length;
            if (source == null)
            {
                return;
            }

            source.transform.position = position;
            source.pitch = Random.Range(0.92f, 1.08f);
            source.PlayOneShot(clip);
        }

        private static void StartLoop(AudioSource source, AudioClip clip, float volume)
        {
            if (source == null || clip == null)
            {
                return;
            }

            source.clip = clip;
            source.loop = true;
            source.spatialBlend = 0f;
            source.volume = volume;
            source.Play();
        }

        private static void FadeTowards(AudioSource source, float target, float step)
        {
            if (source != null && source.clip != null)
            {
                source.volume = Mathf.MoveTowards(source.volume, target, step);
            }
        }

        private static AudioClip PickRandom(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            return clips[Random.Range(0, clips.Length)];
        }
    }
}
