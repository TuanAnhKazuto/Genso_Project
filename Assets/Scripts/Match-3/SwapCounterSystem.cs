using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SwapCounterSystem : MonoBehaviour
{
    [SerializeField] private int baseSwapCount = 2;
    [SerializeField] private int currentSwapCount;

    public event Action<int> OnSwapCountChanged;

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

    public void ApplySwapResultByMatchedGroupSize(int groupSize)
    {
        int oldSwapCount = currentSwapCount;
        int newSwapCount = currentSwapCount;

        if (groupSize >= 5)
        {
            // Luật: tốn 1 swap, được +2 swap => lời 1 swap
            newSwapCount += 1;

            Debug.Log($"Match >= 5. Swap: {oldSwapCount} -> {newSwapCount}");
        }
        else if (groupSize == 4)
        {
            // Luật: tốn 1 swap, được +1 swap => không đổi
            Debug.Log($"Match 4. Swap unchanged: {oldSwapCount}");
        }
        else
        {
            // Match 3: tốn 1 swap
            newSwapCount -= 1;

            Debug.Log($"Match 3. Swap: {oldSwapCount} -> {newSwapCount}");
        }

        currentSwapCount = Mathf.Max(0, newSwapCount);

        OnSwapCountChanged?.Invoke(currentSwapCount);
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


    // ======= Không cần quan tâm =========
    public void ResetAll()
    {
        SceneManager.LoadScene(0);
    }
}
