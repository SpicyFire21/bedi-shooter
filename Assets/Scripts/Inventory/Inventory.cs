using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

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
        isOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
