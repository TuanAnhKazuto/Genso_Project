using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreBattleUI : MonoBehaviour
{
    [Header("Core Health")]
    [SerializeField] private CoreHealth alphaCore;
    [SerializeField] private CoreHealth omegaCore;
    [SerializeField] private Slider alphaHpSlider;
    [SerializeField] private Slider omegaHpSlider;
    [SerializeField] private TextMeshProUGUI alphaHpText;
    [SerializeField] private TextMeshProUGUI omegaHpText;

    [Header("Omega Marks")]
    [SerializeField] private ElementMarkContainer omegaMarkContainer;
    [SerializeField] private Transform omegaMarkParent;
    [SerializeField] private MarkIconUI markIconPrefab;

    [Header("Element Icons")]
    [SerializeField] private List<ElementIconPair> elementIcons = new();

    private void OnEnable()
    {
        if (alphaCore != null) alphaCore.OnHpChanged += UpdateHpText;
        if (omegaCore != null) omegaCore.OnHpChanged += UpdateHpText;
        if (omegaMarkContainer != null) omegaMarkContainer.OnMarksChanged += UpdateOmegaMarkUI;
    }

    private void OnDisable()
    {
        if (alphaCore != null) alphaCore.OnHpChanged -= UpdateHpText;
        if (omegaCore != null) omegaCore.OnHpChanged -= UpdateHpText;
        if (omegaMarkContainer != null) omegaMarkContainer.OnMarksChanged -= UpdateOmegaMarkUI;
    }

    private void Start()
    {
        if (alphaCore != null)
            UpdateHpText(alphaCore.Owner, alphaCore.CurrentHp, alphaCore.MaxHp);
        if (omegaCore != null)
            UpdateHpText(omegaCore.Owner, omegaCore.CurrentHp, omegaCore.MaxHp);
        if (omegaMarkContainer != null)
            UpdateOmegaMarkUI(omegaMarkContainer.Owner);
    }

    private void UpdateHpText(BattleSide side, int currentHp, int maxHp)
    {
        string text = $"{currentHp}/{maxHp}";
        if (side == BattleSide.Alpha)
        {
            UpdateSlider(alphaHpSlider, currentHp, maxHp);
            if (alphaHpText != null) alphaHpText.text = text;
        }
        else if (side == BattleSide.Omega)
        {
            UpdateSlider(omegaHpSlider, currentHp, maxHp);
            if (omegaHpText != null) omegaHpText.text = text;
        }
    }

    private static void UpdateSlider(Slider slider, int currentHp, int maxHp)
    {
        if (slider == null) return;

        slider.maxValue = maxHp;
        slider.value = currentHp;
    }

    private void UpdateOmegaMarkUI(BattleSide side)
    {
        if (side != BattleSide.Omega || omegaMarkContainer == null || omegaMarkParent == null || markIconPrefab == null)
            return;

        for (int i = omegaMarkParent.childCount - 1; i >= 0; i--)
            Destroy(omegaMarkParent.GetChild(i).gameObject);

        foreach (ElementMark mark in omegaMarkContainer.ActiveMarks)
        {
            MarkIconUI iconUI = Instantiate(markIconPrefab, omegaMarkParent);
            iconUI.Setup(GetIcon(mark.element), mark.StackCount, mark.MinimumDuration);
        }
    }

    private Sprite GetIcon(ElementType element)
    {
        foreach (ElementIconPair pair in elementIcons)
        {
            if (pair != null && pair.element == element) return pair.icon;
        }

        return null;
    }
}
