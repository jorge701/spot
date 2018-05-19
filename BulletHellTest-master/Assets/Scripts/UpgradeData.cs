using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData {

	// Estado del modificador, vacio, bloqueado, o normal.
    public enum ModStatus
    {
        locked, normal
    }
	// Ranura en la que encaja el modificador, de estadistica, de impacto, de trayectoria, de fragmentacion, o de efecto.
    public enum ModSlot
    {
        trajectory, trajectoryOnSplit, effect, bonus, none
    }
    // Tipo de modificador del que se trata, utilizado a la hora de leerlo
    public enum ModType
    {
        none, trajectoryMod, splitMod, effectPhoton, effectNuclear, effectEnergySteal, effectExplosive, effectBouncing
    }

    // Comportamientos de movimiento, diferentes formas de moverse que tendran las balas disparadas.
    public enum Trajectory
    {
        none,
        deflected,
        stickToPlayer,
        normal,
        helix,
        wave,
        tracking,
        unstable,
        delayedSurge,
        delayedTrack,
        coil,
        arc
    }

    public ModStatus m_current_status;								// Estado actual del modificador
    public ModType m_currentType;

    public int m_modStrenghtValue;
    public float m_modVariationValue;
    public Trajectory m_modTrajectoryValue;

    public float m_penalty_frateMultiplier;						    // Multiplicador de penalizacion de velocidad de disparo
    public float m_penalty_effectivenessMultiplier;



    public UpgradeData(ModType type, ModStatus status = ModStatus.normal)
    {
        m_current_status = status;
        m_currentType = type;
        SetInitialState();
		TypeSetup ();
    }

	// Configuracion base de cada parametro
    void SetInitialState()
    {
        m_current_status = ModStatus.normal;
        m_modStrenghtValue = 0;
        m_modVariationValue = 0f;
        m_modTrajectoryValue = Trajectory.none;
        m_penalty_frateMultiplier = 1;
        m_penalty_effectivenessMultiplier = 1;
}

	// Ajuste en funcion del tipo de mod, si es necesario
    void TypeSetup()
    {
        if (m_currentType == ModType.trajectoryMod)
        {
            SetRandomTrajectoryModification();
        }
        else if (m_currentType == ModType.splitMod)
        {
            SetRandomSplitTrajectoryModification();
        }
        else
        {
            // Nada necesario (?).
        }
    }

	// Ajuste de un tipo de trayectoria al azar, con una cantidad de multidisparo acorde con el modificador
    void SetRandomTrajectoryModification()
    {
        int randomVar = Random.Range(1, 8);
        float apertureMultiplier = 1;
        switch (randomVar)
        {
            case 1:
                {
                    m_modTrajectoryValue = Trajectory.normal;
                    m_modStrenghtValue = Random.Range(1, 3) == 1 ? 1 : Random.Range(3, 6);
                    apertureMultiplier = 1f;
                    m_penalty_effectivenessMultiplier = 1.15f;
                    break;
                }
            case 2:
                {
                    m_modTrajectoryValue = Trajectory.helix;
                    m_modStrenghtValue = Random.Range(2, 6);
                    apertureMultiplier = m_modStrenghtValue == 2? 1.15f : 1.05f;
					m_penalty_effectivenessMultiplier = 0.85f;
                    break;
                }
            case 3:
                {
                    m_modTrajectoryValue = Trajectory.wave;
                    m_modStrenghtValue = Random.Range(2, 6);
                    apertureMultiplier = 1.1f;
                    m_penalty_effectivenessMultiplier = 0.9f;
                    break;
                }
            case 4:
                {
                    m_modTrajectoryValue = Trajectory.tracking;
                    m_modStrenghtValue = Random.Range(1, 6);
                    apertureMultiplier = 1f;
                    m_penalty_effectivenessMultiplier = 0.65f;
                    break;
                }
            case 5:
                {
                    m_modTrajectoryValue = Trajectory.delayedTrack;
                    m_modStrenghtValue = Random.Range(3, 6);
                    apertureMultiplier = 1.25f;
					m_penalty_effectivenessMultiplier = 0.6f;
                    break;
                }
            case 6:
                {
                    m_modTrajectoryValue = Trajectory.delayedSurge;
                    m_modStrenghtValue = Random.Range(3, 6);
                    apertureMultiplier = 1.25f;
					m_penalty_effectivenessMultiplier = 1.15f;
                    break;
                }
            case 7:
                {
                    m_modTrajectoryValue = Trajectory.arc;
                    m_modStrenghtValue = Random.Range(2, 6);
                    apertureMultiplier = 1.15f;
                    m_penalty_effectivenessMultiplier = 1.25f;
                    break;
                }

        }
        m_modVariationValue = (115 / m_modStrenghtValue) * Random.Range(0.65f, 1.25f) * apertureMultiplier;
        m_penalty_frateMultiplier = 1f / m_modStrenghtValue;

    }

	// Ajuste de trayectoria de fragmentacion, y de cantidad de fragmentos acorde con el modificador
	void SetRandomSplitTrajectoryModification()
	{
		int randomVar = Random.Range(1, 8);
		switch (randomVar)
		{
		case 1:
			{
				m_modTrajectoryValue = Trajectory.normal;
				m_modStrenghtValue = Random.Range(2, 6);
				m_penalty_frateMultiplier = 1 - (0.035f * m_modStrenghtValue);
				break;
			}
		case 2:
			{
				m_modTrajectoryValue = Trajectory.coil;
				m_modStrenghtValue = Random.Range(1, 3) * 2;
                m_penalty_frateMultiplier = 1 - (0.05f * m_modStrenghtValue);
                break;
			}
		case 3:
			{
				m_modTrajectoryValue = Trajectory.tracking;
				m_modStrenghtValue = Random.Range(2, 6);
                m_penalty_frateMultiplier = 1 - (0.08f * m_modStrenghtValue);
                break;
			}
		case 4:
			{
				m_modTrajectoryValue = Trajectory.delayedTrack;
				m_modStrenghtValue = Random.Range(2, 6);
                m_penalty_frateMultiplier = 1 - (0.075f * m_modStrenghtValue);
                break;
			}
		case 5:
			{
				m_modTrajectoryValue = Trajectory.delayedSurge;
			    m_modStrenghtValue = Random.Range(2, 6);
				m_penalty_frateMultiplier = 1 - (0.03f * m_modStrenghtValue);
				break;
			}
		case 6:
			{
				m_modTrajectoryValue = Trajectory.helix;
				m_modStrenghtValue = Random.Range(2, 6);
				m_penalty_frateMultiplier = 1 - (0.045f * m_modStrenghtValue);
				break;
			}
         case 7:
            {
                m_modTrajectoryValue = Trajectory.arc;
                m_modStrenghtValue = Random.Range(1, 3) * 2;
                m_penalty_frateMultiplier = 1 - (0.04f * m_modStrenghtValue);
                break;
            }
        }
	}

    #region Getters
    public string GetModName()
    {
        string txt;
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    txt = "Trajectory: " + TrajectoryModName(m_modTrajectoryValue) + " " + m_modStrenghtValue;
                    break;
                }
            case ModType.splitMod:
                {
                    txt = "Split: " + TrajectoryModName(m_modTrajectoryValue) + " " + m_modStrenghtValue;
                    break;
                }
            case ModType.effectPhoton:
                {
                    txt = "Effect: Photon";
                    break;
                }
            case ModType.effectNuclear:
                {
                    txt = "Effect: Nuclear";
                    break;
                }
            case ModType.effectExplosive:
                {
                    txt = "Effect: Explosive";
                    break;
                }
            case ModType.effectEnergySteal:
                {
                    txt = "Effect: Energy Steal";
                    break;
                }
            case ModType.effectBouncing:
                {
                    txt = "Effect: Bouncing";
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
    public string GetModInfo_Description()
    {
        string txt;
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    txt = "Changes the projectile trajectory and the amount of bullets per shoot.";
                    break;
                }
            case ModType.splitMod:
                {
                    txt = "Adds a split effect to the projectile, when colliding with an enemy, or running out of bounces, each projectile will split into smaller fragments. Those fragments will have its damage and effectiveness reduced by 66%.";
                    break;
                }
            case ModType.effectPhoton:
                {
                    txt = "Shoots will deal aditional fire damage. Fire damage ignores deffense, but its damage is dealt over time instead of directly.";
                    break;
                }
            case ModType.effectNuclear:
                {
                    txt = "Shoots will deal aditional nuclear damage. Nuclear damage ignores deffense.";
                    break;
                }
            case ModType.effectExplosive:
                {
                    txt = "Shoots will explode dealing area of effect damage, this damage ignores deffenses and can not hit critically.";
                    break;
                }
            case ModType.effectEnergySteal:
                {
                    txt = "Shoots restore energy on impact, critical hits restore twice the amount.";
                    break;
                }
            case ModType.effectBouncing:
                {
                    txt = "Makes projectiles bounce of walls and enemies.";
                    break;
                }
            default:
                {
                    txt = "";
                    break;
                }
        }
        return txt;
    }
    public string GetModDescription_Penalty()
    {
        string txt;
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    if (m_penalty_effectivenessMultiplier < 1)
                    {
                        txt = "Weapon damage and effectiveness " + (int)((1 - m_penalty_effectivenessMultiplier) * -100) + "%.";
                    }
                    else
                    {
                        txt = "";
                    }
                    break;
                }
            case ModType.splitMod:
                {
                    txt = "Weapon firerate - " + (int)((1 - m_penalty_frateMultiplier) * 100) + " %.";
                    break;
                }
            default:
                {
                    txt = "";
                    break;
                }
        }
        return txt;
    }
    public string GetModDescription_Enhancement()
    {
        string txt;
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    txt = "Multishoot: " + m_modStrenghtValue + "\nTrajectory: " + TrajectoryModName(m_modTrajectoryValue);
                    if (m_penalty_effectivenessMultiplier > 1)
                    {
                        txt += "\nWeapon damage and effectiveness +" + (int)((1 - m_penalty_effectivenessMultiplier) * -100) + "%.";
                    }
                    break;
                }
            case ModType.splitMod:
                {
                    txt = "Bullets will split into " + m_modStrenghtValue + " fragments.\nFragments trajectory set to " + TrajectoryModName(m_modTrajectoryValue);
                    break;
                }
            case ModType.effectPhoton:
                {
                    txt = "+ 16 photon damage.";
                    break;
                }
            case ModType.effectNuclear:
                {
                    txt = "+ 13 nuclear damage.";
                    break;
                }
            case ModType.effectExplosive:
                {
                    txt = "+ 12 explosive damage.";
                    break;
                }
            case ModType.effectEnergySteal:
                {
                    txt = "+ 5 energy steal on hit.";
                    break;
                }
            case ModType.effectBouncing:
                {
                    txt = "+ 1 bounce.";
                    break;
                }
            default:
                {
                    txt = "";
                    break;
                }
        }
        return txt;
    }
    public string GetModDescription_Slot()
    {
        string txt = "";
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    txt = "Slot: Trajectory";
                    break;
                }
            case ModType.splitMod:
                {
                    txt = "Slot: Propagation";
                    break;
                }
            case ModType.effectPhoton:
                {
                    txt = "Slot: Offensive Upgrade";
                    break;
                }
            case ModType.effectNuclear:
                {
                    txt = "Slot: Offensive Upgrade";
                    break;
                }
            case ModType.effectExplosive:
                {
                    txt = "Slot: Offensive Upgrade";
                    break;
                }
            case ModType.effectEnergySteal:
                {
                    txt = "Slot: Offensive Upgrade";
                    break;
                }
            case ModType.effectBouncing:
                {
                    txt = "Slot: Offensive Upgrade";
                    break;
                }
            default:
                {
                    txt = "Slot: Any";
                    break;
                }
        }
        return txt;
    }
    public void ApplyModToWeapon(WeaponData wpn)
    {
        switch (m_currentType)
        {
            case ModType.trajectoryMod:
                {
                    wpn.weapon_trajectory = m_modTrajectoryValue;
                    wpn.weapon_multishoot = m_modStrenghtValue;
                    wpn.weapon_multishootSpread = m_modVariationValue;
                    wpn.weapon_firerateMultiplier *= m_penalty_frateMultiplier;
                    wpn.weapon_effectivenessMultiplier *= m_penalty_effectivenessMultiplier;
                    break;
                }
            case ModType.splitMod:
                {
                    wpn.weapon_trajectoryAfterSplit = m_modTrajectoryValue;
                    wpn.weapon_split = m_modStrenghtValue;
                    wpn.weapon_firerateMultiplier *= m_penalty_frateMultiplier;
                    break;
                }
            case ModType.effectPhoton:
                {
                    wpn.baseEffect_photon += 16;
                    break;
                }
            case ModType.effectNuclear:
                {
                    wpn.baseEffect_nuclear += 13;
                    break;
                }
            case ModType.effectExplosive:
                {
                    wpn.baseEffect_explosive += 8;
                    break;
                }
            case ModType.effectEnergySteal:
                {
                    wpn.baseEffect_energySteal += 5;
                    break;
                }
            case ModType.effectBouncing:
                {
                    wpn.weapon_bounces++;
                    break;
                }
            default:
                {
                    // Nada.
                    break;
                }
        }
        if (wpn.baseEffect_normal < 0)
            wpn.baseEffect_normal = 0;
    }
    #endregion

    #region Static functions

    // Conversor simple de Trajectory a String
    public static string TrajectoryModName(Trajectory arg)
    {
        string txt = "";
        switch (arg)
        {
            case Trajectory.normal:
                {
                    txt = "Normal";
                    break;
                }
            case Trajectory.arc:
                {
                    txt = "Arc";
                    break;
                }
            case Trajectory.coil:
                {
                    txt = "Coil";
                    break;
                }
            case Trajectory.delayedSurge:
                {
                    txt = "Delayed Surge";
                    break;
                }
            case Trajectory.delayedTrack:
                {
                    txt = "Delayed Tracking";
                    break;
                }
            case Trajectory.helix:
                {
                    txt = "Helix";
                    break;
                }
            case Trajectory.tracking:
                {
                    txt = "Tracking";
                    break;
                }
            case Trajectory.wave:
                {
                    txt = "Wave";
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
    public static ModType GetRandomEffectModifier()
    {
        int variation = Random.Range(0, 5);
        ModType randomEffect = ModType.none;
        switch (variation)
        {
            case 0:
                {
                    randomEffect = ModType.effectPhoton;
                    break;
                }
            case 1:
                {
                    randomEffect = ModType.effectNuclear;
                    break;
                }
            case 2:
                {
                    randomEffect = ModType.effectExplosive;
                    break;
                }
            case 3:
                {
                    randomEffect = ModType.effectEnergySteal;
                    break;
                }
            case 4:
                {
                    randomEffect = ModType.effectBouncing;
                    break;
                }
        }
        return randomEffect;
    }
    public static ModStatus GetRandomScaledSlotStatus(float scaling)
    {
        float randomFactor = Random.Range(0.33f, 1f) * scaling;
        if (randomFactor < 1.2f)
        {
            return ModStatus.locked;
        }
        else
        {
            return ModStatus.normal;
        }
    }
    #endregion

}
