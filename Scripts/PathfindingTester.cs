using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PathfindingTester : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    private int count = 0;
    private int stop = 0;
    private bool reverse = false;
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();

    // Array of possible waypoints.
    List<GameObject> Waypoints = new List<GameObject>();

    // Array of waypoint map connections. Represents a path.
    List<Connection> ConnectionArray = new List<Connection>();

    // The start and end target point.
    public GameObject start;
    public GameObject end;

    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);

    //public Transform targetA;
    //public Transform targetB;

    public float collectedbox = 0;

    public Text WoodenLogs;

    private int Pnt = 0;

    public Text distance;
    //float distancetravelled = 0;
    //public float seconds, minutes;



    // Start is called before the first frame update
    void Start()
    {
        Pnt = 0;
        text();
        text1();

        rb = GetComponent<Rigidbody>();
    

        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }

        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            if (tmpWaypointCon)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                Connection aConnection = new Connection();
                aConnection.SetFromNode(waypoint);
                aConnection.SetToNode(WaypointConNode);

                AStarManager.AddConnection(aConnection);
            }
        }
        // Run A Star...
        ConnectionArray = AStarManager.PathfindAStar(start, end);
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.GetFromNode().transform.position + OffSet), (aConnection.GetToNode().transform.position + OffSet));
        }
    }

    //trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            Pnt += 1;
            text();
            text1();

            if (speed > 5)
            {
                speed = speed - (speed * 0.1f);
                collectedbox++;
            }
        }

        if (other.gameObject.CompareTag("Car"))
        {
            rb.transform.position = transform.position + new Vector3(0, 3, 0);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (count < ConnectionArray.Count)
        {
            if (!reverse)
            {
                if (transform.position != ConnectionArray[count].GetToNode().transform.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, 
                        (ConnectionArray[count].GetToNode().transform.position), speed * Time.deltaTime);

                    Vector3 relativePos = (ConnectionArray[count].GetToNode().transform.position) - transform.position;
                    transform.rotation = Quaternion.LookRotation(relativePos);
                                
                }
                else
                {
                    count++;
                }
            }
            else
            {
                if (transform.position != ConnectionArray[count].GetFromNode().transform.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                       (ConnectionArray[count].GetFromNode().transform.position), speed * Time.deltaTime);

                    Vector3 relativePos = (ConnectionArray[count].GetFromNode().transform.position) - transform.position;
                    transform.rotation = Quaternion.LookRotation(relativePos);

                   // transform.rotation = Quaternion.Slerp(transform.rotation, (ConnectionArray[count].GetFromNode().transform.rotation), 5.0f * Time.deltaTime);
                }
                else
                {
                    count++;
                }
            }
        }

        if (count == ConnectionArray.Count)
        {
            //reset count
            if (stop < 1)
            {
                stop += 1;
                count = 0;
            }
            //reverse both variable and the list
            reverse = !reverse;
            ConnectionArray.Reverse();
        }

    }

    void text()
    {
        WoodenLogs.text = "Wooden Logs: " + Pnt;
    }

    void text1()
    {
        distance.text = "Speed:" + speed;
    }
}

 ////reset count
 //   if (stop< 1)
 //   {
 //       stop += 1;
 //       count = 0;
 //   }
 ////reverse both variable and the list
 //   reverse = !reverse;
 //   ConnectionArray.Reverse();