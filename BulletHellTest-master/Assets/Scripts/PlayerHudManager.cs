using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour {

	public PlayerEntity PE;
    [Header("Health and shield HUD")]
    public Image hud_radial_health;
    public Image hud_radial_shield;
    public Image hud_radial_recharge;
    public Image hud_glow;
    public Image hud_back;
	public Text hud_text_health;
    public Text hud_text_shield;
    public Text hud_text_offline;

    [Header("HUD Colors")]
    public Color shieldColor;
    public Color fullHealhColor;
    public Color lowHealthColor;

	private Vector3 initialPosition;

	private float screenShakeStrenghtRemaining = 0;
	private const float SCREEN_SHAKE_SPEED = 400f;
	private const float SCREEN_SHAKE_THRESHOLD = 5;
	private const float SCREEN_SHAKE_DECAY = 0.7f;

	private const float BAR_FILL_SPEED = 1f;
	private const float BAR_NUMBER_SPEED = 500;

    [Header("Buff references")]
	public List<BuffIconBehaviour> buffIcons;

    public static PlayerHudManager currentInstance;


    // Use this for initialization
    void Awake()
    {
        currentInstance = this;
    }
    void Start () {
		initialPosition = transform.localPosition;
		StartCoroutine ("UpdateHealthAndShield");
        StartCoroutine("UpdateBuffs");
	}
    public void DisplayDamageNumber(int dmg, WeaponData.DamageType dmgtype, Vector3 worldPos)
    {

        GameObject lastInstantiatedObject = ObjectPool.currentInstance.GetFromPool_dmgNum();
        lastInstantiatedObject.SetActive(true);
        lastInstantiatedObject.GetComponent<DamageNotificationBehaviour>().SetAs(dmg, dmgtype, Camera.main.WorldToScreenPoint(worldPos));

    }
    public void ShakeHud(float intensity)
	{
		screenShakeStrenghtRemaining = intensity * 0.3f;
		StopCoroutine ("ShakeAnimation");
		StartCoroutine ("ShakeAnimation");
	}
	public void SendLastBuffDuration(float _dur, int _slot)
    {
		buffIcons [_slot].SendNewDuration (_dur);
    }
    void AnimateGlow()
    {
        StopCoroutine("GlowAnimation");
        StartCoroutine("GlowAnimation");
    }
    IEnumerator UpdateBuffs()
    {
        while (true)
        {
			for (int i = 0; i < buffIcons.Count; i++) {
				buffIcons [i].UpdateDisplay (PE.m_ref_entityBase.GetBuffStacks (i), PE.m_ref_entityBase.GetBuffStrenght (i));
			}
			yield return null;
        }
    }
	IEnumerator UpdateHealthAndShield()
	{
        float targetHealthPercent = 0;
        float targetShieldPercent = 0;
        float targetRechargePercent = 0;

        float tempHealthPercent = 0;
        float tempShieldPercent = 0;
        float tempRechargePercent = 0;

        float targetHealthAmount = 0;
        float targetShieldAmount = 0;
        float tempHealthAmount = 0;
        float tempShieldAmount = 0;

        Color tempGlowColor = shieldColor;
        Color healthColor = fullHealhColor;

        bool shieldBroken = true;

		while (true) {

			targetShieldPercent = PE.GetShieldPercent ();
			targetHealthPercent = PE.GetHealthPercent ();
            targetHealthAmount = PE.GetCurrentHealth();
            targetShieldAmount = PE.GetCurrentShield();
            targetRechargePercent = PE.GetShieldRechargePercent();

			tempHealthPercent = Mathf.MoveTowards (tempHealthPercent, targetHealthPercent, Time.deltaTime * BAR_FILL_SPEED);
			tempShieldPercent = Mathf.MoveTowards (tempShieldPercent, targetShieldPercent, Time.deltaTime * BAR_FILL_SPEED);
            tempRechargePercent = Mathf.MoveTowards(tempRechargePercent, targetRechargePercent, Time.deltaTime * BAR_FILL_SPEED);
			tempHealthAmount = Mathf.MoveTowards (tempHealthAmount, PE.GetCurrentHealth(), Time.deltaTime * BAR_NUMBER_SPEED);
			tempShieldAmount = Mathf.MoveTowards (tempShieldAmount, PE.GetCurrentShield(), Time.deltaTime * BAR_NUMBER_SPEED);

            hud_radial_health.fillAmount = tempHealthPercent * 0.75f;
            hud_radial_shield.fillAmount = tempShieldPercent * 0.75f;
            hud_radial_recharge.fillAmount = tempRechargePercent * 0.75f;
            hud_text_health.text = ((int)tempHealthAmount).ToString();
            hud_text_shield.text = ((int)tempShieldAmount).ToString();

            if (tempShieldPercent > 0)
            {
                if (shieldBroken)
                {
                    hud_text_shield.gameObject.SetActive(true);
                    hud_text_offline.gameObject.SetActive(false);
                    shieldBroken = false;
                    AnimateGlow();
                }
                tempGlowColor = shieldColor;
            }
            else
            {
                if (!shieldBroken)
                {
                    hud_text_shield.gameObject.SetActive(false);
                    hud_text_offline.gameObject.SetActive(true);
                    shieldBroken = true;
                    AnimateGlow();
                }
                tempGlowColor = Color.Lerp(lowHealthColor, fullHealhColor, tempHealthPercent);
            }
            hud_radial_health.color = hud_text_health.color = Color.Lerp(lowHealthColor, fullHealhColor, tempHealthPercent);
            hud_glow.color = hud_back.color = hud_radial_recharge.color = tempGlowColor;

			yield return null;
		}
	}
    IEnumerator GlowAnimation()
    {
        float t = 0;
        float animSpeed = 3f;
        while (t < 1)
        {
            t += Time.deltaTime * animSpeed;
            hud_glow.transform.localScale = Vector3.one * (1 + (t*1.5f));
            yield return null;
        }
        t = 0;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        t = 1;
        while (t > 0)
        {
            t = Mathf.MoveTowards(t, 0, Time.deltaTime * animSpeed);
            hud_glow.transform.localScale = Vector3.one * (1 + (t*1.5f));
            yield return null;
        }

    }
	IEnumerator ShakeAnimation()
	{
		float currentX = 0;
		float targetX = screenShakeStrenghtRemaining;
		int inverted = 1;

		while (screenShakeStrenghtRemaining > 0) {
			currentX = Mathf.MoveTowards (currentX, targetX, Time.deltaTime * SCREEN_SHAKE_SPEED * screenShakeStrenghtRemaining * 0.1f);
			if (currentX == targetX) {
				screenShakeStrenghtRemaining *= SCREEN_SHAKE_DECAY;
				if (screenShakeStrenghtRemaining < SCREEN_SHAKE_THRESHOLD) {
					screenShakeStrenghtRemaining = 0;
				}
				inverted *= -1;
				targetX = screenShakeStrenghtRemaining * inverted;
			}

			transform.localPosition = initialPosition + Vector3.up * currentX;
			yield return null;
		}
		while (currentX != 0) {
			currentX = Mathf.MoveTowards (currentX, 0, Time.deltaTime * SCREEN_SHAKE_SPEED);
			transform.localPosition = initialPosition + Vector3.up * currentX;
			yield return null;
		}
	}
}
