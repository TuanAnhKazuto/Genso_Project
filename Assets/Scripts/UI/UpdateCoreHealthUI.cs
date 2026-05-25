using UnityEngine;
using UnityEngine.UI;

public class UpdateCoreHealthUI : MonoBehaviour
{
    [SerializeField] private CoreHealth coreHealth;
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        coreHealth.OnHpChanged += UpdateHpSlider;
    }

    private void OnDisable()
    {
        coreHealth.OnHpChanged -= UpdateHpSlider;
    }

    private void UpdateHpSlider(BattleSide side, int currentHp, int maxHp)
    {
        healthSlider.value = currentHp;
    }
}