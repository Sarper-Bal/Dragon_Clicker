using UnityEngine;
using DG.Tweening;

public class BattleCharacter : MonoBehaviour
{
    // Artık iki ayrı stat var
    public int currentAttack;
    public int currentShield;

    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    // Kurulum fonksiyonu yeni statlara göre güncellendi
    public void Setup(int attack, int shield)
    {
        this.currentAttack = attack;
        this.currentShield = shield;
        this.originalPosition = transform.position;
        // SpriteRenderer'ı almayı deneyelim, null ise hata vermesin
        if (!TryGetComponent<SpriteRenderer>(out this.spriteRenderer))
        {
            // Eğer ejderha prefabının altında bir child objede sprite varsa
            // bu kodu ona göre düzenlemek gerekebilir. Şimdilik ana objede arıyoruz.
            Debug.LogWarning($"{gameObject.name} objesinde SpriteRenderer bulunamadı.");
        }
    }

    // Hasar alma artık sadece shield'ı etkiliyor
    public void TakeDamage(int damage)
    {
        currentShield -= damage;
        if (currentShield < 0) currentShield = 0;

        if (spriteRenderer != null)
        {
            spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => spriteRenderer.DOColor(Color.white, 0.1f));
        }
    }

    public void PlayAttackAnimation(Transform target)
    {
        Sequence attackSequence = DOTween.Sequence();
        attackSequence.Append(transform.DOMove(target.position, 0.25f).SetEase(Ease.OutQuad));
        attackSequence.AppendCallback(() => Camera.main.DOShakePosition(0.15f, 0.5f));
        attackSequence.Append(transform.DOMove(originalPosition, 0.25f).SetEase(Ease.InQuad));
    }

    public void PlayDeathAnimation()
    {
        transform.DORotate(new Vector3(0, 0, 45), 0.5f);
        transform.DOMove(transform.position + new Vector3(-2, -2, 0), 0.5f).SetRelative();
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0, 0.5f);
        }
    }
}