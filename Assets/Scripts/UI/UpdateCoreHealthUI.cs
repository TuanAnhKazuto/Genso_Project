using UnityEngine;
using UnityEngine.UI;

public class UpdateCoreHealthUI : MonoBehaviour
{
    [SerializeField] private CoreHealth coreHealth;
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        if (coreHealth != null) coreHealth.OnHpChanged += UpdateHpSlider;
    }

    private void OnDisable()
    {
        if (coreHealth != null) coreHealth.OnHpChanged -= UpdateHpSlider;
    }

    private void Start()
    {
        if (coreHealth != null)
        {
            UpdateHpSlider(coreHealth.Owner, coreHealth.CurrentHp, coreHealth.MaxHp);
        }
    }

    private void UpdateHpSlider(BattleSide side, int currentHp, int maxHp)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = maxHp;
        healthSlider.value = currentHp;
    }
}
