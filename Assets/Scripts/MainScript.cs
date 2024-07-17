using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{

    [SerializeField] GameObject container;

    [SerializeField] int numberOfCircles = 9;
    [SerializeField] GameObject circlePrefab;

    // this List contains all the circles/targets created
    private List<GameObject> circles;  
    private int currentTargetIndex = -1;
    private int clickedCount;

    private float CAMERA_PROJECT_SIZE_VALUE;

    //these are the values that the prof told us to input divided by the camera...
    //If the camera screen is square, 5f directly represents 5 cm and 0.5f directly represents 0.5cm (hopefully this is the same on your computer)(pls check if it is).
    //However I could not figure out how to prevent the circles from going out of the screen - Dup
    private float scaleRadius = 0.5f;
    private float scaleDistance = 5f;

    //In the array in each index of combinations, the first index represents the radius. the second index represents the distance.
    public float[][] combinations;

    //the first round is set as 0
    private int round;

    void Start()
    {
        //the value written in the settings.
        CAMERA_PROJECT_SIZE_VALUE = Camera.main.orthographicSize;
        scaleRadius = scaleRadius / CAMERA_PROJECT_SIZE_VALUE;
        scaleDistance = (scaleDistance / CAMERA_PROJECT_SIZE_VALUE);

        GenerateCombinations(scaleRadius,scaleDistance);
        GenerateCircles(combinations[round][0], combinations[round][1]);

        HighlightStartingCurrentTarget();
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

            circle.AddComponent<CircleCollider2D>().isTrigger = true;
            circle.AddComponent<Target>().Initialize(i, OnTargetSelected);

            circles.Add(circle);

        }
    }

    //gives the 9 combinations into combinations[][] 
    void GenerateCombinations(float scaleRadius, float scaleDistance) {
        //For a fair evaluation, the particpants should get the same pattern of combinations in the same order. For dynamic change patterns, create a different method.
        combinations = new float[9][];
        float[] radiuses = new float[] { scaleRadius*1, scaleRadius*2, scaleRadius*3 };
        float[] distance = new float[] { scaleDistance*1, scaleDistance * 2, scaleDistance * 3 };
        int radiusCount = 0;
        int distanceCount = 0;
        for (int i = 0; i < numberOfCircles; i++) {
            //the first index will represent the radius. The second distance.
            combinations[i] = new float[2];
            combinations[i][0] = radiuses[radiusCount];
            combinations[i][1] = distance[distanceCount];
            distanceCount++;
            if (distanceCount== distance.Length) {
                radiusCount++;
                distanceCount = 0;
            }
        }
        //resulting array if scaleRadius and scaleDistance are equal to 1f: [[1,1],[1,2],[1,3],[2,1],[2,2],...[3,3]]
        
    }
    void HighlightStartingCurrentTarget()
    {
        //Seems to work without this if clause, so I commented it out -- Dup (revert if necessary)
        /*if (currentTargetIndex != -1)
        {
            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.white;  
        }*/

        currentTargetIndex = UnityEngine.Random.Range(0, numberOfCircles);
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red;

        //set the current circle to be above any other circle (so that when it is overlapping with another circle, the current circle comes up
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
//        circles[currentTargetIndex].gameObject.transform.SetSiblingIndex(0);
    }

    void OnTargetSelected(int idx)
    {
        if (idx == currentTargetIndex)
        {
            clickedCount++;
            Debug.Log($"Circle Selected: Reference - {circles[idx]}, Position ID - {idx}");

            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.white;

            //set the previous circle to a lower sorting order.
            circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 0;
            //If the index is at 5, it becomes (5+(9/2))%9 = 0. The next index is set to 0.
            currentTargetIndex = (currentTargetIndex + (numberOfCircles / 2)) % numberOfCircles;

            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red;
            circles[currentTargetIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        if(clickedCount > 8) {
            GoToNextRound();
            clickedCount = 0;
        }
    }
    public void DestroyList() {
        foreach (GameObject circle in circles) {
            Destroy(circle);
        }
    }
    public void GoToNextRound() {
        round++;
        if (round > 8) {
            //end of test
            DestroyList();

            return;
        }
        DestroyList();
        //Create new circles.
        GenerateCircles(combinations[round][0], combinations[round][1]);
        //Highlight random circle.
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



