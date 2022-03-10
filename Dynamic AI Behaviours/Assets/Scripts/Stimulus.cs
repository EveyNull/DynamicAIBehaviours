using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StimulusType
{
    ADJACENCY,
    GREETED,
    INSULTED,
    NEEDHALF,
    NEEDCRITICAL,
    ASKEDFORFOOD,
    COUNT
}

public enum StimulusInput
{
    TRAIT,
    RELATIONSHIP,
    MOOD,
    HEALTH,
    COUNT
}

[System.Serializable]
public class StimulusInputNode
{
    public StimulusInput inputType;
    public PersonalityTrait traitType;
    public float input;
    public List<float> weights;
    public float bias;
    public List<float> outputs;
}

[System.Serializable]
public class MiddleNode
{
    public float summedInput;
    public List<float> weights;
    public float bias;
    public List<float> outputs;
}

[CreateAssetMenu(fileName = "NewStimulus", menuName = "ScriptableObjects/CreateStimulus")]
public class Stimulus : ScriptableObject
{
    public StimulusType type;
    public bool overrideCurrentGoal = false;
    public int middleNodeNumber = 3;
    public List<StimulusInputNode> inputNodes;
    public List<MiddleNode> middleNodes;
    public List<GoalBehaviour> potentialResponses;
    public List<float> finalValues;

    private void OnValidate()
    {
        foreach (StimulusInputNode inputNode in inputNodes)
        {
            if (inputNode.inputType != StimulusInput.TRAIT) inputNode.traitType = PersonalityTrait.NONE;
            if (inputNode.weights.Count != middleNodeNumber && middleNodeNumber != 0)
            {
                List<float> previousWeightings = inputNode.weights;
                inputNode.weights = new List<float>();
                for(int i = 0; i < middleNodeNumber; ++i)
                {
                    if (i < previousWeightings.Count)
                    {
                        inputNode.weights.Add(previousWeightings[i]);
                    }
                    else
                    {
                        inputNode.weights.Add(Random.Range(-1.0f, 1.0f));
                    }
                }
            }
        }
        if (middleNodes == null)
        {
            middleNodes = new List<MiddleNode>();
        }
        if (middleNodes.Count != middleNodeNumber && middleNodeNumber != 0)
        {
            middleNodes = new List<MiddleNode>();
            List<MiddleNode> previousWeightings = middleNodes;
            for (int i = 0; i < middleNodeNumber; ++i)
            {
                if (i < previousWeightings.Count)
                {
                    middleNodes.Add(previousWeightings[i]);
                    if (middleNodes[i].weights.Count > potentialResponses.Count)
                    {
                        middleNodes[i].weights.RemoveRange(potentialResponses.Count, middleNodes[i].weights.Count - potentialResponses.Count);
                    }
                }
                else
                {
                    MiddleNode newNode = new MiddleNode();
                    newNode.weights = new List<float>();
                    for (int j = 0; j < potentialResponses.Count; ++j)
                    {
                        newNode.weights.Add(Random.Range(-1.0f, 1.0f)); ;
                    }
                    middleNodes.Add(newNode);
                }
            }
        }
        foreach(MiddleNode node in middleNodes)
        {
            if(node.weights.Count > potentialResponses.Count)
            {
                node.weights.RemoveRange(potentialResponses.Count, node.weights.Count - potentialResponses.Count);
            }
            while(node.weights.Count < potentialResponses.Count)
            {
                node.weights.Add(0);
            }
        }

        finalValues = new List<float>();
        for(int i = 0; i < potentialResponses.Count; ++i)
        {
            finalValues.Add(0.0f);
        }
    }

    public void RandomiseWeights()
    {
        foreach(var node in inputNodes)
        {
            for(int i = 0; i < node.weights.Count; ++i)
            {
                node.weights[i] = Random.Range(-1.0f, 1.0f);
                node.bias = 1;
            }
        }
        foreach (var node in middleNodes)
        {
            for (int i = 0; i < node.weights.Count; ++i)
            {
                node.weights[i] = Random.Range(-1.0f, 1.0f);
                node.bias = 1;
            }
        }
    }

