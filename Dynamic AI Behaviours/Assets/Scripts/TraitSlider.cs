using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitSlider : MonoBehaviour
{
    public PersonalityTrait trait;

    [HideInInspector]
    public Slider slider;

    [SerializeField]
    private Text sliderValueLabel;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateLabel()
    {
        sliderValueLabel.text = slider.value.ToString();
    }
}
