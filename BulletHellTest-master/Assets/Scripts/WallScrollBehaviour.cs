using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScrollBehaviour : MonoBehaviour {

	void FixedUpdate () {
        transform.position += Vector3.right * StageManager.currentInstance.scrollSpeed * Time.fixedDeltaTime;
	}
}
