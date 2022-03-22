using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1280, 720, true);
        QualitySettings.vSyncCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
