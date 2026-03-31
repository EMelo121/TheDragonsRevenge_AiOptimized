using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles communication between Unity and the backend taunt server.
/// Sends requests and receives generated taunts.
/// </summary>
public class BattleTauntService : MonoBehaviour
{
    [Header("Backend Settings")]
    [Tooltip("URL of the backend server endpoint.")]
    [SerializeField]
    private string backendUrl = "http://localhost:3000/api/battle-taunt";

    /// <summary>
    /// Sends a taunt request to the backend server.
    /// </summary>
    /// <param name="requestData">The taunt request payload.</param>
    /// <param name="onSuccess">Callback for successful response.</param>
    /// <param name="onError">Callback for failed request.</param>
    private void Start()
    {
        Debug.Log("BattleTauntService using URL: " + backendUrl);
    }
    public IEnumerator RequestTaunt(
        BattleTauntRequest requestData,
        Action<string> onSuccess,
        Action<string> onError)
    {
        Debug.Log("Sending taunt request to backend...");

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Check if request failed
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Taunt request failed: " + request.error);
            onError?.Invoke(request.error);
            yield break;
        }

        Debug.Log("Backend response received: " + request.downloadHandler.text);

        BattleTauntResponse response =
            JsonUtility.FromJson<BattleTauntResponse>(request.downloadHandler.text);

        if (response == null || string.IsNullOrWhiteSpace(response.taunt))
        {
            Debug.LogWarning("Backend returned empty taunt.");
            onError?.Invoke("Empty response");
            yield break;
        }

        onSuccess?.Invoke(response.taunt);
    }

    // AI revision note:
    // This script was added to separate backend communication from gameplay logic.
    // It ensures that networking is handled cleanly and that failures do not break the game.
}