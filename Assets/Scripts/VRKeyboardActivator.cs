using TMPro;
using UnityEngine;
using WPinyin;

public class VRKeyboardActivator : MonoBehaviour
{
    public GameObject keyboardPrefab;
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        // asegura que el teclado esté desactivado al inicio
        if (keyboardPrefab != null)
            keyboardPrefab.SetActive(false);

        inputField.onSelect.AddListener(OnInputSelected);
        inputField.onDeselect.AddListener(OnInputDeselected);
    }

    void OnInputSelected(string _)
    {
        if (keyboardPrefab != null)
        {
            keyboardPrefab.SetActive(true);
            KeyboardManager.SetTargetField(inputField);
        }
    }

    void OnInputDeselected(string _)
    {
        if (keyboardPrefab != null)
            keyboardPrefab.SetActive(false);
    }
}
