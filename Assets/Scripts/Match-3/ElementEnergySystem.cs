using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class ElementEnergyAmount
{
    public ElementType element;
    public int amount;
}

public class ElementEnergySystem : MonoBehaviour
{
    [SerializeField] public List<ElementEnergyAmount> energies = new();

    public event Action<ElementType, int> OnEnergyChanged;

    private void Awake()
    {
        EnsureAllElementsExist();
    }

    private void OnValidate()
    {
        EnsureAllElementsExist();
    }

    public int GetEnergy(ElementType element)
    {
        ElementEnergyAmount energyData = FindEnergyData(element);
        return energyData != null ? energyData.amount : 0;
    }

    public void AddEnergy(ElementType element, int amount)
    {
        if (amount <= 0) return;

        ElementEnergyAmount energyData = FindEnergyData(element);

        if(energyData == null)
        {
            energyData = new ElementEnergyAmount
            {
                element = element,
                amount = 0
            };

            energies.Add(energyData);
        }

        energyData.amount += amount;

        Debug.Log($"Alpha +{amount} {element} energy. Total: {energyData.amount}");

        OnEnergyChanged?.Invoke(element, energyData.amount);
    }

    public bool CanSpend(ElementType element, int amount)
    {
        return GetEnergy(element) >= amount;
    }

    public bool SpendEnergy(ElementType element, int amount)
    {
        if (amount <= 0) return true;

        ElementEnergyAmount energyData = FindEnergyData(element);

        if(energyData == null || energyData.amount < amount)
        {
            Debug.LogWarning($"Khong du EC {element} energy to spend. Required: {amount}, Available: {GetEnergy(element)}");
            return false;
        }

        energyData.amount -= amount;

        Debug.Log($"Alpha -{amount} {element} energy. Total: {energyData.amount}");

        OnEnergyChanged?.Invoke(element, energyData.amount);
        return true;
    }

    private ElementEnergyAmount FindEnergyData(ElementType element)
    {
        foreach (ElementEnergyAmount energyData in energies)
        {
            if (energyData.element.Equals(element))
                return energyData;
        }

        return null;
    }

    private void EnsureAllElementsExist()
    {
        foreach (ElementType element in Enum.GetValues(typeof(ElementType)))
        {
            bool exists = false;

            foreach(ElementEnergyAmount energyData in energies)
            {
                exists = true;
                break;
            }

            if (!exists)
            {
                energies.Add(new ElementEnergyAmount
                {
                    element = element,
                    amount = 0
                });
            }
        }
    }
}
