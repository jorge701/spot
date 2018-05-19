using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour {

	public static PlayerEntity currentInstance;

    public GameObject m_shieldVisual;
	public EntityShoot m_ref_entityShoot;
	public PlayerMovement m_ref_playerMovement;
	public PlayerVisualAnimation m_ref_playerVisualAnimation;
	public PlayerHudManager m_ref_playerHudManager;
    public PlayerBuffManager m_ref_playerBuffManager;
    public EntityBase m_ref_entityBase;

	public WeaponData normalWeapon;

	private bool turned = false;
	private float shootReady = 1;
	private float turnReady = 1;

    private float shieldRechargeReady = 1;
    private float shieldRechargeSpeed = 1f;

	void Awake()
	{
		currentInstance = this;
	}
	void Start () {
        normalWeapon = GlobalGameData.currentInstance.playerWeaponTEST;
        print("Player weapon has the following upgrades ============");
        for (int i = 0; i < normalWeapon.UpgradesInstalled.Count; i++)
        {
            print (normalWeapon.UpgradesInstalled[i].GetModName());
        }
        print("=====================================================");
	}
	
	// Update is called once per frame
	void Update () {
        m_shieldVisual.gameObject.SetActive(m_ref_entityBase.GetShieldPercent() > 0);
		shootReady = Mathf.MoveTowards(shootReady, 1, Time.deltaTime * normalWeapon.weapon_firerateMultiplier);
		turnReady = Mathf.MoveTowards (turnReady, 1, Time.deltaTime * 2f);

        if (shieldRechargeReady < 1)
        {
            shieldRechargeReady = Mathf.MoveTowards(shieldRechargeReady, 1, Time.deltaTime * shieldRechargeSpeed * 0.2f);
        }
        else if (GetShieldPercent() < 1)
        {
            m_ref_entityBase.m_shield_current += m_ref_entityBase.m_shield_max * Time.deltaTime * shieldRechargeSpeed * 0.1f;
        }

		if (Input.GetKeyDown (KeyCode.LeftControl) && turnReady >= 1) {
			turnReady = 0;
			turned = !turned;
			m_ref_playerVisualAnimation.SetTurned (turned);
		}
		if (Input.GetKey (KeyCode.Space) && shootReady >= 1) {
			m_ref_entityShoot.Shoot (normalWeapon, m_ref_entityBase, turned? 180: 0);
			shootReady = 0;
		}
		// DEBUG
		if (Input.GetKeyDown (KeyCode.K)) {
			m_ref_entityBase.DealDamage (250, WeaponData.DamageType.normal);
		}
		// DEBUG
        if (Input.GetKeyDown(KeyCode.B))
        {
		    m_ref_entityBase.AddBuff (EntityBase.BuffType.directDamage, 8, 5);
			//m_ref_playerBuffManager.AddBuff (PlayerBuffManager.BuffType.criticalChance, 10, 5);
			//m_ref_playerBuffManager.AddBuff (PlayerBuffManager.BuffType.criticalDamage, 4, 5);
			//m_ref_playerBuffManager.AddBuff (PlayerBuffManager.BuffType.damageOverTime, 2, 5);
        }
    }
	public float GetHealthPercent()
	{
        return m_ref_entityBase.GetHealthPercent();
	}
	public float GetShieldPercent()
	{
        return m_ref_entityBase.GetShieldPercent();
	}
	public float GetCurrentHealth()
	{
        return m_ref_entityBase.m_health_current;
	}
	public float GetCurrentShield()
	{
        return m_ref_entityBase.m_shield_current;
	}
    public float GetShieldRechargePercent()
    {
        return shieldRechargeReady;
    }
    public void ResetShieldRecharge()
    {
        shieldRechargeReady = 0;
    }
}
