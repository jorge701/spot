using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private const float MOVEMENT_LIMIT_MAX_X = 8f;
	private const float MOVEMENT_LIMIT_MAX_Y = 8f;

    public PlayerVisualAnimation PVA;
    public Rigidbody RB;

	private const float MOVEMENT_SPEED_BASE = 20f;
	private float moveSpeedMultiplier = 1;

	private float inputX = 0;
	private float inputY = 0;

    private void Start()
    {
        RB = this.GetComponent<Rigidbody>();
    }
	void Update()
	{
		inputX = Input.GetAxisRaw ("Horizontal");
		inputY = Input.GetAxisRaw ("Vertical");
        PVA.SetVerticalInput(inputY);
	}
	void FixedUpdate()
	{
        //RB.velocity = new Vector3(inputX, inputY, 0) * MOVEMENT_SPEED_BASE * moveSpeedMultiplier;
        transform.Translate(new Vector3(inputX, inputY, 0) * MOVEMENT_SPEED_BASE * moveSpeedMultiplier * Time.fixedDeltaTime);
		//transform.position = new Vector3 (Mathf.Clamp (transform.position.x, MOVEMENT_LIMIT_MIN_X, MOVEMENT_LIMIT_MAX_X), Mathf.Clamp (transform.position.y, MOVEMENT_LIMIT_MIN_Y, MOVEMENT_LIMIT_MAX_Y), 0);
	}
}
