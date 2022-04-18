using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtNameTag : MonoBehaviour
{
    Transform cameraTransform;

    private void Update()
    {
        cameraTransform = GameObject.Find("Camera").transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
