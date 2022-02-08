using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsText : MonoBehaviour
{
    public TMPro.TMP_Text text;

    private void Start()
    {
        text.text = @$"
Movement: {Inputs.Forward}{Inputs.Left}{Inputs.Backwards}{Inputs.Right}
Special: {Inputs.Special}
Console: ~
";
    }
}
