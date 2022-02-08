using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputProfile
{
    public KeyCode forward = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode backwards = KeyCode.S;
    public KeyCode special = KeyCode.Space;

    public InputProfile()
    {
        forward = KeyCode.W;
        left = KeyCode.A;
        right = KeyCode.D;
        backwards = KeyCode.S;
        special = KeyCode.Space;
    }
}
