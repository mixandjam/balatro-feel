using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTransformChildrenChanged()
    {
        print("changed");
    }
}
