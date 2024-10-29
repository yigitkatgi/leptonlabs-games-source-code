using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class Speedometer : MonoBehaviour
{
    public TMP_Text speedometer;
    public Rigidbody2D rb;
    public int digits = 2;

    private void Update()
    {
        
        speedometer.text = Math.Round(rb.velocity.magnitude * 3, digits) + " km/h";
    }
}
