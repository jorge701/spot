using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropEquipMenu : MonoBehaviour {

    private bool menuOpen = false;
    private bool draggingItem = false;




    [Header("Main Panel")]
    public CanvasGroup CG;
    public GameObject menuParent;

    [Header("Weapon Panel")]
    public List<InventoryIcon> weaponUpgradesInstalledIcons;
    public Text weaponInfo_type;
    public Text weaponInfo_baseDmg;
    public Text weaponInfo_photonDmg;
    public Text weaponInfo_nuclearDmg;
    public Text weaponInfo_explosiveDmg;

    [Header("Inventory Icons")]
    public GameObject inventoryIconPrefab;
    public List<InventoryIcon> inventoryIcons;
    public Transform inventoryContentParent;

    [Header("Dragged Icon")]
    public Image draggedIconImage;
    public Image draggedIconBorder;
    public CanvasGroup draggedIconCG;
    public InventoryIcon draggedIconReference;

    [Header("Tooltip")]
    public TooltipManager tooltip;

    private WeaponData weaponReaded;
    public static DragAndDropEquipMenu currentInstance;
    public UpgradeData lastUpgradeReadedByTooltip;


    private void Awake()
    {
        currentInstance = this;
    }
    void Update()
    {
        if (draggingItem)
            draggedIconCG.transform.position = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopAllCoroutines();
            if (menuOpen)
            {
                StartCoroutine("CloseMenu");
            }
            else
            {
                StartCoroutine("OpenMenu");
            }
        }
    }

    // Actualiza todos los iconos del inventario, llamado al abrir el menu.
    void UpdateAllIcons()
    {
        for (int i = 0; i < weaponUpgradesInstalledIcons.Count; i++)
        {
            if (i < weaponReaded.UpgradesInstalled.Count)
            {
                weaponUpgradesInstalledIcons[i].SetAs(weaponReaded.UpgradesInstalled, i, true);
            }
            else
            {
                weaponUpgradesInstalledIcons[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < inventoryIcons.Count; i++)
        {
            if (i < GlobalGameData.currentInstance.playerWeaponUpgradeInventory.Count)
            {
                inventoryIcons[i].SetAs(GlobalGameData.currentInstance.playerWeaponUpgradeInventory, i, false);
            }
            else
            {
                inventoryIcons[i].gameObject.SetActive(false);
            }

        }
    }

    #region Public Functions

    // Funciones publicas del menu, accedidas sobretodo desde InventoryIcon

    // Cambia el tooltip actual, llamada cuando el raton entra en un icono.
    public void SetNewTooltipItem(UpgradeData upg)
    {
        lastUpgradeReadedByTooltip = upg;
        if (upg == null)
            tooltip.SetVisible(false);
        else
        {
            tooltip.SetVisible(true);
            tooltip.SetNewInfo(upg.GetModName(), upg.GetModInfo_Description(), upg.GetModDescription_Enhancement(), upg.GetModDescription_Penalty(), upg.GetModDescription_Slot());
        }

    }

    // Desactiva el tooltip, llamada cuando el raton sale de un icono.
    public void RemoveTooltip()
    {
        tooltip.SetVisible(false);
    }

    // Inicia el arrastre de un icono, llamado al empezar a arrastrar un icono del inventario.
    public void StartDragging(InventoryIcon upg_ico)
    {
        draggedIconReference = upg_ico;
        draggedIconCG.alpha = 1;
        draggedIconImage.sprite = PrefabManager.currentInstance.GetSpriteIconForUpgrade(upg_ico.GetUpgradeReferenced());
        draggingItem = true;
    }
    // Termina el arrastre de un icono, llamado al dejar de arrastrar un icono y al cerrar el menu
    public void StopDragging()
    {
        draggedIconCG.alpha = 0;
        draggingItem = false;
        draggedIconReference = null;
        RemoveTooltip();
    }
    public void UpdateWeaponReaded()
    {
        weaponReaded.UpdateWeaponModReading();
        weaponInfo_type.text = "Type: " + weaponReaded.GetString_WeaponType();
        weaponInfo_baseDmg.text = "Normal: " + ((int)weaponReaded.GetNormalDamage()).ToString();
        weaponInfo_photonDmg.text = "Photon: " + ((int)weaponReaded.GetPhotonDamage()).ToString();
        weaponInfo_nuclearDmg.text = "Nuclear: " + ((int)weaponReaded.GetNuclearDamage()).ToString();
        weaponInfo_explosiveDmg.text = "Explosive: " + ((int)weaponReaded.GetExplosiveDamage()).ToString();
    }
    #endregion
    #region Co-Routines
    IEnumerator OpenMenu()
    {
        weaponReaded = GlobalGameData.currentInstance.playerWeaponTEST;
        UpdateAllIcons();
        UpdateWeaponReaded();
        menuParent.gameObject.SetActive(true);
        float animSpeed = 5f;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime * animSpeed;
            CG.alpha = t;
            yield return null;
        }
        menuOpen = true;
    }
    IEnumerator CloseMenu()
    {
        StopDragging();
        menuOpen = false;
        menuParent.gameObject.SetActive(true);
        float animSpeed = 5f;
        float t = 1f;
        while (t > 0)
        {
            t -= Time.deltaTime * animSpeed;
            CG.alpha = t;
            yield return null;
        }
        menuParent.gameObject.SetActive(false);
        RemoveTooltip();
    }
    #endregion
}
