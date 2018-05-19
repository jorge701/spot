using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameData : MonoBehaviour {

    public WeaponData playerWeaponTEST;

    public List<UpgradeData> playerWeaponUpgradeInventory;
    private const int BASE_INVENTORY_SIZE = 100;

    public static GlobalGameData currentInstance;

    private void Awake()
    {
        playerWeaponTEST = new WeaponData(WeaponData.WeaponPresets.randomScaledPlayer, 3);
        currentInstance = this;
        playerWeaponUpgradeInventory = new List<UpgradeData>();
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.trajectoryMod));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.trajectoryMod));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.splitMod));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectEnergySteal));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectEnergySteal));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectExplosive));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectExplosive));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectNuclear));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectNuclear));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectPhoton));
        playerWeaponUpgradeInventory.Add(new UpgradeData(UpgradeData.ModType.effectPhoton));
        for (int i = 0; i < BASE_INVENTORY_SIZE - playerWeaponUpgradeInventory.Count; i++)
        {
            playerWeaponUpgradeInventory.Add(null);
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
