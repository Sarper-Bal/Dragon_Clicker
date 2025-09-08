using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Dragon's Max Potential")]
    // Bu değerleri Unity editöründen ayarlayacağız.
    public int maxAttackPotential = 500;
    public int maxShieldPotential = 1000;

    // Ejderhanın o anki, savaşa hazır olan değerleri.
    public int currentAttack { get; private set; }
    public int currentShield { get; private set; }

    // Kayıt için kullanılacak anahtarlar
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

    /// <summary>
    /// Ana sahnede tıklandığında hem attack hem de shield'ı 1 artırır.
    /// </summary>
    public void ChargeStats()
    {
        // Statları artır, ama potansiyel limitlerini geçemezler.
        currentAttack = Mathf.Min(currentAttack + 1, maxAttackPotential);
        currentShield = Mathf.Min(currentShield + 1, maxShieldPotential);
        SaveData();
    }

    /// <summary>
    /// Savaşın sonucuna göre statları günceller.
    /// </summary>
    public void SetStatsAfterBattle(int remainingShield)
    {
        if (remainingShield <= 0)
        {
            // Eğer ejderha öldüyse, her şeyi sıfırla.
            currentAttack = 0;
            currentShield = 0;
        }
        else
        {
            // Eğer kazandıysa, kalan kalkan değerini hem attack hem de shield'a ata (Seçenek A).
            currentAttack = remainingShield;
            currentShield = remainingShield;
        }
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(CURRENT_ATTACK_KEY, currentAttack);
        PlayerPrefs.SetInt(CURRENT_SHIELD_KEY, currentShield);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        // Oyunu ilk açtığında veya savaştan döndüğünde en son kayıtlı değerleri yükler.
        // Başlangıçta 0 olacaklar.
        currentAttack = PlayerPrefs.GetInt(CURRENT_ATTACK_KEY, 0);
        currentShield = PlayerPrefs.GetInt(CURRENT_SHIELD_KEY, 0);
    }
}