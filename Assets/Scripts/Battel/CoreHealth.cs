using System;
using UnityEngine;

public class CoreHealth : MonoBehaviour
{
    [SerializeField] private BattleSide owner;
    [SerializeField] private int maxHp = 1000;
    [SerializeField] private int currentHp;

    public BattleSide Owner => owner;
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<BattleSide, int, int> OnHpChanged; 
    public event Action<BattleSide> OnCoreDestroyed;

    private bool isDestroyed;

    private void Awake()
    {
        maxHp = Mathf.Max(1, maxHp);
        currentHp = Mathf.Clamp(currentHp <= 0 ? maxHp : currentHp, 0, maxHp);
    }

    private void Start()
    {
        OnHpChanged?.Invoke(owner, currentHp, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHp = Mathf.Max(0, currentHp - damage);

        Debug.Log($"Core of {owner} took {damage} damage. HP: {currentHp}/{maxHp}");

        OnHpChanged?.Invoke(owner, currentHp, maxHp);

        if (currentHp <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            Debug.Log($"Core of {owner} destroyed!");
            OnCoreDestroyed?.Invoke(owner);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHp = Mathf.Min(maxHp, currentHp + amount);

        Debug.Log($"Core of {owner} healed {amount}. HP: {currentHp}/{maxHp}");

        OnHpChanged?.Invoke(owner, currentHp, maxHp);
    }
}
