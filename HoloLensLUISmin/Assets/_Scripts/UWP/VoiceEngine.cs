//#if UNITY_WSA && !UNITY_EDITOR
//using Microsoft.CognitiveServices.Speech;
//using Microsoft.CognitiveServices.Speech.Translation;
//using System.Threading.Tasks;
//using UnityEngine;

//public class VoiceEngine : MonoBehaviour
//{

//    IUnityVoiceScene UnityApp;

//    // Translation source language.
//    // Replace with a language of your choice.
//    string fromLanguage = "fr-FR";

//    SpeechTranslationConfig config;

//    // Use this for initialization
//    void Start()
//    {
//        // Creates an instance of a speech translation config with specified subscription key and service region.
//        // Replace with your own subscription key and service region (e.g., "westus").
//        config = SpeechTranslationConfig.FromSubscription("8c28dd83b85045db8b89eff44b4975da", "westeurope");
//        config.SpeechRecognitionLanguage = fromLanguage;

//        // Translation target language(s).
//        // Replace with language(s) of your choice.
//        config.AddTargetLanguage("en");
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }



//    public async Task Inititalize(IUnityVoiceScene unityApp)
//    {
//        UnityApp = unityApp;

//        using (var recognizer = new TranslationRecognizer(config))
//        {
//            // Subscribes to events.
//            recognizer.Recognizing += (s, e) =>
//            {
//                unityApp.ModifyOutputText($"RECOGNIZING in '{fromLanguage}': Text={e.Result.Text}");
//                System.Diagnostics.Debug.WriteLine($"RECOGNIZING in '{fromLanguage}': Text={e.Result.Text}");
//                foreach (var element in e.Result.Translations)
//                {
//                    System.Diagnostics.Debug.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
//                }
//            };

//            recognizer.Recognized += (s, e) =>
//            {
//                if (e.Result.Reason == ResultReason.TranslatedSpeech)
//                {
//                    unityApp.ModifyOutputText($"RECOGNIZED in '{fromLanguage}': Text={e.Result.Text}");
//                    System.Diagnostics.Debug.WriteLine($"RECOGNIZED in '{fromLanguage}': Text={e.Result.Text}");
//                    foreach (var element in e.Result.Translations)
//                    {
//                        System.Diagnostics.Debug.WriteLine($"    TRANSLATED into '{element.Key}': {element.Value}");
//                    }
//                }
//                else if (e.Result.Reason == ResultReason.RecognizedSpeech)
//                {
//                    System.Diagnostics.Debug.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
//                    System.Diagnostics.Debug.WriteLine($"    Speech not translated.");
//                }
//                else if (e.Result.Reason == ResultReason.NoMatch)
//                {
//                    unityApp.ModifyOutputText($"NOMATCH: Speech could not be recognized.");
//                    System.Diagnostics.Debug.WriteLine($"NOMATCH: Speech could not be recognized.");
//                }
//            };

//            //await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
//            await recognizer.RecognizeOnceAsync();
//        }
//    }
//}
//#endif