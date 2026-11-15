using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ElementMark;

[Serializable]
public class  ElementMark
{
    public ElementType element;
    public List<int> stackDuration = new();

    public int StackCount => stackDuration.Count;
    public int MinimumDuration
    {
        get
        {
            if (stackDuration.Count == 0) return 0;

            int minimum = stackDuration[0];
            for (int i = 1; i < stackDuration.Count; i++)
            {
                if (stackDuration[i] < minimum) minimum = stackDuration[i];
            }

            return minimum;
        }
    }

    public ElementMark(ElementType element, int duration)
    {
        this.element = element;
        stackDuration.Add(duration);
    }

    public void AddStackOrRefreshOldest(int maxStack, int duration)
    {
        if (stackDuration.Count < maxStack)
        {
            stackDuration.Add(duration);
            return;
        }

        int oldestIndex = FindOldestStackIndex();
        stackDuration[oldestIndex] = duration;
    }

    public void RemoveOneStack()
    {
        if(stackDuration.Count <= 0) return;

        stackDuration.RemoveAt(FindOldestStackIndex());
    }

    private int FindOldestStackIndex()
    {
        int oldestIndex = 0;
        for (int i = 1; i < stackDuration.Count; i++)
        {
            if (stackDuration[i] < stackDuration[oldestIndex]) oldestIndex = i;
        }

        return oldestIndex;
    }

    public void TickDuration()
    {
        for (int i = stackDuration.Count - 1; i >= 0; i--)
        {
            stackDuration[i]--;

            if (stackDuration[i] <= 0)
            {
                stackDuration.RemoveAt(i);
            }
        }
    }

    public struct MarkReactionResult
    {
        public bool hasReaction;
        public ElementType incomingElement;
        public ElementType existingElement;

        public MarkReactionResult(bool hasReation, ElementType incomingElement, ElementType existingElement)
        {
            this.hasReaction = hasReation;
            this.incomingElement = incomingElement;
            this.existingElement = existingElement;
        }
    }
}

public class ElementMarkContainer : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private BattleSide owner;

    [Header("References")]
    [SerializeField] private TurnManager turnManager;

    [Header("Mark Rules")]
    [SerializeField] private int maxMarkTypes = 3;
    [SerializeField] private int maxStackPerMark = 3;
    [SerializeField] private int defaultDuration = 3;

    [Header("Debug")]
    [SerializeField] private List<ElementMark> activeMarks = new();

    public event Action<BattleSide> OnMarksChanged;
    public event Action<MarkReactionResult> OnReactionTriggered;

    public IReadOnlyList<ElementMark> ActiveMarks => activeMarks;
    public BattleSide Owner => owner;

    private void OnEnable()
    {
        if(turnManager != null)
        {
            turnManager.OnPhaseChanged += HandlePhaseChanged;
        }
    }

    private void OnDisable()
    {
        if(turnManager != null)
        {
            turnManager.OnPhaseChanged -= HandlePhaseChanged;
        }
    }

    public bool TryApplyMark(ElementType incomingElement, out MarkReactionResult reactionResult)
    {
        reactionResult = new(false, incomingElement, incomingElement);

        int reactionTargetIndex = FindReactionTargetIndex(incomingElement);

        if (reactionTargetIndex >= 0)
        {
            ElementMark targetMark = activeMarks[reactionTargetIndex];
            ElementType existingElement = targetMark.element;

            targetMark.RemoveOneStack();

            if (targetMark.StackCount <= 0)
            {
                activeMarks.RemoveAt(reactionTargetIndex);
            }

            reactionResult = new(true, incomingElement, existingElement);

            Debug.Log($"{owner}: Reaction {incomingElement} sinh {existingElement}");

            OnReactionTriggered?.Invoke(reactionResult);
            OnMarksChanged?.Invoke(owner);

            return true;
        }

        ElementMark sameMark = FindMark(incomingElement);

        if (sameMark != null)
        {
            sameMark.AddStackOrRefreshOldest(maxStackPerMark, defaultDuration);

            Debug.Log($"{owner}: Add stack {incomingElement}. Stack = {sameMark.StackCount}");

            OnMarksChanged?.Invoke(owner);
            return true;
        }

        if (activeMarks.Count >= maxMarkTypes)
        {
            Debug.Log($"{owner}: Full mark. Không thể thêm {incomingElement}.");
            return false;
        }

        activeMarks.Add(new ElementMark(incomingElement, defaultDuration));

        Debug.Log($"{owner}: Add new mark {incomingElement}");

        OnMarksChanged?.Invoke(owner);
        return true;
    }

    private int FindReactionTargetIndex(ElementType incomingElement)
    {
        for (int i = 0; i < activeMarks.Count; i++)
        {
            ElementMark mark = activeMarks[i];

            if (ElementRelation.Generates(incomingElement, mark.element))
            {
                return i;
            }
        }

        return -1;
    }

    private ElementMark FindMark(ElementType element)
    {
        foreach (ElementMark mark in activeMarks)
        {
            if (mark.element.Equals(element))
            {
                return mark;
            }
        }

        return null;
    }

    private void HandlePhaseChanged(BattleSide side, BattlePhase phase)
    {
        if (side != owner) return;
        if (phase != BattlePhase.StartTurn) return;

        TickMarks();
    }

    private void TickMarks()
    {
        bool changed = false;

        for (int i = activeMarks.Count - 1; i >= 0; i--)
        {
            activeMarks[i].TickDuration();

            if (activeMarks[i].StackCount <= 0)
            {
                Debug.Log($"{owner}: Mark {activeMarks[i].element} expired.");
                activeMarks.RemoveAt(i);
            }

            changed = true;
        }

        if (changed)
        {
            OnMarksChanged?.Invoke(owner);
        }
    }
}
