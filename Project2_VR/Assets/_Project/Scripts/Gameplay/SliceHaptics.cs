using Oculus.Haptics;
using UnityEngine;

namespace BaratieBlade.Gameplay
{
    // Plays a short pulse on the controller holding the knife that made the cut.
    public class SliceHaptics : MonoBehaviour
    {
        [SerializeField] private HapticSource hapticSource;
        [SerializeField] private SliceDetector[] knives;

        private void OnEnable()
        {
            if (hapticSource == null || knives == null)
            {
                Debug.LogError($"{name}: haptic source or knives are not assigned.", this);
                return;
            }

            foreach (SliceDetector knife in knives)
            {
                if (knife != null)
                {
                    knife.Sliced += HandleSliced;
                }
            }
        }

        private void OnDisable()
        {
            if (knives == null)
            {
                return;
            }

            foreach (SliceDetector knife in knives)
            {
                if (knife != null)
                {
                    knife.Sliced -= HandleSliced;
                }
            }
        }

        private void HandleSliced(SliceDetector knife, Ingredient ingredient)
        {
            bool left = knife.TryGetComponent(out KnifeController controller) && controller.Hand == KnifeHand.Left;
            hapticSource.Play(left ? Controller.Left : Controller.Right);
        }
    }
}
