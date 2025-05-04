using UnityEngine;
namespace World
{

    [CreateAssetMenu(fileName = "NewMushroomData", menuName = "Mushrooms/Mushroom Data")]
    public class MushroomData : ScriptableObject
    {
        public string displayName;
        public string description;
        public Color glowColor;
        public float scoreValue;
        public bool isPoisonous;
        public float spawnWeight = 1f;
        
        
    }
}