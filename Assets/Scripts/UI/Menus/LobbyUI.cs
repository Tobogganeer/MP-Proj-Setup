using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public RawImage pfpImage;
    public TMP_Text steamNameText;

    [HideInInspector]
    public int index;

    public bool IsNull()
    {
        return gameObject == null || pfpImage.gameObject == null || steamNameText.gameObject == null;
    }

    // TODO: Maybe options for each character model later
}
