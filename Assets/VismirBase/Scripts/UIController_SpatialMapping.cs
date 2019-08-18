using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class UIController_SpatialMapping : MonoBehaviour
{
    [SerializeField]
    private bool enableOnStartup = false;

    private bool isSpatialMappingActive = false;
    private bool isSpatialMeshVisible = false;

    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;
    protected IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
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

    private IMixedRealitySpatialAwarenessMeshObserver spatialObserver = null;
    protected IMixedRealitySpatialAwarenessMeshObserver SpatialObserver
    {
        get
        {
            MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessMeshObserver>(out spatialObserver);
            return spatialObserver;
        }
    }

    // Start is called before the first frame update
    private async void Start()
    {
        if (enableOnStartup)
        {
            await new WaitUntil(() => SpatialAwarenessSystem != null);
            UpdateToGivenStates();
        }
    }

    private void UpdateToGivenStates()
    {
        UpdateSpatialAwarenessTracking(isSpatialMappingActive);
        UpdateSpatialAwarenessVisibility(isSpatialMeshVisible);
    }

    #region Spatial awareness tracking
    private void UpdateSpatialAwarenessTracking(bool enable)
    {
        if (SpatialAwarenessSystem != null) 
        {
            DebugConsole.Console.Text("UpdateSpatialAwarenessTracking: " + enable);
            if (enable)
            {
                SpatialAwarenessSystem.Enable();
            }
            else
            {
                SpatialAwarenessSystem.Disable();
            }

            isSpatialMappingActive = enable; 
        }
    }

    public void Enable_Tracking()
    {
        UpdateSpatialAwarenessTracking(true);
    }

    public void Disable_Tracking()
    {
        UpdateSpatialAwarenessTracking(false);
    }

    public void Toggle_Tracking()
    {
        UpdateSpatialAwarenessTracking(!isSpatialMappingActive);
    }
    #endregion

    #region Visbility of spatial mesh
    private void UpdateSpatialAwarenessVisibility(bool enable)
    {
        if (SpatialObserver != null)
        {
            DebugConsole.Console.Text("UpdateSpatialAwarenessVisibility: " + enable);

            if (enable)
            {
                SpatialObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
            }
            else
            {
                SpatialObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
            }

            isSpatialMeshVisible = enable;             
        }
    }

    public void Enable_SpatialMeshVis()
    {
        UpdateSpatialAwarenessVisibility(true);
    }

    public void Disable_SpatialMeshVis()
    {
        UpdateSpatialAwarenessVisibility(false);
    }

    public void Toggle_SpatialMeshVis()
    {
        UpdateSpatialAwarenessVisibility(!isSpatialMeshVisible);
    }
    #endregion
}
