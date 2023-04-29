using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private GameObject Screen;
    [SerializeField] private TMP_InputField field;
    [SerializeField] private TextMeshProUGUI AutoCompleteArea;

    private readonly List<string> autoCompletes = new();
    private readonly ConsoleCommands commands = new();

    private int autoCompleteIndex = 0;
    private bool IsActive;

    private void Start() {
        field.onValueChanged.AddListener(AutoCompleteEntries);

        commands.InitDict();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F2))
            ToggleScreen();

        if (!IsActive)
            return;

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!field.text.Contains(' '))
                throw new Exception("Must be two Bits");

            var split = field.text.Split(' ');

            if (commands.actions.ContainsKey(split[0]))
                if (float.TryParse(split[1], out float value))
                    commands.actions[split[0]].Invoke(value);
                else
                    throw new Exception("Second Bit Must be a Value");
            else
                throw new Exception("No Such Command");

            ToggleScreen();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            AutoComplete();
    }

    private void ToggleScreen() {
        IsActive = !IsActive;

        Screen.SetActive(IsActive);

        if (IsActive)
            field.Select();
        else {
            field.ReleaseSelection();
            field.text = "";
            AutoCompleteArea.text = "";
        }
    }

    private void AutoCompleteEntries(string value) {
        autoCompletes.Clear();
        autoCompleteIndex = 0;

        if (value == "") {
            AutoCompleteArea.text = "";
            return;
        }

        foreach (var item in commands.actions.Keys.ToList())
            if (item.ToLower().Contains(value.ToLower()))
                autoCompletes.Add(item);

        string output = "";
        foreach (var item in autoCompletes)
            output += item + "\n";
        AutoCompleteArea.text = output;
    }

    private void AutoComplete() {
        field.onValueChanged.RemoveAllListeners();

        field.text = autoCompletes[autoCompleteIndex];
        field.MoveToEndOfLine(false, false);

        autoCompleteIndex++;

        if (autoCompleteIndex > autoCompletes.Count - 1)
            autoCompleteIndex = 0;

        field.onValueChanged.AddListener(AutoCompleteEntries);
    }
}

public class ConsoleCommands
{
    public Dictionary<string, Action<float>> actions = new();

    public void InitDict() {
        actions.Add("/AddMoney", AddMoney);
        actions.Add("/AddLevel", AddLevel);
        actions.Add("/AddItems", AddItems);
    }

    private void AddMoney(float value) {
        DebugVariables.PlayerMoney += value;
    }

    private void AddLevel(float value) {
        DebugVariables.PlayerLevel += value;
    }

    private void AddItems(float value) {
        DebugVariables.PlayerItems += value;
    }
}
