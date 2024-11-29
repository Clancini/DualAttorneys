using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogViewer : MonoBehaviour
{
    [SerializeField] TMP_Text outputText;

    Queue<string> logQueue = new Queue<string>();

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;

        outputText.text += "\n["+ System.DateTime.UtcNow.ToString("HH:mm") + "]" + " [Start]";
    }

    private void OnEnable()
    {
        foreach (string log in logQueue)
        {
            outputText.text += logQueue.Dequeue();
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logText = "\n[" + System.DateTime.UtcNow.ToString("HH:mm") + "] [" + type + "] " + logString;

        if (outputText.enabled)
            outputText.text += logText;
        else
            logQueue.Enqueue(logText);
    }
}
