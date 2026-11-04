using System;
using UnityEngine;

namespace BaratieBlade.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ingredient : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Flying,
            Sliced
        }

        [SerializeField] private GameObject wholeVisual;
        [SerializeField] private Rigidbody halfA;
        [SerializeField] private Rigidbody halfB;
        [Tooltip("Local axis the two halves split along.")]
        [SerializeField] private Vector3 splitAxis = Vector3.right;
        [SerializeField] private float separationSpeed = 1.2f;
        [SerializeField] private float halfLifetime = 2f;
        [SerializeField] private float missHeight = 0.4f;
        [SerializeField] private float maxFlightTime = 6f;
        [SerializeField] private int points = 10;

        private Rigidbody body;
        private Collider hitCollider;
        private Vector3 halfALocalPosition;
        private Quaternion halfALocalRotation;
        private Vector3 halfBLocalPosition;
        private Quaternion halfBLocalRotation;
        private State state = State.Idle;
        private float stateTime;

        public event Action<Ingredient> Sliced;
        public event Action<Ingredient> Missed;
        public event Action<Ingredient> Finished;

        public int Points => points;
        public bool IsSliced => state == State.Sliced;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            hitCollider = GetComponent<Collider>();

            if (wholeVisual == null || halfA == null || halfB == null)
            {
                Debug.LogError($"{name}: whole visual or halves are not assigned.", this);
                return;
            }

            halfALocalPosition = halfA.transform.localPosition;
            halfALocalRotation = halfA.transform.localRotation;
            halfBLocalPosition = halfB.transform.localPosition;
            halfBLocalRotation = halfB.transform.localRotation;
        }

        public void Launch(Vector3 position, Vector3 velocity, Vector3 angularVelocity)
        {
            ResetPieces();
            transform.SetPositionAndRotation(position, Quaternion.identity);
            gameObject.SetActive(true);

            body.isKinematic = false;
            body.linearVelocity = velocity;
            body.angularVelocity = angularVelocity;

            state = State.Flying;
            stateTime = 0f;
        }

        public bool TrySlice(Vector3 knifeVelocity, Vector3 bladeNormal)
        {
            if (state != State.Flying || halfA == null || halfB == null)
            {
                return false;
            }

            Vector3 inheritedVelocity = body.linearVelocity * 0.5f + knifeVelocity * 0.15f;

            body.isKinematic = true;
            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            // Turn the ingredient so its cut lines up with the blade before showing the halves.
            Vector3 currentSplit = transform.TransformDirection(splitAxis);
            if (Vector3.Dot(currentSplit, bladeNormal) < 0f)
            {
                bladeNormal = -bladeNormal;
            }
            transform.rotation = Quaternion.FromToRotation(currentSplit, bladeNormal) * transform.rotation;

            wholeVisual.SetActive(false);
            ReleaseHalf(halfA, inheritedVelocity - bladeNormal * separationSpeed, bladeNormal);
            ReleaseHalf(halfB, inheritedVelocity + bladeNormal * separationSpeed, -bladeNormal);

            state = State.Sliced;
            stateTime = 0f;
            Sliced?.Invoke(this);
            return true;
        }

        // Used by the launcher when a session ends early.
        public void ForceFinish()
        {
            if (state == State.Idle)
            {
                return;
            }

            Finish();
        }

        private void Update()
        {
            if (state == State.Idle)
            {
                return;
            }

            stateTime += Time.deltaTime;

            if (state == State.Flying)
            {
                bool falling = body.linearVelocity.y < 0f;
                if ((falling && transform.position.y < missHeight) || stateTime > maxFlightTime)
                {
                    Missed?.Invoke(this);
                    Finish();
                }
            }
            else if (stateTime > halfLifetime)
            {
                Finish();
            }
        }

        private void ReleaseHalf(Rigidbody half, Vector3 velocity, Vector3 spinAxis)
        {
            half.gameObject.SetActive(true);
            half.isKinematic = false;
            half.linearVelocity = velocity;
            half.angularVelocity = Vector3.Cross(spinAxis, Vector3.up) * 6f;
        }

        private void ResetPieces()
        {
            if (wholeVisual != null)
            {
                wholeVisual.SetActive(true);
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            ResetHalf(halfA, halfALocalPosition, halfALocalRotation);
            ResetHalf(halfB, halfBLocalPosition, halfBLocalRotation);
        }

        private static void ResetHalf(Rigidbody half, Vector3 localPosition, Quaternion localRotation)
        {
            if (half == null)
            {
                return;
            }

            half.isKinematic = true;
            half.transform.SetLocalPositionAndRotation(localPosition, localRotation);
            half.gameObject.SetActive(false);
        }

        private void Finish()
        {
            state = State.Idle;
            body.isKinematic = true;
            Finished?.Invoke(this);
        }
    }
}
