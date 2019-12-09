using UnityEngine;

public class HistoryPointCloud : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem pointCloudParticleSystem;

    [SerializeField]
    private History_EyeGazeRaw pointCloudData = null;
    
    [SerializeField]
    private float pointSizeInMeters = .01f;

    public bool isRunning = true;

    private ParticleSystem.EmissionModule emissionModule;
    
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

    public void DisplayPointCloud()
    {
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[pointCloudData.GetHistory().Length];
        for (int i = 0; i < pointCloudData.GetHistory().Length; i++)
        {
            InputMemory_EyeGazeRaw inputMemory = (InputMemory_EyeGazeRaw)pointCloudData.GetHistory()[i];
            if (inputMemory.lookedAtTarget != null)
            {
                particleArray[i].position = inputMemory.lookedAtPoint;
            }
            else 
            {
                particleArray[i].position = inputMemory.eyeGaze.origin + inputMemory.eyeGaze.direction.normalized * 2;
            }
            
            // particleArray[i].startColor = pointColor;
            particleArray[i].startSize = pointSizeInMeters;
        }
        pointCloudParticleSystem.SetParticles(particleArray, particleArray.Length);
        ShowPointCloud();
    }

    public void ShowPointCloud()
    {
        emissionModule.enabled = true;
    }

    public void HidePointCloud()
    {
        emissionModule.enabled = false;
    }
}
