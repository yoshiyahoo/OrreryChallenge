using UnityEngine;

public class CelestialBodySelector : MonoBehaviour
{
    public OrbitalSimulationUI uiController;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                KeplerOrbitalMotion.CelestialBody selectedBody = hit.collider.GetComponent<CelestialBodyIdentifier>()?.celestialBody;
                if (selectedBody != null)
                {
                    uiController.SelectCelestialBody(selectedBody);
                }
            }
        }
    }
}