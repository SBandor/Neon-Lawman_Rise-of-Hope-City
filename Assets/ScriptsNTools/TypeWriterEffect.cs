using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;
    private string currentText = "", npcText;
   
    
    public void ActivaTypeWriterEffect()
    {
        
       
           
            StartCoroutine(ShowText());
          
        
    }
   
    IEnumerator ShowText()
    {
        
        for (int i=0; i< fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            GetComponent<Text>().text=currentText;
            yield return new WaitForSeconds(delay);
            
        }
    }
   public void SetFullText(string s) { fullText = s; }
    public void ClearText()
    {
        fullText = "";
        currentText = "";
    }
}
