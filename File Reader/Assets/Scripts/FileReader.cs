using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileReader : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject dialogueSystemObject;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image portrait;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject buttonPrefab;

    [Header("CommandSettings")]
    [SerializeField] private char commandChar;
    [SerializeField] private char optionChar;
    [SerializeField] private char sectionChar;
    [SerializeField] private char autoNextChar;

    [Header("Visual Settings")]
    [SerializeField] private float timeBetweenChars;

    public float CurrentTimeBetweenChars { get; set; }

    private Dictionary<string, string[]> Files = new();
    private DialogFunctionality funcs = new();

    private string[] currentDialog;
    private int index;
    private bool IsWriting;

    void Awake() {
        funcs.Owner = this;
        funcs.SetEvents();

        CurrentTimeBetweenChars = timeBetweenChars;

        var tmp = Resources.LoadAll<TextAsset>("Files/");

        foreach (var item in tmp) {
            Files.Add(item.name, PrepFile(item));
        }
    }

    private string[] PrepFile(TextAsset file) {
        return file.ToString().Replace("\n\r\n", "\n").Split("\n");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            SetDialog("Test 1");
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) {
            NextLine();
        }
    }

    private void SetDialog(string DialogName) {
        if (Files.ContainsKey(DialogName)) {
            index = 0;
            currentDialog = Files[DialogName];
            dialogueSystemObject.SetActive(true);
            NextLine();
        }
        else
            Debug.LogError("No File named " + DialogName + " found!");
    }

    private void NextLine() {
        if (currentDialog == null)
            return;

        if (IsWriting) {
            FullLine(currentDialog[index - 1].Trim());
            return;
        }

        if (currentDialog.Length - 1 < index) {
            StopAllCoroutines();
            currentDialog = null;
            mainText.text = "";
            nameText.text = "";
            index = 0;
            return;
        }

        CurrentTimeBetweenChars = timeBetweenChars;

        var command = CheckCommand(currentDialog[index], commandChar);
        if (command != null) {
            CallCommand(command);

            index++;
            NextLine();
            return;
        }

        var option = CheckCommand(currentDialog[index], optionChar);
        if (option != null) {
            DisplayOptions(currentDialog);
            return;
        }

        var section = CheckCommand(currentDialog[index], sectionChar);
        if (section != null) {
            var line = currentDialog[index].Trim().Split(" ");
            if (line[1] == "jump") {
                JumpToSection(line[2]);
                return;
            }
            if(line[1] == "end") {
                return;
            }
            index++;
            NextLine();
            return;
        }

        DisplayLine();

        index++;
    }

    private void DisplayLine() {
        StopAllCoroutines();
        StartCoroutine(DisplayText(currentDialog[index].Trim()));
    }

    private string[] CheckCommand(string line, char commandChar) {
        var tmp = line.Trim().ToCharArray();

        if (tmp.Length < 1)
            return null;

        if (tmp[0] == commandChar) {
            return line.Split(" ");
        }

        return null;
    }

    private void CallCommand(string[] command) {
        if (float.TryParse(command[2], out var floatParse))
            EventManager<float>.Invoke(ParseEnum<EventType>(command[1]), floatParse);
        else if (bool.TryParse(command[2], out var boolParse))
            EventManager<bool>.Invoke(ParseEnum<EventType>(command[1]), boolParse);
        else 
            EventManager<string>.Invoke(ParseEnum<EventType>(command[1]), command[2]);
    }

    private void DisplayOptions(string[] file) {
        buttonPanel.SetActive(true);

        List<GameObject> buttons = new();

        while (file[index].Trim().ToCharArray()[0] == '@') {
            var tmpButton = Instantiate(buttonPrefab, buttonContainer.transform);
            var text = file[index].Trim().Split(" ", 2)[1];
            tmpButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            tmpButton.GetComponent<Button>().onClick.AddListener(() => RemoveOptions());

            index++;
            
            if (CheckCommand(file[index], sectionChar) != null) {
                if (file[index].Trim().Split(" ")[1] == "jump") {
                    string sectionName = file[index].Trim().Split(" ")[2];
                    tmpButton.GetComponent<Button>().onClick.AddListener(() => JumpToSection(sectionName));
                    index++;
                }
            }

            buttons.Add(tmpButton);
        }
        
        for (int i = 0; i < buttons.Count; i++) {
            var rect = buttons[i].GetComponent<RectTransform>();

            if(buttons.Count % 2 != 0)
                rect.localPosition = new Vector2(0, (400 / buttons.Count) * ((buttons.Count - 1 - i) - (buttons.Count) / 2));
            else
                rect.localPosition = new Vector2(0, (400 / buttons.Count) * ((buttons.Count - 1 - i) - (buttons.Count) / 2) + 200 / buttons.Count);

            rect.sizeDelta = new Vector2(1500, (400 / buttons.Count - 1));
        }
    }

    private void RemoveOptions() {
        buttonPanel.SetActive(false);

        for (int i = buttonContainer.transform.childCount - 1; i >= 0; i--) {
            Destroy(buttonContainer.transform.GetChild(i).gameObject);
        }
    }

    private void JumpToSection(string SectionName) {
        for (int i = 0; i < currentDialog.Length; i++) {
            if (CheckCommand(currentDialog[i], sectionChar) != null) {
                var line = currentDialog[i].Trim().Split(" ");
                if (line[1] == "start" && line[2] == SectionName) {
                    index = i;
                    NextLine();
                    return;
                }
            }
        }
    }

    private IEnumerator DisplayText(string text) {
        IsWriting = true;

        yield return new WaitForEndOfFrame();

        List<char> charList = new();

        var frontAndBack = text.Split(" ", 2);
        var name = frontAndBack[0];
        nameText.text = name;
        var sentence = frontAndBack[1];

        for (int i = 0; i < sentence.Length; i++) {
            if(sentence[i] == commandChar) {
                List<char> command = new() {
                    sentence[i]
                };
                i++;

                while (sentence[i] != commandChar) {
                    command.Add(sentence[i]);
                    i++;
                }

                command.Add(sentence[i]);
                i += 2;

                CallCommand(new string(command.ToArray()).Split(" "));
            }

            if(sentence[i] == '<') {
                List<char> stylePartOne = new();
                List<char> TextBetween = new();
                List<char> stylePartTwo = new();

                stylePartOne.Add(sentence[i]);
                i++;
                while (sentence[i] != '>') {
                    stylePartOne.Add(sentence[i]);
                    i++;
                }
                stylePartOne.Add(sentence[i]);
                i++;

                while (sentence[i] != '<') {
                    TextBetween.Add(sentence[i]);
                    i++;
                }

                stylePartTwo.Add(sentence[i]);
                i++;
                while (sentence[i] != '>') {
                    stylePartTwo.Add(sentence[i]);
                    i++;
                }
                stylePartTwo.Add(sentence[i]);
                i++;

                List<char> tmp = new();
                for (int j = 0; j < TextBetween.Count; j++) {

                    tmp.Add(TextBetween[j]);

                    var Final = new List<char>(charList);
                    Final.AddRange(stylePartOne);
                    Final.AddRange(tmp);
                    Final.AddRange(stylePartTwo);

                    mainText.text = new string(Final.ToArray());
                    //Do Typewriter Noise

                    yield return new WaitForSeconds(CurrentTimeBetweenChars);
                }

                charList.AddRange(stylePartOne);
                charList.AddRange(TextBetween);
                charList.AddRange(stylePartTwo);
            }

            if(i >= sentence.Length) 
                continue;

            charList.Add(sentence[i]);
            mainText.text = new string(charList.ToArray());
            //Do Typewriter Noise

            yield return new WaitForSeconds(CurrentTimeBetweenChars);
        }

        var autoSkip = CheckCommand(currentDialog[index], autoNextChar);
        if (autoSkip != null) {
            index++;
            IsWriting = false;
            NextLine();
        }

        IsWriting = false;
    }

    private void FullLine(string text) {
        StopAllCoroutines();

        List<char> charList = new();

        var frontAndBack = text.Split(" ", 2);
        var name = frontAndBack[0];
        nameText.text = name;
        var sentence = frontAndBack[1];

        int i = 0;

        while (i < sentence.Length) {
            if (sentence[i] == commandChar) {
                List<char> command = new() {
                    sentence[i]
                };
                i++;

                while (sentence[i] != commandChar) {
                    command.Add(sentence[i]);
                    i++;
                }

                command.Add(sentence[i]);
                i += 2;

                CallCommand(new string(command.ToArray()).Split(" "));
            }

            charList.Add(sentence[i]);

            i++;

            if (i >= sentence.Length)
                break;
        }

        mainText.text = new string(charList.ToArray());

        var autoSkip = CheckCommand(currentDialog[index], autoNextChar);
        if (autoSkip != null) {
            index++;
            IsWriting = false;
            NextLine();
        }

        IsWriting = false;
    }

    private T ParseEnum<T>(string value) {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}