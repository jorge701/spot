using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAI : MonoBehaviour {

	public EntityShoot ES;
	public EntityBase EB;

	private WeaponData weaponUsed;
	private EntityBase target;
	private float findNewTargetCooldown;
	private float shootCooldown = 0.1f;

	// Use this for initialization
	void Start () {
		weaponUsed = new WeaponData (WeaponData.WeaponPresets.droneMinigun);
	}
	
	// Update is called once per frame
	void Update () {
		if (EB.m_health_current <= 0)
			gameObject.SetActive (false);
		if (findNewTargetCooldown > 0)
			findNewTargetCooldown -= Time.deltaTime;
		else if (target == null)
			target = StageManager.currentInstance.GetRandomEnemy ();
		else {
			if (shootCooldown > 0)
				shootCooldown -= Time.deltaTime;
			else {
				ES.Shoot (weaponUsed, EB, -TrackTo (target.transform));
				shootCooldown = 0.3f;
			}
			
		}
	}

	float TrackTo(Transform targTransform)
	{
		if (targTransform == null)
			return transform.rotation.eulerAngles.x;

		float difX = targTransform.position.x - transform.position.x;
		float difY = targTransform.position.y - transform.position.y;
		if (difX == 0)
		{
			if (difY > 0) { return 90; }
			else { return 270; }
		}

		if (difX > 0) { return Mathf.Atan(difY / difX) * Mathf.Rad2Deg; }
		else { return (Mathf.Atan(difY / difX) * Mathf.Rad2Deg) + 180; }

	}

}
