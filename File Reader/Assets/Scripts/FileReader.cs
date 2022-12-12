using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool thingy;
    private float thingy2;

    void Awake() {
        funcs.SetEvents();

        Debug.Log(ParseType("Boolean"));
        Debug.Log(ParseType("Boolean") == typeof(bool));

        var tmp = Resources.LoadAll<TextAsset>("Files/");

        if (tmp.Length < 1) {
            Debug.Log("No Files");
            return;
        }

        foreach (var item in tmp) {
            Files.Add(item.name, PrepFile(item));
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
            print(command[0] + command[1]);
            CallCommand(command);
            return;
        }

        DisplayLine();
    }

    private void DisplayLine() {
        StopAllCoroutines();
        StartCoroutine(DisplayText(currentDialog[index]));
    }

    private string[] CheckCommand(string line, char commandChar) {
        var tmp = line.Trim().ToCharArray();
        Debug.Log(tmp[0]);

        if (tmp[0] == commandChar) {
            return line.Split(" ");
        }

        return null;
    }

    private void CallCommand(string[] command) {
        Debug.Log(EventCommand + " Called with " + command[1] + " as Command.");

        //replace with parse system, you know what I mean
        if (ParseType(command[2]) == typeof(float))
            EventManager<float>.Invoke(ParseEnum<EventType>(command[1]), float.Parse(command[3]));
        if (ParseType(command[2]) == typeof(bool))
            EventManager<bool>.Invoke(ParseEnum<EventType>(command[1]), bool.Parse(command[3]));
        if (ParseType(command[2]) == typeof(string))
            EventManager<string>.Invoke(ParseEnum<EventType>(command[1]), command[3]);

        NextLine();
    }

    private void DisplayOptions() {

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
    
    private T ParseEnum<T>(string value) {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    private Type ParseType(string value) {
        return Type.GetType("System." + value);
    }
}