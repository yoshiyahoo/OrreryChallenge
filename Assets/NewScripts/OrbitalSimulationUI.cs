using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OrbitalSimulationUI : MonoBehaviour
{
    public KeplerOrbitalMotion orbitalMotion;
    public GameObject celestialBodyUIPrefab;
    public Transform contentParent;
    public Slider timeScaleSlider;
    public TMP_Text timeScaleText;
    public TMP_Text realTimeSpeedText;
    public GameObject celestialBodyUIPanel;
    public Button closePanelButton;

    private CelestialBodyUI activeCelestialBodyUI;

    void Start()
    {
        InitializeUI();
        UpdateTimeScaleUI();

        celestialBodyUIPanel.SetActive(false);

        if (closePanelButton != null)
        {
            closePanelButton.onClick.AddListener(CloseCelestialBodyPanel);
        }
        else
        {
            Debug.LogError("Close panel button is not assigned!");
        }
    }
    void InitializeUI()
    {
        if (timeScaleSlider != null)
        {
            timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
        }
        else
        {
            Debug.LogError("Time scale slider is null!");
        }

    }

    void OnTimeScaleChanged(float value)
    {
        orbitalMotion.timeScale = value;
        UpdateTimeScaleUI();
    }

    void UpdateTimeScaleUI()
    {
        if (timeScaleText != null && realTimeSpeedText != null)
        {
            timeScaleText.text = $"Time Scale: {orbitalMotion.timeScale:F2}x";
            float realTimeSpeed = orbitalMotion.timeScale * 365.25f * 24f * 60f * 60f; // Convert years to seconds
            realTimeSpeedText.text = $"Real-time Speed: {realTimeSpeed:F2} seconds/second";
        }
        else
        {
            Debug.LogError("Time scale text or real-time speed text is null!");
        }
    }

    void OnCelestialBodyDataChanged(KeplerOrbitalMotion.CelestialBody body)
    {
        orbitalMotion.UpdateCelestialBodyData(body);
    }

    public void SelectCelestialBody(KeplerOrbitalMotion.CelestialBody body)
    {
        if (celestialBodyUIPanel == null)
        {
            Debug.LogError("Celestial body UI panel is null!");
            return;
        }

        if (activeCelestialBodyUI == null)
        {
            GameObject uiInstance = Instantiate(celestialBodyUIPrefab, celestialBodyUIPanel.transform);
            activeCelestialBodyUI = uiInstance.GetComponent<CelestialBodyUI>();
            
            if (activeCelestialBodyUI == null)
            {
                Debug.LogError("Failed to get CelestialBodyUI component from instantiated prefab!");
                return;
            }
        }

        activeCelestialBodyUI.Initialize(body, OnCelestialBodyDataChanged);
        celestialBodyUIPanel.SetActive(true);
    }

    public void CloseCelestialBodyPanel()
    {
        if (celestialBodyUIPanel != null)
        {
            celestialBodyUIPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Cannot close celestial body panel: panel is null!");
        }
    }
}