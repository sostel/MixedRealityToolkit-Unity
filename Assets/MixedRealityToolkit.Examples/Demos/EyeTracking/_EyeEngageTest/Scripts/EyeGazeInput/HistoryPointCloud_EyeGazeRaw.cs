using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class HistoryPointCloud_EyeGazeRaw : HistoryPointCloud
{
    [SerializeField]
    private float distFromHitPoint = 0.01f;

    [SerializeField]
    private Color color_NoHit = Color.black;

    public override void DisplayPointCloud()
    {
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[pointCloudData.GetHistory().Length];
        for (int i = 0; i < pointCloudData.GetHistory().Length; i++)
        {
            InputMemory_EyeGazeRaw inputMemory = (InputMemory_EyeGazeRaw)pointCloudData.GetHistory()[i];
            if (inputMemory.lookedAtTarget != null)
            {
                particleArray[i].position = inputMemory.lookedAtPoint - (inputMemory.eyeGaze.direction.normalized * distFromHitPoint);
                particleArray[i].startColor = color_Default;
                // ToDo: For rotation, use hit normal instead
                // ToDo: Do not constantly update
                particleArray[i].rotation3D = Quaternion.LookRotation(particleArray[i].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
            }
            else
            {
                particleArray[i].position = inputMemory.eyeGaze.origin + inputMemory.eyeGaze.direction.normalized * 2;
                particleArray[i].startColor = color_NoHit;
                particleArray[i].rotation3D = Quaternion.LookRotation(particleArray[i].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
            }
           
            particleArray[i].startSize = pointSizeInMeters;
        }
        pointCloudParticleSystem.SetParticles(particleArray, particleArray.Length);
        ShowPointCloud();
    }
}
