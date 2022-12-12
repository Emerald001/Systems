using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TestEnum { 
    ON_CALL_EVENT
}

public class FileReader : MonoBehaviour {
    public Text debugtext;

    private Dictionary<string, string[]> Files = new();
    private List<char> Commands = new();
    private string[] currentDialog;
    private DialogFunctionality funcs = new();

    [Header("CommandSettings")]
    public char EventCommand;
    public char SpriteCommand;

    private int index;

    void Awake() {
        funcs.SetEvents();

        var tmp = Resources.LoadAll<TextAsset>("Files/");

        if (tmp.Length < 1) {
            Debug.Log("No Files");
            return;
        }

        foreach (var item in tmp) {
            Files.Add(item.name, PrepFile(item));

            Debug.Log(item.name + " " + Files[item.name].Length.ToString());
        }

        Commands.Add(EventCommand);
        Commands.Add(SpriteCommand);
    }

    private string[] PrepFile(TextAsset file) {
        return file.ToString().Split("\n");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            NextLine();
        }
    }

    private void NextLine() {
        if(currentDialog != null)
            index++;
        else {
            currentDialog = Files["Test 1"];
        }

        if (currentDialog.Length - 1 < index) {
            currentDialog = null;
            index = 0;
            return;
        }

        var command = CheckCommand(currentDialog[index], EventCommand);
        if (command != null) {
            Debug.Log(EventCommand + " Called with " + command[0] + " as Command.");

            if (ParseType(command[1]) == typeof(float))
                EventManager<float>.Invoke(ParseEnum<EventType>(command[0]), float.Parse(command[2]));
            if (ParseType(command[1]) == typeof(bool))
                EventManager<bool>.Invoke(ParseEnum<EventType>(command[0]), bool.Parse(command[2]));
            if (ParseType(command[1]) == typeof(string))
                EventManager<string>.Invoke(ParseEnum<EventType>(command[0]), command[2]);

            NextLine();
            return;
        }

        DisplayLine();
    }

    private void DisplayLine() {
        StopAllCoroutines();
        StartCoroutine(DisplayText(currentDialog[index]));
    }

    private string[] CheckCommand(string line, char commandChar) {
        var tmp = line.ToCharArray();

        if (tmp[0] == commandChar) {
            var output = line.Split(" "[0]);
            var output2 = output[1].Split(" ");

            return output2;
        }

        return null;
    }

    //private void SendCommand(string event) => EventManager.Invoke(EventType.event);

    private void DisplayCommand(TestEnum test) {
        Debug.Log((int)test);
    }

    private IEnumerator DisplayText(string text) {
        List<char> charList = new();

        for (int i = 0; i < text.Length; i++) {
            charList.Add(text[i]);

            debugtext.text = new string(charList.ToArray());
            //Do Typewriter Noise

            yield return new WaitForSeconds(.1f);
        }
    }
    
    public static T ParseEnum<T>(string value) {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    public static Type ParseType(string value) {
        return Type.GetType(value);
    }
}