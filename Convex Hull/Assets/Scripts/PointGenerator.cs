using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGenerator : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;
    [SerializeField] private float rangeZ;
    [SerializeField] private int pointCount;
    [SerializeField] private GameObject[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = new GameObject[pointCount];
        for (int i = 0; i < pointCount; i++) { 
            points[i] = Instantiate(point, new Vector3(Random.Range(-1 * rangeX, rangeX), Random.Range(-1 * rangeY, rangeY), Random.Range(-1 * rangeZ, rangeZ)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
