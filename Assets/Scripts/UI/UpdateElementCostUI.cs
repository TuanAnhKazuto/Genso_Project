using TMPro;
using UnityEngine;

public class UpdateElementCostUI : MonoBehaviour
{
    [Header("UI elements cost")]
    [SerializeField] private ElementEnergySystem elementEnergySystem;
    [SerializeField] private TextMeshProUGUI metalCostTxt;
    [SerializeField] private TextMeshProUGUI woodCostTxt;
    [SerializeField] private TextMeshProUGUI waterCostTxt;
    [SerializeField] private TextMeshProUGUI fireCostTxt;
    [SerializeField] private TextMeshProUGUI earthCostTxt;

    private void OnEnable()
    {
        elementEnergySystem.OnEnergyChanged += UpdateElementCost;


        foreach (ElementEnergyAmount energyData in elementEnergySystem.energies)
        {
            switch (energyData.element)
            {
                case ElementType.Metallic:
                    metalCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Botanical:
                    woodCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Aqueous:
                    waterCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Thermal:
                    fireCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Telluric:
                    earthCostTxt.text = $"{energyData.amount}";
                    break;
            }
        }
    }

    private void OnDisable()
    {
        elementEnergySystem.OnEnergyChanged -= UpdateElementCost;
    }

    private void UpdateElementCost(ElementType element, int amount)
    {
        foreach (ElementEnergyAmount energyData in elementEnergySystem.energies)
        {
            switch (energyData.element)
            {
                case ElementType.Metallic:
                    metalCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Botanical:
                    woodCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Aqueous:
                    waterCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Thermal:
                    fireCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Telluric:
                    earthCostTxt.text = $"{energyData.amount}";
                    break;
            }
        }
    }
}