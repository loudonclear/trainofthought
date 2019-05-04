using UnityEngine;
using System.Collections;


public class DemoOne : MonoBehaviour
{
    //private Vector2 _scrollPosition; // for the scroll view
    //public AnimationCurve touchPadInputCurve = AnimationCurve.Linear( 0.0f, 0.0f, 1.0f, 1.0f );
    public Vector2 swipeMinMax = new Vector2(1, 10);

    private void Start()
    {
        var recognizer = new TKSwipeRecognizer(swipeMinMax.x, swipeMinMax.y);
        recognizer.gestureRecognizedEvent += (r) =>
        {
            Debug.Log("swipe recognizer fired: " + r);
        };
        TouchKit.addGestureRecognizer(recognizer);
    }
}
