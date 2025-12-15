using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GamaManager : MonoBehaviour
{
    [Header("Slots in order (left -> right)")]
    public Transform[] slots;

    [Header("Correct answer")]
    public string correctWord = "apple";

    [Header("How far each slot searches for a letter cube")]
    public float slotRadius = 0.08f; 

    [Header("Only your letter-cube layer")]
    public LayerMask letterLayer = ~0;

    [Header("Optional UI feedback")]
    public TextMeshProUGUI resultText;

    public void CheckOrder()
    {
        if (slots == null || slots.Length == 0) return;

        if (correctWord.Length != slots.Length)
        {
            Debug.LogError("correctWord length must match number of slots!");
            return;
        }

 
        List<string> letters = new List<string>(slots.Length);
        string playerWord = "";

        for (int i = 0; i < slots.Length; i++)
        {
            Transform slot = slots[i];

         
            Collider[] hits = Physics.OverlapSphere(slot.position, slotRadius, letterLayer);

           
            LetterBox best = null;
            float bestDist = float.MaxValue;

            foreach (Collider h in hits)
            {
               
                LetterBox lb = h.GetComponentInParent<LetterBox>();
                if (lb == null) continue;

                float d = Vector3.Distance(lb.transform.position, slot.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = lb;
                }
            }

            string ch = (best != null) ? best.letter : "_";
            letters.Add(ch);
            playerWord += ch;
        }

        bool correct = (playerWord == correctWord);

        Debug.Log($"Letters list: [{string.Join(",", letters)}]");
        Debug.Log($"Player word: {playerWord} | Correct: {correctWord} | Result: {correct}");

        if (resultText != null)
            resultText.text = correct ? " Correct!" : $" Try again: {playerWord}";
    }
}
