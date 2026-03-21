using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BgmVolumeSlider : MonoBehaviour
{
    [SerializeField] BgmManager bgmManager;
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        if (bgmManager == null)
        {
            bgmManager = BgmManager.Instance;
        }

        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (bgmManager != null)
        {
            slider.SetValueWithoutNotify(bgmManager.Volume);
        }

        slider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        if (bgmManager == null)
        {
            return;
        }

        bgmManager.Volume = value;
    }
}
