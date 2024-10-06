using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public KeplerOrbitalMotion keplerOrbitalMotion;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Planet"))
                {
                    CelestialBodyIdentifier identifier = hit.collider.GetComponent<CelestialBodyIdentifier>();
                    if (identifier != null)
                    {
                        keplerOrbitalMotion.SelectPlanet(identifier.celestialBody);
                    }
                }
            }
        }
    }
}