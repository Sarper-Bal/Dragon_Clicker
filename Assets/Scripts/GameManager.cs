using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Seviye Ayarları")]
    public DragonLevelData level1Data;

    [Header("Sahne Objeleri")]
    public GameObject initialSprite;
    public Transform dragonSpawnPoint;
    public GameObject battleButton;
    public VillageManager villageManager;

    [Header("Animasyon Ayarları")]
    public float launchHeight = 5f;
    public float animationDuration = 1f;

    private const string HAS_DRAGON_KEY = "PlayerHasDragon";
    private GameObject currentDragonInstance;

    void Start()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("Sahnede PlayerDataManager objesi bulunamadı!");
            return;
        }

        if (PlayerPrefs.GetInt(HAS_DRAGON_KEY, 0) == 1)
        {
            ShowDragon();
        }
        else
        {
            PrepareForFirstSpawn();
        }
    }

    void PrepareForFirstSpawn()
    {
        initialSprite.SetActive(true);
        if (villageManager != null) villageManager.UpdateVillageVisuals(0);
        if (battleButton != null) battleButton.SetActive(false);
    }

    void Update()
    {
        if (PlayerPrefs.GetInt(HAS_DRAGON_KEY, 0) == 1 || !initialSprite.activeInHierarchy)
        {
            return;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            this.enabled = false;

            initialSprite.transform.DOMoveY(initialSprite.transform.position.y + launchHeight, animationDuration).OnComplete(() =>
            {
                initialSprite.SetActive(false);
                PlayerPrefs.SetInt(HAS_DRAGON_KEY, 1);
                PlayerPrefs.Save();

                PlayerDataManager.Instance.SetDragonLevel(
                    level1Data.levelNumber,
                    level1Data.newMaxAttackPotential,
                    level1Data.newMaxShieldPotential
                );

                ShowDragon();
            });
        }
    }

    void ShowDragon()
    {
        if (initialSprite != null) initialSprite.SetActive(false);
        if (battleButton != null) battleButton.SetActive(true);

        int currentLevel = PlayerDataManager.Instance.dragonLevel;

        if (currentLevel < 1) currentLevel = 1;

        if (level1Data != null && currentLevel == 1)
        {
            if (currentDragonInstance != null)
            {
                Destroy(currentDragonInstance);
            }

            // Burası artık hatasız çalışmalı.
            currentDragonInstance = Instantiate(level1Data.dragonPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation);
        }

        if (villageManager != null)
        {
            villageManager.UpdateVillageVisuals(currentLevel);
        }
    }

    public void GoToBattle()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("BattleScene");
    }

    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Tüm kayıtlı veriler silindi!");
    }
}