using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodTextScript : MonoBehaviour
{
    private TextMeshProUGUI foodText;
    // Start is called before the first frame update
    void Start()
    {
        foodText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateFoodText(PlayerInventoryScript playerInventory)
    {
        foodText.text = playerInventory.NumberOfSushi.ToString();
    }


}
