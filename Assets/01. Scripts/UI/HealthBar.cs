using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(int maxHp)
    {
        slider.maxValue = maxHp;
        slider.value = maxHp;
    }

    public void SetCurrentHealth(int currntHp)
    {
        slider.value = currntHp;
    }
}
