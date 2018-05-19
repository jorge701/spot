using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour {

    public Text m_info_weaponName;
    public Text m_info_weaponBaseStats;
    public Text m_info_weaponStatBoosts;
    public Text m_info_weaponSplit;
    public Text m_info_weaponEffects;

    public void UpdateTooltip(WeaponData wpn)
    {
        m_info_weaponName.text = wpn.GetString_WeaponName();
        m_info_weaponBaseStats.text = wpn.GetString_WeaponDpsAndFirerate();
        m_info_weaponStatBoosts.text = wpn.GetString_WeaponStatBoosts();
        m_info_weaponSplit.text = wpn.GetString_WeaponSplit();
        m_info_weaponEffects.text = wpn.GetString_WeaponEffects();
    }
}
