using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ElementMark;

[Serializable]
public class TestElementSkill
{
    public string skillName;
    public ElementType element;
    public int ecCost = 2;
    public int damage = 100;
    public int reactionDamage = 50;
}

public class ElementSkillTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ElementEnergySystem alphaEnergySystem;
    [SerializeField] private CoreHealth omegaCore;
    [SerializeField] private ElementMarkContainer omegaMarkContainer;

    [Header("Test Skills")]
    [SerializeField] private List<TestElementSkill> testSkills = new();

    private void Reset()
    {
        testSkills = new List<TestElementSkill>
        {
            new()
            {
                skillName = "Metallic Test Skill",
                element = ElementType.Metallic,
                ecCost = 2,
                damage = 100,
                reactionDamage = 50,
            },

            new()
            {
                skillName = "Botanical Test Skill",
                element = ElementType.Botanical,
                ecCost = 2,
                damage = 100,
                reactionDamage = 50,
            },

            new()
            {
                skillName = "Aqueous Test Skill",
                element = ElementType.Aqueous,
                ecCost = 2,
                damage = 100,
                reactionDamage = 50,
            },

            new()
            {
                skillName = "Thermal Test Skill",
                element = ElementType.Thermal,
                ecCost = 2,
                damage = 100,
                reactionDamage = 50,
            },

            new()
            {
                skillName = "Telluric Test Skill",
                element = ElementType.Telluric,
                ecCost = 2,
                damage = 100,
                reactionDamage = 50,
            },
        };
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TryUseSkillByIndex(0);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            TryUseSkillByIndex(1);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            TryUseSkillByIndex(2);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            TryUseSkillByIndex(3);
        }

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            TryUseSkillByIndex(4);
        }
    }

    public void TryUseSkillByIndex(int index)
    {
        if(index < 0 || index >= testSkills.Count)
        {
            Debug.LogError($"Invalid skill index: {index}");
            return;
        }

        TryUseSkill(testSkills[index]);
    }

    private void TryUseSkill(TestElementSkill skill)
    {
        if (turnManager != null && !turnManager.CanAlphaUseSkill())
        {
            Debug.Log("Không thể dùng skill. Hiện tại không phải Alpha Skill Phase.");
            return;
        }

        if (alphaEnergySystem == null)
        {
            Debug.LogError("ElementSkillTester chưa được gán AlphaEnergySystem.");
            return;
        }

        if (omegaCore == null)
        {
            Debug.LogError("ElementSkillTester chưa được gán OmegaCore.");
            return;
        }

        bool spentSuccessfully = alphaEnergySystem.SpendEnergy(
            skill.element,
            skill.ecCost
        );

        if (!spentSuccessfully)
        {
            Debug.Log($"Không đủ {skill.element} EC để dùng {skill.skillName}.");
            return;
        }

        omegaCore.TakeDamage(skill.damage);

        Debug.Log(
            $"Alpha dùng {skill.skillName}: " +
            $"-{skill.ecCost} {skill.element} EC, gây {skill.damage} damage."
        );

        TryApplyMarkAndReaction(skill);
    }

    private void TryApplyMarkAndReaction(TestElementSkill skill)
    {
        if (omegaMarkContainer == null)
        {
            Debug.LogWarning("ElementSkillTester chưa được gán OmegaMarkContainer.");
            return;
        }

        bool markApplied = omegaMarkContainer.TryApplyMark(
            skill.element,
            out MarkReactionResult reactionResult
            );

        if (!markApplied)
        {
            Debug.Log($"Không thể gắn {skill.element} mark lên Omega.");
            return;
        }

        if (!reactionResult.hasReaction)
        {
            Debug.Log($"{skill.element} Mark đã được gắn lên Omega.");
            return;
        }

        omegaCore.TakeDamage(skill.reactionDamage);

        Debug.Log(
            $"Reaction Triggered: {reactionResult.incomingElement} sinh {reactionResult.existingElement}. " +
            $"Gây thêm {skill.reactionDamage} reaction damage.");
    }
}
