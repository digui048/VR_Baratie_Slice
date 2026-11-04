using System;
using UnityEngine;

namespace BaratieBlade.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class SliceDetector : MonoBehaviour
    {
        [SerializeField] private Transform bladeCenter;
        [SerializeField] private float minSliceSpeed = 1.5f;

        private Rigidbody body;
        private Vector3 lastPosition;
        private bool hasLastPosition;

        public event Action<SliceDetector, Ingredient> Sliced;

        public Vector3 Velocity { get; private set; }

        // The blade's flat side; the cut plane follows it.
        public Vector3 BladeNormal => bladeCenter.right;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            if (bladeCenter == null)
            {
                bladeCenter = transform;
            }
        }

        private void OnEnable()
        {
            hasLastPosition = false;
            Velocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            // The knife follows the tracked controller, so its speed has to come from position deltas.
            Vector3 position = bladeCenter.position;
            if (hasLastPosition)
            {
                Velocity = (position - lastPosition) / Time.fixedDeltaTime;
            }

            lastPosition = position;
            hasLastPosition = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Velocity.sqrMagnitude < minSliceSpeed * minSliceSpeed)
            {
                return;
            }

            Rigidbody otherBody = other.attachedRigidbody;
            if (otherBody == null || !otherBody.TryGetComponent(out Ingredient ingredient))
            {
                return;
            }

            if (ingredient.TrySlice(Velocity, BladeNormal))
            {
                Sliced?.Invoke(this, ingredient);
            }
        }
    }
}
