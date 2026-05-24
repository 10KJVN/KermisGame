using UnityEngine;
using UnityEngine.UI;

public class PowerMeterUI : MonoBehaviour
{
    public Slider powerSlider;
    public HoldReleaseOscillator oscillator;

    void Start()
    {
        if (oscillator)
        {
            oscillator.onPowerChanged.AddListener(UpdatePower);
            powerSlider.gameObject.SetActive(false);
            oscillator.onRelease.AddListener((p) => powerSlider.gameObject.SetActive(false));
            // Je kunt ook bij indrukken de slider tonen – doe dat in de oscillator of met een extra event.
        }
    }

    void UpdatePower(float normalizedPower)
    {
        powerSlider.value = normalizedPower;
        if (!powerSlider.gameObject.activeSelf)
            powerSlider.gameObject.SetActive(true);
    }
}