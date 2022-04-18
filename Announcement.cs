using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcement : MonoBehaviour
{
    public string[] annoucements;
    public float time = 50f;

    private void Start()
    {
        GetComponent<TMPro.TMP_Text>().text = annoucements[Random.Range(0, annoucements.Length - 1)];
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.x <= -100)
        {
            gameObject.transform.position = new Vector3(1800, gameObject.transform.position.y, 0);
            GetComponent<TMPro.TMP_Text>().text = annoucements[Random.Range(0, annoucements.Length - 1)];
        }
        else
        {
            gameObject.transform.Translate(-time * Time.fixedDeltaTime, 0, 0, Space.World);
        }
    }
}
