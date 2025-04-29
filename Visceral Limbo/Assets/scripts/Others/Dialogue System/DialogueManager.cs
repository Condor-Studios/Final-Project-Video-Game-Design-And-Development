using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Visceral_Script
{
    [Header("UI")]
    public TextMeshProUGUI DialogueText;
    public Image SpeakerIcon;
    public TextMeshProUGUI SpeakerName;
    public GameObject DialoguePanel;
    public GameObject OptionsPanel;
    public GameObject OptionButtonPrefab;
    public GameObject HandMesh;


    [Header("Settings")]
    [SerializeField] private float _TextSpeed;
    [SerializeField] private int _CurrentNodeIndex = 0;
    [SerializeField] private bool _IsTyping = false;

    [SerializeField]private List<GameObject> CurrentOptionsList = new List<GameObject>();

    private Coroutine TypingCoroutine;
    private Coroutine AutoAdvanceTextCoroutine;

    public DialogueData CurrentDialogue;


    public override void VS_Initialize()
    {
        
    }

    public void StartDialogue(DialogueData DialogeDT)
    {
        CurrentDialogue = DialogeDT;
        _CurrentNodeIndex = 0;
        DialoguePanel.SetActive(true);
        HandMesh.SetActive(true);
        NextNode();
    }


    private void NextNode()
    {
        if(CurrentDialogue == null)
        {
            Debug.LogError("<Color=blue> Visceral Error: Dialoge not found, are you sure you passed a dialoge?</Color>");
        }
        if( _CurrentNodeIndex < 0 || _CurrentNodeIndex >= CurrentDialogue.DialogueNodes.Count)
        {
            EndDialoge();
            return;
        }

        StopAllCoroutines();
        ClearOptions();
        StartCoroutine(TypeText(CurrentDialogue.DialogueNodes[_CurrentNodeIndex]));
        SpeakerName.text = CurrentDialogue.DialogueNodes[_CurrentNodeIndex].SpeakerName;
        SpeakerIcon.sprite = CurrentDialogue.DialogueNodes[_CurrentNodeIndex].SpeakerSprite;
    }

    private IEnumerator TypeText(DialogueNode NodeDT)
    {
        _IsTyping = true;
        DialogueText.text = "";
        

        foreach(char Letter in NodeDT.TextData.ToCharArray())  
        {
            DialogueText.text += Letter;
            yield return new WaitForSeconds(_TextSpeed);
        }

        _IsTyping = false;

        if(NodeDT.Options != null && NodeDT.Options.Count > 0)
        {
            ShowOptions(NodeDT.Options);
        }
        else
        {
            if (NodeDT.AutoAdvance)
            {
                AutoAdvanceTextCoroutine = StartCoroutine(AutoAdvanceDialogue(NodeDT.TimeToAdvance));
            }
        }
    }

    private void ShowOptions(List<DialogeOption> options)
    {
        OptionsPanel.SetActive(true);

        foreach(var Option in options)
        {
            GameObject ButtonOBJ = Instantiate(OptionButtonPrefab, OptionsPanel.transform);
            TMP_Text ButtonText = ButtonOBJ.GetComponentInChildren<TMP_Text>();
            ButtonText.text = Option.TextReply;

            Button Button = ButtonOBJ.GetComponent<Button>();
            int nextOption = Option.nextDialogueID;
            CurrentOptionsList.Add(ButtonOBJ);
        }
    }

    public void SelectOption(int NextIntID)
    {
        _CurrentNodeIndex = NextIntID;
        OptionsPanel.SetActive(false);
        NextNode();
    }
    private void ClearOptions()
    {
        foreach(var Button in CurrentOptionsList)
        {
            Destroy(Button);
        }
        CurrentOptionsList.Clear();
    }

    private IEnumerator AutoAdvanceDialogue(float Duration)
    {

        //esperar la cantidad deseada
        yield return new WaitForSeconds(Duration);

        _CurrentNodeIndex = CurrentDialogue.DialogueNodes[_CurrentNodeIndex].NextDialogeOption;
        NextNode();
    }

    private void EndDialoge()
    {
        DialogueText.text = "";
        DialoguePanel.SetActive(false);
        OptionsPanel.SetActive(false);
        HandMesh.SetActive(false);
        Debug.Log("FinishDialoge");
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartDialogue(CurrentDialogue);
        }
    }

}




