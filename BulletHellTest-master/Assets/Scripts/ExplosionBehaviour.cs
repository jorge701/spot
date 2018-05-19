using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehaviour : MonoBehaviour
{

    private EntityBase m_entityUser;
    private WeaponData m_weaponReferenced;

    private float m_inheritedStrenght;
    private float lifetimeRemaining;
    private EntityBase m_lastEntityDamaged;

    private const float BASE_SCALE = 10f;

    private ParticleSystem PS;

    public void SetAs(Vector3 pos, float inhStr, EntityBase EB, WeaponData WD)
    {
        transform.position = pos;
        m_entityUser = EB;
        m_weaponReferenced = WD;
        lifetimeRemaining = 0.25f;
        m_inheritedStrenght = inhStr;
        transform.localScale = Vector3.one * BASE_SCALE * inhStr;
        m_lastEntityDamaged = null;
        m_entityUser.m_referencesToThisEntityInUse++;

        if (PS == null)
        {
            PS = GetComponent<ParticleSystem>();
        }
        var m = PS.main;
        m.startColor = m_entityUser.m_isEnemy ? EntityBase.ENEMY_COLOR_TINT : EntityBase.ALLY_COLOR_TINT;

        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        if (lifetimeRemaining > 0)
        {
            lifetimeRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            gameObject.SetActive(false);
            m_entityUser.m_referencesToThisEntityInUse--;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DamageableEntity")
        {
            m_lastEntityDamaged = other.GetComponent<EntityBase>();
            if (m_lastEntityDamaged == null)
                return;
            if (m_entityUser.m_isEnemy == m_lastEntityDamaged.m_isEnemy)
                return;
            m_lastEntityDamaged.DealExplosiveDamage(m_weaponReferenced.baseEffect_explosive * m_inheritedStrenght * m_weaponReferenced.weapon_effectivenessMultiplier * m_entityUser.GetDamageMultiplier());
        }
    }
}
