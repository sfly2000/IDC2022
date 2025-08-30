using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cameraFollow;
    public GameObject cameraFollowMouse;
    public GameObject obCamFree;
    public GameObject obCamFollow;
    public GameObject obCamFollowUI;

    private bool IsPlayer = true;

    void Start()
    {
        // cameraFollow = GameObject.Find("cameraFollow");
        // cameraFollowMouse = GameObject.Find("cameraFollowMouse");
        // cameraFollow.SetActive(true);
        // cameraFollowMouse.SetActive(false);
        Debug.Log("camera controller activated");
        cameraFollow.SetActive(true);
        cameraFollowMouse.SetActive(false);
        obCamFree.SetActive(false);
        obCamFollow.SetActive(false);
        obCamFollowUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayer)
        {
            // Debug.Log("CameraController: Is Player");
            obCamFree.SetActive(false);
            obCamFollow.SetActive(false);
            if (Input.GetKeyDown(KeyCode.C))
            {
                cameraFollow.SetActive(true);
                cameraFollowMouse.SetActive(false);
                Debug.Log("player switch to follow camera");
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                cameraFollow.SetActive(false);
                cameraFollowMouse.SetActive(true);

                Debug.Log("player switch to moveable camera");
            }
        }
        else
        {
            cameraFollow.SetActive(false);
            cameraFollowMouse.SetActive(false);

            if (Input.GetKeyDown(KeyCode.C))
            {
                obCamFollow.SetActive(true);
                obCamFollowUI.SetActive(true);
                obCamFree.SetActive(false);

                Debug.Log("ob switch to follow camera");
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                obCamFollow.SetActive(false);
                obCamFree.SetActive(true);
                obCamFollowUI.SetActive(false);

                Debug.Log("switch to free camera");
            }
        }
    }

    public void SetIsPlayer(bool InBool)
    {
        IsPlayer = InBool;
        if (InBool == false)
        {
            obCamFree.SetActive(true);
        }
    }
}
