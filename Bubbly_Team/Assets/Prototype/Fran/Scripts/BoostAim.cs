using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoostAim : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject aimArrow;

    private Vector2 _startPosition = new Vector2();
    private Vector2 _endPosition = new Vector2();

    private float _lineMax = 10.0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        aimArrow = transform.Find("AimArrow").gameObject;
    }

    public void AimLine(Vector2 mouseWorldPosition, Vector2 playerToMouseDirection)
    {
        lineRenderer.enabled = true;

        _startPosition = transform.position;
        lineRenderer.SetPosition(0, _startPosition);

        _endPosition = mouseWorldPosition;
        float lineLength = Mathf.Clamp(Vector2.Distance(_startPosition, _endPosition), 0, _lineMax);
        _endPosition = _startPosition + (playerToMouseDirection * lineLength);
        lineRenderer.SetPosition(1, _endPosition);
    }

    public void AimArrow(Vector2 mouseWorldPosition, float playerRotationDeg)
    {
        aimArrow.SetActive(true);
        aimArrow.transform.position = mouseWorldPosition;
        aimArrow.transform.rotation = Quaternion.Euler(0f, 0f, playerRotationDeg - 135.0f);
    }
}