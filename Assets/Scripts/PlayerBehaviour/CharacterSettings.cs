using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    public float turnSpeed = 10f;

    public float walkSpeed = 5f;
}
