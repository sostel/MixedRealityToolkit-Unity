using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

[System.Serializable]
public class RawEyeGazeRenderSettings
{
    public bool show_RawEyeGazes = true;
    public Color color_RawEyeGaze = Color.black;
    public ParticleSystem pointCloudParticleSystem_RawEyeGaze = null;
}

[RequireComponent(typeof(InputMemory_EyeGazeProcessed))]
public class HistoryPointCloud_EyeGazeProcessed : HistoryPointCloud
{
    [Header("Fixations")]
    [SerializeField]
    private bool show_Fixations = true;

    [SerializeField]
    private float distFromHitPoint = 0.01f;

    [SerializeField]
    private Color color_NoHit = Color.black;


    public RawEyeGazeRenderSettings rawEyeGazeSettings;

    [Header("Raw Eye Gaze")]
    [SerializeField]
    private bool show_RawEyeGazes = true;

    [SerializeField]
    private Color color_RawEyeGaze = Color.black;

    [SerializeField]
    internal ParticleSystem pointCloudParticleSystem_RawEyeGaze = null;

       
    public override void DisplayPointCloud()
    {
        // Render raw eye gaze points which are part of the fixations
        if (show_RawEyeGazes)
        {
            Display_RawEyeGaze();
        }

        // Render fixations
        if (show_Fixations)
        {
            Display_Fixations();
        }
    }

    public void Display_Fixations()
    {
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[pointCloudData.GetHistory().Length];

        for (int i = 0; i < pointCloudData.GetHistory().Length; i++)
        {
            InputMemory_EyeGazeProcessed inputMemory = (InputMemory_EyeGazeProcessed)pointCloudData.GetHistory()[i];

            // Render fixations
            if (show_Fixations && (inputMemory.FixationMeanDirection != null))
            {
                Ray ray = new Ray(inputMemory.FixationMeanOrigin, inputMemory.FixationMeanDirection);
                RaycastHit hitInfo = new RaycastHit();
                bool isHit = UnityEngine.Physics.Raycast(ray, out hitInfo);
                if (isHit)
                {
                    particleArray[i].position = hitInfo.point - ((hitInfo.point - ray.origin).normalized * distFromHitPoint);
                    particleArray[i].startColor = color_Default;
                    particleArray[i].startSize = inputMemory.FixationRadius * 3; // pointSizeInMeters;
                }
                else
                {
                    particleArray[i].position = ray.origin + ray.direction.normalized * 2;
                    particleArray[i].startColor = color_NoHit;
                    particleArray[i].startSize = inputMemory.FixationRadius * 3;
                }

                // ToDo: For rotation, use hit normal instead
                // ToDo: Do not constantly update
                particleArray[i].rotation3D = Quaternion.LookRotation(particleArray[i].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
            }
        }
        pointCloudParticleSystem.SetParticles(particleArray, particleArray.Length);
        ShowPointCloud();
    }

    public void Display_RawEyeGaze()
    {
        if (pointCloudParticleSystem_RawEyeGaze != null)
        {
            // Compute total amount of raw eye gazes that are part of any fixation
            int count = 0;
            for (int i = 0; i < pointCloudData.GetHistory().Length; i++)
            {
                InputMemory_EyeGazeProcessed inputMemory = (InputMemory_EyeGazeProcessed)pointCloudData.GetHistory()[i];
                count += inputMemory.rawEyeGazes.Count;
            }

            // Set up empty data structure for the individual particles
            ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[count];
            int index = 0;
            for (int j = 0; j < pointCloudData.GetHistory().Length; j++)
            {
                InputMemory_EyeGazeProcessed inputMemory_fixations = (InputMemory_EyeGazeProcessed)pointCloudData.GetHistory()[j];

                for (int i = 0; i < inputMemory_fixations.rawEyeGazes.Count; i++)
                {
                    InputMemory_EyeGazeRaw inputMemory = (InputMemory_EyeGazeRaw)inputMemory_fixations.rawEyeGazes[i];
                    if (inputMemory.lookedAtTarget != null)
                    {
                        particleArray[index].position = inputMemory.lookedAtPoint - (inputMemory.eyeGaze.direction.normalized * distFromHitPoint);
                        particleArray[index].startColor = color_RawEyeGaze;
                        // ToDo: For rotation, use hit normal instead
                        // ToDo: Do not constantly update
                        particleArray[index].rotation3D = Quaternion.LookRotation(particleArray[index].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
                    }
                    else
                    {
                        particleArray[index].position = inputMemory.eyeGaze.origin + inputMemory.eyeGaze.direction.normalized * 2;
                        particleArray[index].startColor = color_RawEyeGaze;
                        particleArray[index].rotation3D = Quaternion.LookRotation(particleArray[index].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
                    }

                    particleArray[index].startSize = pointSizeInMeters;
                    index++;
                }
            }
            pointCloudParticleSystem_RawEyeGaze.SetParticles(particleArray, particleArray.Length);
            ShowPointCloud();
        }
    }
}
