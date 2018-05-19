using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {

    // Implementamos IPointerEnterHandler, y IPointerExitHandler para detectar cuando el raton entra o sale de un icono.
    // Implementamos IBeginDragHandler, IDragHandler, y IEndDragHandler para detectar cuando un icono se esta arrastrando.
    // Implementamos IDropHandler para detectar cuando "soltamos" un icono sobre otro.
    // IMPORTANTE: Esta clase esta pensada para funcionar junto a DragAndDropEquipMenu, ya que esta actua como manager, y contiene parametros estaticos de los que esta clase hace uso.

    public CanvasGroup CG;                                      // CanvasGroup del icono
    public Image iconBorder;                                    // Borde del icono
    public Image iconImage;                                     // Imagen principal del icono

    private List<UpgradeData> listReferenced;                   // Lista en la que se encuentra la mejora representada por el icono.
    private int indexReferenced = 0;                            // Indice de la mejora representada por el icono en la lista a la que pertenece.
    private bool isWeaponSlot;                                  // Se trata de una ranura de arma? si es de inventario, false.

    // Prepara el icono con los datos indicados, automaticamente llama a UpdateIcon para ajustar su icono/color de borde dependiendo de la mejora que contenga.
    public void SetAs(List<UpgradeData> listRef, int indexRef, bool isWpn)
    {
        indexReferenced = indexRef;
        listReferenced = listRef;
        isWeaponSlot = isWpn;
        UpdateIcon();
    }

    // Actualiza el icono, dependiendo del tipo de mejora que contenga
    // TO-DO: demomento solo cambia el color entre dos para testear, hay que añadir iconos.
    void UpdateIcon()
    {
        if (listReferenced[indexReferenced] == null)
        {
            iconImage.sprite = PrefabManager.currentInstance.blankSprite;
            iconImage.color = Color.gray;
            return;
        }
        iconImage.color = Color.white;
        iconBorder.color = Color.black;
        iconImage.sprite = PrefabManager.currentInstance.GetSpriteIconForUpgrade(listReferenced[indexReferenced]);
    }

    #region Interface Implementations

    /* Implementacion de los interfaces que utilizara esta clase, estos interfaces vienen de EventSystems, ayudan a controlar acciones como arrastrar, hacer click, y detectar
    cuando el raton esta sobre un objeto, o cuando sale de el. Aunque no haga nada el codigo con OnDrag, debe ser implementado y dejado aunque sea vacio, para que los demas funcionen.*/

    // Implementacion de OnPointerEnter
    public void OnPointerEnter(PointerEventData eventData)
    {
        DragAndDropEquipMenu.currentInstance.SetNewTooltipItem(listReferenced[indexReferenced]);
    }

    // Implementacion de OnPointerExit
    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragAndDropEquipMenu.currentInstance.lastUpgradeReadedByTooltip == listReferenced[indexReferenced])
            DragAndDropEquipMenu.currentInstance.RemoveTooltip();
    }

    // Implementacion de OnBeginDrag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (listReferenced[indexReferenced] == null)
            return;
        CG.alpha = 0.5f;
        DragAndDropEquipMenu.currentInstance.StartDragging(this);
    }

    // Implementacion de OnEndDrag
    public void OnEndDrag(PointerEventData eventData)
    {
        CG.alpha = 1;
        DragAndDropEquipMenu.currentInstance.StopDragging();
    }

    // Implementacion necesaria de OnDrag.
    public void OnDrag(PointerEventData eventData)
    {
        // Se necesita implementar esto, aunque no haga nada para que funcionen los interfaces OnBeginDrag y OnEndDrag(???)
    }

    // Intercambia los items: objetoArrastrado -> objetoSobreElQueSeArrastra, y viceversa.
    public void OnDrop(PointerEventData eventData)
    {
        if (DragAndDropEquipMenu.currentInstance.draggedIconReference == this || DragAndDropEquipMenu.currentInstance.draggedIconReference == null)
            return;

        // Para facilidad de lectura lo separamos en variables auxiliares.
        bool otherIsWpn = DragAndDropEquipMenu.currentInstance.draggedIconReference.IsWeaponSlot();
        bool thisIsWpn = isWeaponSlot;
        int otherIndex = DragAndDropEquipMenu.currentInstance.draggedIconReference.GetIndexReferenced();
        int thisIndex = indexReferenced;
        List<UpgradeData> otherList = DragAndDropEquipMenu.currentInstance.draggedIconReference.GetListReferenced();
        List<UpgradeData> thisList = listReferenced;
        UpgradeData otherVar = DragAndDropEquipMenu.currentInstance.draggedIconReference.GetUpgradeReferenced();
        UpgradeData thisVar = listReferenced[indexReferenced];

        otherList[otherIndex] = thisVar;
        thisList[thisIndex] = otherVar;

        UpdateIcon();
        DragAndDropEquipMenu.currentInstance.draggedIconReference.UpdateIcon();

        if (thisIsWpn || otherIsWpn)
        {
            DragAndDropEquipMenu.currentInstance.UpdateWeaponReaded();
        }
    }

    #endregion

    #region Getters

    // Devuelve la lista que en la que esta el objeto que representa este icono.
    public List<UpgradeData> GetListReferenced()
    {
        return listReferenced;
    }
    // Devuelve la mejora a la que representa este icono.
    public UpgradeData GetUpgradeReferenced()
    {
        return listReferenced[indexReferenced];
    }
    // Devuelve el indice del objeto que representa este icono en la lista a la que pertenece
    /* NOTA: Deberia ser innecesario, ya que listReferenced.IndexOf(upgradeReferenced) devuelve este mismo valor, pero hay una excepcion,
       ya que una ranura de inventario puede estar vacia y contener null, y IndexOf(null) no tiene ningun sentido. */ 
    public int GetIndexReferenced()
    {
        return indexReferenced;
    }
    // Representa a una ranura de un arma?
    public bool IsWeaponSlot()
    {
        return isWeaponSlot;
    }

    #endregion
}
