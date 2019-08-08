using UnityEngine;

public class KeepSameVisualAngle : MonoBehaviour
{
    public Vector3 minTargetSizeInVisAngle = new Vector3(0.5f, 0.5f, 0.01f);
    public Vector3 maxTargetSizeInVisAngle = new Vector3(2.5f, 2.5f, 0.01f);

    private Vector3 sizeInVisAngle;

    // Update is called once per frame
    void Update()
    {
        KeepConstantVisAngleTargetSize();
    }
    
    /// <summary>
    /// Keeps the target size in visual angle constant.
    /// </summary>
    private void KeepConstantVisAngleTargetSize()
    {
        float distObjToCam = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        sizeInVisAngle = MetersToVisAngleInDegrees(transform.localScale, distObjToCam);
               
        if ((sizeInVisAngle.x < minTargetSizeInVisAngle.x) || (sizeInVisAngle.y < minTargetSizeInVisAngle.y))
        {
            this.transform.localScale = VisAngleInDegreesToMeters(minTargetSizeInVisAngle, distObjToCam);
        }
        else if ((sizeInVisAngle.x > maxTargetSizeInVisAngle.x) || (sizeInVisAngle.y > maxTargetSizeInVisAngle.y))
        {
            this.transform.localScale = VisAngleInDegreesToMeters(maxTargetSizeInVisAngle, distObjToCam);
        }
    }

    /// <summary>
    /// Returns the metric size in meters of a given Vector3 in visual angle in degrees and a given viewing distance in meters.
    /// </summary>
    /// <param name="visAngleInDegrees">In degrees.</param>
    /// <param name="distInMeters">In meters.</param>
    /// <returns></returns>
    public static Vector3 VisAngleInDegreesToMeters(Vector3 visAngleInDegrees, float distInMeters)
    {
        return new Vector3(
            VisAngleInDegreesToMeters(visAngleInDegrees.x, distInMeters),
            VisAngleInDegreesToMeters(visAngleInDegrees.y, distInMeters),
            VisAngleInDegreesToMeters(visAngleInDegrees.z, distInMeters));
    }

    /// <summary>
    /// Computes the metric size (in meters) for a given visual angle size.
    /// </summary>
    /// <param name="visAngleInDegrees">In degrees.</param>
    /// <param name="distInMeters">In meters.</param>
    /// <returns></returns>
    public static float VisAngleInDegreesToMeters(float visAngleInDegrees, float distInMeters)
    {
        return (2 * Mathf.Tan(Mathf.Deg2Rad * visAngleInDegrees / 2) * distInMeters);
    }

    /// <summary>
    /// Returns the metric size in meters of a given Vector3 in visual angle in degrees and a given viewing distance in meters.
    /// </summary>
    /// <param name="visAngleInDegrees">In degrees.</param>
    /// <param name="distInMeters">In meters.</param>
    /// <returns></returns>
    public static Vector3 MetersToVisAngleInDegrees(Vector3 sizeInMeters, float distInMeters)
    {
        return new Vector3(
            MetersToVisAngleInDegrees(sizeInMeters.x, distInMeters),
            MetersToVisAngleInDegrees(sizeInMeters.y, distInMeters),
            MetersToVisAngleInDegrees(sizeInMeters.z, distInMeters));
    }

    /// <summary>
    /// Computes the metric size (in meters) for a given visual angle size.
    /// </summary>
    /// <param name="visAngleInDegrees">In degrees.</param>
    /// <param name="distInMeters">In meters.</param>
    /// <returns></returns>
    public static float MetersToVisAngleInDegrees(float sizeInMeters, float distInMeters)
    {
        return 2 * Mathf.Atan(sizeInMeters / (2 * distInMeters)) / Mathf.Deg2Rad;
    }
}