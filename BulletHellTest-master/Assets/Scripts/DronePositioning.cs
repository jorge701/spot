using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePositioning : MonoBehaviour {

	public GameObject followingObject;
	public Transform parent;

	private float moveSpeedBase = 20;
	private float moveSpeedMultiplier = 1;
	private float targetPositionX = 0;
	public float targetPositionY = 5;
	private float posYMultiplier = 1;
	private Vector3 targetPosition;
	
	// Update is called once per frame
	void Start()
	{
	}
	void FixedUpdate () {
		posYMultiplier = Mathf.MoveTowards (posYMultiplier, 1, Time.fixedDeltaTime * 0.5f);
		targetPosition = new Vector3 (targetPositionX, targetPositionY * posYMultiplier, 0);
		parent.position = Vector3.MoveTowards(transform.position, followingObject.transform.position + targetPosition, Time.fixedDeltaTime * moveSpeedBase * moveSpeedMultiplier);

		if (transform.position != followingObject.transform.position + targetPosition)
			moveSpeedMultiplier += Time.fixedDeltaTime * 2f;
		else
			moveSpeedMultiplier = 1;
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag != "Wall")
			return;
		posYMultiplier = Mathf.MoveTowards(posYMultiplier, 0, 0.1f);
		
	}
}
