using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour {

    public bool holdingObject;

    public GameObject obj;
    public Transform playerHand;

    public float throwSpeed;

    public GameObject pickupText;
    public GameObject throwText;

    PlayerProgressController progController;

    private void Awake()
    {
        progController = FindObjectOfType<PlayerProgressController>();
        throwSpeed = progController.throwSpeed;
    }
    public void Update()
    {
         // If Holding an Object, and throw button pressed, throw object
        if (holdingObject)
        {
            throwText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                ThrowObject();
            }
        }
    }
    public void OnTriggerStay(Collider col)
    {   // Show Pickup text if not holding object
        if (col.gameObject.CompareTag("Throwable")) 
        {
            if (holdingObject == false)
            {
                pickupText.SetActive(true);
            }
        } // If Pickup button is pressed and on throwable object, pickup
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (col.gameObject.CompareTag("Throwable"))
            {
                if (holdingObject == false)
                { 
                    PickupObject(col);
                }
            }
        }
    }
    public void OnTriggerExit(Collider col)
    {
        pickupText.SetActive(false);
    }
    void PickupObject(Collider col)
    {
        // Set the objects position to players hand and child of hand
        col.gameObject.transform.SetParent(playerHand);
        col.transform.position = playerHand.transform.position;
        obj = col.gameObject;

        // Disable its pickup zone and collider as well as its gravity
        obj.GetComponent<ThrowableObject>().PickupZone.enabled = false;
        obj.GetComponent<ThrowableObject>().bCollider.enabled = false;
        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.GetComponent<Rigidbody>().useGravity = false;

        holdingObject = true;
        pickupText.SetActive(false);
    }
    void ThrowObject()
    {
        // set to no parent
        obj.transform.parent = null;
        // add force to throw the object and reenable gravity
        obj.GetComponent<Rigidbody>().velocity = transform.forward * throwSpeed;
        obj.GetComponent<Rigidbody>().useGravity = true;
        // reenable colliders
        obj.GetComponent<ThrowableObject>().PickupZone.enabled = true;
        obj.GetComponent<ThrowableObject>().bCollider.enabled = true;

        holdingObject = false;
        throwText.SetActive(false);
    }
}
