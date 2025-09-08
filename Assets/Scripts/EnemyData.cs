using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "Dragon Clicker/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public string enemyName;
    public Sprite enemySprite;

    // "Power" yerine artık bu iki değer var.
    public int attack;
    public int shield;
}