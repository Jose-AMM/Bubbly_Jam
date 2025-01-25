using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float swimVelocity = 0.1f;
    [SerializeField] private float rotateVelocity = 1.0f;
    [SerializeField] private float boostVelocity = 1.0f;
    [SerializeField] private float boostDelay = 0.5f;

    private Vector2 _mouseWorldPosition = new Vector2();
    private Vector2 _playerToMouseDirection = new Vector2();

    private float _playerRotationDeg = 0.0f;
    private bool _boosting = false;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mouseWorldPosition);

        _playerToMouseDirection = new Vector2(_mouseWorldPosition.x - transform.position.x,
            _mouseWorldPosition.y - transform.position.y).normalized;

        if (!_boosting && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _rb.velocity = _playerToMouseDirection * boostVelocity;
            _boosting = true;

            StartCoroutine(ExampleCoroutine());
        }
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(boostDelay);
        _boosting = false;
    }

    private void FixedUpdate()
    {
        _playerRotationDeg = Mathf.Atan2(_playerToMouseDirection.y, _playerToMouseDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.Euler(0f, 0f, _playerRotationDeg - 90.0f), rotateVelocity * Time.deltaTime);
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