using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//point count over point.getsize()
public class GiftWrapper : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;
    [SerializeField] private float rangeZ;
    [SerializeField] private int pointCount;
    [SerializeField] private GameObject[] points;

    //to-do check function 
    [SerializeField] private Queue<Facet> facets = new Queue<Facet>();
    [SerializeField] private List<Edge> subFacets = new List<Edge>();
    [SerializeField] private Queue<Facet> finalFacets = new Queue<Facet>();

    public Material red;
    public Material blue;

    //game object to vector3
    public struct Facet
    {
        public Facet(GameObject a, GameObject b, GameObject c, Material m)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.a.GetComponent<MeshRenderer>().material = m;
            this.b.GetComponent<MeshRenderer>().material = m;
            this.c.GetComponent<MeshRenderer>().material = m;
        }

        public GameObject a { get; }
        public GameObject b { get; }
        public GameObject c { get; }
    }
    public struct Edge
    {
        public Edge(GameObject a, GameObject b, Material m)
        {
            this.a = a;
            this.b = b;
            this.a.GetComponent<MeshRenderer>().material = m;
            this.b.GetComponent<MeshRenderer>().material = m;
            Debug.DrawLine(a.transform.position, b.transform.position, Color.green, 250f);
            //Debug.DrawLine(Vector3.zero, new Vector3(0, 5, 0), Color.green);

        }

        public GameObject a { get; }
        public GameObject b { get; }
    }

    // Start is called before the first frame update
    void Start()
    {
        points = new GameObject[pointCount];
        points[0] = Instantiate(point, new Vector3(-100f, 20f, 15f), Quaternion.identity);
        points[1] = Instantiate(point, new Vector3(-50f, 66f, 70f), Quaternion.identity);
        points[2] = Instantiate(point, new Vector3(-50f, 80f, 20f), Quaternion.identity);
        

        for (int i = 3; i < pointCount; i++)
        {
            points[i] = Instantiate(point, new Vector3(Random.Range(-1 * rangeX, rangeX), Random.Range(-1 * rangeY, rangeY), Random.Range(-1 * rangeZ, rangeZ)), Quaternion.identity);
        }
        //Debug.DrawLine(Vector3.zero, new Vector3(0, 5, 0), Color.green, 250f);
        /*
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 1, 2 };
        */

    }


    //bence burada
    void insertEdges(Facet f)
    {
        Edge e1 = new Edge(f.a, f.b, blue);
        Edge e2 = new Edge(f.b, f.c, blue);
        Edge e3 = new Edge(f.a, f.c, blue);

        insertEdge(e1);
        insertEdge(e2);
        insertEdge(e3);
    }

    void insertEdge(Edge e)
    {
        foreach (Edge i in subFacets)
        {
            if ((e.a == i.a && e.b == i.b) || (e.a == i.b && e.b == i.a))
            {
                //Debug.Log("edge already inserted" + i.a.transform.position.ToString() + " , " + i.b.transform.position.ToString());
                subFacets.Remove(i);
                return;
            }

        }
        subFacets.Add(e);
    }
    //extra edge input, 
    GameObject findFacetPoint(GameObject a, GameObject b, GameObject c, Edge e)
    {
        //var cotangentList = new ArrayList();

        var max = -2147483648f;
        int index = -1;

        for (int i = 0; i < pointCount; i++)
        {
            GameObject g = points[i];

            //transform compare
            if (g.transform.position != a.transform.position && g.transform.position != b.transform.position && g.transform.position != c.transform.position)
            {

                Vector3 va = e.a.transform.position;
                Vector3 vb = e.b.transform.position;
                Vector3 vg = g.transform.position;

                Vector3 vc;

                if (compareEdge(e, new Edge(a, b, blue)))
                {
                    vc = c.transform.position;
                }
                else if (compareEdge(e, new Edge(a, c, blue)))
                {
                    vc = b.transform.position;
                }
                else if (compareEdge(e, new Edge(b, c, blue)))
                {
                    vc = a.transform.position;
                }
                else
                {
                    Debug.Log("!!!!!!!!!! findFacetPoint comparison error !!!!!!!!");
                    vc = a.transform.position;
                }


                Vector3 vk = vg - vb;
                Vector3 e1 = vc - va;
                Vector3 e2 = vb - va;


                Vector3 pNorm = (Vector3.Cross(e1, e2));

                Vector3 x = Vector3.Cross(e2, pNorm);
/*
                Debug.Log("vk is " + vk);
                Debug.Log("x is " + x);
                Debug.Log("pNorm is " + pNorm);

                Debug.Log("1st dot product " + Vector3.Dot(vk, x));
                Debug.Log("2nd dot product " + Vector3.Dot(vk, pNorm));
*/
                var cotangent = -1f * ((Vector3.Dot(vk, x) / Vector3.Dot(vk, pNorm)));

                if (cotangent > max)
                {
                    max = cotangent;
                    index = i;
                }
                Debug.Log("cotangent:  " + cotangent + " for point " + points[i].transform.position + " from " + e.a.transform.position + " , " + e.b.transform.position);


                //Debug.Log(vk.ToString());
                //Debug.Log(pNorm.ToString());
                //Debug.Log("edge already inserted");
            }
        }
        return points[index];
    }
    void Giftwrap()
    {

        
        Facet f = new Facet(points[0], points[1], points[2], red);
        //Facet f = findFirstFacet();
        facets.Enqueue(f);
        finalFacets.Enqueue(f);
        insertEdges(facets.Peek());


        while (facets.Count > 0)
        {

            Facet curF = facets.Dequeue();
            Edge[] T = new Edge[3];

            T[0] = new Edge(curF.a, curF.b, blue);
            T[1] = new Edge(curF.b, curF.c, blue);
            T[2] = new Edge(curF.c, curF.a, blue);
            Debug.Log("edge s in T inserted ");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < subFacets.Count; j++)
                {
                    Edge e = subFacets[j];
                    //Debug.Log("2");
                    if (compareEdge(T[i], e))
                    {
                        GameObject c = findFacetPoint(curF.a, curF.b, curF.c, e);
                        //Facet fPrime = new Facet(curF.a, curF.b, c, red);
                        Facet fPrime = new Facet(e.a, e.b, c, red);
                        Debug.Log("edge: " + e.a.transform.position + " , " + e.b.transform.position + " fprime: " + c.transform.position.ToString());
                        facets.Enqueue(fPrime);
                        finalFacets.Enqueue(fPrime);
                        insertEdges(fPrime);/*
                        T[i] = new Edge(curF.a, curF.a, blue);/*
                        subFacets.RemoveAt(j);
                        i = 3;
                        j = subFacets.Count;*/

                    }
                }
            }
        }

        foreach (GameObject p in points)
        {
            p.GetComponent<MeshRenderer>().material = blue;

        }

        for (int i = 0; i < pointCount; i++) {
            Debug.Log("point: " + i + " is " + points[i].transform.position);
        }

        foreach (Facet i in finalFacets)
        {
            i.a.GetComponent<MeshRenderer>().material = red;
            i.b.GetComponent<MeshRenderer>().material = red;
            i.c.GetComponent<MeshRenderer>().material = red;
            Debug.DrawLine(i.a.transform.position, i.b.transform.position, Color.blue, 250f);
            Debug.DrawLine(i.b.transform.position, i.c.transform.position, Color.blue, 250f);
            Debug.DrawLine(i.a.transform.position, i.c.transform.position, Color.blue, 250f);
            /*
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = new Vector3[] { i.a.transform.position, i.b.transform.position, i.c.transform.position };
            //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
            mesh.triangles = new int[] { 0, 1, 2 };
            */
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire2"))
        {
            Giftwrap();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            findFirstFacet();
        }

    }

    bool compareEdge(Edge e1, Edge e2)
    {
        if ((e1.a == e2.a && e1.b == e2.b) || (e1.a == e2.b && e1.b == e2.a))
        {
            return true;
        }
        else
            return false;
    }

    Facet findFirstFacet()
    {
        GameObject p1 = findMinPoint(points);
        GameObject p2 = findEdgeOnHull(points, p1);
        GameObject p3 = findFacetOnHull(points, p1, p2);
        return new Facet(p1, p2, p3, blue);
    }

    GameObject findMinPoint(GameObject[] points)
    {
        List<GameObject> listOfX = new List<GameObject>();
        float minX = minXFromCoordinates(points);

        foreach (GameObject p in points)
        {
            if (p.transform.position.x == minX)
                listOfX.Add(p);
        }

        if (listOfX.Count == 1)
            return listOfX[0];

        List<GameObject> listOfY = new List<GameObject>();
        float minY = minYFromCoordinates(points);

        foreach (GameObject p in listOfY)
        {
            if (p.transform.position.y == minY)
                listOfY.Add(p);
        }

        if (listOfY.Count == 1)
            return listOfY[0];

        List<GameObject> listOfZ = new List<GameObject>();
        float minZ = minZFromCoordinates(points);

        foreach (GameObject p in listOfZ)
        {
            if (p.transform.position.z == minZ)
                listOfZ.Add(p);
        }

        return listOfZ[0];

    }

    float minXFromCoordinates(GameObject[] points)
    {
        float minValue = 2147483648f;

        foreach (GameObject p in points)
        {
            if (p.transform.position.x < minValue)
            {
                minValue = p.transform.position.x;
            }
        }
        return minValue;
    }

    float minYFromCoordinates(GameObject[] points)
    {
        float minValue = 2147483648f;

        foreach (GameObject p in points)
        {
            if (p.transform.position.y < minValue)
            {
                minValue = p.transform.position.y;
            }
        }
        return minValue;
    }

    float minZFromCoordinates(GameObject[] points)
    {
        float minValue = 2147483648f;

        foreach (GameObject p in points)
        {
            if (p.transform.position.z < minValue)
            {
                minValue = p.transform.position.z;
            }
        }
        return minValue;
    }

    GameObject findEdgeOnHull(GameObject[] points, GameObject p1)
    {
        Vector3 tempVector = Vector3.right - p1.transform.position;

        float maxAngle = 0f;
        GameObject maxAnglePoint = new GameObject();

        foreach (GameObject p in points)
        {
            Vector3 angularVector = p.transform.position - p1.transform.position;
            float angle = Vector3.Angle(tempVector, angularVector);

            if (angle > maxAngle)
            {
                maxAngle = angle;
                maxAnglePoint = p;
            }
        }

        return maxAnglePoint;
    }

    GameObject findFacetOnHull(GameObject[] points, GameObject p1, GameObject p2)
    {
        Vector3 np1 = Vector3.right - p1.transform.position; // plane normal

        Vector3 v1;
        Vector3 v2;
        Vector3 nF; // facet normal

        float maxAngle = 0f;
        GameObject maxAnglePoint = new GameObject();

        foreach(GameObject p in points)
        {
            v1 = p2.transform.position - p1.transform.position;
            v2 = p.transform.position - p1.transform.position;

            nF = Vector3.Cross(v1, v2);

            float planeAngle = Vector3.Angle(np1, nF);

            if(planeAngle > maxAngle)
            {
                maxAngle = planeAngle;
                maxAnglePoint = p;
            }
        }

        return maxAnglePoint;

    }


} // end
