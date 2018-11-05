using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public interface IMicrophoneManager
{
    void ModifyOutputText(string newText);
}


public class MicrophoneManager : MonoBehaviour, IMicrophoneManager
{


    public static MicrophoneManager instance; //help to access instance of this object
    private DictationRecognizer dictationRecognizer;  //Component converting speech to text
    public TextMesh dictationText; //a UI object used to debug dictation result
    // string to be affected to the TextMesh object
    private string OutputTextString = string.Empty;
    // Indicate if we have to Update the text displayed
    private bool OutputTextChanged = false;

    private void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            StartCapturingAudio();
            Debug.Log("Mic Detected");
        }
    }


    /// <summary>
    /// Start microphone capture, by providing the microphone as a continual audio source (looping),
    /// then initialise the DictationRecognizer, which will capture spoken words
    /// </summary>
    public void StartCapturingAudio()
    {
        DictationRecognizer r = new DictationRecognizer();       
        if (dictationRecognizer == null)
        {
            dictationRecognizer = new DictationRecognizer
            {
                InitialSilenceTimeoutSeconds = 60,
                AutoSilenceTimeoutSeconds = 5
            };

            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        }
        dictationRecognizer.Start();
        Debug.Log("Capturing Audio...");
    }

    /// <summary>
    /// Stop microphone capture
    /// </summary>
    public void StopCapturingAudio()
    {
        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Stop();
        dictationRecognizer.Dispose();
        dictationRecognizer = null;
        Debug.Log("Stop Capturing Audio...");
    }


    /// <summary>
    /// This handler is called every time the Dictation detects a pause in the speech. 
    /// This method will stop listening for audio, send a request to the LUIS service 
    /// and then start listening again.
    /// </summary>
    private void DictationRecognizer_DictationResult(string dictationCaptured, ConfidenceLevel confidence)
    {
        StopCapturingAudio();
        ModifyOutputText(dictationCaptured);
        Debug.Log("Dictation captured: " + dictationCaptured);
        StartCoroutine(LuisManager.instance.SubmitRequestToLuis(dictationCaptured, StartCapturingAudio, this));
        //dictationText.text = dictationCaptured;
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.Log("Dictation exception: " + error);
    }


    public void ModifyOutputText(string newText)
    {
        OutputTextString = newText;
        OutputTextChanged = true;
    }


    void Update()
    {
        if (OutputTextChanged)
        {
            dictationText.text = OutputTextString;
            OutputTextChanged = false;
        }
    }

}