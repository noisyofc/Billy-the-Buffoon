using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollectibleScript : MonoBehaviour {

	public enum CollectibleTypes {NoType, Type1, Type2, Type3, Type4, Type5}; // you can replace this with your own labels for the types of collectibles in your game!

	public CollectibleTypes CollectibleType; // this gameObject's type

	public bool rotate; // do you want it to rotate?

	public float rotationSpeed;

	public GameObject collectEffect;

	public static SimpleCollectibleScript instance;

	public int starsCollected = 0;

	public AudioSource collect;

	private void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		starsCollected = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (collect == null)
        {
			collect = GameObject.Find("Collect").GetComponent<AudioSource>();
        }

		if (rotate)
			transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			Collect ();
		}
	}

	public void Collect()
	{
		collect.Play();

		if (collectEffect)
			Instantiate(collectEffect, transform.position, Quaternion.identity);

		//Below is space to add in your code for what happens based on the collectible type

		if (CollectibleType == CollectibleTypes.NoType)
		{

			//Add in code here;
			starsCollected += 1;

			Debug.Log("Do NoType Command");
		}
		if (CollectibleType == CollectibleTypes.Type1)
		{

			//Add in code here;

			Debug.Log("Do NoType Command");
		}
		if (CollectibleType == CollectibleTypes.Type2)
		{

			//Add in code here;

			Debug.Log("Do NoType Command");
		}
		if (CollectibleType == CollectibleTypes.Type3)
		{

			//Add in code here;

			Debug.Log("Do NoType Command");
		}
		if (CollectibleType == CollectibleTypes.Type4)
		{

			//Add in code here;

			Debug.Log("Do NoType Command");
		}
		if (CollectibleType == CollectibleTypes.Type5)
		{

			//Add in code here;

			Debug.Log("Do NoType Command");
		}

		//Destroy (gameObject);
		gameObject.SetActive(false);
	}
}
