using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

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

    private ItemData equipedHeadItem;
    private ItemData equipedChestItem;
    private ItemData equipedLegsItem;
    private ItemData equipedFeetsItem;
    private ItemData equipedWeaponItem;

    [SerializeField]
    private Button headSlotDesequipButton;

    [SerializeField]
    private Button chestSlotDesequipButton;

    [SerializeField]
    private Button legsSlotDesequipButton;

    [SerializeField]
    private Button feetsSlotDesequipButton;

    [SerializeField]
    private Button weaponSlotDesequipButton;

    [Header("Equipment Panel References")]
    [SerializeField]
    private Image headSlotImage;

    [SerializeField]
    private Image chestSlotImage;

    [SerializeField]
    private Image legsSlotImage;

    [SerializeField]
    private Image feetsSlotImage;

    [SerializeField]
    private Image weaponSlotImage;

    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseInventory();
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


        UpdateEquipmentDesequipButton();
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

        DisablePreviousEquipedEquipment(itemCurrentlySelected);

        switch (itemCurrentlySelected.EquipmenttSlot)
        {
            case EquipmentSlot.Head:
                headSlotImage.sprite = itemCurrentlySelected.itemSprite;
                equipedHeadItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Chest:
                chestSlotImage.sprite = itemCurrentlySelected.itemSprite;
                equipedChestItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Legs:
                legsSlotImage.sprite = itemCurrentlySelected.itemSprite;
                equipedLegsItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Feet:
                feetsSlotImage.sprite = itemCurrentlySelected.itemSprite;
                equipedFeetsItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Hands:
                weaponSlotImage.sprite = itemCurrentlySelected.itemSprite;
                equipedWeaponItem = itemCurrentlySelected;
                break;
        }

        EquipmentManager.instance.Equip(
            itemCurrentlySelected,
            equipmentLibraryItem.itemPrefab
        );
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

    private void UpdateEquipmentDesequipButton()
    {
        headSlotDesequipButton.onClick.RemoveAllListeners();
        headSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Head); });
        headSlotDesequipButton.gameObject.SetActive(equipedHeadItem);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Chest); });
        chestSlotDesequipButton.gameObject.SetActive(equipedChestItem);

        legsSlotDesequipButton.onClick.RemoveAllListeners();
        legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Legs); });
        legsSlotDesequipButton.gameObject.SetActive(equipedLegsItem);

        feetsSlotDesequipButton.onClick.RemoveAllListeners();
        feetsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Feet); });
        feetsSlotDesequipButton.gameObject.SetActive(equipedFeetsItem);

        weaponSlotDesequipButton.onClick.RemoveAllListeners();
        weaponSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Hands); });
        weaponSlotDesequipButton.gameObject.SetActive(equipedWeaponItem);
    }

    public void DesequipEquipment(EquipmentSlot equipmentType)
    {
        if (IsFull()) return;

        ItemData currentItem = null;

        switch (equipmentType)
        {
            case EquipmentSlot.Head:
                currentItem = equipedHeadItem;
                if (currentItem != null)
                    EquipmentManager.instance.Unequip(EquipmentSlot.Head);
                equipedHeadItem = null;
                headSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Chest:
                currentItem = equipedChestItem;
                if (currentItem != null)
                    EquipmentManager.instance.Unequip(EquipmentSlot.Chest);
                equipedChestItem = null;
                chestSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Legs:
                currentItem = equipedLegsItem;
                if (currentItem != null)
                    EquipmentManager.instance.Unequip(EquipmentSlot.Legs);
                equipedLegsItem = null;
                legsSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Feet:
                currentItem = equipedFeetsItem;
                if (currentItem != null)
                    EquipmentManager.instance.Unequip(EquipmentSlot.Feet);
                equipedFeetsItem = null;
                feetsSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Hands:
                currentItem = equipedWeaponItem;
                equipedWeaponItem = null;
                weaponSlotImage.sprite = emptySlotVisual;
                break;
        }

        if (currentItem == null)
        {
            return;
        }
        AddItem(currentItem);
        RefreshContent();
    }


    private void DisablePreviousEquipedEquipment(ItemData itemToDisable)
    {
        if (itemToDisable == null) return;

        ItemData previousItem = null;
        EquipmentSlot slot = itemToDisable.EquipmenttSlot;

        switch (slot)
        {
            case EquipmentSlot.Head:
                previousItem = equipedHeadItem;
                equipedHeadItem = null;
                headSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Chest:
                previousItem = equipedChestItem;
                equipedChestItem = null;
                chestSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Legs:
                previousItem = equipedLegsItem;
                equipedLegsItem = null;
                legsSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Feet:
                previousItem = equipedFeetsItem;
                equipedFeetsItem = null;
                feetsSlotImage.sprite = emptySlotVisual;
                break;

            case EquipmentSlot.Hands:
                previousItem = equipedWeaponItem;
                equipedWeaponItem = null;
                weaponSlotImage.sprite = emptySlotVisual;
                break;
        }

        if (previousItem != null)
        {
            AddItem(previousItem); 
            EquipmentManager.instance.Unequip(slot);
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public ItemData getEquippedHelmet()
    {
        return equipedHeadItem;
    }

    public ItemData getEquippedChest()
    {
        return equipedChestItem;
    }

    public ItemData getEquippedLeggings()
    {
        return equipedLegsItem;
    }

    public ItemData getEquippedBoots()
    {
        return equipedFeetsItem;
    }


}
