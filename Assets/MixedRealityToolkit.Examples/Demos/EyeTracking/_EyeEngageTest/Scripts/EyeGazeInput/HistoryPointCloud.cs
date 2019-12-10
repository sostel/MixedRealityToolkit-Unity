using UnityEngine;

public abstract class HistoryPointCloud : MonoBehaviour
{
    [SerializeField]
    internal ParticleSystem pointCloudParticleSystem;

    [SerializeField]
    internal HistoryBase pointCloudData = null;
    
    [SerializeField]
    internal float pointSizeInMeters = .01f;

    [SerializeField]
    internal bool isRunning = true;

    private ParticleSystem.EmissionModule emissionModule;

    public Color color_Default = Color.gray;
    
    private void Start()
    {
        if (pointCloudParticleSystem == null) 
        {
            pointCloudParticleSystem = GetComponent<ParticleSystem>();
        }
        
        emissionModule = pointCloudParticleSystem.emission;
    }

    public void Update()
    {
        if (isRunning)
        {
            DisplayPointCloud();
        }
    }

    public abstract void DisplayPointCloud();

    public void ShowPointCloud()
    {
        emissionModule.enabled = true;
    }

    public void HidePointCloud()
    {
        emissionModule.enabled = false;
    }
}
