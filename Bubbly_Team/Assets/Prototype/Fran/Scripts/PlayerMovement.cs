using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float swimVelocity = 1.0f;
    [SerializeField] private float rotateVelocity = 1.0f;

    [SerializeField] private float boostVelocity = 1.0f;
    [SerializeField] private float boostDuration = 1.0f;
    [SerializeField] private AnimationCurve boostCurve;
    [SerializeField] private GameObject boostLight;

    private Light2D boostLight2D;

    private Vector2 _mouseWorldPosition = new Vector2();
    private Vector2 _playerToMouseDirection = new Vector2();

    private bool _canRotate = true;
    private float _playerRotationDeg = 0.0f;
    private bool _boosting = false;
    private float _boostSpeedCurrent = 0.0f;
    private float _boostTimeCurrent = 0.0f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        boostLight2D = boostLight.GetComponent<Light2D>();
    }

    void Start()
    {
        GameManager.Instance.EnablePlayer();
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
                SoundManager.Instance.PlaySound("DASH", 1.0f);
                _boostSpeedCurrent = boostVelocity *
                                     Mathf.Clamp(boostCurve.Evaluate(0.0f), swimVelocity, boostVelocity * 2.0f);
                _rb.velocity = transform.right * _boostSpeedCurrent;

                _boostTimeCurrent = 0.0f;
                _boosting = true;

                boostLight2D.intensity = 1.0f + boostCurve.Evaluate(1.0f / boostDuration);
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
                _boostSpeedCurrent = Mathf.Clamp(boostVelocity * boostCurve.Evaluate(_boostTimeCurrent / boostDuration),
                    swimVelocity, boostVelocity * 2.0f);

                _rb.velocity = transform.right * _boostSpeedCurrent;

                boostLight2D.intensity =
                    1.0f + boostCurve.Evaluate((1.0f / boostDuration) - (_boostTimeCurrent / boostDuration));

                _boostTimeCurrent += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (_canRotate)
        {
            _sr.flipY = transform.rotation.eulerAngles.z is >= 90.0f and < 270.0f;

            _playerRotationDeg = Mathf.Atan2(_playerToMouseDirection.y, _playerToMouseDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0.0f, 0.0f, _playerRotationDeg);
            // transform.rotation = Quaternion.RotateTowards(transform.rotation,
            //    Quaternion.Euler(0f, 0f, _playerRotationDeg), rotateVelocity * Time.deltaTime);

            gameObject.GetComponent<BoostAim>().AimLine(_mouseWorldPosition, _playerToMouseDirection);
            gameObject.GetComponent<BoostAim>().AimArrow(_mouseWorldPosition, _playerRotationDeg);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public void Stop()
    {
        _rb.velocity = Vector2.zero;
        _canRotate = false;
        _boosting = false;
        _rb.isKinematic = true;
    }

    public void AsNew()
    {
        _rb.isKinematic = false;
        _canRotate = true;
    }
}