using UnityEngine;

public class InterfaseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private BattlePhase battlePhase;

    [Header("Battle TurnBase interfase")]
    [SerializeField] private GameObject coreHpUIObj;
    [SerializeField] private GameObject cardBattelObj;

    [Header("Macth-3 interfase")]
    [SerializeField] private GameObject macthThreeGameBoardObj;
    [SerializeField] private GameObject macthThreeCanvasOjb;

    private void Awake()
    {
        ShowMacthThreeGame();
    }
    private void OnEnable()
    {
        if (turnManager != null)
        {
            turnManager.OnPhaseChanged += HandlePhaseChanged;
        }
    }

    private void OnDisable()
    {
        if (turnManager != null)
        {
            turnManager.OnPhaseChanged -= HandlePhaseChanged;
        }
    }

    private void HandlePhaseChanged(BattleSide side, BattlePhase phase)
    {
        if (phase == BattlePhase.SwapPhase)
        {
            ShowMacthThreeGame();
        }
        else if (phase == BattlePhase.SkillPhase)
        {
            ShowBattleTurnBaseUI();
        }
        else
        {
            return;
        }
    }


    private void ShowBattleTurnBaseUI()
    {
        coreHpUIObj.SetActive(true);
        cardBattelObj.SetActive(true);
        macthThreeGameBoardObj.SetActive(false);
        macthThreeCanvasOjb.SetActive(false);
    }

    private void ShowMacthThreeGame()
    {
        coreHpUIObj.SetActive(false);
        cardBattelObj.SetActive(false);
        macthThreeGameBoardObj.SetActive(true);
        macthThreeCanvasOjb.SetActive(true);
    }
}