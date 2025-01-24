using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBar : MonoBehaviour
{
    private float _maxOxygen = 100;
    private float _currentOxygen;
    [SerializeField] private Image _oxygenBarFill;
    [SerializeField] private bool _isDrowning;
    [SerializeField] private float _drowningSpeed;

    public enum OxygenLevel
    {
        High,
        Medium,
        Low,
    }

    public OxygenLevel _oxygenLevel;

    void Start()
    {
        _currentOxygen = _maxOxygen;
        CheckOxygenLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDrowning)
        {
            _currentOxygen -= Time.deltaTime * _drowningSpeed;
            _currentOxygen = Mathf.Clamp(_currentOxygen, 0f, _maxOxygen);
            UpdateOxygen();
        }
        CheckOxygenLevel();
    }

    void UpdateOxygen()
    {
        if (_oxygenBarFill != null) 
        {
            _oxygenBarFill.fillAmount = _currentOxygen/_maxOxygen;
        }
    }

    void AddOxygen(float oxygenAmount) 
    { 
        _currentOxygen += oxygenAmount;
        _currentOxygen = Mathf.Clamp(_currentOxygen, 0f, _maxOxygen);
        UpdateOxygen();
    }

    void StartDrowning()
    {
        if (!_isDrowning)
        {
            _isDrowning = true;
        }
    }

    void StopDrowning()
    {
        if (_isDrowning)
        {
            _isDrowning = false;
        }
    }

    void CheckOxygenLevel()
    {
        float oxygenPercentage = _currentOxygen / _maxOxygen;

        if (oxygenPercentage > 0.66f)
        {
            _oxygenLevel = OxygenLevel.High;
        }
        else if(oxygenPercentage > 0.33f)
        {
            _oxygenLevel = OxygenLevel.Medium;
        }
        else
        {
            _oxygenLevel = OxygenLevel.Low;
        }
    }
}
