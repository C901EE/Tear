using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryScript : MonoBehaviour
{

    public int NumberOfSushi { get; private set; }

    public UnityEvent<PlayerInventoryScript> OnSushiColledted;

    public void AddSushi()
    {
        NumberOfSushi++;
        OnSushiColledted.Invoke(this);
    }
}
