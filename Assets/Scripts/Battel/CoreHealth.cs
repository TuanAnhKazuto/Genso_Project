using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

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

    private void Awake()
    {
        currentHp = maxHp;
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

        if(currentHp < 0)
        {
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