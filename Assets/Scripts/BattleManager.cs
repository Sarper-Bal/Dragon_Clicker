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
        // --- Oyuncu Kurulumu ---
        var playerData = PlayerDataManager.Instance;
        GameObject playerGO = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        // ÖNEMLİ: Artık iki script'i de ekliyoruz
        player = playerGO.AddComponent<BattleCharacter>();
        playerGO.AddComponent<CharacterAnimator>(); // Animator'ü de ekle
        player.Setup(playerData.currentAttack, playerData.currentShield);

        // --- Düşman Kurulumu ---
        EnemyData enemyData = currentLevel.enemiesInLevel[0];
        GameObject enemyGO = new GameObject("Enemy");
        enemyGO.transform.position = enemySpawnPoint.position;
        SpriteRenderer sr = enemyGO.AddComponent<SpriteRenderer>();
        sr.sprite = enemyData.enemySprite;
        sr.sortingOrder = 5;
        enemyGO.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        // ÖNEMLİ: Artık iki script'i de ekliyoruz
        enemy = enemyGO.AddComponent<BattleCharacter>();
        enemyGO.AddComponent<CharacterAnimator>(); // Animator'ü de ekle
        enemy.Setup(enemyData.attack, enemyData.shield);

        UpdateUI();
        StartCoroutine(BattleRoutine());
    }

    // --- YENİ EŞ ZAMANLI SAVAŞ RUTİNİ ---
    IEnumerator BattleRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (player.currentShield > 0 && enemy.currentShield > 0)
        {
            // O anki hasar değerlerini önceden al
            int playerDamage = player.currentAttack;
            int enemyDamage = enemy.currentAttack;

            // 1. Her iki karakterin saldırı animasyonunu AYNI ANDA başlat
            player.Animator.PlayAttackAnimation(enemy.transform);
            enemy.Animator.PlayAttackAnimation(player.transform);
            Camera.main.DOShakePosition(0.15f, 0.4f); // Kamera tek sefer sallansın

            // Animasyonların bitmesini bekle
            yield return new WaitForSeconds(attackAnimDuration);

            // 2. Her iki karaktere de hasarı AYNI ANDA uygula
            player.TakeDamage(enemyDamage);
            enemy.TakeDamage(playerDamage);
            UpdateUI();

            // Çatışmalar arası bekleme
            yield return new WaitForSeconds(turnDelay);
        }

        EndBattle();
    }

    void EndBattle()
    {
        PlayerDataManager.Instance.SetStatsAfterBattle(player.currentShield);

        // Not: Ölüm animasyonları artık Animator üzerinden çağrılıyor
        if (player.currentShield <= 0 && enemy.currentShield <= 0)
        {
            resultText.text = "Berabere!";
            player.Animator.PlayDeathAnimation();
            enemy.Animator.PlayDeathAnimation();
        }
        else if (player.currentShield <= 0)
        {
            resultText.text = "Kaybettin!";
            player.Animator.PlayDeathAnimation();
        }
        else
        {
            resultText.text = "Kazandın!";
            enemy.Animator.PlayDeathAnimation();
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
        // Sahne değiştirmeden önce tüm DOTween animasyonlarını durdurmak iyi bir pratiktir.
        DOTween.KillAll();
        SceneManager.LoadScene("SampleScene");
    }
}
