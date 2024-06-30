using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventoryScript playerInventory = other.GetComponent<PlayerInventoryScript>();

        if(playerInventory != null)
        {
            playerInventory.AddSushi();
            gameObject.SetActive(false);
        }
    }
}
