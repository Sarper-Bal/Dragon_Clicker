using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    // --- DEĞİŞİKLİK BURADA ---
    // Bu değişkenler artık Inspector'da görünmeyecek ve sadece kod içinden atanabilecek.
    public int maxAttackPotential { get; private set; }
    public int maxShieldPotential { get; private set; }
    // --- DEĞİŞİKLİK SONU ---

    public int currentAttack { get; private set; }
    public int currentShield { get; private set; }
    public int currentCoins { get; private set; }
    public int dragonLevel { get; private set; }

    // Kayıt için kullanılacak anahtarlar
    private const string DRAGON_LEVEL_KEY = "PlayerDragonLevel";
    private const string MAX_ATTACK_KEY = "PlayerMaxAttack"; // Kaydetmek için yeni key
    private const string MAX_SHIELD_KEY = "PlayerMaxShield"; // Kaydetmek için yeni key
    private const string CURRENT_ATTACK_KEY = "PlayerCurrentAttack";
    private const string CURRENT_SHIELD_KEY = "PlayerCurrentShield";
    private const string CURRENT_COINS_KEY = "PlayerCurrentCoins";

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

    public void SetDragonLevel(int level, int newMaxAttack, int newMaxShield)
    {
        dragonLevel = level;
        maxAttackPotential = newMaxAttack;
        maxShieldPotential = newMaxShield;

        Debug.Log($"Ejderha {level}. seviye oldu! Yeni potansiyel: ATK {newMaxAttack}, SH {newMaxShield}");
        SaveData();
    }

    public void ChargeStats()
    {
        // Burası hala max potential değerlerini kullandığı için değişkenleri silmedik.
        currentAttack = Mathf.Min(currentAttack + 1, maxAttackPotential);
        currentShield = Mathf.Min(currentShield + 1, maxShieldPotential);
        SaveData();
    }
    public void SetStatsAfterBattle(int remainingShield)
    {
        if (remainingShield <= 0) { currentAttack = 0; currentShield = 0; }
        else { currentAttack = remainingShield; currentShield = remainingShield; }
        SaveData();
    }
    public void AddCoins(int amount)
    {
        if (amount < 0) return;
        currentCoins += amount;
        SaveData();
    }
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount) { currentCoins -= amount; SaveData(); return true; }
        return false;
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(CURRENT_ATTACK_KEY, currentAttack);
        PlayerPrefs.SetInt(CURRENT_SHIELD_KEY, currentShield);
        PlayerPrefs.SetInt(CURRENT_COINS_KEY, currentCoins);
        PlayerPrefs.SetInt(DRAGON_LEVEL_KEY, dragonLevel);

        // --- DEĞİŞİKLİK BURADA ---
        // Oyuncu oyundan çıktığında max potansiyelinin de kayıtlı kalması gerekir.
        PlayerPrefs.SetInt(MAX_ATTACK_KEY, maxAttackPotential);
        PlayerPrefs.SetInt(MAX_SHIELD_KEY, maxShieldPotential);
        // --- DEĞİŞİKLİK SONU ---

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentAttack = PlayerPrefs.GetInt(CURRENT_ATTACK_KEY, 0);
        currentShield = PlayerPrefs.GetInt(CURRENT_SHIELD_KEY, 0);
        currentCoins = PlayerPrefs.GetInt(CURRENT_COINS_KEY, 0);
        dragonLevel = PlayerPrefs.GetInt(DRAGON_LEVEL_KEY, 0);

        // --- DEĞİŞİKLİK BURADA ---
        // Kayıtlı max potansiyel değerlerini de yükle.
        // Eğer kayıt yoksa, başlangıç değeri olarak 500 ve 1000 kullan.
        maxAttackPotential = PlayerPrefs.GetInt(MAX_ATTACK_KEY, 500);
        maxShieldPotential = PlayerPrefs.GetInt(MAX_SHIELD_KEY, 1000);
        // --- DEĞİŞİKLİK SONU ---
    }
}