﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


#if UNITY_WSA && !UNITY_EDITOR
    private VoiceEngine Voice;
#endif

    // Use this for initialization
    void Start ()
    {
        OutputTextMesh = OutputText.GetComponent<TextMesh>();

#if UNITY_WSA && !UNITY_EDITOR // RUNNING ON WINDOWS
        Voice = new VoiceEngine();
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
