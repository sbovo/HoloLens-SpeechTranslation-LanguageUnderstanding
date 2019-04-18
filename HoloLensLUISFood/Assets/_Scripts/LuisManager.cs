using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class LuisManager : MonoBehaviour
{
         
    public GameObject PrefabToUse;


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
    string luisEndpoint = "https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/c33e39dd-863f-48fc-bad4-16e8a73b1148?verbose=true&timezoneOffset=60&subscription-key=a20a129b9c4d43238bf9c0a889a71762&q=";

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

        // Depending on the topmost recognised intent, 
        // read the entities name
        switch (topIntent)
        {
            case "show.calories":
                unityApp.ModifyOutputText(":-( Not implemented yet...");
                break;

            case "hide.unhealthy":
                StartCoroutine(CreateWall());
                break;

            case "show.bananas":
                StartCoroutine(BulkBananaCreation());
                break;
        }
    }

    private IEnumerator CreateWall()
    {
        var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = System.Guid.NewGuid().ToString();
        o.transform.localScale = new Vector3(2f, 2f, 0.5f);
        o.transform.position = new Vector3(-0.13f, 3f, 7.2f);
        o.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 0.6f, 1f, 1f, 1f, 0f, 0.2f);
        o.AddComponent<Rigidbody>();
        yield return 1;
    }

    private IEnumerator BulkBananaCreation()
    {
        for (int i = 0; i < 80; i++)
        {
            StartCoroutine(CreateBanana());
            yield return new WaitForSeconds(0.05f);
        }
        yield return 1;
    }

    private IEnumerator CreateBanana()
    {
        var o = Instantiate(PrefabToUse, new Vector3(0f, 10f, 8f), UnityEngine.Random.rotation);
        yield return 1;
    }
}
