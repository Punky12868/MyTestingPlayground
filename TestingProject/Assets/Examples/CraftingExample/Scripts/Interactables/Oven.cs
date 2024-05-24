using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform itemHolder;
    bool hasItem;
    float timer;

    private void Update()
    {
        if (hasItem)
        {
            Item item = itemHolder.gameObject.GetComponentInChildren<Item>();
            timer += Time.deltaTime;

            if (timer >= item.GetTimeToToast() && item.GetItemState() == Item.ItemState.Normal)
            {
                item.SetItemState(Item.ItemState.Toasted);
            }
            else if (timer >= item.GetTimeToBurn() && item.GetItemState() == Item.ItemState.Toasted)
            {
                item.SetItemState(Item.ItemState.Burnt);
            }
        }
        else if (timer > 0)
        {
            timer = 0;
        }
    }

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

        Debug.Log("Interacting with Oven");
    }

    private void SetItemTransform(Item item)
    {
        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        FindObjectOfType<ExamplePlayerController>().TransferItem();
    }
}
