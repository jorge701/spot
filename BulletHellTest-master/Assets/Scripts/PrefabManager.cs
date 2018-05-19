using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabManager : MonoBehaviour {

    public Sprite blankSprite;
    public List<Sprite> MenuIcons;

    public static PrefabManager currentInstance;

    private void Awake()
    {
        currentInstance = this;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public Sprite GetSpriteIconForUpgrade(UpgradeData upg)
    {
        Sprite spr;
        switch(upg.m_currentType)
        {
            case UpgradeData.ModType.trajectoryMod:
            {
                spr = MenuIcons[6];
                break;
            }
            case UpgradeData.ModType.splitMod:
            {
                spr = MenuIcons[5];
                break;
            }
            case UpgradeData.ModType.effectBouncing:
            {
                spr = MenuIcons[4];
                break;
            }
            case UpgradeData.ModType.effectPhoton:
            {
                spr = MenuIcons[0];
                break;
            }
            case UpgradeData.ModType.effectNuclear:
            {
                spr = MenuIcons[1];
                break;
            }
            case UpgradeData.ModType.effectExplosive:
            {
                spr = MenuIcons[2];
                break;
            }
            case UpgradeData.ModType.effectEnergySteal:
            {
                spr = MenuIcons[3];
                break;
            }
            default:
            {
                spr = blankSprite;
                break;
            }
        }
        return spr;
    }
}
