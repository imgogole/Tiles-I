using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenu : MonoBehaviour
{
    [SerializeField] GameObject[] menus;

    private void Start()
    {
        SwitchTo(0);
    }

    public void Switch(int menu)
    {
        SwitchTo(menu);
    }

    void SwitchTo(int menuIndex)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[menuIndex].SetActive(true);
    }
}
