using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

[RequireComponent(typeof(InputMemory_EyeGazeProcessed))]
public class HistoryPointCloud_EyeGazeProcessed: HistoryPointCloud
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
            InputMemory_EyeGazeProcessed inputMemory = (InputMemory_EyeGazeProcessed)pointCloudData.GetHistory()[i];
            if (inputMemory.FixationMeanDirection != null)
            {
                Vector3 origin = CameraCache.Main.transform.position;
                Ray ray = new Ray(origin, inputMemory.FixationMeanDirection);
                RaycastHit hitInfo = new RaycastHit();
                bool isHit = UnityEngine.Physics.Raycast(ray, out hitInfo);
                if (isHit)
                {
                    particleArray[i].position = hitInfo.point - ((hitInfo.point - origin).normalized * distFromHitPoint);
                    particleArray[i].startColor = color_Default;
                }
                else
                {
                    particleArray[i].position = origin + inputMemory.FixationMeanDirection.normalized * 2;
                    particleArray[i].startColor = color_NoHit;
                }               
                
                // ToDo: For rotation, use hit normal instead
                // ToDo: Do not constantly update
                particleArray[i].rotation3D = Quaternion.LookRotation(particleArray[i].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
            }
            
            particleArray[i].startSize = pointSizeInMeters;
        }
        pointCloudParticleSystem.SetParticles(particleArray, particleArray.Length);
        ShowPointCloud();
    }
}
