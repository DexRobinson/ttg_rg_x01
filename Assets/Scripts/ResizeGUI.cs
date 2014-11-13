using UnityEngine;
using System.Collections;

public class ResizeGUI : MonoBehaviour 
{
    public GUISkin loginSkin;

    private int iPadFontSize = 26;

	// Use this for initialization
	void Start () 
    {
        float sreenSize = Screen.width;

        if (sreenSize > 700)
        {
            loginSkin.button.fontSize = iPadFontSize;
            loginSkin.label.fontSize = iPadFontSize;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
