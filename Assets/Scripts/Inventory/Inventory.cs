using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    [SerializeField]
    private List<ItemData> inventoryContent = new List<ItemData>();

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public void AddItem(ItemData item)
    {
        inventoryContent.Add(item);
        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        inventoryContent.Remove(item);
        RefreshContent();
    }

    private void RefreshContent()
    {
        for (int i = 0; i < inventoryContent.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();
            currentSlot.itemVisual.sprite = inventoryContent[i].itemSprite;
        }
    }
}
