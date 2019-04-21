using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class LuisManager : MonoBehaviour
{
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
    string luisEndpoint = 
        "https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/c33e39dd-863f-48fc-bad4-16e8a73b1148?verbose=true&timezoneOffset=60&subscription-key=a20a129b9c4d43238bf9c0a889a71762&q=";

    private void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }


    /// <summary>
    /// Call LUIS to submit a dictation result.
    /// The done Action is called at the completion of the method.
    /// </summary>
    public IEnumerator SubmitRequestToLuis(
        string dictationResult, 
        Action done, 
        IMicrophoneManager unityApp)
    {
        string queryString = string.Concat(
            Uri.EscapeDataString(dictationResult));
        using (UnityWebRequest unityWebRequest = 
            UnityWebRequest.Get(luisEndpoint + queryString))
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
                    AnalysedQuery analysedQuery = 
                        JsonUtility.FromJson<AnalysedQuery>(
                            unityWebRequest.downloadHandler.text);
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



    private void AnalyseResponseElements(
        AnalysedQuery aQuery, IMicrophoneManager unityApp)
    {
        // INTENT
        // ======
        string topIntent = aQuery.topScoringIntent.intent;

        // Create a dictionary of entities associated with their type
        Dictionary<string, string> entityDic = 
            new Dictionary<string, string>();

        string message = string.Empty;
        // ENTITIES
        // ========
        foreach (EntityData ed in aQuery.entities)
        {
            entityDic.Add(ed.type, ed.entity);
            message += $"Entity: {ed.entity} ({ed.type})\n";
        }

        message += $"Intent: {topIntent}";
        message = message.Replace("\\n", "\n");
        unityApp.ModifyOutputText(message);
        System.Diagnostics.Debug.WriteLine(message);


        string targetObject = null;
        string scaleValue = null;
        foreach (var pair in entityDic)
        {
            if (pair.Key == "Food")
            {
                targetObject = pair.Value;
            }
            if (pair.Key == "Scale")
            {
                scaleValue = pair.Value;
            }
        }
        
        // Depending on the topmost recognised intent, 
        // read the entities name
        switch (topIntent)
        {
            case "show.calories":
                unityApp.ModifyOutputText(":-( Not implemented yet...");
                break;

            case "show.food":
                Behaviours.instance.Show(targetObject);
                break;

            case "hide.food":
                Behaviours.instance.Hide(targetObject);
                break;

            case "changesize.food":
                if (scaleValue == "bigger")
                {
                    Behaviours.instance.UpSizeTarget(targetObject);
                }
                else if (scaleValue == "smaller")
                {
                    Behaviours.instance.DownSizeTarget(targetObject);
                }
                break;
        }
    }

    

    
}
