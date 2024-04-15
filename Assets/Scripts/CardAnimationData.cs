using UnityEngine;

[CreateAssetMenu(fileName = "NewCardAnimationData", menuName = "Card Animation Data", order = 51)]
public class CardAnimationData : ScriptableObject
{
    public float cursorFollowSpeed = 20;
    public float slotReturnDuration = .05f;
    public float dragRotationAmount = 30;
    public float dragRotationLerp = 10;
    public float dragRotationSensitivity = 10;
    public float mouseDeltaLerp = 30;
}