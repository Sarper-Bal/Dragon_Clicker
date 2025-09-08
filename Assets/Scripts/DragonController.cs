using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement; // Sahne adını kontrol etmek için bu satır gerekli!

public class DragonController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float scaleFactor = 1.1f;
    public float scaleDuration = 0.15f;

    private Vector3 originalScale;
    private bool isAnimating = false;
    private Camera mainCamera;

    void Start()
    {
        originalScale = transform.localScale;
        mainCamera = Camera.main;
    }

    void Update()
    {
        // --- YENİ KONTROL ---
        // Eğer mevcut sahne "BattleScene" ise, bu script'in hiçbir işlevi çalışmasın.
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            return; // Fonksiyonun geri kalanını çalıştırmadan çık.
        }
        // --- KONTROL SONU ---

        if (isAnimating || Touchscreen.current == null || !Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return;
        }

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            var dataManager = PlayerDataManager.Instance;

            // Ejderhanın statları maksimum potansiyele ulaştı mı diye kontrol et
            if (dataManager.currentAttack < dataManager.maxAttackPotential || dataManager.currentShield < dataManager.maxShieldPotential)
            {
                // Ulaşmadıysa, statları şarj et.
                dataManager.ChargeStats();
                Debug.Log($"Ejderha hazırlanıyor! ATK: {dataManager.currentAttack}/{dataManager.maxAttackPotential} - SH: {dataManager.currentShield}/{dataManager.maxShieldPotential}");
            }
            else
            {
                // Zaten tam güçteyse, mesaj ver.
                Debug.Log("Ejderha savaşa tamamen hazır!");
            }

            PlayClickAnimation();
        }
    }

    void PlayClickAnimation()
    {
        isAnimating = true;
        transform.DOScale(originalScale * scaleFactor, scaleDuration / 2).OnComplete(() =>
        {
            transform.DOScale(originalScale, scaleDuration / 2).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    // Bu obje yok edilmeden önce DOTween animasyonlarını durdurur.
    void OnDestroy()
    {
        transform.DOKill();
    }
}