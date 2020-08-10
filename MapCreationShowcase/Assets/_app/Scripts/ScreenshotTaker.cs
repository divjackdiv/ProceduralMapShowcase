using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenshotTaker : MonoBehaviour
{
    public MapGenerator mapGenerator;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("ProcgenScreenshots/" + SceneManager.GetActiveScene().name + "_"+ mapGenerator .m_seed + ".png");
        }
    }
}
