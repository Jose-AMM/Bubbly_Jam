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

    //Variables da�o por contacto
    private bool _inContactWithEnemy;
    private float _damageInContact;

    public enum OxygenLevel
    {
        Max,
        High,
        Medium,
        Low,
        Zero,
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
            StartCoroutine(Respawn());
        }
        CheckOxygenLevel();
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.RespawnPlayer();
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

    public void StartDrowning()
    {
        if (!_isDrowning)
        {
            _isDrowning = true;
        }
    }

    public void StopDrowning()
    {
        if (_isDrowning)
        {
            _isDrowning = false;
        }
    }

    void CheckOxygenLevel()
    {
        float oxygenPercentage = _currentOxygen / _maxOxygen;
        SoundManager.Instance.SetVolume("BUBBLE-LOOP", 1.0f-oxygenPercentage, 0.0f);
        
        if (oxygenPercentage == 1.0f && _oxygenLevel != OxygenLevel.Max)
        {
            _oxygenLevel = OxygenLevel.Max;
            SoundManager.Instance.SetVolume("CHORD-TENSION", 0.0f, 0.0f);
            SoundManager.Instance.SetVolume("ALARMA", 0.0f, 0.0f);
        }
        if (oxygenPercentage <= 1.0f && oxygenPercentage > 0.66f && _oxygenLevel != OxygenLevel.High)
        {
            _oxygenLevel = OxygenLevel.High;
            SoundManager.Instance.SetVolume("CHORD-TENSION", 0.0f, 0.0f);
            SoundManager.Instance.SetVolume("ALARMA", 0.0f, 0.0f);
        }
        else if(oxygenPercentage <= 0.66f && oxygenPercentage > 0.33f  && _oxygenLevel != OxygenLevel.Medium)
        {
            _oxygenLevel = OxygenLevel.Medium;
            SoundManager.Instance.SetVolume("CHORD-TENSIONP", 0.5f, 0.0f);
        }
        else if (oxygenPercentage <= 0.33f && oxygenPercentage > 0.0f  && _oxygenLevel != OxygenLevel.Low)
        {
            SoundManager.Instance.PlaySound("AHOGANDOSE", 1.0f);
            SoundManager.Instance.SetVolume("CHORD-TENSION", 1.0f, 0.0f);
            SoundManager.Instance.SetVolume("ALARMA", 0.5f, 0.0f);
            _oxygenLevel = OxygenLevel.Low;
        } 
        else if (oxygenPercentage <= 0.0f  && _oxygenLevel != OxygenLevel.Zero){
            _oxygenLevel = OxygenLevel.Zero;
            SoundManager.Instance.PlaySound("AHOGANDOSE", 1.0f);
        }
    }

    public void Damaged(float damageAmount)
    {
        if(_invulnerabilityCD <= 0)
        {
            SoundManager.Instance.PlaySound("GOLPE", 1.0f);
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

    public void SetStartLevelOxygen(float amount)
    {
        _maxOxygen = amount;
        _currentOxygen = _maxOxygen;
        UpdateOxygen();
    }

    public float GetCurrentOxygen()
    {
        return _currentOxygen;
    }
}
