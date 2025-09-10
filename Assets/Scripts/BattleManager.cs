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
    public float attackAnimDuration = 0.4f; // Saldırı animasyonlarının toplam süresi
    public float turnDelay = 1.0f; // Her çatışma arasındaki bekleme süresi

    private BattleCharacter player;
    private BattleCharacter enemy;

    void Start()
    {
        resultPanel.SetActive(false);
        SetupBattle();
    }

    void SetupBattle()
    {
        var playerData = PlayerDataManager.Instance;
        GameObject playerGO = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        player = playerGO.AddComponent<BattleCharacter>();
        playerGO.AddComponent<CharacterAnimator>();
        player.Setup(playerData.currentAttack, playerData.currentShield);

        EnemyData enemyData = currentLevel.enemiesInLevel[0];
        GameObject enemyGO = new GameObject("Enemy");
        enemyGO.transform.position = enemySpawnPoint.position;
        SpriteRenderer sr = enemyGO.AddComponent<SpriteRenderer>();
        sr.sprite = enemyData.enemySprite;
        sr.sortingOrder = 5;
        enemyGO.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        enemy = enemyGO.AddComponent<BattleCharacter>();
        enemyGO.AddComponent<CharacterAnimator>();
        enemy.Setup(enemyData.attack, enemyData.shield);

        UpdateUI();
        StartCoroutine(BattleRoutine());
    }

    IEnumerator BattleRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (player.currentShield > 0 && enemy.currentShield > 0)
        {
            int playerDamage = player.currentAttack;
            int enemyDamage = enemy.currentAttack;

            player.Animator.PlayAttackAnimation(enemy.transform);
            enemy.Animator.PlayAttackAnimation(player.transform);
            Camera.main.DOShakePosition(0.15f, 0.4f);

            yield return new WaitForSeconds(attackAnimDuration);

            player.TakeDamage(enemyDamage);
            enemy.TakeDamage(playerDamage);
            UpdateUI();

            yield return new WaitForSeconds(turnDelay);
        }

        EndBattle();
    }

    void EndBattle()
    {
        PlayerDataManager.Instance.SetStatsAfterBattle(player.currentShield);

        // Ödül verilecek düşmanın verisini al
        EnemyData defeatedEnemyData = currentLevel.enemiesInLevel[0];

        if (player.currentShield <= 0 && enemy.currentShield <= 0)
        {
            resultText.text = "Berabere!";
            player.Animator.PlayDeathAnimation();
            enemy.Animator.PlayDeathAnimation();

            // Berabere kalınca ödülün yarısını ver
            int coinAmount = Mathf.CeilToInt(defeatedEnemyData.coinReward / 2f);
            PlayerDataManager.Instance.AddCoins(coinAmount);
        }
        else if (player.currentShield <= 0)
        {
            resultText.text = "Kaybettin!";
            player.Animator.PlayDeathAnimation();
            // Kaybedince coin verilmez.
        }
        else
        {
            resultText.text = "Kazandın!";
            enemy.Animator.PlayDeathAnimation();

            // Kazanınca ödülün tamamını ver
            PlayerDataManager.Instance.AddCoins(defeatedEnemyData.coinReward);
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
        DOTween.KillAll();
        SceneManager.LoadScene("SampleScene");
    }
}