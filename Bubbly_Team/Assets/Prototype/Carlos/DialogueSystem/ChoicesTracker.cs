using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ChoicesTracker : MonoBehaviour
{
    [System.Serializable]
    public class Choice
    {
        public string choiceID;
        public int choiceIndex;

        public bool chosenA = false;
    }

    public List<Choice> choices = new List<Choice>();

    public Choice FindChoice(string inChoiceID)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceID == inChoiceID)
                return choice;
        }

        return null;
    }

    public Choice FindChoice(int inIndex)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceIndex == inIndex)
                return choice;

        }

        return null;
    }

    public bool FindChoiceOutput(string inChoiceID)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceID == inChoiceID)
                return choice.chosenA;

        }

        return false;
    }

    public bool FindChoiceOutput(int inIndex)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceIndex == inIndex)
                return choice.chosenA;

        }

        return false;
    }

    public bool SetChoiceOutput(string inChoiceID, bool inChoice)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceID == inChoiceID)
                return choice.chosenA = inChoice;
        }

        return false;
    }

    public bool SetChoiceOutput(int inIndex, bool inChoice)
    {
        foreach (Choice choice in choices)
        {
            if (choice.choiceIndex == inIndex)
                return choice.chosenA = inChoice;
        }

        return false;
    }
}
