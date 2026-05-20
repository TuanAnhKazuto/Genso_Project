using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapCounterSystem : MonoBehaviour
{
    [SerializeField] private int baseSwapCount = 2;
    [SerializeField] private int currentSwapCount;

    public event Action<int> OnSwapCountChanged;

    private void Start()
    {
        //ResetSwapCount();
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ResetSwapCount();
        }
    }

    public int CurrentSwapCount => currentSwapCount;
    
    public bool HasSwapLeft()
    {
        return currentSwapCount > 0;
    }

    public void ResetSwapCount()
    {
        currentSwapCount = baseSwapCount; 
        Debug.Log($"Swap count reset to {currentSwapCount}");

        OnSwapCountChanged?.Invoke(currentSwapCount);
    }

    public void SpendOneSwap()
    {
        currentSwapCount = Mathf.Max(0, currentSwapCount - 1);

        Debug.Log($"One swap spent. Remaining swaps: {currentSwapCount}");

        OnSwapCountChanged?.Invoke(currentSwapCount);
    }

    public void AddSwap(int amount)
    {
        if(amount <= 0) return;
        currentSwapCount += amount;

        Debug.Log($"Added {amount} swap(s). Total swaps: {currentSwapCount}");
      
        OnSwapCountChanged?.Invoke(currentSwapCount);
    }
}
