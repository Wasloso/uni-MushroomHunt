using SoundManager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SoundManager.SoundManager.PlayAmbient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
