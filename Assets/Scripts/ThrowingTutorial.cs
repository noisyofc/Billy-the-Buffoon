using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowingTutorial : MonoBehaviour
{
    public int choice = 0;
    public GameObject[] objectsToThrow;

    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    public int totalThrows;
    public float throwCooldown;

    public KeyCode trampKey = KeyCode.Mouse0;
    public KeyCode bananaKey = KeyCode.Mouse2;
    public KeyCode glueKey = KeyCode.Mouse1;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(trampKey) && readyToThrow && totalThrows > 0)
        {
            Throw(0);
        }
        if (Input.GetKeyDown(bananaKey) && readyToThrow && totalThrows > 0)
        {
            Throw(1);
        }
        if (Input.GetKeyDown(glueKey) && readyToThrow && totalThrows > 0)
        {
            Throw(2);
        }
    }

    private void Throw(int choice)
    {
        readyToThrow = false;

        if (choice == 0)
        {
            objectToThrow = objectsToThrow[0];
        }

        if (choice == 1)
        {
            objectToThrow = objectsToThrow[1];
        }

        if (choice == 2)
        {
            objectToThrow = objectsToThrow[2];
        }

        // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        // implement throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}