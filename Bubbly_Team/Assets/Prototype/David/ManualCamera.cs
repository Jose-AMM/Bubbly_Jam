using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCamera : MonoBehaviour
{
    private bool autoscroll;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float deadZoneY;

    void Start()
    {
        autoscroll = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoscroll)
        {
            //Se mueve hacia la derecha a una velocidad constante
            Vector3 cameraPosition = gameObject.transform.position;
            cameraPosition.x += cameraSpeed * Time.deltaTime;
            //Calcular la y de la camara con respecto al player
            Vector3 playerPosition = GameManager.Instance.Player.transform.position;
            if (playerPosition.y - gameObject.transform.position.y > deadZoneY) 
            {
                cameraPosition.y = playerPosition.y - deadZoneY;
            }
            else if(gameObject.transform.position.y - playerPosition.y > deadZoneY)
            {
                cameraPosition.y = playerPosition.y + deadZoneY;
            }

            gameObject.transform.position = cameraPosition;
        }

        //Testing Camera
        if (Input.GetKey(KeyCode.A))
        {
            GameManager.Instance.StartAutoScroll();
        }
        if (Input.GetKey(KeyCode.D))
        {
            GameManager.Instance.StopAutoScroll();
        }
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
}
