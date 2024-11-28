using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxStamina(int maxPoint)
    {
        slider.maxValue = maxPoint;
        slider.value = maxPoint;
    }

    public void SetCurrentStamina(int currentPoint)
    {
        slider.value = currentPoint;
    }
}
