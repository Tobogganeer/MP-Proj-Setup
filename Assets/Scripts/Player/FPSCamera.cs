using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FPSCamera : MonoBehaviour
{
    public static FPSCamera instance;
    private void Awake()
    {
        instance = this;
    }

    public Transform holder;
    public float sens = 3;
    public float max = 15;
    public float qSmooth = 8;
    public float vSmooth = 3;

    float x;
    float y;

    // TODO: Smoothing / lower sens when nearing max

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (holder == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;

        x += mouseX;
        y += mouseY;

        x = Mathf.Clamp(x, -max, max);
        y = Mathf.Clamp(y, -max, max);

        holder.localRotation = Quaternion.Slerp(holder.localRotation, Quaternion.Euler(x, y, 0), Time.deltaTime * qSmooth);
        x = Mathf.Lerp(x, 0, Time.deltaTime * vSmooth);
        y = Mathf.Lerp(y, 0, Time.deltaTime * vSmooth);
    }

    public static void Shake(ShakePreset preset)
    {
        Shaker.ShakeAllSeparate(preset);
    }
}
