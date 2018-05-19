using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour {

	public bool m_isEnemy;

	public int m_referencesToThisEntityInUse = 0;

	public string m_entity_name;
    public float m_entity_effectivenessScaling;
    public float m_health_current;
	public float m_health_max;
    public float m_shield_current;
    public float m_shield_max;

    public float m_bonusDamageMultiplier;
    public float m_bonusCritDamageMultiplier;
    public float m_bonusCritChance;
	public float m_bonusDefMultiplier;
    public GameObject m_entityParent;


    [Header("Difficulty Scaling")]
    public float m_projectileSpeedScale = 1;
    [Header("Only for player")]
	public PlayerEntity m_attachToPlayerEntity;

    private const float DOT_MINIMUM_DAMAGE_THS = 1f;
    private const float PHOTON_TICK_DAMAGE = 0.075f;

	private float m_debuff_fire = 0;
	private bool m_fireCoroutineRunning = false;

	private bool m_isActive = true;

    public static Color ENEMY_COLOR_TINT = new Color32(255, 37, 0, 255);
    public static Color ALLY_COLOR_TINT = new Color32(0, 170, 255, 255);

    public enum BuffType
    {
        directDamage,
        criticalDamage,
        criticalChance,
        protection
    }

    private int[] buffStacks = { 0, 0, 0, 0 };
    private float[] buffStrenght = { 0f, 0f, 0f, 0f };

    private float tempCRStartDuration;
    private float tempCRStartStrenght;
    private BuffType tempCRStartType;

    void Start () {
		if (m_isEnemy)
			StageManager.currentInstance.RegisterEnemy (this);
		else
			StageManager.currentInstance.RegisterAlly (this);
		if (m_attachToPlayerEntity != null)
			StageManager.currentInstance.RegisterPlayer (this);
		
        /*if (m_isEnemy)
            StartCoroutine("DpsMeter"); */
	}
    void SubstractHealthBasedOnDamageType(float dmg, WeaponData.DamageType type)
    {
        if (m_health_current <= 0)
            return;

        if (m_isEnemy)
            PlayerHudManager.currentInstance.DisplayDamageNumber((int)dmg, type, transform.position);
        if (m_attachToPlayerEntity != null)
        {
            m_attachToPlayerEntity.m_ref_playerHudManager.ShakeHud(dmg);
            if (type != WeaponData.DamageType.photon)
            {
                m_attachToPlayerEntity.ResetShieldRecharge();
            }
        }

        m_shield_current -= dmg;
        if (m_shield_current < 0)
        {
            m_health_current += m_shield_current;
            m_shield_current = 0;
        }
        if (m_health_current < 0)
        {
            DestroyEntity();
        }
    }
    #region Co-Routines
    IEnumerator DpsMeter()
    {
        float lastRegistredHealth = m_health_current;
        float dps = 0;
        while (true)
        {
            dps = (lastRegistredHealth - m_health_current) / 5;
            Debug.Log("DPS: " + (int)dps);
            lastRegistredHealth = m_health_current;
            yield return new WaitForSeconds(5f);
        }
    }
    IEnumerator TickDamageOverTime_Fire()
    {
        float tickDamage = 0;
		while (m_debuff_fire > 0 && m_health_current > 0)
        {
            tickDamage = m_debuff_fire * PHOTON_TICK_DAMAGE;
            if (tickDamage > DOT_MINIMUM_DAMAGE_THS)
            {
                m_debuff_fire -= tickDamage;
                SubstractHealthBasedOnDamageType(tickDamage, WeaponData.DamageType.photon);
            }
            else
            {
                m_debuff_fire -= DOT_MINIMUM_DAMAGE_THS;
                SubstractHealthBasedOnDamageType(DOT_MINIMUM_DAMAGE_THS, WeaponData.DamageType.photon);
            }
           
            yield return new WaitForSeconds(0.25f);
        }
        m_fireCoroutineRunning = false;
    }
    IEnumerator BuffProcessing()
    {
        float CR_buffStrenght = tempCRStartStrenght;
        float CR_buffDuration = tempCRStartDuration;
        int CR_buffType = (int)tempCRStartType;
        buffStacks[CR_buffType]++;
        buffStrenght[CR_buffType] += CR_buffStrenght;
        yield return new WaitForSeconds(CR_buffDuration);
        buffStacks[CR_buffType]--;
        buffStrenght[CR_buffType] -= CR_buffStrenght;
    }
    #endregion
    #region Public Functions
    public void HealWithEnergySteal(float amount)
    {
        if (m_health_current <= 0)
            return;
        m_health_current = Mathf.MoveTowards(m_health_current, m_health_max, amount);
    }
    public void DealDamage(float dmg, WeaponData.DamageType dmgType)
    {
        if (m_health_current <= 0)
            return;
        SubstractHealthBasedOnDamageType(dmg, dmgType);
        dmg *= m_bonusDefMultiplier;
        if (dmg < 1)
            dmg = 1;
    }
    public void DealFireDamage(float fdmg)
    {
        if (m_health_current <= 0)
            return;

        m_debuff_fire += fdmg;
        if (!m_fireCoroutineRunning)
        {
            m_fireCoroutineRunning = true;
            StartCoroutine("TickDamageOverTime_Fire");
        }
    }
    public void DealNuclearDamage(float ndmg)
    {
        if (m_health_current <= 0)
            return;

        SubstractHealthBasedOnDamageType(ndmg, WeaponData.DamageType.nuclear);
    }
    public void DealExplosiveDamage(float edmg)
    {
        if (m_health_current <= 0)
            return;
        SubstractHealthBasedOnDamageType(edmg, WeaponData.DamageType.explosive);
    }
    public void AddBuff(BuffType _buffType, float _buffDuration, float _buffStrenght)
    {
        tempCRStartDuration = _buffDuration;
        tempCRStartStrenght = _buffStrenght;
        tempCRStartType = _buffType;
        StartCoroutine("BuffProcessing");

        if (m_attachToPlayerEntity != null)
            PlayerHudManager.currentInstance.SendLastBuffDuration(_buffDuration, (int)_buffType);
    }
    public void RemoveAllBuffs()
    {
        for (int i = 0; i < buffStacks.Length; i++)
        {
            buffStacks[i] = 0;
        }
        for (int i = 0; i < buffStacks.Length; i++)
        {
            buffStrenght[i] = 0;
        }
    }
    public void DestroyEntity()
    {
        m_health_current = 0;
        if (m_isEnemy)
        {
            StageManager.currentInstance.RemoveEnemy(this);
        }
        else
        {
            StageManager.currentInstance.RemoveAlly(this);
        }
        StopAllCoroutines();
        RemoveAllBuffs();
        m_entityParent.gameObject.SetActive(false);
    }
    #endregion
    #region Getters
    public float GetHealthPercent()
	{
		return m_health_current / m_health_max;
	}
    public float GetShieldPercent()
    {
        return m_shield_current / m_shield_max;
    }
    public float GetBuffMultiplier(BuffType buff)
    {
        return buff == BuffType.criticalChance ? buffStrenght[(int)buff] : 1 + buffStrenght[(int)buff] / 100f;
    }
    public float GetBuffMultiplier(int index)
    {
        return index == (int)BuffType.criticalChance? buffStrenght[index] : 1 + buffStrenght[index] / 100f;
    }

    public float GetBuffStrenght(BuffType buff) { return buffStrenght[(int)buff]; }
    public float GetBuffStrenght(int index) { return buffStrenght[index]; }

    public int GetBuffStacks(BuffType buff) { return buffStacks[(int)buff]; }
    public int GetBuffStacks(int index) { return buffStacks[index]; }

    public float GetDamageMultiplier()
    {
        return (m_bonusDamageMultiplier + GetBuffMultiplier(BuffType.directDamage)) * m_entity_effectivenessScaling;
    }
    public float GetCriticalChance()
    {
        return m_bonusCritChance + GetBuffMultiplier(BuffType.criticalChance);
    }
    public float GetCriticialDamageMultiplier()
    {
        return m_bonusCritDamageMultiplier + GetBuffMultiplier(BuffType.criticalDamage) -1;
    }
    #endregion
}
