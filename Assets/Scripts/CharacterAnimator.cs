using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("Saldırı Animasyon Ayarları")]
    public float attackMoveDistance = 0.5f;
    public float attackScaleMultiplier = 1.2f;
    public float attackDuration = 0.4f;

    [Header("Ölüm Animasyon Ayarları")]
    [Tooltip("Ruhun ne kadar yukarı çıkacağı.")]
    public float soulRiseHeight = 2.0f;

    [Tooltip("Ruhun yükselme ve kaybolma süresi.")]
    public float soulAnimationDuration = 1.5f;

    [Tooltip("Bedenin kaybolma süresi.")]
    public float bodyFadeDuration = 2.0f;

    private SpriteRenderer bodyRenderer;
    private Vector3 originalPosition;
    private Vector3 originalScale;

    void Awake()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    public void PlayAttackAnimation(Transform target)
    {
        Vector3 direction = (target.position - originalPosition).normalized;
        Vector3 movePosition = originalPosition + direction * attackMoveDistance;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(movePosition, attackDuration / 2).SetEase(Ease.OutQuad));
        sequence.Join(transform.DOScale(originalScale * attackScaleMultiplier, attackDuration / 2).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOMove(originalPosition, attackDuration / 2).SetEase(Ease.InQuad));
        sequence.Join(transform.DOScale(originalScale, attackDuration / 2).SetEase(Ease.InQuad));
    }

    public void PlayTakeDamageAnimation()
    {
        if (bodyRenderer != null)
        {
            Sequence damageSequence = DOTween.Sequence();
            damageSequence.Append(bodyRenderer.DOColor(Color.red, 0.1f));
            damageSequence.Join(transform.DOShakePosition(0.2f, new Vector3(0.1f, 0, 0), 10, 0));
            damageSequence.Append(bodyRenderer.DOColor(Color.white, 0.1f));
        }
    }

    // --- YENİ, GÖRSELSİZ RUH ANİMASYONU ---
    public void PlayDeathAnimation()
    {
        // 1. Ruh Efektini Dinamik Olarak Yarat ve DOTween ile Animate Et
        CreateAndAnimateSoulEffect();

        // 2. Orijinal Bedeni DOTween ile Yok Et
        if (bodyRenderer != null)
        {
            bodyRenderer.DOFade(0, bodyFadeDuration).SetEase(Ease.InQuad);
        }
    }

    private void CreateAndAnimateSoulEffect()
    {
        // Geçici bir "ruh" objesi yarat
        GameObject soulGO = new GameObject("Soul Effect");
        // Pozisyonunu, rotasyonunu ve boyutunu bedenle aynı yap
        soulGO.transform.position = transform.position;
        soulGO.transform.rotation = transform.rotation;
        soulGO.transform.localScale = transform.localScale;

        // Bedenin Sprite'ını kopyalamak için bir SpriteRenderer ekle
        SpriteRenderer soulRenderer = soulGO.AddComponent<SpriteRenderer>();
        soulRenderer.sprite = bodyRenderer.sprite; // O anki sprite'ı kopyala
        soulRenderer.color = new Color(1, 1, 1, 0.7f); // Yarı saydam beyaz yap
        soulRenderer.sortingOrder = bodyRenderer.sortingOrder + 1; // Her zaman bedenin önünde olmasını sağla

        // Ruh animasyonunu tamamen DOTween ile başlat
        Sequence soulSequence = DOTween.Sequence();
        // Yukarı hareket et
        soulSequence.Append(soulGO.transform.DOMoveY(transform.position.y + soulRiseHeight, soulAnimationDuration).SetEase(Ease.OutSine));
        // Yükselirken aynı anda kaybol
        soulSequence.Join(soulRenderer.DOFade(0, soulAnimationDuration).SetEase(Ease.InSine));
        // Animasyon bittiğinde geçici objeyi sahneden temizle
        soulSequence.OnComplete(() => Destroy(soulGO));
    }
    // --- YENİ Fonksiyon Sonu ---

    void OnDestroy()
    {
        transform.DOKill();
    }
}

