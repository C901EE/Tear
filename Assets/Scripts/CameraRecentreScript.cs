using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NewBehaviourScript : MonoBehaviour
{
    private CinemachineFreeLook camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("CameraRecenter"))
        {
            camera.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            camera.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}
