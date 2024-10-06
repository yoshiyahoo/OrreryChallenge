using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitLineVisualizer : MonoBehaviour
{
    public KeplerOrbitalMotion keplerMotion;
    public int celestialBodyIndex;
    public int segments = 360;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        DrawOrbit();
    }

    void DrawOrbit()
    {
        if (keplerMotion == null || celestialBodyIndex < 0 || celestialBodyIndex >= keplerMotion.celestialBodies.Count)
        {
            Debug.LogError("Invalid KeplerOrbitalMotion reference or celestialBodyIndex");
            return;
        }

        var celestialBody = keplerMotion.celestialBodies[celestialBodyIndex];

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float eccentricAnomaly = 2f * Mathf.Atan(Mathf.Tan(angle / 2f) * Mathf.Sqrt((1f - celestialBody.eccentricity) / (1f + celestialBody.eccentricity)));
            float distance = celestialBody.semiMajorAxis * (1f - celestialBody.eccentricity * Mathf.Cos(eccentricAnomaly));

            Vector3 position = new Vector3(
                distance * Mathf.Cos(angle),
                0f,
                distance * Mathf.Sin(angle)
            );

            position = RotateVectorRelativeToEcliptic(position, celestialBody.inclination, celestialBody.longitudeOfAscendingNode, celestialBody.argumentOfPerihelion);
            lineRenderer.SetPosition(i, position * keplerMotion.auToUnityUnits);
        }
    }

    public Vector3 eclipticNormal = Vector3.up; // The normal vector of the ecliptic plane


    Vector3 RotateVectorRelativeToEcliptic(Vector3 vector, float inclination, float longitudeOfAscendingNode, float argumentOfPerihelion)
    {
        Quaternion eclipticRotation = Quaternion.FromToRotation(Vector3.up, eclipticNormal);
        
        // Convert angles to radians
        float inclinationRad = inclination * Mathf.Deg2Rad;
        float longitudeOfAscendingNodeRad = longitudeOfAscendingNode * Mathf.Deg2Rad;
        float argumentOfPerihelionRad = argumentOfPerihelion * Mathf.Deg2Rad;

        // Create rotation for the orbital plane
        Quaternion orbitalPlaneRotation = Quaternion.AngleAxis(longitudeOfAscendingNode, Vector3.up) 
            * Quaternion.AngleAxis(inclination, Vector3.right) 
            * Quaternion.AngleAxis(argumentOfPerihelion, Vector3.up);

        // Combine rotations: first apply orbital plane rotation, then align with ecliptic
        Quaternion totalRotation = eclipticRotation * orbitalPlaneRotation;

        // Apply the total rotation to the vector
        return totalRotation * vector;
    }
}