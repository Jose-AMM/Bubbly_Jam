using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _boostVelocity = 1.0f;
    [SerializeField] private float _boostDelay = 0.5f;

    private Vector2 mouseWorldPosition;
    private bool boosting = false;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mouseWorldPosition); //Jose
        
        if (!boosting && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _rb.velocity =
                new Vector2(mouseWorldPosition.x - transform.position.x, mouseWorldPosition.y - transform.position.y)
                    .normalized * _boostVelocity;
            boosting = true;

            StartCoroutine(ExampleCoroutine());
        }
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(_boostDelay);
        boosting = false;
    }

    //Jose
    public void Stop(){
        _rb.velocity = Vector2.zero;
        boosting = false;
        _rb.isKinematic = true;
    }

    public void AsNew(){
        _rb.isKinematic = false;
    }
}