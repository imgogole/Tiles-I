using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMenu : MonoBehaviour
{
    public Outline outline;
    public int action;
    bool canClick;

    public string[][] strings;


    private void Awake()
    {
        canClick = true;
    }

    private void OnMouseOver()
    {
        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
    }

    private void OnMouseDown()
    {
        outline.enabled = false;
        if (canClick)
        {
            switch (action)
            {
                case 1:
                    canClick = false;
                    Application.Quit();
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }
}
