using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DarkModeSwitcher : MonoBehaviour
{
    bool isDarkMode;

    public bool EnableAnnoncement, EnableText, EnableInputField, EnableButton, EnableCamera, EnableImage, EnableDropdown, EnableBackground;

    public TMP_Text[] texts;
    public TMP_InputField[] inputs;
    public Button[] buttons;
    public Image[] images, imagesBackground;
    public TMP_Dropdown[] dropdowns;
    public Camera mainCamera;

    public TMP_Text darkModeSwitcher;

    public Color backgroundColorLightMode, backgroundColorDarkMode, UILightMode, UIDarkMode;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("dark"))
        {
            PlayerPrefs.SetString("dark", "false");
            isDarkMode = false;
        }
        else
        {
            isDarkMode = bool.Parse(PlayerPrefs.GetString("dark"));
        }
        UpdateMode();
    }

    public void ChangeMode()
    {
        bool newValue = !isDarkMode;
        PlayerPrefs.SetString("dark", newValue.ToString());
        isDarkMode = newValue;
        PlayerPrefs.Save();

        UpdateMode();
    }

    void UpdateMode()
    {
        if (isDarkMode)
        {
            if (EnableAnnoncement) darkModeSwitcher.text = "OMG it's dark !";
            if (EnableText)
            {
                foreach (TMP_Text text in texts)
                {
                    text.color = UILightMode;
                }
            }
            if (EnableInputField)
            {
                foreach (TMP_InputField input in inputs)
                {
                    input.image.color = UIDarkMode;
                    input.textComponent.color = UILightMode;
                }
            }
            if (EnableButton)
            {
                foreach (Button button in buttons)
                {
                    button.image.color = UIDarkMode;
                }
            }
            if (EnableCamera) mainCamera.backgroundColor = backgroundColorDarkMode;
            if (EnableImage)
            {
                foreach (Image image in images)
                {
                    image.color = UIDarkMode;
                }
            }
            if (EnableDropdown)
            {
                foreach (TMP_Dropdown dropdown in dropdowns)
                {
                    dropdown.image.color = UIDarkMode;
                    dropdown.captionText.color = UILightMode;
                    dropdown.itemText.color = UIDarkMode;
                }
            }
            if (EnableBackground)
            {
                foreach (Image image in imagesBackground)
                {
                    image.color = backgroundColorDarkMode;
                }
            }
        }
        else
        {
            if (EnableAnnoncement) darkModeSwitcher.text = "Don't press me !";
            if (EnableText)
            {
                foreach (TMP_Text text in texts)
                {
                    text.color = UIDarkMode;
                }
            }
            if (EnableInputField)
            {
                foreach (TMP_InputField input in inputs)
                {
                    input.image.color = UILightMode;
                    input.textComponent.color = UIDarkMode;
                }
            }
            if (EnableButton)
            {
                foreach (Button button in buttons)
                {
                    button.image.color = UILightMode;
                }
            }
            if (EnableCamera) mainCamera.backgroundColor = backgroundColorLightMode;
            if (EnableImage)
            {
                foreach (Image image in images)
                {
                    image.color = UILightMode;
                }
            }
            if (EnableDropdown)
            {
                foreach (TMP_Dropdown dropdown in dropdowns)
                {
                    dropdown.image.color = UILightMode;
                    dropdown.captionText.color = UIDarkMode;
                    dropdown.itemText.color = UIDarkMode;
                }
            }
            if (EnableBackground)
            {
                foreach (Image image in imagesBackground)
                {
                    image.color = backgroundColorLightMode;
                }
            }
        }
    }

}
