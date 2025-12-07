using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Inventory : MonoBehaviour
{

    [SerializeField]
    private List<ItemData> inventoryContent = new List<ItemData>();

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;

    const int inventorySize = 24;

    private bool isOpen = false;

    [Header("Action Panel References")]
    [SerializeField]
    private GameObject actionPanel;

    [SerializeField]
    private GameObject useItemButton;

    [SerializeField]
    private GameObject equipItemButton;

    [SerializeField]
    private GameObject dropItemButton;

    [SerializeField]
    private GameObject destroyItemButton;

    [SerializeField]
    private Sprite emptySlotVisual;

    [SerializeField]
    private Transform dropPoint;

    private ItemData itemCurrentlySelected;

    public static Inventory instance;

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;


    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshContent();
    }

    // Update is called once per frame
    private void Update()
    {
  
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen)
            {
                CloseInventory();
            } else
            {
                OpenInventory();
            }
        }
    }

    public void AddItem(ItemData item)
    {
        if (IsFull()) return;
        inventoryContent.Add(item);
        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        inventoryContent.Remove(item);
        RefreshContent();
    }

    private bool IsFull()
    {
        return inventorySize == inventoryContent.Count;
    }

    private void RefreshContent()
    {

        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
        }


        for (int i = 0; i < inventoryContent.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = inventoryContent[i];
            currentSlot.itemVisual.sprite = inventoryContent[i].itemSprite;
        }
    }

    private void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        isOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseInventory()
    {
        TooltipSystem.instance.Hide();
        inventoryPanel.SetActive(false);
        CloseActionPanel();
        isOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenActionPanel(ItemData item, Vector3 panelPosition)
    {

        itemCurrentlySelected = item;

        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }
           
        switch (item.itemType)
        {
            case ItemType.Consumable:
                useItemButton.SetActive(true);
                equipItemButton.SetActive(false);
                break;
            case ItemType.Equipment:
                useItemButton.SetActive(false);
                equipItemButton.SetActive(true);
                break;
        }

        actionPanel.transform.position = panelPosition;
        actionPanel.SetActive(true);
    }

    public void CloseActionPanel()
    {
        actionPanel.SetActive(false);
        itemCurrentlySelected = null;
    }

    public void UseActionButton()
    {
        RefreshContent();
        CloseActionPanel();
    }

    public void DropActionButton()
    {
        GameObject instantiatedItem = Instantiate(itemCurrentlySelected.itemPrefab);
        instantiatedItem.transform.position = dropPoint.position;
        inventoryContent.Remove(itemCurrentlySelected);
        RefreshContent();
        CloseActionPanel();
    }

    public void EquipActionButton()
    {
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content
            .Where(elem => elem.itemData == itemCurrentlySelected)
            .FirstOrDefault();

        if (equipmentLibraryItem == null)
        {
            Debug.Log("Item introuvable dans la library : " + itemCurrentlySelected.name);
            return;
        }

        EquipmentManager.instance.Equip(
            itemCurrentlySelected,
            equipmentLibraryItem.itemPrefab
        );

        Debug.Log("equipage de : " + itemCurrentlySelected.name);

        inventoryContent.Remove(itemCurrentlySelected); // optionnel
        RefreshContent();
        CloseActionPanel();
    }

    public void DestroyActionButton()
    {
        inventoryContent.Remove(itemCurrentlySelected);
        RefreshContent();
        CloseActionPanel();
    }
}
