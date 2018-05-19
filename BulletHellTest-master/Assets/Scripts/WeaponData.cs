using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esta clase administra la informacion y parametros de las armas, tanto del jugador, como enemigas.

public class WeaponData {

    public enum WeaponType
    {
        generic, bullet, laser, plasma, photon, missile
    }
    public enum DamageType
    {
        normal, critical, photon, nuclear, explosive
    }

	public enum WeaponPresets
	{
		randomScaledPlayer, randomScaledEnemy, droneNormal, droneRocket, droneMinigun
	}

    public string weaponName;

    public UpgradeData.Trajectory weapon_trajectory;
    public UpgradeData.Trajectory weapon_trajectoryAfterSplit;

    public float baseEffect_normal;
    public float baseEffect_photon;
    public float baseEffect_nuclear;
    public float baseEffect_explosive;
    public float baseEffect_energySteal;

    public WeaponType weapon_type;

    public float weapon_speedScale;

    public float weapon_multishootSpread;
    public int weapon_multishoot;
    public int weapon_split;
    public int weapon_bounces;
    public float weapon_firerateMultiplier;
    public float weapon_effectivenessMultiplier;

    public float weapon_critChanceStat;
    public float weapon_critDamageStat;

    public float weapon_lifetime_beforeSplit;
    public float weapon_lifetime_afterSplit;

    public List<UpgradeData> UpgradesInstalled;

    private const float CRITICAL_CHANCE_BASE = 5f;
    private const float CRITICAL_DAMAGE_BASE = 2f;



