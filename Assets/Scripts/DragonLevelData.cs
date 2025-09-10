using UnityEngine;

[CreateAssetMenu(fileName = "DragonLevel_1", menuName = "Dragon Clicker/Dragon Level Data")]
public class DragonLevelData : ScriptableObject
{
    [Header("Seviye Bilgileri")]
    public int levelNumber = 1;

    [Header("Görsel")]
    // Hatanın olduğu satır buydu, isim artık doğru.
    public GameObject dragonPrefab; // Bu seviyeye ait ejderha prefab'ı

    [Header("Stat Gelişimi")]
    // Bu seviyeye ulaşıldığında oyuncunun maksimum potansiyeli bu değerlere set edilecek.
    public int newMaxAttackPotential = 500;
    public int newMaxShieldPotential = 1000;
}