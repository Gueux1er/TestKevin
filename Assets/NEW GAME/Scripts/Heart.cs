using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public Image image;

    public Sprite activeHeartSprite;
    public Sprite disableHeartSprite;

    public void Active()
    {
        image.sprite = activeHeartSprite;
    }

    public void Disable()
    {
        image.sprite = disableHeartSprite;
    }
}
