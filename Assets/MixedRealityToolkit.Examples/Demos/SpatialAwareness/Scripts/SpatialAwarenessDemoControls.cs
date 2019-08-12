// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class SpatialAwarenessDemoControls : MonoBehaviour
    {
        [SerializeField]
        private bool enableOnStartup = false;

        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;
        private bool isEnabled = false;

        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (spatialAwarenessSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
                }
                return spatialAwarenessSystem;
            }
        }

        private async void Start()
        {
            if (enableOnStartup)
            {
                await new WaitUntil(() => SpatialAwarenessSystem != null);

                // Ensure that the spatial awareness system is enabled
                TurnOn();
            }
        }

        public void Toggle()
        {
            if (isEnabled)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        /// <summary>
        /// Enables the spatial awareness system.
        /// </summary>
        public void TurnOn()
        {
            Debug.Log("Turn it ON");
            if (spatialAwarenessSystem != null)
            {
                SpatialAwarenessSystem.Enable();
                isEnabled = true;
            }
        }

        /// <summary>
        /// Disables the spatial awareness system.
        /// </summary>
        public void TurnOff()
        {
            Debug.Log("Turn it off");
            if (spatialAwarenessSystem != null)
            {

                SpatialAwarenessSystem.Disable();
                isEnabled = false;
            }
        }
    }
}
