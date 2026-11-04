using UnityEngine;

namespace BaratieBlade.Gameplay
{
    public enum KnifeHand
    {
        Left,
        Right
    }

    // Locks the knife to a controller anchor so it always sits in the same grip pose.
    public class KnifeController : MonoBehaviour
    {
        [SerializeField] private KnifeHand hand = KnifeHand.Right;
        [SerializeField] private Transform controllerAnchor;
        [SerializeField] private Vector3 gripPosition = Vector3.zero;
        [SerializeField] private Vector3 gripRotation = Vector3.zero;

        public KnifeHand Hand => hand;

        private void Awake()
        {
            if (controllerAnchor == null)
            {
                controllerAnchor = FindAnchorOnRig();
            }

            if (controllerAnchor == null)
            {
                Debug.LogError($"{name}: no controller anchor found for the {hand} hand.", this);
                return;
            }

            transform.SetParent(controllerAnchor, false);
            transform.localPosition = gripPosition;
            transform.localRotation = Quaternion.Euler(gripRotation);
        }

        private Transform FindAnchorOnRig()
        {
            var rig = FindFirstObjectByType<OVRCameraRig>();
            if (rig == null)
            {
                return null;
            }

            return hand == KnifeHand.Left ? rig.leftControllerAnchor : rig.rightControllerAnchor;
        }
    }
}
