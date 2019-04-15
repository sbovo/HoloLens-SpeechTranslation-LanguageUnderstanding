using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

/// <summary>
/// The Interface gives one method to implement. This method modifies the text to display in Unity
/// Any code can then call this method outside the Unity MonoBehavior object
/// </summary>
public interface IUnityVoiceScene
{
    void ModifyOutputText(string newText);
}

public class UnityVoiceScene : MonoBehaviour, IUnityVoiceScene {

    // Unity 3D Text object that contains 
    // the displayed TextMesh in the FOV
    public GameObject OutputText;
    // TextMesh object provided by the OutputText game object
    private TextMesh OutputTextMesh;
    // string to be affected to the TextMesh object
    private string OutputTextString = string.Empty;
    // Indicate if we have to Update the text displayed
    private bool OutputTextChanged = false;


    private GestureRecognizer recognizer;



    // Use this for initialization
    async void Start ()
    {
        OutputTextMesh = OutputText.GetComponent<TextMesh>();

        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.Tapped += GestureRecognizer_Tapped;
        recognizer.StartCapturingGestures();
    }


    private void GestureRecognizer_Tapped(TappedEventArgs obj)
    {
#if UNITY_WSA && !UNITY_EDITOR // RUNNING ON WINDOWS
        //Voice = new VoiceEngine();
        //Voice.Inititalize(this);
#else                          // RUNNING IN UNITY
        ModifyOutputText("Sorry ;-( The app is not supported in the Unity player.");
#endif

    }

    // Update is called once per frame
    void Update()
    {
        if (OutputTextChanged)
        {
            OutputTextMesh.text = OutputTextString;
            OutputTextChanged = false;
        }
    }


    /// <summary>
    /// Modify the text to be displayed in the FOV
    /// or/and in the debug traces
    /// + Indicate that we have to update the text to display
    /// </summary>
    /// <param name="newText">new string value to display</param>
    public void ModifyOutputText(string newText)
    {
        OutputTextString = newText;
        OutputTextChanged = true;
    }
}
