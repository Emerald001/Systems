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

    private readonly List<FieldInfo> DebugFields = new();
    private readonly List<TextMeshProUGUI> Entries = new();

    private bool IsActive;

    private void Awake() {
        NextTextPos = TextStartPos;

        foreach (FieldInfo field in typeof(DebugVariables).GetFields()) {
            if (field.Name.Contains("EmptyLine")) {
                NextTextPos -= new Vector2(0, 30);
                continue;
            }

            DebugFields.Add(field);

            Entries.Add(Instantiate(TextPrefab, transform.GetChild(0)));
            Entries[^1].transform.localPosition = NextTextPos;
            Entries[^1].text = $"{field.Name}: {field.GetValue(field.GetType())}";

            NextTextPos -= new Vector2(0, 30);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F3))
            ToggleScreen();

        if (!IsActive)
            return;

        for (int i = 0; i < DebugFields.Count; i++)
            Entries[i].text = $"{DebugFields[i].Name}: {DebugFields[i].GetValue(DebugFields[i].GetType())}";
    }

    private void ToggleScreen() {
        IsActive = !IsActive;

        Screen.SetActive(IsActive);
    }
}
