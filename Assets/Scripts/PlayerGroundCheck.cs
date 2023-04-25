using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{

    PlayerController playerController;


    public void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        return;
        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }


}
