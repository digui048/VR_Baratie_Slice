using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BaratieBlade.Gameplay
{
    public class IngredientLauncher : MonoBehaviour
    {
        [SerializeField] private Ingredient[] ingredientPrefabs;
        [SerializeField] private Transform[] launchPoints;
        [Tooltip("Point in front of the player that every arc passes close to.")]
        [SerializeField] private Transform sliceZone;
        [SerializeField] private Vector2 sliceZoneSpread = new Vector2(0.6f, 0.3f);
        [SerializeField] private Vector2 apexHeightRange = new Vector2(0.35f, 0.8f);
        [SerializeField] private Vector2 launchIntervalRange = new Vector2(0.7f, 1.4f);
        [SerializeField] private float maxSpin = 6f;
        [SerializeField] private int prewarmPerType = 4;
        [SerializeField] private bool launchOnStart;

        private readonly List<Ingredient> activeIngredients = new List<Ingredient>();
        private readonly Dictionary<Ingredient, IngredientPool> owningPool = new Dictionary<Ingredient, IngredientPool>();
        private IngredientPool[] pools;
        private float nextLaunchTimer;

        public event Action<Ingredient> IngredientLaunched;
        public event Action<Ingredient> IngredientSliced;
        public event Action<Ingredient> IngredientMissed;

        public bool IsLaunching { get; private set; }
        public int LaunchedCount { get; private set; }

        private void Awake()
        {
            if (ingredientPrefabs == null || ingredientPrefabs.Length == 0 || launchPoints == null || launchPoints.Length == 0 || sliceZone == null)
            {
                Debug.LogError($"{name}: prefabs, launch points and slice zone must be assigned.", this);
                enabled = false;
                return;
            }

            pools = new IngredientPool[ingredientPrefabs.Length];
            for (int i = 0; i < ingredientPrefabs.Length; i++)
            {
                IngredientPool pool = null;
                pool = new IngredientPool(ingredientPrefabs[i], transform, created => Register(created, pool));
                pool.Prewarm(prewarmPerType);
                pools[i] = pool;
            }
        }

        private void Start()
        {
            if (launchOnStart)
            {
                StartLaunching();
            }
        }

        private void OnDestroy()
        {
            foreach (Ingredient ingredient in owningPool.Keys)
            {
                if (ingredient == null)
                {
                    continue;
                }

                ingredient.Sliced -= HandleSliced;
                ingredient.Missed -= HandleMissed;
                ingredient.Finished -= HandleFinished;
            }
        }

        public void StartLaunching()
        {
            LaunchedCount = 0;
            nextLaunchTimer = 0.5f;
            IsLaunching = true;
        }

        public void StopLaunching()
        {
            IsLaunching = false;
        }

        public void SetLaunchInterval(Vector2 intervalRange)
        {
            launchIntervalRange = intervalRange;
        }

        public void RecycleAll()
        {
            // Iterate backwards because finishing an ingredient removes it from the list.
            for (int i = activeIngredients.Count - 1; i >= 0; i--)
            {
                activeIngredients[i].ForceFinish();
            }
        }

        private void Update()
        {
            if (!IsLaunching)
            {
                return;
            }

            nextLaunchTimer -= Time.deltaTime;
            if (nextLaunchTimer <= 0f)
            {
                LaunchOne();
                nextLaunchTimer = Random.Range(launchIntervalRange.x, launchIntervalRange.y);
            }
        }

        private void LaunchOne()
        {
            IngredientPool pool = pools[Random.Range(0, pools.Length)];
            Transform launchPoint = launchPoints[Random.Range(0, launchPoints.Length)];
            if (launchPoint == null)
            {
                return;
            }

            Vector3 start = launchPoint.position;
            Vector3 target = sliceZone.position
                + sliceZone.right * Random.Range(-sliceZoneSpread.x, sliceZoneSpread.x)
                + sliceZone.up * Random.Range(-sliceZoneSpread.y, sliceZoneSpread.y);
            Vector3 velocity = ComputeArcVelocity(start, target, Random.Range(apexHeightRange.x, apexHeightRange.y));
            Vector3 spin = Random.insideUnitSphere * maxSpin;

            Ingredient ingredient = pool.Get();
            activeIngredients.Add(ingredient);
            ingredient.Launch(start, velocity, spin);
            LaunchedCount++;
            IngredientLaunched?.Invoke(ingredient);
        }

        // Ballistic launch that peaks apexHeight above the higher of the two points and lands on target.
        private static Vector3 ComputeArcVelocity(Vector3 start, Vector3 target, float apexHeight)
        {
            float gravity = -Physics.gravity.y;
            float apexY = Mathf.Max(start.y, target.y) + apexHeight;

            float upSpeed = Mathf.Sqrt(2f * gravity * (apexY - start.y));
            float timeUp = upSpeed / gravity;
            float timeDown = Mathf.Sqrt(2f * (apexY - target.y) / gravity);

            Vector3 horizontal = target - start;
            horizontal.y = 0f;

            return horizontal / (timeUp + timeDown) + Vector3.up * upSpeed;
        }

        private void Register(Ingredient ingredient, IngredientPool pool)
        {
            owningPool[ingredient] = pool;
            ingredient.Sliced += HandleSliced;
            ingredient.Missed += HandleMissed;
            ingredient.Finished += HandleFinished;
        }

        private void HandleSliced(Ingredient ingredient)
        {
            IngredientSliced?.Invoke(ingredient);
        }

        private void HandleMissed(Ingredient ingredient)
        {
            IngredientMissed?.Invoke(ingredient);
        }

        private void HandleFinished(Ingredient ingredient)
        {
            activeIngredients.Remove(ingredient);
            if (owningPool.TryGetValue(ingredient, out IngredientPool pool))
            {
                pool.Return(ingredient);
            }
        }
    }
}
