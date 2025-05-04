using TMPro;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    private int mushroomsCollected = 0;

    public TextMeshProUGUI mushroomCountText;

    public void AddMushroom()
    {
        mushroomsCollected++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        mushroomCountText.text = mushroomsCollected.ToString();
    }
}