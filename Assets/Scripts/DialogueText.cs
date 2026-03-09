using UnityEngine;

[CreateAssetMenu(fileName = "DialogueText", menuName = "Scriptable Objects/DialogueText")]
public class DialogueText : ScriptableObject
{
    public string speakerName;
    [TextArea(5, 10)] public string[] lines;
}
