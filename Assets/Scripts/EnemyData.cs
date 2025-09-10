using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Dragon Clicker/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public int attack = 10;
    public int shield = 50;

    [Header("Visuals")]
    public Sprite enemySprite;

    [Header("Rewards")]
    public int coinReward = 25; // Bu düşman yenildiğinde verilecek altın miktarı
}