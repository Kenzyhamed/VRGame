using UnityEngine;
using TMPro;

public class LetterBox : MonoBehaviour
{
    public string letter;          
    public TextMeshPro label1;
    public TextMeshPro label2;
    public TextMeshPro label3;
    public TextMeshPro label4;
    public TextMeshPro label5;
    public TextMeshPro label6;
    


    public void SetLetter(string newLetter)
    {
        letter = newLetter;
        if (label1 != null)
        {
            label1.text = newLetter;
            label2.text = newLetter;
            label3.text = newLetter;
            label4.text = newLetter;
            label5.text = newLetter;
            label6.text = newLetter;
        }
    }

    public void Start()
    {
        Debug.Log("here");
        SetLetter(letter);
        
    }
}



