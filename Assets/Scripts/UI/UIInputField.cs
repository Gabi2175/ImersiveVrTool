using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class UIInputField : MonoBehaviour
{
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        if ( _inputField == null )
            enabled = false;
    }

    public void ChangeText()
    {
        KeyboardManager.Instance.GetInput((result) =>
        {
            _inputField.text = result;
        }, null, _inputField.text);
    }
}
