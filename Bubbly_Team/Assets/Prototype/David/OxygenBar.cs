using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBar : MonoBehaviour
{
    [SerializeField] private float _maxOxygen = 100;
    [SerializeField] private float _currentOxygen;
    [SerializeField] private Image _oxygenBarFill;
    [SerializeField] private bool _isDrowning;
    [SerializeField] private float _drowningSpeed;

    [SerializeField] private float _invulnerabilityTime;
    private float _invulnerabilityCD;

    //Variables daño por contacto
    private bool _inContactWithEnemy;
    private float _damageInContact;

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
        _invulnerabilityCD = 0;
        _inContactWithEnemy = false;
        CheckOxygenLevel();
    }


    void Update()
    {
        if (_isDrowning)
        {
            _currentOxygen -= Time.deltaTime * _drowningSpeed;
            _currentOxygen = Mathf.Clamp(_currentOxygen, 0f, _maxOxygen);
            UpdateOxygen();
        }
        if (_inContactWithEnemy)
        {
            Damaged(_damageInContact);
        } else 
        {
            _damageInContact = 0;
        }
        _invulnerabilityCD = Mathf.Clamp(_invulnerabilityCD - Time.deltaTime, 0f, _invulnerabilityTime);
        if (_currentOxygen <= 0)
        {
            GameManager.Instance.RespawnPlayer();
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

    public void AddOxygen(float oxygenAmount) 
    { 
        _currentOxygen += oxygenAmount;
        _currentOxygen = Mathf.Clamp(_currentOxygen, 0f, _maxOxygen);
        UpdateOxygen();
    }

    public void RemoveOxygen(float oxygenAmount) 
    {
        _currentOxygen -= oxygenAmount;
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

    public void Damaged(float damageAmount)
    {
        if(_invulnerabilityCD <= 0)
        {
            RemoveOxygen(damageAmount);
            _invulnerabilityCD = _invulnerabilityTime;
            if(damageAmount > _damageInContact)
            {
                _damageInContact = damageAmount;
            }
        }
    }

    public void InContactWithEnemy(bool inContact)
    {
        _inContactWithEnemy = inContact;
    }

    public float GetMaxOxygen()
    {
        return _maxOxygen;
    }

    public float GetCurrentOxygen()
    {
        return _currentOxygen;
    }
}