    public GoalBehaviour GetBehaviour(Agent subject, Agent target)
    {
        for (int i = 0; i < inputNodes.Count; ++i)
        {
            float output = GetNodeOutput(subject, target, inputNodes[i]);
            inputNodes[i].input = output;
            inputNodes[i].outputs.Clear();
            inputNodes[i].outputs = new List<float>();
            for(int j = 0; j < inputNodes[i].weights.Count; ++j)
            {
                inputNodes[i].outputs.Add((output * inputNodes[i].weights[j]) + inputNodes[i].bias);
            }
        }
        for(int i = 0; i < middleNodes.Count; ++i)
        {
            float input = 0;
            for(int j = 0; j < inputNodes.Count; ++j)
            {
                input += inputNodes[j].outputs[i];
            }

            input /= inputNodes.Count;

            middleNodes[i].summedInput = input;

            middleNodes[i].outputs.Clear();
            middleNodes[i].outputs = new List<float>();

            for(int j = 0; j < middleNodes[i].weights.Count; ++j)
            {
                middleNodes[i].outputs.Add((input * middleNodes[i].weights[j]) + middleNodes[i].bias);
            }
        }
        List<float> outputNodeInputs = new List<float>(potentialResponses.Count);
        for (int i = 0; i < outputNodeInputs.Capacity; ++i)
        {
            outputNodeInputs.Add(0.0f);
            for(int j = 0; j < middleNodes.Count; ++j)
            {
                outputNodeInputs[i] += middleNodes[j].outputs[i];
            }
            outputNodeInputs[i] /= middleNodeNumber;
            finalValues[i] = outputNodeInputs[i];
        }

        string outputString = string.Join(",", finalValues);

        float highestValue = float.NegativeInfinity;
        int highestIndex = 0;
        for(int i = 0; i < finalValues.Count; ++i)
        {
            if(finalValues[i] > highestValue)
            {
                highestValue = finalValues[i];
                highestIndex = i;
            }
        }

        int responseGiven = highestIndex;

        BackPropagateUI.Instance.UpdateUI(this, responseGiven);

        return potentialResponses[highestIndex];
    }

    public void CorrectOutput(int indexOfExpectedBehaviour, int indexOfActualBehaviour)
    {
        BackPropagate(Sigmoid(finalValues[indexOfExpectedBehaviour] - finalValues[indexOfActualBehaviour]), indexOfExpectedBehaviour);

    }

    private void BackPropagate(float error, int correctIndex)
    {
        List<float> sErrors = new List<float>();
        for(int i = 0; i < potentialResponses.Count; ++i)
        {
            float correctOutput;
            if (i == correctIndex) correctOutput = (finalValues[i] + error);
            else correctOutput = (finalValues[i] - error);

            float nodeError = finalValues[i] - correctOutput;
            float sNodeError = nodeError * Derivative(Sigmoid(finalValues[i]));
            sErrors.Add(sNodeError);

            for(int j = 0; j < middleNodeNumber; ++j)
            {
                middleNodes[j].weights[i] -= sNodeError * middleNodes[j].outputs[i];
                middleNodes[j].bias -= sNodeError;
            }
        }

        for(int i = 0; i < middleNodeNumber; ++i)
        {
            float sNodeError = 0;
            for(int j = 0; j < potentialResponses.Count; ++j)
            {
                sNodeError += sErrors[j] * middleNodes[i].weights[j];

            }
            sNodeError *= Derivative(Sigmoid(middleNodes[i].summedInput));

            for(int j = 0; j < inputNodes.Count; ++j)
            {
                inputNodes[j].weights[i] -= sNodeError * inputNodes[j].outputs[i];
                inputNodes[j].bias -= sNodeError;
            }
        }
    }

    private float Derivative(float input)
    {
        float output = input * (1 - input);
        return output;
    }

    private float Sigmoid(float input)
    {
        float output = 1.0f / (1.0f + Mathf.Exp(-input));
        return output;
    }

    private float GetNodeOutput(Agent subject, Agent target, StimulusInputNode node)
    {
        switch (node.inputType)
        {
            case StimulusInput.TRAIT:
                {
                    return subject.personalityTraits.Contains(node.traitType) ? 1 : 0;
                }
            case StimulusInput.RELATIONSHIP:
                {
                    if(target == null || subject.relationships.ContainsKey(target) == false)
                    {
                        return 0;
                    }
                    return subject.relationships[target];
                }
            case StimulusInput.MOOD:
                {
                    return subject.GetMood();
                }
            default:
                return 0;
        }
    }
}
