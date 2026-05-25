using TMPro;
using UnityEngine;

public class MacthThreeUI : MonoBehaviour
{
    [Header("Swap count:")]
    [SerializeField] private SwapCounterSystem swapCounterSystem;
    public TextMeshProUGUI swapCountText;

    [Header("Turn")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private TextMeshProUGUI curTurnTxt;

    private void OnEnable()
    {
        swapCounterSystem.OnSwapCountChanged += UpdateSwapCountText;
        turnManager.OnPhaseChanged += UpdateTurnText;
    }

    private void OnDisable()
    {
        swapCounterSystem.OnSwapCountChanged -= UpdateSwapCountText;
        turnManager.OnPhaseChanged -= UpdateTurnText;
    }

    private void UpdateSwapCountText(int newCount)
    {
        swapCountText.text = $"Swaps Left: {newCount}";
    }

    private void UpdateTurnText(BattleSide side, BattlePhase phase)
    {
        curTurnTxt.text = $"Turn: {side} - {phase}";
    }
}
