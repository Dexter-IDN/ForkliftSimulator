using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelManager : MonoBehaviour
{
    public String value;
    public Text label;

    // Start is called before the first frame update
    void Start()
    {
        label.text = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
