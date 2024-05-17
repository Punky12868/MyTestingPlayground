using UnityEngine.UI;
using UnityEngine;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image toggleImage;
    [SerializeField] private Sprite[] toggleStatusSprites;

    private bool isOn = true;

    public void Toggle()
    {
        isOn = !isOn;
        toggleImage.sprite = isOn ? toggleStatusSprites[0] : toggleStatusSprites[1];
    }

    public void IncreaseDecreaseSliderValue(int value)
    {
        if (value > 0 && slider.value + value > slider.maxValue || value < 0 && slider.value + value < slider.minValue) return;
        slider.value += value;
    }
}
