using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNotificationBehaviour : MonoBehaviour {

    private Text SelfTextComponent;

    private float animSpeed = 1;
    private float verticalSpeed = 0;
    private float horizontalSpeed = 0;

	private float animSpeedMultiplier = 1;
	private float sizeMultiplier = 1;
	private float lifetimeMultiplier = 1;

    IEnumerator AnimationNormal()
    {
        float t = 0;
        horizontalSpeed = Random.Range(-3f, 3f);
		animSpeed = 5 * animSpeedMultiplier;
        verticalSpeed = 1.5f;

        while (t < 1)
        {
            t += Time.deltaTime * animSpeed;
			transform.localScale = Vector3.one * (1 + t) * sizeMultiplier;
            transform.Translate(horizontalSpeed, verticalSpeed, 0);
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, Time.deltaTime * animSpeed);
            yield return null;
        }

        t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime * animSpeed;
			transform.localScale = Vector3.one * (1 + t) * sizeMultiplier;
            transform.Translate(horizontalSpeed, verticalSpeed, 0);
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, Time.deltaTime * animSpeed);
            yield return null;
        }

        t = 0;

		while (t < 1 * lifetimeMultiplier)
        {
            t += Time.deltaTime;
            transform.Translate(horizontalSpeed, verticalSpeed, 0);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void SetAs(int amount, WeaponData.DamageType dmgType, Vector3 screenPos)
    {
        if (SelfTextComponent == null)
            SelfTextComponent = this.GetComponent<Text>();

        SelfTextComponent.text = amount.ToString();
        transform.position = screenPos;

        switch (dmgType)
        {
			case WeaponData.DamageType.critical:
				{
					animSpeedMultiplier = 2;
					sizeMultiplier = 2f;
					lifetimeMultiplier = 2;
					StartCoroutine ("AnimationNormal");
					SelfTextComponent.color = Color.yellow;
					break;
				}
            case WeaponData.DamageType.photon:
                {
					animSpeedMultiplier = 1.5f;
					sizeMultiplier = 0.75f;
					lifetimeMultiplier = 0.75f;
                    StartCoroutine("AnimationNormal");
					SelfTextComponent.color = Color.red;
                    break;
                }
            case WeaponData.DamageType.nuclear:
                {
                    animSpeedMultiplier = 1;
                    sizeMultiplier = 1;
                    lifetimeMultiplier = 1;
                    StartCoroutine("AnimationNormal");
                    SelfTextComponent.color = ColorPallete.color_effectNuclear;
                    break;
                }
            case WeaponData.DamageType.explosive:
                {
                    animSpeedMultiplier = 1;
                    sizeMultiplier = 1;
                    lifetimeMultiplier = 1;
                    StartCoroutine("AnimationNormal");
                    SelfTextComponent.color = ColorPallete.color_effectExplosive;
                    break;
                }
            default:
                {
                    animSpeedMultiplier = 1;
                    sizeMultiplier = 1;
                    lifetimeMultiplier = 1;
                    StartCoroutine("AnimationNormal");
                    SelfTextComponent.color = Color.white;
                    break;
                }
        }

        

    }
}
