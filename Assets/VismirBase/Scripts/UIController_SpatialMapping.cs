using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

public class UIController_SpatialMapping : MonoBehaviour
{
    public bool isSpatialMappingActive = false;
    public bool isSpatialMeshVisible = false;

    private bool prev_isSpatialMappingActive;
    private bool prev_isSpatialMeshVisible;

    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;
    protected IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
    {
        get
        {
            MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
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
    void Start()
    {
        UpdateToGivenStates();
    }

    // Update is called once per frame
    void Update()
    {
        // Only update if something changed
        if (prev_isSpatialMappingActive != isSpatialMappingActive)
        {
            UpdateSpatialAwarenessTracking(isSpatialMappingActive);
        }

        if (prev_isSpatialMeshVisible != isSpatialMeshVisible)
        {
            UpdateSpatialAwarenessVisibility(isSpatialMeshVisible);
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

            isSpatialMappingActive = enable; // Just in case this has been changed from within the code
            prev_isSpatialMappingActive = enable;
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

            isSpatialMeshVisible = enable; // Just in case this has been changed from within the code
            prev_isSpatialMeshVisible = enable;
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
