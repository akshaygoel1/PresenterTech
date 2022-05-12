using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float yOffSet;
    public float sensX, sensY;
    public Transform playerOrientation = null;
    float xRotation, yRotation;

    public void SetPlayer(Transform player)
    {
        playerOrientation = player;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerOrientation != null)
        {
            transform.position = playerOrientation.position + Vector3.up * yOffSet;
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
