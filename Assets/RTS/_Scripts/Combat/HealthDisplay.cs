using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    /*private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }*/

    public void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        if (currentHealth != maxHealth)
        {
            healthBarParent.SetActive(true);
        }
        healthBarImage.fillAmount = (float) currentHealth / maxHealth;
    }
}
