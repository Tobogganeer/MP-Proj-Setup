using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inputs
{
    public static KeyCode Forward => Profile.forward;
    public static KeyCode Left => Profile.left;
    public static KeyCode Right => Profile.right;
    public static KeyCode Backwards => Profile.backwards;
    public static KeyCode Special => Profile.special;

    private static InputProfile profile;
    public static InputProfile Profile
    {
        get
        {
            if (profile == null) profile = new InputProfile();

            return profile;
        }

        set
        {
            profile = value;
        }
    }

    public static float Horizontal { get; private set; }
    public static float Vertical { get; private set; }

    public static float HorizontalNoSmooth { get; private set; }
    public static float VerticalNoSmooth { get; private set; }

    //private const float AXIS_SPEED = 3;
    //private const float SNAP_THRESHOLD = 0.95f;

    public static void Update()
    {
        if (Profile == null)
        {
            Debug.Log("Had null input profile, using default.");
            Profile = new InputProfile();
        }

        float targetHor = 0;
        float targetVert = 0;

        if (Input.GetKey(Forward)) targetVert++;
        if (Input.GetKey(Backwards)) targetVert--;
        if (Input.GetKey(Left)) targetHor--;
        if (Input.GetKey(Right)) targetHor++;

        Horizontal = Mathf.MoveTowards(Horizontal, targetHor, Time.deltaTime * 3);// AXIS_SPEED);
        Vertical = Mathf.MoveTowards(Vertical, targetVert, Time.deltaTime * 3);// AXIS_SPEED);

        // Through logging I found that this matches EXACTLY with Input.GetAxis (MoveTowards by Time.deltaTime * 3)

        Horizontal = Mathf.Clamp(Horizontal, -1f, 1f);
        Vertical = Mathf.Clamp(Vertical, -1f, 1f);

        HorizontalNoSmooth = targetHor;
        VerticalNoSmooth = targetVert;

        //Horizontal = Mathf.Lerp(Horizontal, targetHor, Time.deltaTime * AXIS_SPEED);
        //Vertical = Mathf.Lerp(Vertical, targetVert, Time.deltaTime * AXIS_SPEED);

        //if (Horizontal > SNAP_THRESHOLD) Horizontal = 1f;
        //else if (Horizontal < -SNAP_THRESHOLD) Horizontal = -1f;
        //
        //if (Vertical > SNAP_THRESHOLD) Vertical = 1f;
        //else if (Vertical < -SNAP_THRESHOLD) Vertical = -1f;
    }
}
