﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

/// <summary>
/// BlazeFace from MediaPile
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/face_detection
/// </summary>
public class FaceDetectionSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")] string faceModelFile = "coco_ssd_mobilenet_quant.tflite";
    [SerializeField] RawImage cameraView = null;

    WebCamTexture webcamTexture;
    FaceDetect faceDetect;
    List<FaceDetect.Result> results;
    PrimitiveDraw draw;
    Vector3[] rtCorners = new Vector3[4];

    void Start()
    {
        string detectionPath = Path.Combine(Application.streamingAssetsPath, faceModelFile);
        faceDetect = new FaceDetect(detectionPath);

        string cameraName = WebCamUtil.FindName(new WebCamUtil.PreferSpec()
        {
            isFrontFacing = false,
            kind = WebCamKind.WideAngle,
        });
        webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        cameraView.texture = webcamTexture;
        webcamTexture.Play();
        Debug.Log($"Starting camera: {cameraName}");

        draw = new PrimitiveDraw()
        {
            color = Color.blue,
        };
    }

    void OnDestroy()
    {
        webcamTexture?.Stop();
        faceDetect?.Dispose();
        draw?.Dispose();
    }

    void OnEnable()
    {
        Camera.onPostRender += OnDrawResults;
    }
    void OnDisable()
    {
        Camera.onPostRender -= OnDrawResults;
    }


    void Update()
    {
        faceDetect.Invoke(webcamTexture);
        cameraView.material = faceDetect.transformMat;
        results = faceDetect.GetResults();
    }

    void OnDrawResults(Camera camera)
    {
        if (results == null)
        {
            return;
        }

        cameraView.rectTransform.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        foreach (var result in results)
        {
            Rect rect = MathTF.Lerp(min, max, result.rect, true);
            draw.Rect(rect, 0.05f);
            foreach (Vector2 p in result.keypoints)
            {
                draw.Point(MathTF.Lerp(min, max, new Vector3(p.x, 1f - p.y, 0)), 0.1f);
            }
        }
    }


}
