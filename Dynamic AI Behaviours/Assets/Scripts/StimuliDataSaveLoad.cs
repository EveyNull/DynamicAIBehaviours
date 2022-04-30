using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimuliDataSaveLoad : MonoBehaviour
{
    [SerializeField]
    private StimuliData stimuliData;

    private bool saveLoadRoutineRunning = false;
    string[] delimiter = { "EndStimulus\n" };

    // Start is called before the first frame update
    void Start()
    {
        FileBrowser.DisplayedEntriesFilter += (entry) =>
        {
            if (entry.IsDirectory) return true;
            return entry.Extension.Equals(".txt");
        };
    }

    public void Save()
    {
        if (saveLoadRoutineRunning == false)
        {
            StartCoroutine(SaveCoroutine());
        }
    }

    public void Load()
    {
        if(saveLoadRoutineRunning == false)
        {
            StartCoroutine(LoadCoroutine());
        }
    }

    private IEnumerator SaveCoroutine()
    {
        saveLoadRoutineRunning = true;
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders);

        if(FileBrowser.Success)
        {
            string fullPath = FileBrowser.Result[0];
            int directoryEndIndex = fullPath.LastIndexOf('\\')+1;
            string filePath = FileBrowserHelpers.CreateFileInDirectory(fullPath.Substring(0, directoryEndIndex), fullPath.Substring(directoryEndIndex, fullPath.Length - directoryEndIndex)+".txt");

            string writeText = "";
            foreach (var stimulus in stimuliData.stimuli)
            {
                writeText += stimulus.name + "\n"
                    + stimulus.inputNodes.Count + "," + stimulus.middleNodeNumber + "," + stimulus.potentialResponses.Count + "\n" 
                    + "input nodes: \n";
                foreach(var inputNode in stimulus.inputNodes)
                {
                    foreach(var weight in inputNode.weights)
                    {
                        writeText += weight + ",";
                    }
                    writeText += inputNode.bias + "\n";
                }
                writeText += "middle nodes: \n";
                foreach(var middleNode in stimulus.middleNodes)
                {
                    foreach(var weight in middleNode.weights)
                    {
                        writeText += weight + ",";
                    }
                    writeText += middleNode.bias + "\n";
                }
                writeText += delimiter[0];
            }
            FileBrowserHelpers.WriteTextToFile(filePath, writeText);
        }

        saveLoadRoutineRunning = false;
    }


    private IEnumerator LoadCoroutine()
    {
        saveLoadRoutineRunning = true;
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

        if(FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            string test = FileBrowserHelpers.ReadTextFromFile(filePath);
            string[] fileData = test.Split(delimiter, System.StringSplitOptions.None);
            if(fileData.Length <= 1)
            {
                Debug.LogError("Could not read stimuli data from file!");
                yield break;
            }
            if(fileData.Length != stimuliData.stimuli.Count)
            {
                Debug.LogWarning("Number of entries in stimuli data file does not match number of stimuli, not all stimuli will be loaded with file data");
            }
            foreach(string stimulusEntry in fileData)
            {
                if (stimulusEntry == "") continue;
                string[] stimulusEntryData = stimulusEntry.Split('\n');
                string stimulusName = stimulusEntryData[0];
                Stimulus stimulusToUpdate = null;
                foreach(Stimulus stimulus in stimuliData.stimuli)
                {
                    if (stimulus.name == stimulusName) stimulusToUpdate = stimulus;
                }
                if(stimulusToUpdate == null)
                {
                    Debug.LogError("Could not find stimulus with name: " + stimulusName);
                    continue;
                }

                string[] nodeValues = stimulusEntryData[1].Split(',');
                int inputNodeNumber = int.Parse(nodeValues[0]);
                int middleNodeNumber = int.Parse(nodeValues[1]);
                int potentialResponses = int.Parse(nodeValues[2]);
                int i;
                for (i = 0; i < inputNodeNumber; ++i)
                {
                    string[] inputNodeData = stimulusEntryData[i+3].Split(',');
                    int j;
                    for (j = 0; j < middleNodeNumber; ++j)
                    {
                        if (inputNodeData[j] == "") continue;
                        stimulusToUpdate.inputNodes[i].weights[j] = float.Parse(inputNodeData[j]);
                    }
                    stimulusToUpdate.inputNodes[i].bias = float.Parse(inputNodeData[j]);
                }
                int endOfInputs = i+3;
                for(i = 0; i < middleNodeNumber; ++i)
                {
                    string[] middleNodeData = stimulusEntryData[i + endOfInputs + 1].Split(',');
                    int j; 
                    for (j = 0; j < potentialResponses; ++j)
                    {
                        if (middleNodeData[j] == "") continue;
                        stimulusToUpdate.middleNodes[i].weights[j] = float.Parse(middleNodeData[j]);
                    }
                    stimulusToUpdate.middleNodes[i].bias = float.Parse(middleNodeData[j]);
                }
            }
        }
        saveLoadRoutineRunning = false;
    }
}
