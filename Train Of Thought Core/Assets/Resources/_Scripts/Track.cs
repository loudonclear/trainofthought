using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public Sprite leftSplit;
    public Sprite ySplit;
    public Sprite rightSplit;
    public SpriteRenderer sr;

    public void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void ChangeTrackDir(int direction)
    {
        switch (direction)
        {
            case 0 :
                sr.sprite = ySplit;
                break;
            case 1:
                sr.sprite = leftSplit;
                break;
            case 2:
                sr.sprite = rightSplit;
                break;
            default:
                break;
        }

    }
}
