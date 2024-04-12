using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickOffset : MonoBehaviour
{
    private RectTransform[] elements; // Your array of elements
    public float amplitude = 1f; // Amplitude of the wave
    public float frequency = 1f; // Frequency of the wave

    void Start()
    {
        elements = GetComponentsInChildren<RectTransform>();
        int middleIndex = elements.Length / 2;
        for (int i = 0; i < elements.Length; i++)
        {
            float yOffset = CalculateArchOffset(i, middleIndex) * .2f;
            // elements[i].position = new Vector3(elements[i].position.x, elements[i].position.y + yOffset, elements[i].position.z);

            if (elements[i] == GetComponent<RectTransform>())
                continue;
            elements[i].pivot = new Vector2(0.5f, elements[i].pivot.y + yOffset);
        }
    }

    float CalculateArchOffset(int currentIndex, int middleIndex)
    {
        return 1f - Mathf.Pow((currentIndex - middleIndex) / (float)middleIndex, 2);
    }

    public class ChildChangeHandler : MonoBehaviour
    {
        // This method will be called whenever the children of this GameObject change
        void OnTransformChildrenChanged()
        {
            // Put your logic here to handle the change in children
            Debug.Log("Children of " + gameObject.name + " have changed.");
        }
    }
}
