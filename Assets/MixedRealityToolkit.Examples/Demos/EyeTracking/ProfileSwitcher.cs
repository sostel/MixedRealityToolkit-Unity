using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSwitcher : MonoBehaviour
{
    [SerializeField]
    private MixedRealityToolkitConfigurationProfile profileToLoad;

    /// <summary>
    /// Load the referenced MixedRealityToolkitConfigurationProfile
    /// </summary>
    public void LoadIt()
    {
        if ((profileToLoad != null) && (MixedRealityToolkit.Instance != null))
        {
            MixedRealityToolkit.Instance.ActiveProfile = profileToLoad;
        }
    }
}
