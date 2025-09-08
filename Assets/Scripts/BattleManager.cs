using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    [Header("Level Bilgisi")]
    public LevelData currentLevel;

    [Header("Sahne Objeleri")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public GameObject playerPrefab;

    [Header("UI Elemanları")]
    public TextMeshProUGUI playerStatsText;
    public TextMeshProUGUI enemyStatsText;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    [Header("Savaş Ayarları")]
    public float attackInterval = 1.5f;

    private BattleCharacter player;
    private BattleCharacter enemy;

    void Start()
    {
        resultPanel.SetActive(false);
        SetupBattle();
    }

    void SetupBattle()
    {
        // Oyuncu verilerini PlayerDataManager'dan çek
        var playerData = PlayerDataManager.Instance;
        GameObject playerGO = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        player = playerGO.AddComponent<BattleCharacter>();
        player.Setup(playerData.currentAttack, playerData.currentShield);

        // Düşman verilerini LevelData'dan çek
        EnemyData enemyData = currentLevel.enemiesInLevel[0];
        GameObject enemyGO = new GameObject("Enemy");
        enemyGO.transform.position = enemySpawnPoint.position;
        SpriteRenderer sr = enemyGO.AddComponent<SpriteRenderer>();
        sr.sprite = enemyData.enemySprite;
        sr.sortingOrder = 5;
        enemyGO.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        enemy = enemyGO.AddComponent<BattleCharacter>();
        enemy.Setup(enemyData.attack, enemyData.shield);

        UpdateUI();
        StartCoroutine(BattleRoutine());
    }

    IEnumerator BattleRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        // Her iki karakterin de kalkanı 0'dan büyük olduğu sürece döngü devam eder
        while (player.currentShield > 0 && enemy.currentShield > 0)
        {
            // O anki attack değerlerini hasar olarak belirle
            int playerDamage = player.currentAttack;
            int enemyDamage = enemy.currentAttack;

            player.PlayAttackAnimation(enemy.transform);
            enemy.PlayAttackAnimation(player.transform);

            yield return new WaitForSeconds(0.6f);

            // Hasarı uygula (sadece shield'a)
            player.TakeDamage(enemyDamage);
            enemy.TakeDamage(playerDamage);

            UpdateUI();

            yield return new WaitForSeconds(attackInterval - 0.6f);
        }

        EndBattle();
    }

    void EndBattle()
    {
        // Savaş bittiğinde, PlayerDataManager'ı ejderhanın son durumuyla güncelle
        PlayerDataManager.Instance.SetStatsAfterBattle(player.currentShield);

        if (player.currentShield <= 0 && enemy.currentShield <= 0)
        {
            resultText.text = "Berabere!";
            player.PlayDeathAnimation();
            enemy.PlayDeathAnimation();
        }
        else if (player.currentShield <= 0)
        {
            resultText.text = "Kaybettin!";
            player.PlayDeathAnimation();
        }
        else
        {
            resultText.text = "Kazandın!";
            enemy.PlayDeathAnimation();
        }

        resultPanel.SetActive(true);
    }

    void UpdateUI()
    {
        playerStatsText.text = $"ATK: {player.currentAttack}\nSH: {player.currentShield}";
        enemyStatsText.text = $"ATK: {enemy.currentAttack}\nSH: {enemy.currentShield}";
    }

    public void GoToHomeScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}