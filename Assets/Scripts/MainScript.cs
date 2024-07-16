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

    // Start is called before the first frame update
    void Start()
    {
        container.transform.localScale = containerDiameter * Vector3.one;
        GenerateCircles();
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
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
