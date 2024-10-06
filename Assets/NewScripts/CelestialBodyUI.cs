using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class CelestialBodyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_InputField semiMajorAxisInput;
    [SerializeField] private TMP_InputField eccentricityInput;
    [SerializeField] private TMP_InputField inclinationInput;
    [SerializeField] private TMP_InputField longitudeOfAscendingNodeInput;
    [SerializeField] private TMP_InputField argumentOfPerihelionInput;
    [SerializeField] private TMP_InputField meanAnomalyAtEpochInput;
    [SerializeField] private TMP_InputField orbitalPeriodInput;

    // AI Chatbot UI elements
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TMP_Text chatHistoryText;
    [SerializeField] private Button sendButton;

    private KeplerOrbitalMotion.CelestialBody celestialBody;
    private Action<KeplerOrbitalMotion.CelestialBody> onDataChanged;

    private const string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";
    private const string API_KEY = "AIzaSyDCufVTJP_FpWFLrWuaEJpq9VcVak5htk4"; // Replace with your actual Gemini API key

    public void Initialize(KeplerOrbitalMotion.CelestialBody body, Action<KeplerOrbitalMotion.CelestialBody> callback)
    {
        Debug.Log("initialized");
        celestialBody = body;
        onDataChanged = callback;
        nameText.text = body.name;
        UpdateUIFromBody();

        semiMajorAxisInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        eccentricityInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        inclinationInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        longitudeOfAscendingNodeInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        argumentOfPerihelionInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        meanAnomalyAtEpochInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());
        orbitalPeriodInput.onEndEdit.AddListener(_ => UpdateBodyFromUI());

        // Initialize AI Chatbot
        sendButton.onClick.AddListener(SendMessage);
    }

    private void UpdateUIFromBody()
    {
        semiMajorAxisInput.text = celestialBody.semiMajorAxis.ToString("F2");
        eccentricityInput.text = celestialBody.eccentricity.ToString("F4");
        inclinationInput.text = celestialBody.inclination.ToString("F2");
        longitudeOfAscendingNodeInput.text = celestialBody.longitudeOfAscendingNode.ToString("F2");
        argumentOfPerihelionInput.text = celestialBody.argumentOfPerihelion.ToString("F2");
        meanAnomalyAtEpochInput.text = celestialBody.meanAnomalyAtEpoch.ToString("F2");
        orbitalPeriodInput.text = celestialBody.orbitalPeriod.ToString("F2");
    }

    private void UpdateBodyFromUI()
    {
        if (float.TryParse(semiMajorAxisInput.text, out float semiMajorAxis))
            celestialBody.semiMajorAxis = semiMajorAxis;
        if (float.TryParse(eccentricityInput.text, out float eccentricity))
            celestialBody.eccentricity = Mathf.Clamp01(eccentricity);
        if (float.TryParse(inclinationInput.text, out float inclination))
            celestialBody.inclination = inclination;
        if (float.TryParse(longitudeOfAscendingNodeInput.text, out float longitudeOfAscendingNode))
            celestialBody.longitudeOfAscendingNode = longitudeOfAscendingNode;
        if (float.TryParse(argumentOfPerihelionInput.text, out float argumentOfPerihelion))
            celestialBody.argumentOfPerihelion = argumentOfPerihelion;
        if (float.TryParse(meanAnomalyAtEpochInput.text, out float meanAnomalyAtEpoch))
            celestialBody.meanAnomalyAtEpoch = meanAnomalyAtEpoch;
        if (float.TryParse(orbitalPeriodInput.text, out float orbitalPeriod))
            celestialBody.orbitalPeriod = Mathf.Max(orbitalPeriod, 0.01f);

        onDataChanged?.Invoke(celestialBody);
        UpdateUIFromBody(); // Refresh UI to show clamped/corrected values
    }

    public void loadLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void SendMessage()
    {
        //Debug.Log("here");
        chatHistoryText.text = "";
        string userMessage = chatInputField.text;
        if (string.IsNullOrEmpty(userMessage)) return;

        AppendToChatHistory("User: " + userMessage);
        chatInputField.text = "";

        string prompt = $"You are an AI assistant for a space simulation. The user is currently viewing {celestialBody.name}. " +
                        $"Provide a brief, informative response to the following question or statement: {userMessage}";

        StartCoroutine(GetGeminiResponse(prompt));
    }

    private IEnumerator GetGeminiResponse(string prompt)
    {
        var requestData = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest request = new UnityWebRequest(GEMINI_API_URL + "?key=" + API_KEY, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                AppendToChatHistory("AI: Sorry, I couldn't process your request.");
            }
            else
            {
                string response = request.downloadHandler.text;
                var responseData = JsonConvert.DeserializeObject<GeminiResponse>(response);
                string aiMessage = responseData.candidates[0].content.parts[0].text;
                AppendToChatHistory("AI: " + aiMessage);
            }
        }
    }

    private void AppendToChatHistory(string message)
    {
        chatHistoryText.text += message + "\n\n";
    }
}

[System.Serializable]
public class GeminiResponse
{
    public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}

[System.Serializable]
public class Content
{
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}