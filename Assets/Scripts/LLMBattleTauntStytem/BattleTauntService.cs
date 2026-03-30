using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BattleTauntService : MonoBehaviour
{
    [SerializeField]
    private string backendUrl = "http://localhost:3000/api/battle-taunt";

    public IEnumerator RequestTaunt(
        BattleTauntRequest requestData,
        Action<string> onSuccess,
        Action<string> onError)
    {
        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        BattleTauntResponse response =
            JsonUtility.FromJson<BattleTauntResponse>(request.downloadHandler.text);

        if (response == null || string.IsNullOrWhiteSpace(response.taunt))
        {
            onError?.Invoke("Backend returned an empty taunt.");
            yield break;
        }

        onSuccess?.Invoke(response.taunt);
    }
}