using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed, runSpeed;

    public Transform player;
    Vector3 playerMovement;
    Vector3 moveVector;
    float currentSpeed;
    // Update is called once per frame
    void Update()
    {
        playerMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = speed;
        }
        MovePlayer();
    }

    void MovePlayer()
    {
        moveVector = playerMovement * currentSpeed * Time.deltaTime;
        player.transform.Translate(new Vector3(moveVector.x, 0f, moveVector.z));
    }


}
