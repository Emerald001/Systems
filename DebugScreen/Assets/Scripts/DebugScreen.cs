using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextPrefab;
    [SerializeField] private GameObject Screen;
    [SerializeField] private Vector2 TextStartPos;

    private Vector2 NextTextPos;

    private List<FieldInfo> DebugFields = new();
    private List<TextMeshProUGUI> Entries = new();

    private bool IsActive;

    private void Start() {
        NextTextPos = TextStartPos;

        foreach (FieldInfo field in typeof(DebugVariables).GetFields()) {
            DebugFields.Add(field);

            TextMeshProUGUI tmp = Instantiate(TextPrefab, transform.GetChild(0));
            tmp.transform.localPosition = NextTextPos;
            tmp.text = $"{field.Name}: {field.GetValue(field.GetType())}";
            Entries.Add(tmp);

            NextTextPos -= new Vector2(0, 30);
        }
    }

    private void Update() {
        for (int i = 0; i < DebugFields.Count; i++) {
            var tmp = Entries[i];
            tmp.text = $"{DebugFields[i].Name}: {DebugFields[i].GetValue(DebugFields[i].GetType())}";
        }

        if (Input.GetKeyDown(KeyCode.F3))
            ToggleScreen();
    }

    private void ToggleScreen() {
        IsActive = !IsActive;

        Screen.SetActive(IsActive);
    }
}
