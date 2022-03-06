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
    public List<float> weights;
    public List<float> biases;
    public List<float> outputs;
}

[System.Serializable]
public class MiddleNode
{
    public float summedInput;
    public List<float> weights;
    public List<float> biases;
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
                        inputNode.weights.Add(0);
                    }
                }
            }
            if(inputNode.biases.Count != middleNodeNumber && middleNodeNumber != 0)
            {
                List<float> previousBiases = inputNode.biases;
                for (int i = 0; i < middleNodeNumber; ++i)
                {
                    if (i < previousBiases.Count)
                    {
                        inputNode.biases.Add(previousBiases[i]);
                    }
                    else
                    {
                        inputNode.biases.Add(1);
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
                    if(middleNodes[i].biases.Count > potentialResponses.Count)
                    {
                        middleNodes[i].biases.RemoveRange(potentialResponses.Count, middleNodes[i].weights.Count - potentialResponses.Count);
                    }
                }
                else
                {
                    MiddleNode newNode = new MiddleNode();
                    newNode.weights = new List<float>();
                    newNode.biases = new List<float>();
                    for (int j = 0; j < potentialResponses.Count; ++j)
                    {
                        newNode.weights.Add(0);
                        newNode.biases.Add(1);
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
            if(node.biases.Count > potentialResponses.Count)
            {
                node.biases.RemoveRange(potentialResponses.Count, node.weights.Count - potentialResponses.Count);
            }
            while(node.weights.Count < potentialResponses.Count)
            {
                node.weights.Add(0);
            }
            while(node.biases.Count < potentialResponses.Count)
            {
                node.biases.Add(1);
            }
        }

        finalValues = new List<float>();
        for(int i = 0; i < potentialResponses.Count; ++i)
        {
            finalValues.Add(0.0f);
        }
    }

    public GoalBehaviour GetBehaviour(Agent subject, Agent target)
    {
        for (int i = 0; i < inputNodes.Count; ++i)
        {
            float output = GetNodeOutput(subject, target, inputNodes[i]);
            inputNodes[i].outputs.Clear();
            inputNodes[i].outputs = new List<float>();
            for(int j = 0; j < inputNodes[i].weights.Count; ++j)
            {
                inputNodes[i].outputs.Add((output * inputNodes[i].weights[j]) + inputNodes[i].biases[j]);
            }
        }
        List<float> outputNodeInputs = new List<float>(potentialResponses.Count);
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
                middleNodes[i].outputs.Add((input * middleNodes[i].weights[j]) + middleNodes[i].biases[j]);
            }
        }
        for(int i = 0; i < outputNodeInputs.Capacity; ++i)
        {
            outputNodeInputs.Add(0.0f);
            for(int j = 0; j < middleNodes.Count; ++j)
            {
                outputNodeInputs[i] += middleNodes[j].outputs[i];
            }
            float finalOutput = 0.0f;
            foreach(var input in outputNodeInputs)
            {
                finalOutput += input;
            }
            finalOutput /= middleNodeNumber;
            finalValues[i] = finalOutput;
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

        return potentialResponses[highestIndex];
    }

    public void CorrectOutputWithValue(int indexOfBehaviour, float expectedValue)
    {
        float trueValue = finalValues[indexOfBehaviour];
        float error = CalculateError(trueValue, expectedValue);
        BackPropagate(error);
    }

    private float CalculateError(float outputValue, float correctValue)
    {
        return Mathf.Abs(correctValue - outputValue);
    }

    private void BackPropagate(float error)
    {
        for(int i = 0; i < middleNodes.Count; ++i)
        {
            float middleError = 0.0f;
            for (int j = 0; j < middleNodes[i].outputs.Count; ++j)
            {
                float biasDiff = Sigmoid(middleNodes[i].outputs[j]) * error;
                middleNodes[i].biases[j] += biasDiff;
                float weightDiff = middleNodes[i].outputs[j] * biasDiff;
                middleNodes[i].weights[j] += weightDiff;

                float middleNodeExpectedOutput = (middleNodes[i].summedInput * middleNodes[i].weights[j]) + middleNodes[i].biases[j];
                middleError += ((middleNodeExpectedOutput - middleNodes[i].outputs[j]) - middleNodes[i].biases[j]) / middleNodes[i].weights[j];
            }
            middleError /= potentialResponses.Count;
            for(int j = 0; j < inputNodes.Count; ++j)
            {
                float biasDiff = Sigmoid(inputNodes[j].outputs[i]) * middleError;
                inputNodes[j].biases[j] += biasDiff;
                float weightDiff = inputNodes[j].outputs[i] * biasDiff;
                inputNodes[j].weights[i] += weightDiff;
            }
        }
    }


    private float Sigmoid(float input)
    {
        return 1.0f / (1.0f + Mathf.Exp(-input));
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
