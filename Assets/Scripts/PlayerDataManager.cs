using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Dragon's Max Potential")]
    public int maxAttackPotential = 500;
    public int maxShieldPotential = 1000;

    // Ejderhanın o anki, savaşa hazır olan değerleri.
    public int currentAttack { get; private set; }
    public int currentShield { get; private set; }

    // --- YENİ EKLENENLER BAŞLANGIÇ ---
    public int currentCoins { get; private set; } // Oyuncunun mevcut altını

    // Kayıt için kullanılacak anahtarlar
    private const string CURRENT_COINS_KEY = "PlayerCurrentCoins";
    // --- YENİ EKLENENLER BİTİŞ ---

    private const string CURRENT_ATTACK_KEY = "PlayerCurrentAttack";
    private const string CURRENT_SHIELD_KEY = "PlayerCurrentShield";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChargeStats()
    {
        currentAttack = Mathf.Min(currentAttack + 1, maxAttackPotential);
        currentShield = Mathf.Min(currentShield + 1, maxShieldPotential);
        SaveData();
    }

    public void SetStatsAfterBattle(int remainingShield)
    {
        if (remainingShield <= 0)
        {
            currentAttack = 0;
            currentShield = 0;
        }
        else
        {
            currentAttack = remainingShield;
            currentShield = remainingShield;
        }
        SaveData();
    }

    // --- YENİ EKLENENLER BAŞLANGIÇ ---
    /// <summary>
    /// Oyuncunun altın hesabına belirtilen miktarda altın ekler.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount < 0) return; // Negatif değer eklemeyi engelle
        currentCoins += amount;
        SaveData();
        Debug.Log($"{amount} altın eklendi. Toplam altın: {currentCoins}");
    }

    /// <summary>
    /// Oyuncunun altın hesabından belirtilen miktarda altın harcar.
    /// Yeterli altın yoksa false döner.
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (amount < 0) return false; // Negatif değer harcamayı engelle

        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveData();
            Debug.Log($"{amount} altın harcandı. Kalan altın: {currentCoins}");
            return true;
        }

        Debug.Log("Yetersiz altın!");
        return false;
    }
    // --- YENİ EKLENENLER BİTİŞ ---

    private void SaveData()
    {
        PlayerPrefs.SetInt(CURRENT_ATTACK_KEY, currentAttack);
        PlayerPrefs.SetInt(CURRENT_SHIELD_KEY, currentShield);
        PlayerPrefs.SetInt(CURRENT_COINS_KEY, currentCoins); // Altını kaydet
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentAttack = PlayerPrefs.GetInt(CURRENT_ATTACK_KEY, 0);
        currentShield = PlayerPrefs.GetInt(CURRENT_SHIELD_KEY, 0);
        currentCoins = PlayerPrefs.GetInt(CURRENT_COINS_KEY, 0); // Altını yükle (başlangıçta 0)
    }
}