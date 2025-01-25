using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float swimVelocity = 1.0f;
    [SerializeField] private float rotateVelocity = 1.0f;

    [SerializeField] private float boostVelocity = 1.0f;
    [SerializeField] private float boostDuration = 1.0f;
    [SerializeField] private AnimationCurve boostCurve;

    private Vector2 _mouseWorldPosition = new Vector2();
    private Vector2 _playerToMouseDirection = new Vector2();

    private float _playerRotationDeg = 0.0f;
    private bool _boosting = false;
    private float _boostSpeedCurrent = 0.0f;
    private float _boostTimeCurrent = 0.0f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _playerToMouseDirection = new Vector2(_mouseWorldPosition.x - transform.position.x,
            _mouseWorldPosition.y - transform.position.y).normalized;

        if (!_boosting)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _boostSpeedCurrent = boostVelocity * boostCurve.Evaluate(0.0f);
                _rb.velocity = transform.right * _boostSpeedCurrent;

                _boostTimeCurrent = 0.0f;
                _boosting = true;
            }
            else
            {
                _rb.velocity = transform.right * swimVelocity;
            }
        }
        else
        {
            if (_boostTimeCurrent >= boostDuration)
            {
                _boosting = false;
            }
            else
            {
                _boostSpeedCurrent =
                    Mathf.Clamp(boostVelocity * boostCurve.Evaluate(_boostTimeCurrent / boostDuration), swimVelocity,
                        boostVelocity);
                _rb.velocity = transform.right * _boostSpeedCurrent;

                _boostTimeCurrent += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        _playerRotationDeg = Mathf.Atan2(_playerToMouseDirection.y, _playerToMouseDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.Euler(0f, 0f, _playerRotationDeg), rotateVelocity * Time.deltaTime);
    }

    //Jose
    public void Stop()
    {
        _rb.velocity = Vector2.zero;
        _boosting = false;
        _rb.isKinematic = true;
    }

    public void AsNew()
    {
        _rb.isKinematic = false;
    }
}