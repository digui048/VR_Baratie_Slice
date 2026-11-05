using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace BaratieBlade.Core
{
    // Forces floor-relative tracking so the camera starts at the player's real height.
    // Over Link the runtime can fall back to device (eye level) tracking, which puts the view on the floor.
    public class FloorTrackingOrigin : MonoBehaviour
    {
        private readonly List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        private bool applied;

        private void Update()
        {
            if (applied)
            {
                return;
            }

            SubsystemManager.GetSubsystems(subsystems);
            if (subsystems.Count == 0)
            {
                return;
            }

            applied = true;
            foreach (XRInputSubsystem subsystem in subsystems)
            {
                if (!subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                {
                    Debug.LogWarning("Floor tracking origin is not supported by the current XR runtime.", this);
                    continue;
                }

                subsystem.TryRecenter();
            }
        }
    }
}
