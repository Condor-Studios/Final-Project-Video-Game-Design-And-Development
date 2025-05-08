using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_UI_Manager : MonoBehaviour
{
    private void Start()
    {
        if(_Instance == null && _Instance != this)
        {
            _Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Combat_UI_Manager _Instance;
    [SerializeField] Slider _PlayerHealthSlider;
    [SerializeField] GameObject _WinWindow;
    [SerializeField] GameObject _LoseWindow;


    public void UpdatePlayerHealthBar(float CurrentHP,float MaxHP)
    {
        if(_PlayerHealthSlider != null)
        {
            _PlayerHealthSlider.value = CurrentHP / MaxHP;
        }
    }



}
