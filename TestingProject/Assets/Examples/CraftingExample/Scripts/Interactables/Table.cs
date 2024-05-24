using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform itemHolder;
    bool hasItem;

    public void Interact()
    {
        if (hasItem && !FindObjectOfType<ExamplePlayerController>().HasItemOnHand())
        {
            FindObjectOfType<ExamplePlayerController>().HoldItem(itemHolder.gameObject.GetComponentInChildren<Item>());
            hasItem = false;
        }
        else if (!hasItem && FindObjectOfType<ExamplePlayerController>().HasItemOnHand())
        {
            SetItemTransform(FindObjectOfType<ExamplePlayerController>().GetCurrentItemHeld());
            FindObjectOfType<ExamplePlayerController>().TransferItem();
            hasItem = true;
        }

        Debug.Log("Interacting with Table");
    }

    private void SetItemTransform(Item item)
    {
        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        FindObjectOfType<ExamplePlayerController>().TransferItem();
    }
}