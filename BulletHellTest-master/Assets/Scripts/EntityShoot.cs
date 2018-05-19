using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityShoot : MonoBehaviour {

	private GameObject lastBulletCreated;

	public void Shoot(WeaponData wpnUsed, EntityBase entityUser, float degree)
	{
		float baseMultishootAngle = degree;
		float tempMultishootAngle = wpnUsed.weapon_multishoot > 1 ? 
			((wpnUsed.weapon_multishoot * wpnUsed.weapon_multishootSpread) - wpnUsed.weapon_multishootSpread) * 0.5f : 0;

		for (int i = 0; i < wpnUsed.weapon_multishoot; i++) {
			lastBulletCreated = ObjectPool.currentInstance.GetFromPool_bullet ();
			lastBulletCreated.GetComponent<BulletProperties> ().InitializeBullet (transform.position, tempMultishootAngle, baseMultishootAngle,
				wpnUsed, entityUser, 1);
			tempMultishootAngle -= wpnUsed.weapon_multishootSpread;
		}
	}
}
