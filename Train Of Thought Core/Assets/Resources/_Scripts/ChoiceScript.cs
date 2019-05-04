using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Text))]
public class ChoiceScript : MonoBehaviour {
    public enum Gender
    {
        none,
        male,
        female
    }
    public Gender gender = Gender.none;
    public bool solid = false;
    public string description;
    public AudioClip decisionSound;
}
