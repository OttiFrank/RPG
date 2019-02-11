using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PlayerLog : MonoBehaviour
    {
        List<string> Eventlog = new List<string>();
        new string guiText = "";

        // Public VARS
        [SerializeField] int maxLines = 10;
        [SerializeField] float secondsToRemoveEvents = 3f;

        float removeTimer = 0;
        private void OnGUI()
        {
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.Label(new Rect(
                Screen.width - (Screen.width / 1.3f), 
                Screen.height - (Screen.height / 1), 
                Screen.width - (Screen.width / 2), 
                Screen.height / 3), 
                guiText, GUI.skin.textArea);    
        }
        

        public void AddEvent(string eventString)
        {
            Eventlog.Add(eventString);
            
            if (Eventlog.Count >= maxLines)
                Eventlog.RemoveAt(0);

            RemoveLogText();
        }

        private void Update()
        {
          
            if(Eventlog.Count >= 1)
            {
                if (Time.time - removeTimer > secondsToRemoveEvents)
                {
                    StartCoroutine(DeleteEvents());
                    removeTimer = Time.time;
                }
                RemoveLogText();
                

            }
            if(Eventlog.Count == 0)
            {
                RemoveLogText();
            }
                

            
        }

        private void RemoveLogText()
        {
            guiText = "";
            foreach (string logEvent in Eventlog)
            {
                guiText += logEvent;
                guiText += "\n";
            }
        }

        IEnumerator DeleteEvents()
        {
            yield return new WaitForSeconds(secondsToRemoveEvents);
            if(Eventlog.Count > 0)
                Eventlog.RemoveAt(0);                
        }
    }
}

