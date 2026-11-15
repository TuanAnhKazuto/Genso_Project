using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarkIconUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stackText;
    [SerializeField] private TextMeshProUGUI durationText;

    public void Setup(Sprite icon, int stackCount, int minimumDuration)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (stackText != null)
        {
            stackText.text = stackCount > 1 ? $"x{stackCount}" : string.Empty;
        }

        if (durationText != null)
        {
            durationText.text = minimumDuration.ToString();
        }
    }
}
