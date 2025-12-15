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

    private ItemInstance itemCurrentlySelected;

    public static Inventory instance;

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    private ItemInstance equipedHeadItem;
    private ItemInstance equipedChestItem;
    private ItemInstance equipedLegsItem;
    private ItemInstance equipedFeetsItem;
    private ItemInstance equipedWeaponItem;


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

    private Player player;

    [SerializeField] public List<ItemInstance> inventoryContent = new List<ItemInstance>();
    private Dictionary<string, RangeWeaponRuntimeData> rangeWeaponRuntime = new Dictionary<string, RangeWeaponRuntimeData>();
    private Dictionary<string, UltimateMeleeWeaponRuntimeData> ultimateMeleeWeaponRuntime = new Dictionary<string, UltimateMeleeWeaponRuntimeData>();

    public RangeWeaponRuntimeData GetRangeWeaponRuntime(ItemInstance instance)
    {
        if (instance == null)
        {
            Debug.LogError("GetWeaponRuntime: instance is null!");
            return null;
        }

        if (!rangeWeaponRuntime.ContainsKey(instance.uniqueID))
        {
            if (instance.data.itemPrefab == null)
            {
                Debug.LogError("GetWeaponRuntime: itemPrefab is null for " + instance.data.name);
                return null;
            }

            RangeWeapon rw = instance.data.itemPrefab.GetComponent<RangeWeapon>();
            int ammo = rw != null ? rw.maxAmmo : 0;
            float lastSoundTime = rw != null ? rw.GetLastSoundTime() : 0f;
            int ammoInMagazine = rw != null ? rw.getAmmoInMagazine() : 0;
            int magazineSize = rw.magazineSize;
            float lastShotTime = rw.GetLastShotTime();
            rangeWeaponRuntime[instance.uniqueID] = new RangeWeaponRuntimeData(instance, ammo, lastSoundTime, ammoInMagazine, magazineSize, lastShotTime);
        }

        return rangeWeaponRuntime[instance.uniqueID];
    }

    public UltimateMeleeWeaponRuntimeData GetUltimateMeleeWeaponRuntime(ItemInstance instance)
    {
        if (instance == null)
        {
            Debug.LogError("GetWeaponRuntime: instance is null!");
            return null;
        }

        if (!ultimateMeleeWeaponRuntime.ContainsKey(instance.uniqueID))
        {
            if (instance.data.itemPrefab == null)
            {
                Debug.LogError("GetWeaponRuntime: itemPrefab is null for " + instance.data.name);
                return null;
            }

            UltimateMeleeWeapon umw = instance.data.itemPrefab.GetComponent<UltimateMeleeWeapon>();
            float maxRightClickDistance = umw.maxRightClickDistance;
            float rightClickCooldown = umw.rightClickCooldown;
            float nextRightClickTime = umw.GetNextRightClickTime();
            LayerMask canRightClickMask = umw.canRightClickMask;
            ultimateMeleeWeaponRuntime[instance.uniqueID] = new UltimateMeleeWeaponRuntimeData(instance, maxRightClickDistance, rightClickCooldown, nextRightClickTime, canRightClickMask);
        }

        return ultimateMeleeWeaponRuntime[instance.uniqueID];
    }
    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
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


    // ajoute un nouvel item en créant une instance
    public void AddItem(ItemData data)
    {
        if (IsFull()) return;
        ItemInstance instance = new ItemInstance(data);
        inventoryContent.Add(instance);
        RefreshContent();
    }

    // ajoute une instance existante (utile pour le déséquipement)
    public void AddItem(ItemInstance instance)
    {
        if (IsFull() || instance == null) return;
        inventoryContent.Add(instance);
        RefreshContent();
    }


    public void RemoveItem(ItemInstance instance)
    {
        inventoryContent.Remove(instance);
        RefreshContent();
    }

    public bool IsFull()
    {
        return inventorySize == inventoryContent.Count;
    }

    private void RefreshContent()
    {
        // vide les slots
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = null; // slot contient un ItemInstance
            currentSlot.itemVisual.sprite = emptySlotVisual;
        }

        // on remplit les slots avec les items de l'inventaire
        for (int i = 0; i < inventoryContent.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.item = inventoryContent[i]; // ItemInstance
            currentSlot.itemVisual.sprite = inventoryContent[i].data.itemSprite; // sprite venant de l'ItemData
        }

        // Mise à jour des boutons de déséquipement
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

    public void OpenActionPanel(ItemInstance item, Vector3 panelPosition)
    {

        itemCurrentlySelected = item;

        if (item == null)
        {
            actionPanel.SetActive(false);
            return;
        }
           
        switch (item.data.itemType)
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
        Consumable consumable = itemCurrentlySelected.data.itemPrefab.GetComponent<Consumable>();
        if (consumable != null)
        {
            consumable.Consume(player);
            inventoryContent.Remove(itemCurrentlySelected);
        }
        RefreshContent();
        CloseActionPanel();
    }

    public void DropActionButton()
    {
        if (itemCurrentlySelected == null)
            return;

        GameObject instantiatedItem = Instantiate(itemCurrentlySelected.data.itemPrefab);
        instantiatedItem.transform.position = dropPoint.position;

        if (itemCurrentlySelected.data.itemType == ItemType.Equipment &&
            itemCurrentlySelected.data.EquipmentSlot == EquipmentSlot.Hands)
        {
            if (rangeWeaponRuntime.ContainsKey(itemCurrentlySelected.uniqueID))
            {
                rangeWeaponRuntime.Remove(itemCurrentlySelected.uniqueID);
            }

            if (ultimateMeleeWeaponRuntime.ContainsKey(itemCurrentlySelected.uniqueID))
            {
                ultimateMeleeWeaponRuntime.Remove(itemCurrentlySelected.uniqueID);
            }
        }

        inventoryContent.Remove(itemCurrentlySelected);

        RefreshContent();
        CloseActionPanel();
    }


    public void EquipActionButton()
    {
        // trouve le prefab correspondant dans la librairie
        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content
            .Where(elem => elem.itemData == itemCurrentlySelected.data)
            .FirstOrDefault();

        if (equipmentLibraryItem == null)
        {
            Debug.Log("Item introuvable dans la library : " + itemCurrentlySelected.data.name);
            return;
        }

        DisablePreviousEquipedEquipment(itemCurrentlySelected);

        switch (itemCurrentlySelected.data.EquipmentSlot)
        {
            case EquipmentSlot.Head:
                headSlotImage.sprite = itemCurrentlySelected.data.itemSprite;
                equipedHeadItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Chest:
                chestSlotImage.sprite = itemCurrentlySelected.data.itemSprite;
                equipedChestItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Legs:
                legsSlotImage.sprite = itemCurrentlySelected.data.itemSprite;
                equipedLegsItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Feet:
                feetsSlotImage.sprite = itemCurrentlySelected.data.itemSprite;
                equipedFeetsItem = itemCurrentlySelected;
                break;
            case EquipmentSlot.Hands:
                weaponSlotImage.sprite = itemCurrentlySelected.data.itemSprite;
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
        headSlotDesequipButton.gameObject.SetActive(equipedHeadItem != null);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Chest); });
        chestSlotDesequipButton.gameObject.SetActive(equipedChestItem != null);

        legsSlotDesequipButton.onClick.RemoveAllListeners();
        legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Legs); });
        legsSlotDesequipButton.gameObject.SetActive(equipedLegsItem != null);

        feetsSlotDesequipButton.onClick.RemoveAllListeners();
        feetsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Feet); });
        feetsSlotDesequipButton.gameObject.SetActive(equipedFeetsItem != null);

        weaponSlotDesequipButton.onClick.RemoveAllListeners();
        weaponSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentSlot.Hands); });
        weaponSlotDesequipButton.gameObject.SetActive(equipedWeaponItem != null);
    }

    public void DesequipEquipment(EquipmentSlot equipmentType)
    {
        if (IsFull()) return;

        // pour les armes, on garde l'instance, pour les autres on peut juste utiliser ItemData
        ItemInstance instanceToReturn = null;
        ItemData dataToReturn = null;

        switch (equipmentType)
        {
            case EquipmentSlot.Head:
                if (equipedHeadItem != null)
                {
                    dataToReturn = equipedHeadItem.data;
                    EquipmentManager.instance.Unequip(EquipmentSlot.Head);
                    equipedHeadItem = null;
                    headSlotImage.sprite = emptySlotVisual;
                }
                break;

            case EquipmentSlot.Chest:
                if (equipedChestItem != null)
                {
                    dataToReturn = equipedChestItem.data;
                    EquipmentManager.instance.Unequip(EquipmentSlot.Chest);
                    equipedChestItem = null;
                    chestSlotImage.sprite = emptySlotVisual;
                }
                break;

            case EquipmentSlot.Legs:
                if (equipedLegsItem != null)
                {
                    dataToReturn = equipedLegsItem.data;
                    EquipmentManager.instance.Unequip(EquipmentSlot.Legs);
                    equipedLegsItem = null;
                    legsSlotImage.sprite = emptySlotVisual;
                }
                break;

            case EquipmentSlot.Feet:
                if (equipedFeetsItem != null)
                {
                    dataToReturn = equipedFeetsItem.data;
                    EquipmentManager.instance.Unequip(EquipmentSlot.Feet);
                    equipedFeetsItem = null;
                    feetsSlotImage.sprite = emptySlotVisual;
                }
                break;

            case EquipmentSlot.Hands:
                if (equipedWeaponItem != null)
                {
                    instanceToReturn = equipedWeaponItem; // on garde l'instance complète pour le runtime
                    EquipmentManager.instance.Unequip(EquipmentSlot.Hands);
                    equipedWeaponItem = null;
                    weaponSlotImage.sprite = emptySlotVisual;
                }
                break;
        }

        // on remet dans l'inventaire
        if (instanceToReturn != null)
        {
            AddItem(instanceToReturn); // ajoute l'instance complète
        }
        else if (dataToReturn != null)
        {
            AddItem(dataToReturn); // crée une nouvelle instance pour les équipements classiques
        }

        RefreshContent();
    }



    private void DisablePreviousEquipedEquipment(ItemInstance itemToDisable)
    {
        if (itemToDisable == null) return;

        EquipmentSlot slot = itemToDisable.data.EquipmentSlot;
        ItemInstance previousItem = null;

        switch (slot)
        {
            case EquipmentSlot.Head:
                previousItem = equipedHeadItem;
                break;
            case EquipmentSlot.Chest:
                previousItem = equipedChestItem;
                break;
            case EquipmentSlot.Legs:
                previousItem = equipedLegsItem;
                break;
            case EquipmentSlot.Feet:
                previousItem = equipedFeetsItem;
                break;
            case EquipmentSlot.Hands:
                previousItem = equipedWeaponItem;
                break;
        }

        if (previousItem != null)
        {
            EquipmentManager.instance.Unequip(slot);
            AddItem(previousItem);
        }

        switch (slot)
        {
            case EquipmentSlot.Head:
                equipedHeadItem = null;
                headSlotImage.sprite = emptySlotVisual;
                break;
            case EquipmentSlot.Chest:
                equipedChestItem = null;
                chestSlotImage.sprite = emptySlotVisual;
                break;
            case EquipmentSlot.Legs:
                equipedLegsItem = null;
                legsSlotImage.sprite = emptySlotVisual;
                break;
            case EquipmentSlot.Feet:
                equipedFeetsItem = null;
                feetsSlotImage.sprite = emptySlotVisual;
                break;
            case EquipmentSlot.Hands:
                equipedWeaponItem = null;
                weaponSlotImage.sprite = emptySlotVisual;
                break;
        }

        RefreshContent();
    }



    public bool IsOpen()
    {
        return isOpen;
    }

    public ItemData getEquippedHelmet()
    {
        return equipedHeadItem.data;
    }

    public ItemData getEquippedChest()
    {
        return equipedChestItem.data;
    }

    public ItemData getEquippedLeggings()
    {
        return equipedLegsItem.data;
    }

    public ItemData getEquippedBoots()
    {
        return equipedFeetsItem.data;
    }


}
