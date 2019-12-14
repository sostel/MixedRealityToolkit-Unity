using Microsoft.MixedReality.Toolkit.Examples;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RawEyeGazeRenderSettings
{
    public bool show = true;
    public ParticleSystem particleSystem = null;
    public Color particleColor = Color.black;
}

[RequireComponent(typeof(InputMemory_EyeGazeProcessed))]
public class HistoryPointCloud_EyeGazeProcessed : HistoryPointCloud
{
    [SerializeField]
    private bool show_Fixations = true;

    [SerializeField]
    private float distFromHitPoint = 0.01f;

    [SerializeField]
    private Color color_NoHit = Color.black;

    public RawEyeGazeRenderSettings rawEyeGazeSettings;
       
    public override void DisplayPointCloud()
    {
        // Render raw eye gaze points which are part of the fixations
        if (rawEyeGazeSettings.show)
        {
            Display_RawEyeGaze();
        }

        // Render fixations
        if (show_Fixations)
        {
            Display_Fixations();
        }


        if ((showLines) && (templateLineRenderer != null))
        {
            InputMemory[] memories = History_EyeGazeRaw.Instance.GetHistory();
            ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[memories.Length];
            rawEyeGazeSettings.particleSystem.GetParticles(particleArray, particleArray.Length);


            InputMemory_EyeGazeRaw memory1 = (InputMemory_EyeGazeRaw)History_EyeGazeRaw.Instance.GetMostRecentMemory();
            Vector3 v1 = memory1.eyeGaze.origin + memory1.eyeGaze.direction.normalized * 2;

            InputMemory_EyeGazeRaw memory2 = (InputMemory_EyeGazeRaw)History_EyeGazeRaw.Instance.GetMemoryBefore(memory1.timestamp);
            while (memory2 != null)
            {
                Vector3 v2 = memory2.eyeGaze.origin + memory2.eyeGaze.direction.normalized * 2;

                LineRenderer line = GameObject.Instantiate(templateLineRenderer);
                line.SetPosition(0, v1);
                line.SetPosition(1, v2);
                lines.Add(line);

                // Update for next round
                memory1 = memory2;
                memory2 = (InputMemory_EyeGazeRaw)History_EyeGazeRaw.Instance.GetMemoryBefore(memory1.timestamp);
                v1 = new Vector3(v2.x, v2.y, v2.z);
            }
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

    public Color voiceStart;
    public Color voiceRecoFailed;
    public Color voiceRecognized;
    public LineRenderer templateLineRenderer;
    List<LineRenderer> lines;
    public bool showLines = false;

    public void Display_RawEyeGaze()
    {
        if (rawEyeGazeSettings.particleSystem != null)
        {
            if (lines != null)
            {
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    GameObject.Destroy(lines[i]);
                    lines.RemoveAt(i);
                }
            }
            lines = new List<LineRenderer>();
            
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

                    Color pColor = rawEyeGazeSettings.particleColor;

                    if (inputMemory.voiceInputStatus == VoiceDictationStatus.Unknown)
                    {
                        pColor = rawEyeGazeSettings.particleColor;
                    }
                    else if ((inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_Result_High) ||
                        (inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_Result_Medium) ||
                        (inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_Result_Low))
                    {
                        pColor = voiceRecognized;
                    }
                    else if (inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_Result_Rejected)
                    {
                        pColor = voiceRecoFailed;
                    }
                    else if ((inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_HypothesisStart) ||
                        (inputMemory.voiceInputStatus == VoiceDictationStatus.Dictation_HypothesisCont))
                    {
                        pColor = voiceStart;
                    }

                    if (inputMemory.lookedAtTarget != null)
                    {
                        particleArray[index].position = inputMemory.lookedAtPoint - (inputMemory.eyeGaze.direction.normalized * distFromHitPoint);
                        particleArray[index].startColor = pColor;
                        // ToDo: For rotation, use hit normal instead
                        // ToDo: Do not constantly update
                        particleArray[index].rotation3D = Quaternion.LookRotation(particleArray[index].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
                    }
                    else
                    {
                        particleArray[index].position = inputMemory.eyeGaze.origin + inputMemory.eyeGaze.direction.normalized * 2;
                        particleArray[index].startColor = pColor;
                        particleArray[index].rotation3D = Quaternion.LookRotation(particleArray[index].position - CameraCache.Main.transform.position, Vector3.down).eulerAngles;
                    }

                   

                    particleArray[index].startSize = pointSizeInMeters;
                    index++;
                }
            }
            rawEyeGazeSettings.particleSystem.SetParticles(particleArray, particleArray.Length);
            ShowPointCloud();
        }
    }

    private void UpdateConnectorLines(ref LineRenderer line, Vector3 fromPos, Vector3 toPos, bool show)
    {
        //line.SetActive(show);

        line.SetPosition(0, fromPos);
        line.SetPosition(1, toPos);
    }
}