	public WeaponData(WeaponPresets preset, float weaponScalingFactor = 1)
	{
        SetRandomWeaponType();
		switch (preset) {
		    case WeaponPresets.randomScaledPlayer:
			    {
			    	UpgradesInstalled = new List<UpgradeData>() { new UpgradeData(UpgradeData.ModType.trajectoryMod, UpgradeData.ModStatus.locked) };

                    float tempWeaponWeight = 0;
                    if (weaponScalingFactor > 3)
                    {
                        if (Random.Range(0, 3) == 1)
                        {
                            UpgradesInstalled.Add(new UpgradeData(UpgradeData.ModType.splitMod, UpgradeData.ModStatus.normal));
                            tempWeaponWeight += 1.25f;
                        }
                    }
                    while (tempWeaponWeight < weaponScalingFactor)
                    {
                        UpgradesInstalled.Add(new UpgradeData(UpgradeData.GetRandomEffectModifier(), UpgradeData.ModStatus.normal));
                        tempWeaponWeight += Random.Range(0.35f, 0.65f);
                    }
				    weapon_bounces = 0;
				    weapon_speedScale = 2;
				    break;
			    }
            case WeaponPresets.randomScaledEnemy:
                {
                    UpgradesInstalled = new List<UpgradeData>() { new UpgradeData(UpgradeData.ModType.trajectoryMod, UpgradeData.ModStatus.normal) };
                    float tempWeaponWeight = 0;
                    while (tempWeaponWeight < weaponScalingFactor)
                    {
                        UpgradesInstalled.Add(new UpgradeData(UpgradeData.GetRandomEffectModifier(), UpgradeData.ModStatus.normal));
                        tempWeaponWeight += Random.Range(0.45f, 0.55f);
                    }

                    weapon_bounces = 0;
                    weapon_speedScale = 0.75f + weaponScalingFactor*0.2f;

                    break;
                }
		    case WeaponPresets.droneMinigun:
			    {
				    weapon_bounces = 0;
				    weapon_speedScale = 3;
				    break;
			    }

		}
        UpdateWeaponModReading();
	
    }
    public void UpdateWeaponModReading()
    {
        InitializeData();
        if (UpgradesInstalled == null)
			return;
        foreach (UpgradeData upg in UpgradesInstalled)
        {
            if (upg != null)
                upg.ApplyModToWeapon(this);
        }
    }
    void InitializeData()
    {
        weaponName = "Test weapon";
        weapon_lifetime_afterSplit = 4f;
        weapon_lifetime_beforeSplit = 4f;
        weapon_trajectory = UpgradeData.Trajectory.normal;
        weapon_trajectoryAfterSplit = UpgradeData.Trajectory.normal;
        weapon_firerateMultiplier = 4f;
        weapon_effectivenessMultiplier = 1;
        weapon_multishootSpread = 20f;
        weapon_multishoot = 1;
        weapon_split = 0;
        weapon_critChanceStat = CRITICAL_CHANCE_BASE;
        weapon_critDamageStat = CRITICAL_DAMAGE_BASE;

        switch (weapon_type)
        {
            case WeaponType.bullet:
                {
                    weapon_speedScale = 2;
                    weapon_bounces = 0;

                    baseEffect_normal = 100;
                    baseEffect_photon = 0;
                    baseEffect_nuclear = 0;
                    baseEffect_explosive = 0;
                    baseEffect_energySteal = 0;
                    break;
                }
            case WeaponType.laser:
                {
                    weapon_speedScale = 1.5f;
                    weapon_bounces = 2;

                    baseEffect_normal = 80;
                    baseEffect_photon = 0;
                    baseEffect_nuclear = 0;
                    baseEffect_explosive = 0;
                    baseEffect_energySteal = 0;

                    break;
                }
            case WeaponType.plasma:
                {
                    weapon_speedScale = 1f;
                    weapon_bounces = 0;

                    baseEffect_normal = 0;
                    baseEffect_photon = 0;
                    baseEffect_nuclear = 170;
                    baseEffect_explosive = 0;
                    baseEffect_energySteal = 0;
                    break;
                }
            case WeaponType.photon:
                {
                    weapon_speedScale = 1.15f;
                    weapon_bounces = 0;

                    baseEffect_normal = 0;
                    baseEffect_photon = 150;
                    baseEffect_nuclear = 0;
                    baseEffect_explosive = 0;
                    baseEffect_energySteal = 0;

                    break;
                }
            case WeaponType.missile:
                {
                    weapon_speedScale = 1.35f;
                    weapon_bounces = 0;

                    baseEffect_normal = 10;
                    baseEffect_photon = 0;
                    baseEffect_nuclear = 0;
                    baseEffect_explosive = 72;
                    baseEffect_energySteal = 0;
                    break;
                }

            default:
                {
                    weapon_speedScale = 1;
                    weapon_bounces = 0;

                    baseEffect_normal = 100;
                    baseEffect_photon = 0;
                    baseEffect_nuclear = 0;
                    baseEffect_explosive = 0;
                    baseEffect_energySteal = 0;
                    break;
                }
        }

    }
    void SetRandomWeaponType()
    {
        int var = Random.Range(0, 5);
        switch (var)
        {
            case 0:
                {
                    weapon_type = WeaponType.bullet;
                    break;
                }
            case 1:
                {
                    weapon_type = WeaponType.laser;
                    break;
                }
            case 2:
                {
                    weapon_type = WeaponType.plasma;
                    break;
                }
            case 3:
                {
                    weapon_type = WeaponType.photon;
                    break;
                }
            case 4:
                {
                    weapon_type = WeaponType.missile;
                    break;
                }

            default:
                {
                    weapon_type = WeaponType.generic;
                    break;
                }
        }
    }
    #region Getters
    public float GetCritChance()
	{
		return weapon_critChanceStat;
	}
	public float GetCritDamage()
	{
		return weapon_critDamageStat;
	}
    public float GetNormalDamage()
    {
        return baseEffect_normal * weapon_effectivenessMultiplier;
    }
    public float GetPhotonDamage()
    {
        return baseEffect_photon * weapon_effectivenessMultiplier;
    }
    public float GetNuclearDamage()
    {
        return baseEffect_nuclear * weapon_effectivenessMultiplier;
    }
    public float GetExplosiveDamage()
    {
        return baseEffect_explosive * weapon_effectivenessMultiplier;
    }
    #endregion
    #region String Getters
    public string GetString_WeaponName()
    {
        return weaponName;
    }
    public string GetString_WeaponDpsAndFirerate()
    {
        return "????";
    }
    public string GetString_WeaponStatBoosts()
    {
        return "????";
    }
    public string GetString_WeaponEffects()
    {
        return "????";
    }
    public string GetString_WeaponSplit()
    {
        return "????";
    }
    public string GetString_WeaponType()
    {
        string txt = "";
        switch (weapon_type)
        {
            case WeaponType.bullet:
                {
                    txt = "Pulse cannon";
                    break;
                }
            case WeaponType.laser:
                {
                    txt = "Laser cannon";
                    break;
                }
            case WeaponType.plasma:
                {
                    txt = "Plasma cannon";
                    break;
                }
            case WeaponType.photon:
                {
                    txt = "Photon cannon";
                    break;
                }
            case WeaponType.missile:
                {
                    txt = "Missile launcher";
                    break;
                }
            default:
                {
                    txt = "Unknown";
                    break;
                }
        }
        return txt;
    }
    #endregion
}
