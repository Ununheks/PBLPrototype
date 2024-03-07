using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSystem : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public GameObject player;
    public GameObject buttonObject;
    public float interactionDistance = 3f;

    private bool isInRange = false;

    void Start()
    {
        SetButtonVisibility(false);
        button1.onClick.AddListener(UpgradeDamage);
        button2.onClick.AddListener(UpgradeSpeed);
    }

    void Update()
    {
        isInRange = Vector3.Distance(player.transform.position, buttonObject.transform.position) <= interactionDistance;

        if (isInRange)
        {
            SetButtonVisibility(true);
            if (Input.GetKeyDown(KeyCode.N))
            {
                button1.onClick.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                button2.onClick.Invoke();
            }
        }
        else
        {
            SetButtonVisibility(false);
        }
    }

    void UpgradeDamage()
    {
        Debug.Log("Upgraded damage");
    }

    void UpgradeSpeed()
    {
        Debug.Log("Upgraded Speed");
    }
    
    void SetButtonVisibility(bool isVisible)
    {
        button1.gameObject.SetActive(isVisible);
        button2.gameObject.SetActive(isVisible);
    }
}
