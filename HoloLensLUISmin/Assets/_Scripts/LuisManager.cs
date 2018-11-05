using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class LuisManager : MonoBehaviour {

    [Serializable] //this class represents the LUIS response
    public class AnalysedQuery
    {
        public TopScoringIntentData topScoringIntent;
        public EntityData[] entities;
        public string query;
    }

    // This class contains the Intent LUIS determines 
    // to be the most likely
    [Serializable]
    public class TopScoringIntentData
    {
        public string intent;
        public float score;
    }

    // This class contains data for an Entity
    [Serializable]
    public class EntityData
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public float score;
    }



    public static LuisManager instance;

    //Substitute the value of luis Endpoint with your own End Point
    string luisEndpoint = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/bbd2c240-41a8-4a95-903e-b4712e59b1b7?subscription-key=cf7df406e28a46c1b1f9d34e29730ffd&timezoneOffset=-360&q=";

    private void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }


    /// <summary>
    /// Call LUIS to submit a dictation result.
    /// The done Action is called at the completion of the method.
    /// </summary>
    public IEnumerator SubmitRequestToLuis(string dictationResult, Action done, IMicrophoneManager unityApp)
    {
        string queryString = string.Concat(Uri.EscapeDataString(dictationResult));
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(luisEndpoint + queryString))
        {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.Log(unityWebRequest.error);
            }
            else
            {
                try
                {
                    AnalysedQuery analysedQuery = JsonUtility.FromJson<AnalysedQuery>(unityWebRequest.downloadHandler.text);
                    //analyse the elements of the response 
                    AnalyseResponseElements(analysedQuery, unityApp);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine("Luis Request Exception Message: " + exception.Message);
                }
            }

            done();
            yield return null;
        }
    }



    private void AnalyseResponseElements(AnalysedQuery aQuery, IMicrophoneManager unityApp)
    {
        string topIntent = aQuery.topScoringIntent.intent;

        // Create a dictionary of entities associated with their type
        Dictionary<string, string> entityDic = new Dictionary<string, string>();

        string message = string.Empty;
        foreach (EntityData ed in aQuery.entities)
        {
            entityDic.Add(ed.type, ed.entity);

            message += $"Entity: {ed.entity} ({ed.type})\n";
        }

        message += $"Intent: {topIntent}";
        message = message.Replace("\\n", "\n");
        unityApp.ModifyOutputText(message);
        System.Diagnostics.Debug.WriteLine(message);

        // Depending on the topmost recognised intent, read the entities name
        switch (topIntent)
        {
            case "ChangeObjectColor":
                string targetForColor = null;
                string color = null;

                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForColor = pair.Value;
                    }
                    else if (pair.Key == "color")
                    {
                        color = pair.Value;
                    }
                }

                Behaviours.instance.ChangeTargetColor(targetForColor, color);
                break;

            case "ChangeObjectSize":
                string targetForSize = null;
                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForSize = pair.Value;
                    }
                }

                if (entityDic.ContainsKey("upsize") == true)
                {
                    Behaviours.instance.UpSizeTarget(targetForSize);
                }
                else if (entityDic.ContainsKey("downsize") == true)
                {
                    Behaviours.instance.DownSizeTarget(targetForSize);
                }
                break;
        }
    }
}
