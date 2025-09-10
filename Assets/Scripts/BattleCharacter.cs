using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
public class BattleCharacter : MonoBehaviour
{
    // Stat'lar
    public int currentAttack;
    public int currentShield;

    // Animasyonları yönetecek olan script'e referans
    public CharacterAnimator Animator { get; private set; }

    void Awake()
    {
        // Animator bileşenini al
        Animator = GetComponent<CharacterAnimator>();
    }

    /// <summary>
    /// Karakterin başlangıç stat'larını ayarlar.
    /// </summary>
    public void Setup(int attack, int shield)
    {
        this.currentAttack = attack;
        this.currentShield = shield;
    }

    /// <summary>
    /// Hasar alır ve hasar animasyonunu tetikler.
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentShield -= damage;
        if (currentShield < 0) currentShield = 0;

        // Hasar animasyonunu oynatması için Animator'e komut ver
        Animator.PlayTakeDamageAnimation();
    }
}
