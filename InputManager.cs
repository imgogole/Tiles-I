using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{

    TouchScreenKeyboard keyboard;
    [SerializeField] TMP_InputField input;

    public void OnKeyboardPressed()
    {
        keyboard = TouchScreenKeyboard.Open(PlayerPrefs.GetString("nickname"), TouchScreenKeyboardType.Default, false, false, false);
        keyboard.characterLimit = 24;
        Debug.Log("Ouverture du clavier");
    }

    private void Update()
    {
        if (!TouchScreenKeyboard.visible && keyboard != null)
        {
            if (keyboard.done)
            {
                input.text = keyboard.text;
            }
        }
    }
}
