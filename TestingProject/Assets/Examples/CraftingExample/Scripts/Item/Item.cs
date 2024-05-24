using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName;

    [SerializeField] private float timeToToast = 5f;
    [SerializeField] private float timeToBurn = 10f;

    public enum ItemState { Normal, Toasted, Burnt }
    [SerializeField] private ItemState itemState;

    // Se pueden cambiar por distintos sprites y cambiar el sprite del objeto si es que tiene un SpriteRenderer
    [SerializeField] private Material itemNormalMaterial;
    [SerializeField] private Material itemToastedMaterial;
    [SerializeField] private Material itemBurntMaterial;

    public string GetItemName()
    {
        return itemName;
    }

    public ItemState GetItemState()
    {
        return itemState;
    }

    public void SetItemState(ItemState newState)
    {
        itemState = newState;
        SetItemMaterial(itemState);
    }

    public void SetItemMaterial(ItemState itemState)
    {
        switch (itemState)
        {
            case ItemState.Normal:
                GetComponent<Renderer>().material = itemNormalMaterial;
                break;
            case ItemState.Toasted:
                GetComponent<Renderer>().material = itemToastedMaterial;
                break;
            case ItemState.Burnt:
                GetComponent<Renderer>().material = itemBurntMaterial;
                break;
        }
    }

    public float GetTimeToToast()
    {
        return timeToToast;
    }

    public float GetTimeToBurn()
    {
        return timeToBurn;
    }

    public void Interact()
    {
        FindObjectOfType<ExamplePlayerController>().HoldItem(this);

        Debug.Log("Interacting with Item");
    }
}
