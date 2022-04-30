using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPropagateUI : MonoBehaviour
{
    public static BackPropagateUI Instance;

    private Stimulus currentStimulus;
    private int outputBehaviour;

    [SerializeField]
    private StimuliData stimuliData;

    [SerializeField]
    private Dropdown stimulusFilter;

    private Stimulus filteredStimulus;

    public Text currentStimulusText;
    public Button initializeButton;
    public InputField inputField;

    public List<Text> inputTexts;
    public List<Text> outputTexts;


    void Start()
    {
        Instance = this;
        foreach (Text text in inputTexts) text.text = "";
        foreach (Text text in outputTexts)
        {
            text.gameObject.SetActive(false);
        }
        currentStimulusText.text = "No stimulus to examine";
        initializeButton.gameObject.SetActive(false);
        foreach(var stimulus in stimuliData.stimuli)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = stimulus.name;
            stimulusFilter.options.Add(newOption);
        }
        stimulusFilter.RefreshShownValue();
    }

    public void UpdateFilter()
    {
        Stimulus newFilter = stimuliData.stimuli.Find(stimulus => stimulus.name == stimulusFilter.options[stimulusFilter.value].text);
        if(newFilter)
        {
            filteredStimulus = newFilter;
        }
        else
        {
            filteredStimulus = null;
        }
    }

    public void ShowHide()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }

    public void RandomiseStimulusWeights()
    {
        currentStimulus.RandomiseWeights();
    }

    public void UpdateUI(Stimulus stimulus, int responseGiven)
    {
        if (filteredStimulus != null && filteredStimulus != stimulus) return;

        currentStimulus = stimulus;
        outputBehaviour = responseGiven;
        currentStimulusText.text = stimulus.ToString();
        for(int i = 0; i < inputTexts.Count; ++i)
        {
            if (i < stimulus.inputNodes.Count)
            {
                string text = stimulus.inputNodes[i].inputType.ToString() + " ";
                if (stimulus.inputNodes[i].inputType == StimulusInput.TRAIT) text += stimulus.inputNodes[i].traitType + " ";
                text += stimulus.inputNodes[i].input;
                inputTexts[i].text = text;
            }
            else inputTexts[i].text = "";
        }
        for(int i = 0; i < outputTexts.Count; ++i)
        {
            if (i < stimulus.potentialResponses.Count)
            {
                string text = (stimulus.potentialResponses[i]?.ToString() ?? "No response") + " ";
                text += stimulus.finalValues[i].ToString();
                outputTexts[i].text = text;
                outputTexts[i].gameObject.SetActive(true);
            }
            else
            {
                outputTexts[i].text = "";
                outputTexts[i].gameObject.SetActive(false);
            }
        }
        initializeButton.gameObject.SetActive(true);
    }

    public void InitiateBackPropagation(int expectedBehaviour)
    {
        currentStimulus.CorrectOutput(expectedBehaviour, outputBehaviour);
    }
}
