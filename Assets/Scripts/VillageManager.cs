using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [Header("Seviye Köy Grupları")]
    [Tooltip("1. seviyede aktif olacak tüm binaları içeren ana obje")]
    public GameObject level1VillageGroup;

    [Tooltip("2. seviyede aktif olacak tüm binaları içeren ana obje")]
    public GameObject level2VillageGroup;

    // Yeni seviyeler için buraya public GameObject level3VillageGroup; ekleyebilirsiniz.

    void Awake()
    {
        // Oyun başlarken tüm köy gruplarını gizleyelim ki sadece GameManager'ın
        // istediği görünsün.
        if (level1VillageGroup != null) level1VillageGroup.SetActive(false);
        if (level2VillageGroup != null) level2VillageGroup.SetActive(false);
    }

    /// <summary>
    /// Belirtilen seviyeye ait köy grubunu görünür yapar, diğerlerini gizler.
    /// </summary>
    /// <param name="level">Gösterilecek seviye numarası</param>
    public void UpdateVillageVisuals(int level)
    {
        // Önce hepsini tekrar bir kapatalım.
        if (level1VillageGroup != null) level1VillageGroup.SetActive(false);
        if (level2VillageGroup != null) level2VillageGroup.SetActive(false);

        // İstenen seviyeyi görünür yapalım.
        switch (level)
        {
            case 1:
                if (level1VillageGroup != null)
                {
                    level1VillageGroup.SetActive(true);
                    Debug.Log("Seviye 1 köyü aktifleştirildi.");
                }
                break;
            case 2:
                if (level2VillageGroup != null)
                {
                    level2VillageGroup.SetActive(true);
                    Debug.Log("Seviye 2 köyü aktifleştirildi.");
                }
                break;
            // Yeni seviyeler için buraya 'case 3:' vb. eklenecek.
            default:
                // Hiçbiri değilse, hiçbir şey gösterme.
                Debug.Log("Geçerli bir köy seviyesi bulunamadı, tümü gizli kalacak.");
                break;
        }
    }
}