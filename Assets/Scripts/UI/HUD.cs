using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD instance;
    
    private void Awake()
    {
        instance = this;
    }

    public CanvasGroup masterGroup;

    [Space]
    public GameObject playerInteractIcon;
    public GameObject scourgeInteractIcon;

    [Space]
    public CanvasGroup oxygenHolder;
    public Image oxygenFillBar;

    [Space]
    public CanvasGroup staminaHolder;
    public Image staminaFillBar;
    public Gradient fillBarGradient;

    [Space]
    public GameObject flashlightIcon;
    public GameObject minimapIcon;

    [Space]
    public GameObject scourgeEyeIcon;
    public Image scourgeEyeFill;

    [Space]
    public Image interactCooldownFill;

    [Space]
    public CanvasGroup injuredVignette;
    public CanvasGroup oxygenVignette;

    private static float stamina = 1;
    private static float oxygen = 1;
    const float FadeSpeed = 7;
    const float MinScourgeEyeDelay = 3;
    static float scourgeTimer;

    static float maxInteractCooldown;
    static float interactCooldown;

    public bool HUDVisible = true;

    private void Start()
    {
        oxygenHolder.alpha = 0f;
        staminaHolder.alpha = 0f;

        stamina = 1f;
        oxygen = 1f;

        flashlightIcon.SetActive(false);
        minimapIcon.SetActive(false);

        scourgeEyeIcon.SetActive(false);
        oxygenVignette.alpha = 0;
        injuredVignette.alpha = 0;
    }

    private void Update()
    {
        masterGroup.alpha = HUDVisible ? 1 : 0;

        oxygenFillBar.fillAmount = oxygen;
        oxygenFillBar.color = fillBarGradient.Evaluate(oxygen);

        if (oxygen < 1f)
            oxygenHolder.alpha = Mathf.Lerp(oxygenHolder.alpha, 1f, Time.deltaTime * FadeSpeed);
        else
            oxygenHolder.alpha = Mathf.Lerp(oxygenHolder.alpha, 0f, Time.deltaTime * FadeSpeed);


        staminaFillBar.fillAmount = stamina;
        staminaFillBar.color = fillBarGradient.Evaluate(stamina);

        if (stamina < 1f)
            staminaHolder.alpha = Mathf.Lerp(staminaHolder.alpha, 1f, Time.deltaTime * FadeSpeed);
        else
            staminaHolder.alpha = Mathf.Lerp(staminaHolder.alpha, 0f, Time.deltaTime * FadeSpeed);


        scourgeTimer -= Time.deltaTime;

        if (scourgeTimer > 0)
        {
            scourgeEyeIcon.SetActive(true);
            scourgeEyeFill.fillAmount = Remap.Float(scourgeTimer, 0, MinScourgeEyeDelay, 0, 1);
        }
        else
        {
            scourgeEyeIcon.SetActive(false);
        }


        interactCooldown -= Time.deltaTime;

        interactCooldownFill.fillAmount = Remap.Float(Mathf.Clamp(interactCooldown, 0, maxInteractCooldown), 0, maxInteractCooldown, 0, 1);
        interactCooldownFill.gameObject.SetActive(interactCooldown > 0);
    }


    public static void SetPlayerInteract(bool enabled)
    {
        instance.playerInteractIcon.SetActive(enabled);
    }

    public static void SetScourgeInteract(bool enabled)
    {
        instance.scourgeInteractIcon.SetActive(enabled);
    }


    public static void SetStamina(float percent0_1)
    {
        stamina = Mathf.Clamp01(percent0_1);
    }

    public static void SetOxygen(float percent0_1)
    {
        oxygen = Mathf.Clamp01(percent0_1);

        float emptyPercent = Remap.Float(oxygen, 0, 1, 1, 0);
        //emptyPercent *= emptyPercent;
        instance.oxygenVignette.alpha = Mathf.Clamp01(emptyPercent + 0.15f);
    }


    public static void SetFlashlight(bool on) => instance.flashlightIcon.SetActive(on);
    public static void SetMinimap(bool on) => instance.minimapIcon.SetActive(on);

    public static void OnScourgeEyeFlash()
    {
        scourgeTimer = MinScourgeEyeDelay;
    }

    public static void SetInteractCooldown(float cooldown)
    {
        maxInteractCooldown = cooldown;
        interactCooldown = cooldown;
    }
}
