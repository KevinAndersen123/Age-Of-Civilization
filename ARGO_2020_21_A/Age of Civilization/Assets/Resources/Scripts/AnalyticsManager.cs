using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class GameStartData
{
    public string device_id = "";
    public string state = "Game Start";
    public string seed = "";
    public int noOfPlayers = 0;
    public List<string> playerColours = new List<string>();
    public List<string> playerTypes = new List<string>();
}

[System.Serializable]
public class GameEndData
{
    public string device_id = "";
    public string state = "Game End";
    public int time = 0;
    public int turnAmount = 0;
    public int citiesControlled = 0;
    public int unitsControlled = 0;
    public string mostFrequentUnit = "";
    public int MFUnitCount = 0;
}

public class AnalyticsManager : MonoBehaviour
{
    public static IEnumerator PostMethod(string jsonData)
    {
        string url = "http://34.228.111.166:5000/upload_data";

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (!request.isNetworkError && request.responseCode == (int)System.Net.HttpStatusCode.OK)
            {
                Debug.Log("Data successfully sent to the server");
            }
            else
            {
                Debug.Log("Error sending data to the server: Error " + request.responseCode);
            }
        }
    }
}
