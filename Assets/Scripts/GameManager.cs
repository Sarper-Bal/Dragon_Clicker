using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening; // DOTween komutları için bu satır gerekli!

public class GameManager : MonoBehaviour
{
    [Header("Sahne Objeleri")]
    public GameObject initialSprite;    // Başlangıçtaki yumurta/obje
    public GameObject dragonPrefab;     // Ejderha prefabımız
    public Transform dragonSpawnPoint;  // Ejderhanın duracağı yer (boş bir obje)
    public GameObject battleButton;     // Savaş butonu

    [Header("Animasyon Ayarları")]
    public float launchHeight = 5f;
    public float animationDuration = 1f;

    // Oyuncunun ejderhası olup olmadığını kontrol eden kayıt anahtarı
    private const string HAS_DRAGON_KEY = "PlayerHasDragon";

    void Start()
    {
        // PlayerDataManager'ın var olduğundan emin ol
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("Sahnede PlayerDataManager objesi bulunamadı!");
            return;
        }

        // Oyuncunun daha önceden bir ejderhası var mı diye kontrol et
        if (PlayerPrefs.GetInt(HAS_DRAGON_KEY, 0) == 1)
        {
            // Eğer varsa, ejderhayı direkt olarak göster
            ShowDragon();
        }
        else
        {
            // Eğer yoksa (ilk oynanış), ejderhayı doğurma sürecini başlat
            PrepareForFirstSpawn();
        }
    }

    // Bu fonksiyon sadece ilk oynanışta çalışır
    void PrepareForFirstSpawn()
    {
        initialSprite.SetActive(true);
        if (battleButton != null) battleButton.SetActive(false); // Ejderha yokken savaş butonu olmasın
        // Ejderhayı getirmek için tıklama bekleniyor...
    }

    // Bu fonksiyon, ejderhası olmayan oyuncu ekrana tıkladığında çalışır
    void Update()
    {
        // Eğer oyuncunun ejderhası zaten varsa veya başlangıç objesi aktif değilse, bu fonksiyon çalışmasın
        if (PlayerPrefs.GetInt(HAS_DRAGON_KEY, 0) == 1 || !initialSprite.activeInHierarchy)
        {
            return;
        }

        // Ekrana tıklandı mı?
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            // Tıklama algılandığında bu script'in Update'ini devre dışı bırak ki tekrar çalışmasın
            this.enabled = false;

            // Yumurtayı uçur ve ejderhayı doğur
            initialSprite.transform.DOMoveY(initialSprite.transform.position.y + launchHeight, animationDuration).OnComplete(() =>
            {
                initialSprite.SetActive(false);
                PlayerPrefs.SetInt(HAS_DRAGON_KEY, 1); // Artık bir ejderhası olduğunu kaydet
                PlayerPrefs.Save();
                ShowDragon(); // Ejderhayı göster
            });
        }
    }

    // Bu fonksiyon, ejderhayı sahnede görünür yapar
    void ShowDragon()
    {
        if (initialSprite != null) initialSprite.SetActive(false);
        Instantiate(dragonPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation);
        if (battleButton != null) battleButton.SetActive(true); // Ejderha geldi, savaş butonu görünsün
    }

    // Bu fonksiyonu Unity'deki butona bağlayacağız
    public void GoToBattle()
    {
        // --- YENİ GÜVENLİK KOMUTU ---
        // Sahne değişmeden hemen önce, çalışan TÜM DOTween animasyonlarını anında öldür.
        DOTween.KillAll();

        // Sahneyi yükle
        SceneManager.LoadScene("BattleScene");
    }

    // --- Test için Yardımcı Fonksiyon ---
    // Oyunu sıfırlamak istersen bu fonksiyonu çağıran geçici bir buton yapabilirsin
    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Tüm kayıtlı veriler silindi!");
    }
}