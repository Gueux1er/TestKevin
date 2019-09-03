using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public Image image;
    public Shadow shadow;

    public Color activeColor;
    public Color disableColor;

    public void Active()
    {
        image.color = activeColor;
        shadow.enabled = true;
    }

    public void Disable()
    {
        image.color = disableColor;
        shadow.enabled = false;
    }
}
