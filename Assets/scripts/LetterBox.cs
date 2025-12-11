using UnityEngine;
using TMPro;

public class LetterBox : MonoBehaviour
{
    public string letter;          // The actual letter (e.g. "A")
    public TextMeshPro label;      // Reference to the text on the box

    public void SetLetter(string newLetter)
    {
        letter = newLetter;
        if (label != null)
        {
            label.text = newLetter;
        }
    }
}

