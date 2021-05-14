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
                Debug.Log("edge already inserted" + i.a.transform.position.ToString() + " , " + i.b.transform.position.ToString());
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

                Vector3 va = a.transform.position;
                Vector3 vb = b.transform.position;
                Vector3 vc = c.transform.position;
                Vector3 vg = g.transform.position;

                Vector3 vk;
                Vector3 e1;
                Vector3 e2;


                if (compareEdge(e, new Edge(a, b, blue)))
                {
                    vk = vg - vb;
                    e1 = vc - va;
                    e2 = vb - va;
                }
                else if (compareEdge(e, new Edge(a, c, blue)))
                {
                    vk = vg - va;
                    e1 = vb - vc;
                    e2 = va - vc;
                }
                else if (compareEdge(e, new Edge(b, c, blue)))
                {
                    vk = vg - vc;
                    e1 = va - vb;
                    e2 = vc - vb;
                }
                else
                {
                    Debug.Log("!!!!!!!!!! findFacetPoint comparison error !!!!!!!!");
                    vk = vg - vb;
                    e1 = vc - va;
                    e2 = vb - va;
                }


                Vector3 pNorm = (Vector3.Cross(e1, e2)).normalized;

                Vector3 x = Vector3.Cross(e2, pNorm);
/*
                Debug.Log("vk is " + vk);
                Debug.Log("x is " + x);
                Debug.Log("pNorm is " + pNorm);

                Debug.Log("1st dot product " + Vector3.Dot(vk, x));
                Debug.Log("2nd dot product " + Vector3.Dot(vk, pNorm));
*/
                var angle = -1f * ((Vector3.Dot(vk, x) / Vector3.Dot(vk, pNorm)));

                if (angle > max)
                {
                    max = angle;
                    index = i;
                }
                //Debug.Log("angle:  " + angle);


                //Debug.Log(vk.ToString());
                //Debug.Log(pNorm.ToString());
                //Debug.Log("edge already inserted");
            }
        }
        return points[index];
    }
    void Giftwrap()
    {

        //
        Facet f = new Facet(points[0], points[1], points[2], red);
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
                        insertEdges(fPrime);
                        
                        subFacets.RemoveAt(j);/*
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


}
// end
