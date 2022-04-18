using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EdgesStats : MonoBehaviour
{
    public GameServerManager GSM;
    int hasEdges = 1;
    GameObject[] edges;
    Toggle toggle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("edges")) PlayerPrefs.SetInt("edges", hasEdges);
        hasEdges = PlayerPrefs.GetInt("edges");
        toggle.isOn = hasEdges != 0;
    }

    public void OnEdgesStateChanged(bool state)
    {
        if (state)
        {
            hasEdges = 1;
            PlayerPrefs.SetInt("edges", hasEdges);
        }
        else
        {
            hasEdges = 0;
            PlayerPrefs.SetInt("edges", hasEdges);
        }
    }

    private void LateUpdate()
    {
        edges = GameObject.FindGameObjectsWithTag("edge");
        if (hasEdges == 0)
        {
            foreach (GameObject edge in edges)
            {
                edge.GetComponent<Renderer>().enabled = false;
            }
        }
        else
        {
            foreach (GameObject edge in edges)
            {
                edge.GetComponent<Renderer>().enabled = true;
            }
        }
    }
}
