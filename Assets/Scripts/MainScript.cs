using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    [SerializeField] float containerDiameter = 4f;
    [SerializeField] int numberOfCircles = 9;
    [SerializeField] GameObject circlePrefab;
    [SerializeField] GameObject container;

    private List<GameObject> circles;  // this List contains all the circles/targets created
    private int currentTargetIndex = -1; 

    // Start is called before the first frame update
    void Start()
    {
        container.transform.localScale = containerDiameter * Vector3.one;
        GenerateCircles();
        HighlightCurrentTarget();
    }

    void GenerateCircles()
    {
        float radius = containerDiameter / 2f;
        circles = new List<GameObject>();

        for (int i = 0; i < numberOfCircles; i++)
        {
            float angle = i * (360f / numberOfCircles);
            Vector3 position = new Vector3(
                container.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                container.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                container.transform.position.z
            );

            GameObject circle = Instantiate(circlePrefab, position, Quaternion.identity);
            circle.transform.SetParent(container.transform);
            circles.Add(circle);

            
            circle.AddComponent<CircleCollider2D>().isTrigger = true;
            circle.AddComponent<Target>().Initialize(i, OnTargetSelected);
        }
    }

    void HighlightCurrentTarget()
    {
        if (currentTargetIndex != -1)
        {
            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.white;  
        }

        currentTargetIndex = Random.Range(0, numberOfCircles);
        circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red; 
    }

    void OnTargetSelected(int idx)
    {
        if (idx == currentTargetIndex)
        {
            Debug.Log($"Circle Selected: Reference - {circles[idx]}, Position ID - {idx}");

            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.white;

            currentTargetIndex = (currentTargetIndex + (numberOfCircles / 2)) % numberOfCircles;

            circles[currentTargetIndex].GetComponent<SpriteRenderer>().color = Color.red;
        }
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
            onSelectedCallback.Invoke(idx);
        }
    }
}





