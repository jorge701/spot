using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_test : MonoBehaviour {

	public EntityBase EB;
	public EntityShoot ES;
	private WeaponData WD;
    private WeaponData WD2;

	float shootready = 0f;
    float shootready2 = 0.5f;
	private Transform shootTarget;
    private float fireRate1 = 0.5f;
    private float fireRate2 = 0.1f;

    private EntityBase ebFound;

	// Use this for initialization
	void Start () {
        WD = StageManager.currentInstance.enemyWeapons[0];
        WD2 = StageManager.currentInstance.enemyWeapons[1];
	}

    // Update is called once per frame
    void Update() {
        if (shootTarget == null)
        {
            ebFound = StageManager.currentInstance.GetRandomAlly();
            if (ebFound != null)
            {
                shootTarget = ebFound.transform;
            }
        }
		if (EB.m_health_current <= 0 || shootTarget == null)
			return;
		shootready += Time.deltaTime * WD.weapon_firerateMultiplier;
        shootready2 += Time.deltaTime * WD2.weapon_firerateMultiplier;
		if (shootready > 1) {
			shootready = 0;
			ES.Shoot (WD, EB, -TrackTo(shootTarget));
			shootTarget = null;
		}
        if (shootready2 > 1)
        {
            shootready2 = 0;
            ES.Shoot(WD2, EB, -TrackTo(shootTarget));
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
