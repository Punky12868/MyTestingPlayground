using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemPrefab;

    public void Interact()
    {
        if (FindObjectOfType<ExamplePlayerController>().HasItemOnHand())
        {
            Debug.Log("Already Has An Item On Hand!!");
        }
        else
        {
            GameObject item = Instantiate(itemPrefab, transform);
            FindObjectOfType<ExamplePlayerController>().HoldItem(item.gameObject.GetComponentInChildren<Item>());
        }

        Debug.Log("Interacting with ItemHolder");
    }
}
