using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOxygenBar : MonoBehaviour
{
    [SerializeField] private GameObject[] bubbleBar;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        float oxygen = GameManager.Instance.Player.GetComponent<OxygenBar>().GetCurrentOxygen();
        float maxOxygen = GameManager.Instance.Player.GetComponent<OxygenBar>().GetMaxOxygen();
        bool[] enables;
        if (oxygen <= maxOxygen)
        {
            enables = new bool[] { true, true, true, true, true, true, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.9)
        {
            enables = new bool[]{ false, true, true, true, true, true, true, true, true, true};
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.8)
        {
            enables = new bool[] { false, false, true, true, true, true, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.7)
        {
            enables = new bool[] { false, false, false, true, true, true, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.6)
        {
            enables = new bool[] { false, false, false, false, true, true, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.5)
        {
            enables = new bool[] { false, false, false, false, false, true, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.4)
        {
            enables = new bool[] { false, false, false, false, false, false, true, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.3)
        {
            enables = new bool[] { false, false, false, false, false, false, false, true, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.2)
        {
            enables = new bool[] { false, false, false, false, false, false, false, false, true, true };
            UpdateSprites(enables);
        }
        if (oxygen <= maxOxygen * 0.1)
        {
            enables = new bool[] { false, false, false, false, false, false, false, false, false, true };
            UpdateSprites(enables);
        }
        if (oxygen <= 0)
        {
            enables = new bool[] { false, false, false, false, false, false, false, false, false, false };
            UpdateSprites(enables);
        }
    }

    void UpdateSprites(bool[] enables)
    {
        bubbleBar[9].SetActive(enables[0]);
        bubbleBar[8].SetActive(enables[1]);
        bubbleBar[7].SetActive(enables[2]);
        bubbleBar[6].SetActive(enables[3]);
        bubbleBar[5].SetActive(enables[4]);
        bubbleBar[4].SetActive(enables[5]);
        bubbleBar[3].SetActive(enables[6]);
        bubbleBar[2].SetActive(enables[7]);
        bubbleBar[1].SetActive(enables[8]);
        bubbleBar[0].SetActive(enables[9]);
    }
}
