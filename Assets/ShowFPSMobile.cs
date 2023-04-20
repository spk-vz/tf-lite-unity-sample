using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add this line to include TextMeshPro

public class ShowFPSMobile : MonoBehaviour
{
    private string fps = "";

    private WaitForSecondsRealtime waitForFrequency;

    // Replace GUIStyle and Rect with TMP_Text
    public TMP_Text fpsText;

    bool isInicialized = false;

    private void Awake()
    {
        fpsText.text = "";
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Inicialize(true);
    }

    private IEnumerator FPS()
    {
        int lastFrameCount;
        float lastTime;
        float timeSpan;
        int frameCount;
        for (; ; )
        {
            lastFrameCount = Time.frameCount;
            lastTime = Time.realtimeSinceStartup;
            yield return waitForFrequency;
            timeSpan = Time.realtimeSinceStartup - lastTime;
            frameCount = Time.frameCount - lastFrameCount;

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
            fpsText.text = fps; // Update the TextMeshPro text
        }
    }

    private void Inicialize(bool showFps)
    {
        isInicialized = true;

        if (showFps)
            StartCoroutine(FPS());
    }

    public void SetNewConfig(GraphicSettingsMB gSettings)
    {
        Application.targetFrameRate = gSettings.targetFrameRate;
        waitForFrequency = new WaitForSecondsRealtime(gSettings.testFpsFrequency);

        if (!isInicialized) Inicialize(gSettings.showFps);

        if (!gSettings.showFps)
            Destroy(this.gameObject);
    }
}

[SerializeField]
public class GraphicSettingsMB
{
    public byte targetFrameRate = 30;
    public byte testFpsFrequency = 1;
    public bool showFps = false;
}
