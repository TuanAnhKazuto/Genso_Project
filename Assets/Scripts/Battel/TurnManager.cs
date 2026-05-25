using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    [Header("References")]
    [SerializeField] private SwapCounterSystem alphaSwapCounter;

    [Header("Omega Dummy Turn")]
    [SerializeField] private float omegaTurnDuration = 1.2f;

    public BattleSide CurrentSide { get; private set; }
    public BattlePhase CurrentPhase { get; private set; }

    public event Action<BattleSide, BattlePhase> OnPhaseChanged;

    private bool isChangingTurn;

    private void OnEnable()
    {
        if (alphaSwapCounter != null)
        {
            alphaSwapCounter.OnSwapCountChanged += HandleAlphaSwapCountChanged;
        }
    }

    private void OnDisable()
    {
        if (alphaSwapCounter != null)
        {
            alphaSwapCounter.OnSwapCountChanged -= HandleAlphaSwapCountChanged;
        }
    }

    private void Start()
    {
        BeginAlphaTurn();
    }

    private void Update()
    {
        // Flash Test - Remove later
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            RequestEndTurn();
        }
    }

    public bool CanAlphaSwap()
    {
        return CurrentSide == BattleSide.Alpha &&
            CurrentPhase == BattlePhase.SwapPhase &&
            !isChangingTurn;
    }

    public bool CanAlphaUseSkill()
    {
        return CurrentSide == BattleSide.Alpha &&
            CurrentPhase == BattlePhase.SkillPhase &&
            !isChangingTurn;
    }

    private void RequestEndTurn()
    {
        if (isChangingTurn) return;
        if (CurrentSide != BattleSide.Alpha) return;

        StartCoroutine(EndAlphaTurnRoutine());
    }

    private void BeginAlphaTurn()
    {
        isChangingTurn = false;

        SetPhase(BattleSide.Alpha, BattlePhase.StartTurn);

        if (alphaSwapCounter != null)
        {
            alphaSwapCounter.ResetSwapCount();
        }

        SetPhase(BattleSide.Alpha, BattlePhase.SwapPhase);
    }

    private IEnumerator EndAlphaTurnRoutine()
    {
        isChangingTurn = true;

        SetPhase(BattleSide.Alpha, BattlePhase.EndTurn);

        yield return _waitForSeconds0_5;

        yield return OmegaDummyTurnRoutine();

        BeginAlphaTurn();
    }

    private IEnumerator OmegaDummyTurnRoutine()
    {
        SetPhase(BattleSide.Omega, BattlePhase.StartTurn);

        Debug.Log("Omega dummy turn bắt đầu.");

        yield return new WaitForSeconds(omegaTurnDuration);

        SetPhase(BattleSide.Omega, BattlePhase.EndTurn);

        Debug.Log("Omega dummy turn kết thúc.");
    }

    private void HandleAlphaSwapCountChanged(int curentSwapCount)
    {
        if (CurrentSide != BattleSide.Alpha) return;
        if (CurrentPhase != BattlePhase.SwapPhase) return;

        if (curentSwapCount <= 0)
        {
            EnterAlphaSkillPhase();
        }
    }

    private void EnterAlphaSkillPhase()
    {
        SetPhase(BattleSide.Alpha, BattlePhase.SkillPhase);
    }

    private void SetPhase(BattleSide side, BattlePhase phase)
    {
        CurrentSide = side;
        CurrentPhase = phase;

        Debug.Log("TurnManager: Phase changed to " + CurrentSide + " - " + CurrentPhase);

        OnPhaseChanged?.Invoke(CurrentSide, CurrentPhase);
    }
}
