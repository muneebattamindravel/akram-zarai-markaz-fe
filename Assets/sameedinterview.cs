using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sameedinterview : MonoBehaviour
{
    string myString = "My name is Sameed";
    Stack<string> myStack = new Stack<string>();
 

    private void Start()
    {
        string tempString = "";
        for(int i = 0;i< myString.Length;i++)
        {
            if (myString[i] != ' ')
            {
                tempString += myString[i];
            }
            else
            {
                myStack.Push(tempString);
                tempString = "";
            }
        }
        myStack.Push(tempString);

        string reverseString = "";
        while (myStack.Count != 0)
        {
            reverseString += myStack.Pop();
            reverseString += " ";
        }

        Debug.Log("Reverse String Is = " + reverseString);
    }
}
