using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MainScript : MonoBehaviour
{
    Stopwatch stopwatch;

    [SerializeField] GameObject container;

    [SerializeField] int numberOfCircles = 9;
    [SerializeField] GameObject circlePrefab;
    [SerializeField] GameObject background;
    private List<GameObject> circles;  // this List contains all the circles/targets created
    private int currentTargetIndex = -1;
    private int clickedCount;
    private int round;
    private string dataLogFilePath = "data_log.csv";

    private float CAMERA_PROJECT_SIZE_VALUE;

    //these are the values that the prof told us to input divided by the camera...
    //If the camera screen is square, 5f directly represents 5 cm and 0.5f directly represents 0.5cm (hopefully this is the same on your computer)(pls check if it is).
    //However I could not figure out how to prevent the circles from going out of the screen - Dup
    private float scaleRadius = 0.5f;
    private float scaleDistance = 5f;

    public object[][] combinations; // example: [ [technique1, radius1, distance1], [t2, r2, d2], ..., [tn, rn, dn] ]

    void Start()
    {
        stopwatch = new Stopwatch();

        using (StreamWriter writer = new StreamWriter(dataLogFilePath, true))
        {
            writer.WriteLine("Technique,Width,Amplitude,Time(ms),Correct");
        }

        background.AddComponent<Target>().Initialize(-1, OnTargetSelected);

        //the value written in the settings.
        CAMERA_PROJECT_SIZE_VALUE = Camera.main.orthographicSize;
        scaleRadius = scaleRadius / CAMERA_PROJECT_SIZE_VALUE;
        scaleDistance = (scaleDistance / CAMERA_PROJECT_SIZE_VALUE);

        GenerateCombinations(scaleRadius,scaleDistance);
        GenerateCircles((float)combinations[round][1], (float)combinations[round][2]);

        HighlightStartingCurrentTarget();
        stopwatch.Start();
    }

    public void GenerateCircles(float radius, float distance)
    {
        circles = new List<GameObject>();

        for (int i = 0; i < numberOfCircles; i++)
        {
            float angle = i * (360f / numberOfCircles);

            Vector3 position = new Vector3(
                //make sure container is positioned at 0.
                (container.transform.position.x+ Mathf.Cos(angle * Mathf.Deg2Rad)) * distance,
                (container.transform.position.y+ Mathf.Sin(angle * Mathf.Deg2Rad)) * distance,
               container.transform.position.z
            );

            GameObject circle = Instantiate(circlePrefab, position, Quaternion.identity);
            circle.transform.SetParent(container.transform);
            circle.transform.localScale = new Vector3(radius, radius, circle.transform.localScale.z);
            circle.transform.position = new Vector3(circle.transform.position.x, circle.transform.position.y, 0);
            circle.GetComponent<SpriteRenderer>().sortingOrder = 0;

            circle.AddComponent<CircleCollider2D>().isTrigger = true;
            circle.AddComponent<Target>().Initialize(i, OnTargetSelected);

            circles.Add(circle);

        }
    }

    //gives the 9 combinations into combinations[][] 
    void GenerateCombinations(float scaleRadius, float scaleDistance) {
        //For a fair evaluation, the particpants should get the same pattern of combinations in the same order. For dynamic change patterns, create a different method.
        string[] techniques = new string[] { "Mouse", "TouchPad" };
        float[] radiuses = new float[] { scaleRadius * 1, scaleRadius * 2, scaleRadius * 3 };
        float[] distances = new float[] { scaleDistance * 1, scaleDistance * 2, scaleDistance * 3 };
        combinations = new object[techniques.Length * radiuses.Length * distances.Length][];

        int index = 0;
        for (int i = 0; i < techniques.Length; i++)
        {
            for (int j = 0; j < radiuses.Length; j++)
            {
                for (int k = 0; k < distances.Length; k++)
                {
                    combinations[index] = new object[] { techniques[i], radiuses[j], distances[k] };
                    index++;
                }
            }
        }
    }

    void HighlightStartingCurrentTarget()
    {
        currentTargetIndex = UnityEngine.Random.Range(0, numberOfCircles);
        Vector3 pos = circles[currentTargetIndex].transform.position;
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red;
        circles[currentTargetIndex].transform.position = new Vector3(pos.x, pos.y, -1);
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    void OnTargetSelected(int idx)
    {
        bool isCorrect = idx == currentTargetIndex ? true : false;

        clickedCount++;
        LogClickEvent(isCorrect);

        Vector3 pos = circles[currentTargetIndex].transform.position;
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.white;
        circles[currentTargetIndex].transform.position = new Vector3(pos.x, pos.y, 0);
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 0;

        //If the index is at 5, it becomes (5+(9/2))%9 = 0. The next index is set to 0.
        currentTargetIndex = (currentTargetIndex + (numberOfCircles / 2)) % numberOfCircles;

        pos = circles[currentTargetIndex].transform.position;
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red;
        circles[currentTargetIndex].transform.position = new Vector3(pos.x, pos.y, -1);
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;

        stopwatch.Restart();

        if (clickedCount > 8)
        {
            GoToNextRound();
            clickedCount = 0;
        }
    }

    private void LogClickEvent(bool isHit)
    {
        object[] currentExperiment = combinations[round];
        print($"radius: {currentExperiment[0]} distance: {currentExperiment[1]} Time taken: {stopwatch.ElapsedMilliseconds} ms, ");
        using (StreamWriter writer = new StreamWriter(dataLogFilePath, true))
        {
            writer.WriteLine($"{(string)currentExperiment[0]}, {2 * (float)currentExperiment[1]}, {(float)currentExperiment[2]}, {stopwatch.ElapsedMilliseconds}, {isHit}");
        }
    }
    public void DestroyList() {
        foreach (GameObject circle in circles) {
            Destroy(circle);
        }
    }
    public void GoToNextRound() {
        round++;
        if (round >= combinations.Length)
        {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #endif
            return;
        }
        DestroyList();
        GenerateCircles((float)combinations[round][1], (float)combinations[round][2]);
        HighlightStartingCurrentTarget();
    }
}

public class Target : MonoBehaviour
{
    private int idx;
    private System.Action<int> onSelectedCallback;

    public void Initialize(int idx, System.Action<int> callback)
    {
        this.idx = idx;
        this.onSelectedCallback = callback;
    }

    void OnMouseDown()
    {
        if (onSelectedCallback != null)
        {
            //OnTargetSelected is called with idx as its parameter value
            onSelectedCallback.Invoke(idx);
        }
    }
}



