using UnityEngine;

public class OrreryCameraController : MonoBehaviour
{
    
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 10f;
    public float planetRotationSpeed = 1f;
    public float planetZoomSpeed = 1f;
    public float minZoomDistance = 1f;
    public float maxZoomDistance = 20f;

    private Vector3 lastMousePosition;
    private Transform focusedPlanet;
    private float currentPlanetDistance;
    private bool isPlanetFocused = false;

    void Update()
    {
        if (isPlanetFocused)
        {
            HandlePlanetFocusControls();
        }
        else
        {
            HandleFreeCameraControls();
        }

        // Store mouse position for next frame
        lastMousePosition = Input.mousePosition;
    }

    void HandleFreeCameraControls()
    {
        // Pan camera with middle mouse button
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Translate(-delta.x * moveSpeed * Time.deltaTime, -delta.y * moveSpeed * Time.deltaTime, 0);
        }

        // Rotate camera with right mouse button
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.RotateAround(transform.position, transform.right, -delta.y * rotationSpeed * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.up, delta.x * rotationSpeed * Time.deltaTime);
        }

        // Zoom with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(0, 0, scroll * zoomSpeed, Space.Self);

        // WASD movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Q and E for up and down movement
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void HandlePlanetFocusControls()
    {
        // Rotate around planet with left mouse button
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.RotateAround(focusedPlanet.position, Vector3.up, delta.x * planetRotationSpeed);
            transform.RotateAround(focusedPlanet.position, transform.right, -delta.y * planetRotationSpeed);
        }

        // Zoom in/out from planet with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentPlanetDistance -= scroll * planetZoomSpeed;
        currentPlanetDistance = Mathf.Clamp(currentPlanetDistance, minZoomDistance, maxZoomDistance);
        transform.position = focusedPlanet.position - transform.forward * currentPlanetDistance;

        // Exit planet focus mode with right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            ExitPlanetFocus();
        }
    }

    public void FocusOnPlanet(Transform planet)
    {
        focusedPlanet = planet;
        isPlanetFocused = true;
        currentPlanetDistance = Vector3.Distance(transform.position, planet.position);
        transform.LookAt(planet);
    }

    void ExitPlanetFocus()
    {
        isPlanetFocused = false;
        focusedPlanet = null;
    }
}