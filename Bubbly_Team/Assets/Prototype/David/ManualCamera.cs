using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCamera : MonoBehaviour
{
    private bool autoscroll;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float deadZoneY;

    private bool shake;
    [SerializeField] private float shakeAmount;
    [SerializeField] private float shakeTime;
    private float shakeCounter;
    private bool hasTeleported;


    void Start()
    {
        autoscroll = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = gameObject.transform.position;
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;

        if (autoscroll)
        {
            float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect; // For orthographic cameras
            float cameraRightLimit = gameObject.transform.position.x + cameraHalfWidth;
            float cameraLeftLimit = gameObject.transform.position.x - cameraHalfWidth;
            //Se mueve hacia la derecha a una velocidad constante
            cameraPosition.x += cameraSpeed * Time.deltaTime;
            //Calcular la y de la camara con respecto al player
            if (playerPosition.y - gameObject.transform.position.y > deadZoneY) 
            {
                cameraPosition.y = playerPosition.y - deadZoneY;
            }
            else if(gameObject.transform.position.y - playerPosition.y > deadZoneY)
            {
                cameraPosition.y = playerPosition.y + deadZoneY;
            }

            if (playerPosition.x > cameraRightLimit) 
            {
                // Stop camera from moving further right if player is leaving view
                cameraPosition.x = cameraRightLimit - cameraHalfWidth;
            } 
            else if (playerPosition.x < cameraLeftLimit) 
            {
                // Stop camera from moving further left if player is leaving view
                cameraPosition.x = cameraLeftLimit + cameraHalfWidth;
            }
            gameObject.transform.position = cameraPosition;
        }

        if (shake)
        {
            Vector2 randomPos = Random.insideUnitCircle;
            cameraPosition = playerPosition + new Vector3(randomPos.x, randomPos.y, -10) * shakeAmount;
            shakeCounter += Time.deltaTime;
            Debug.Log("Me estoy batiendo");

            //GameManager.Instance.Player.transform.position = playerPosition;
            gameObject.transform.position = cameraPosition;

            if (shakeCounter > (shakeAmount / 2) && !hasTeleported)
            {
                GameManager.Instance.SetFollowToPlayer();
                GameManager.Instance.TPPlayerToPosition(Vector2.zero);
                GameManager.Instance.CleanVirtualCameraFollow();
                hasTeleported = true;
            }

            if(shakeCounter > shakeAmount)
            {
                shake = false;
                hasTeleported = false;
                GameManager.Instance.SetFollowToPlayer();
                //GameManager.Instance.TPPlayerToPosition(playerPosition);
            }
        }

        //Testing Camera

    }

    public void SetCameraSpeed(float speed)
    {
        cameraSpeed = speed;
    }

    public void StartAutoScroll()
    {
        autoscroll = true;
    }

    public void StopAutoScroll()
    {
        autoscroll = false;
    }

    public void StartShakeCamera()
    {
        shake = true;
        shakeCounter = 0;
    }
}
