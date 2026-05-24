using TMPro;
using UnityEngine;

public class MacthThreeUI : MonoBehaviour
{
    [Header("Swap count:")]
    [SerializeField] private SwapCounterSystem swapCounterSystem;
    public TextMeshProUGUI swapCountText;

    [Header("UI elements cost")]
    [SerializeField] private ElementEnergySystem elementEnergySystem;
    [SerializeField] private TextMeshProUGUI metalCostTxt;
    [SerializeField] private TextMeshProUGUI woodCostTxt;
    [SerializeField] private TextMeshProUGUI waterCostTxt;
    [SerializeField] private TextMeshProUGUI fireCostTxt;
    [SerializeField] private TextMeshProUGUI earthCostTxt;

    [Header("Turn")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private TextMeshProUGUI curTurnTxt;

    private void OnEnable()
    {
        swapCounterSystem.OnSwapCountChanged += UpdateSwapCountText;
        elementEnergySystem.OnEnergyChanged += UpdateElementCostUI;
        turnManager.OnPhaseChanged += UpdateTurnText;
    }

    private void OnDisable()
    {
        swapCounterSystem.OnSwapCountChanged -= UpdateSwapCountText;
        elementEnergySystem.OnEnergyChanged -= UpdateElementCostUI;
        turnManager.OnPhaseChanged -= UpdateTurnText;
    }

    private void UpdateSwapCountText(int newCount)
    {
        swapCountText.text = $"Swaps Left: {newCount}";
    }

    private void UpdateElementCostUI(ElementType element, int amount)
    {
        foreach (ElementEnergyAmount energyData in elementEnergySystem.energies)
        {
            switch (energyData.element)
            {
                case ElementType.Metal:
                    metalCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Wood:
                    woodCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Water:
                    waterCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Fire:
                    fireCostTxt.text = $"{energyData.amount}";
                    break;
                case ElementType.Earth:
                    earthCostTxt.text = $"{energyData.amount}";
                    break;
            }
        }
    }

    private void UpdateTurnText(BattleSide side, BattlePhase phase)
    {
        curTurnTxt.text = $"Turn: {side} - {phase}";
    }
}
