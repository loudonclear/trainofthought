using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Text text;

    private Image im;
    private float transparency;

    void Start()
    {
        im = GetComponent<Image>();
        transparency = im.color.a;
    }

    void Update()
    {
        if (text.text == "")
        {
            im.color = new Color(im.color.r, im.color.g, im.color.b, 0);
        }
        else
        {
            im.color = new Color(im.color.r, im.color.g, im.color.b, transparency);
        }
    }
}
