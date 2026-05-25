using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static ElementMark;

public class TestSkillCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ElementEnergySystem alphaEnergySystem;
    [SerializeField] private CoreHealth omegaCore;
    [SerializeField] private ElementMarkContainer omegaMarkContainer;
    [SerializeField] private int reactionDamage = 50;

    [Header("Test Fire Attack")]
    [SerializeField] private ElementType costElement = ElementType.Fire;
    [SerializeField] private int ecCost = 2;
    [SerializeField] private int damage = 100;

    private void Update()
    {
        if (Keyboard.current == null) return;

        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TryCastFireAttack();
        }
    }

    public void TryCastFireAttack()
    {
        if(turnManager != null && !turnManager.CanAlphaUseSkill())
        {
            Debug.Log("Không thể dùng skill. Hiện tại không phải Alpha Skill Phase.");
            return;
        }

        if (omegaCore == null)
        {
            Debug.LogError("TestSkillCaster chưa được gán OmegaCore.");
            return;
        }

        bool spentSuccessfully = alphaEnergySystem.SpendEnergy(costElement, ecCost);

        if (!spentSuccessfully)
        {
            Debug.Log($"Không đủ {costElement} EC để dùng skill.");
            return;
        }

        omegaCore.TakeDamage(damage);

        if (omegaMarkContainer != null)
        {
            bool markApplied = omegaMarkContainer.TryApplyMark(costElement, out MarkReactionResult reactionResult);

            if (markApplied && reactionResult.hasReaction)
            {
                omegaCore.TakeDamage(reactionDamage);

                Debug.Log(
                    $"Reaction Damage: {reactionResult.incomingElement} sinh {reactionResult.existingElement}. " +
                    $"Gây thêm {reactionDamage} damage."
                );
            }
        }

        Debug.Log($"Alpha dùng Test Fire Skill: -{ecCost} {costElement} EC, gây {damage} damage.");
    }
}
