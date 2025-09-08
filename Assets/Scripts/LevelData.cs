using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Dragon Clicker/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public string levelName; // Levelin adı (örn: Orman Girişi)

    [Header("Enemies")]
    // Bu levelde hangi düşmanların, hangi sırayla geleceğini belirten bir liste.
    public EnemyData[] enemiesInLevel;
}