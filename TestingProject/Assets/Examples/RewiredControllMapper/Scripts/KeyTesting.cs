using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class KeyTesting : MonoBehaviour
{
    Player player;
    [SerializeField] float speed = 5f;
    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        float horizontal = player.GetAxisRaw("Horizontal");
        float vertical = player.GetAxisRaw("Vertical");

        transform.position += new Vector3(horizontal, vertical, 0) * Time.deltaTime * speed;

        if (player.GetButtonDown("Attack"))
        {
            Debug.Log("Attack");
        }

        if (player.GetButtonDown("Roll"))
        {
            Debug.Log("Roll");
        }

        if (player.GetButtonDown("Parry"))
        {
            Debug.Log("Parry");
        }

        if (player.GetButtonDown("Use Ability"))
        {
            Debug.Log("Use Ability");
        }

        if (player.GetButtonDown("Change Ability"))
        {
            Debug.Log("Change Ability");
        }

        if (player.GetButtonDown("Use Item"))
        {
            Debug.Log("Use Item");
        }

        if (player.GetButtonDown("Drop Item"))
        {
            Debug.Log("Drop Item");
        }

        if (player.GetButtonDown("Pause"))
        {
            FindObjectOfType<CurrentController>().Pause();
            Debug.Log("Pause");
        }
    }
}
