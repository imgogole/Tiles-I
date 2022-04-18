using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject[] menus;

    private void Start()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[0].SetActive(true);
    }

    public void SetMenu(int wantedMenu)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        menus[wantedMenu].SetActive(true);
    }
}
