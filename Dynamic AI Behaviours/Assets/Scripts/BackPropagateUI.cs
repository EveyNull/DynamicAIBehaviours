using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPropagateUI : MonoBehaviour
{
    public static BackPropagateUI Instance;

    private Stimulus currentStimulus;
    private int outputBehaviour;
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
    }

    public void RandomiseStimulusWeights()
    {
        currentStimulus.RandomiseWeights();
    }

    public void UpdateUI(Stimulus stimulus, int responseGiven)
    {
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
