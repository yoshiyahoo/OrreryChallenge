using UnityEngine;
using System.Collections.Generic;

public class KeplerOrbitalMotion : MonoBehaviour
{
    
    [System.Serializable]
    public class CelestialBody
    {
        public string name;
        public GameObject prefab;
        public float semiMajorAxis; // in AU
        public float eccentricity;
        public float inclination; // in degrees
        public float longitudeOfAscendingNode; // in degrees
        public float argumentOfPerihelion; // in degrees
        public float meanAnomalyAtEpoch; // in degrees
        public float orbitalPeriod; // in Earth years
        public float radius; // in km
        public float mass; // in kg
        [HideInInspector] public GameObject instance;

    }
    public GameObject planetLabelPrefab; // Assign this in the inspector
    public float labelOffset = 1.5f;

    public Transform centralBody;
    public List<CelestialBody> celestialBodies = new List<CelestialBody>();
    public float timeScale = 1f; // 1 Unity time unit = 1 Earth year
    public float auToUnityUnits = 10f; // Scale factor for visualization
    public float kmToUnityUnits = 1f / 149597870.7f; // Convert km to AU, then to Unity units
    public float planetScaleFactor = 1000f;
    public OrbitalSimulationUI orbitalSimulationUI; // Add this line
    private const float TWO_PI = 2f * Mathf.PI;

    void Start()
    {
        InstantiateCelestialBodies();
    }

    public void SelectPlanet(CelestialBody body)
    {
        OrreryCameraController cameraController = Camera.main.GetComponent<OrreryCameraController>();
        if (cameraController != null)
        {
            cameraController.FocusOnPlanet(body.instance.transform);
        }

        // Add this block to trigger UI
        if (orbitalSimulationUI != null)
        {
            orbitalSimulationUI.SelectCelestialBody(body);
        }
        else
        {
            Debug.LogError("OrbitalSimulationUI is not assigned in KeplerOrbitalMotion!");
        }
    }
    
    void InstantiateCelestialBodies()
    {
        foreach (var body in celestialBodies)
        {
            InstantiateCelestialBody(body, centralBody);
        }
    }

    public void UpdateCelestialBodyData(CelestialBody updatedBody)
    {
        int index = celestialBodies.FindIndex(b => b.name == updatedBody.name);
        if (index != -1)
        {
            celestialBodies[index] = updatedBody;
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime * timeScale;
        UpdateOrbitalPositions(Time.time * timeScale, deltaTime);
    }

    void UpdateOrbitalPositions(float time, float deltaTime)
    {
        foreach (var body in celestialBodies)
        {
            UpdateBodyPosition(body, centralBody, time, deltaTime);
        }
    }

    void UpdateBodyPosition(CelestialBody body, Transform parentBody, float time, float deltaTime)
    {
        Vector3 position = CalculateOrbitalPosition(body, time);
        body.instance.transform.position = parentBody.position + position * auToUnityUnits;

        body.instance.transform.Rotate(Vector3.up, 360f * deltaTime / body.orbitalPeriod, Space.Self);
    }

    void InstantiateCelestialBody(CelestialBody body, Transform parent)
    {
        body.instance = Instantiate(body.prefab, Vector3.zero, Quaternion.identity);
        body.instance.name = body.name;
        body.instance.transform.SetParent(parent);
        body.instance.tag = "Planet";


        GameObject labelObj = Instantiate(planetLabelPrefab, body.instance.transform);
        labelObj.transform.localPosition = Vector3.up * labelOffset;
        PlanetLabel planetLabel = labelObj.GetComponent<PlanetLabel>();
        if (planetLabel != null)
        {
            // Assign a color based on the planet's name or any other criteria
            Color planetColor = GetColorForPlanet(body.name);
            planetLabel.SetLabel(body.name, planetColor);
        }



        float scale = body.radius * kmToUnityUnits * auToUnityUnits * 2f * planetScaleFactor; // Diameter
        body.instance.transform.localScale = new Vector3(scale, scale, scale);

        CelestialBodyIdentifier identifier = body.instance.AddComponent<CelestialBodyIdentifier>();
        identifier.celestialBody = body;

        SphereCollider collider = body.instance.AddComponent<SphereCollider>();
        collider.radius = 0.5f; // Set to half of the object's size
    }

    
    Color GetColorForPlanet(string planetName)
    {
        // You can define specific colors for each planet or use a hash function for random but consistent colors
        switch (planetName.ToLower())
        {
            case "mercury": return Color.gray;
            case "venus": return new Color(0.9f, 0.7f, 0.4f); // Light orange
            case "earth": return Color.blue;
            case "mars": return Color.red;
            case "jupiter": return new Color(0.8f, 0.7f, 0.5f); // Light brown
            case "saturn": return new Color(0.9f, 0.9f, 0.7f); // Light yellow
            case "uranus": return Color.cyan;
            case "neptune": return Color.blue;
            default: return Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f); // Random color for other bodies
        }
    }

    public Vector3 eclipticNormal = Vector3.up; // The normal vector of the ecliptic plane

    Vector3 CalculateOrbitalPosition(CelestialBody body, float time)
    {
        float meanAnomaly = (body.meanAnomalyAtEpoch * Mathf.Deg2Rad + TWO_PI / body.orbitalPeriod * time) % TWO_PI;
        float eccentricAnomaly = SolveKepler(meanAnomaly, body.eccentricity);
        float trueAnomaly = 2f * Mathf.Atan(Mathf.Sqrt((1f + body.eccentricity) / (1f - body.eccentricity)) * Mathf.Tan(eccentricAnomaly / 2f));
        float distance = body.semiMajorAxis * (1f - body.eccentricity * Mathf.Cos(eccentricAnomaly));

        // Calculate position in the orbital plane
        Vector3 positionInOrbitPlane = new Vector3(
            distance * Mathf.Cos(trueAnomaly),
            0f,
            distance * Mathf.Sin(trueAnomaly)
        );

        // Apply rotations to account for orbital orientation
        return RotateVectorRelativeToEcliptic(positionInOrbitPlane, body.inclination, body.longitudeOfAscendingNode, body.argumentOfPerihelion);
    }

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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(centralBody.position, eclipticNormal * 5f);

        foreach (var body in celestialBodies)
        {
            if (body.instance != null)
            {
                Gizmos.color = Color.red;
                Vector3 nodeVector = Vector3.Cross(eclipticNormal, body.instance.transform.position - centralBody.position).normalized;
                Gizmos.DrawRay(centralBody.position, nodeVector * 3f);

                Gizmos.color = Color.green;
                Vector3 orbitalNormal = Vector3.Cross(nodeVector, body.instance.transform.position - centralBody.position).normalized;
                Gizmos.DrawRay(body.instance.transform.position, orbitalNormal * 2f);
            }
        }
    }

    float SolveKepler(float M, float e, int maxIterations = 10, float epsilon = 1e-6f)
    {
        float E = M;
        for (int i = 0; i < maxIterations; i++)
        {
            float E_next = E - (E - e * Mathf.Sin(E) - M) / (1f - e * Mathf.Cos(E));
            if (Mathf.Abs(E_next - E) < epsilon)
            {
                return E_next;
            }
            E = E_next;
        }
        return E;
    }
    
}