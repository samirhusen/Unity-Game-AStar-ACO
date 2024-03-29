﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ACOTester : MonoBehaviour
{
    //Count variable
    private int count = 0;
    private bool reverse = false;
    private Rigidbody rb;
    public Text WoodenLogs;
    public Text distance;

    public GameObject Car;
    private int collectedbox = 0;

    //speed 
    public float speed;
    //private int currentNode = 0;
    //private int stop = 0;

    // The ACO Controller.
    ACOCON MyACOCON = new ACOCON();

    // Array of possible waypoints.
    List<GameObject> Waypoints = new List<GameObject>();

    // Connections between nodes.
    private List<Connection> Connections = new List<Connection>();

    // The route generated by the ACO algorith.
    private List<Connection> MyRoute = new List<Connection>();

    // Debug line offset.
    private Vector3 OffSet = new Vector3(0, 0.5f, 0);

    // The Start node for any created route.
    public GameObject StartNode;

    // The max length of a path created by the ACO.
    public int MaxPathLength;

    public List<GameObject> Goals = new List<GameObject>();


    //public float collectedbox = 0;
    //private float distance = 0;
    //private float endDistance = 0;
    //private float timeCount;
    //private bool timeStop = true;
    //private float rotateSpeed = 13f;
    //private float currentSpeed;

    // The A* manager.
    private AStarManager AStarManager = new AStarManager();
    // Array of possible waypoints.
    List<GameObject> WaypointsP = new List<GameObject>();

    // Array of waypoint map connections. Represents a path.
    List<List<Connection>> ConnectionArray = new List<List<Connection>>();
    List<Connection> ConnectionArrayOnly = new List<Connection>();


    // Start is called before the first frame update
    void Start()
    {
        collectedbox = 0;
        //collectedbox = 0;
        rb = GetComponent<Rigidbody>();
        text();
        text1();

        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;

        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            if (tmpWaypointCon)
            {
                if (tmpWaypointCon.WaypointType == WaypointCON.waypointPropsList.Goal)
                {
                    // We are creating a waypoint map of only the goal nodes. We want out ACO algorithm to create the shortest path between the goal nodes.
                    Waypoints.Add(waypoint);
                }
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
                aConnection.SetConnection(waypoint, WaypointConNode, MyACOCON.GetDefaultPheromone());
                Connections.Add(aConnection);
                AStarManager.AddConnection(aConnection);
            }
        }

        MyRoute = MyACOCON.ACO(150, 50, Waypoints.ToArray(), Connections, StartNode, MaxPathLength);

    
		for(int i=0;i<(Goals.Count);i++)
        {
			if(i==(Goals.Count-1))
            {
				ConnectionArray.Add(AStarManager.PathfindAStar(Goals[i], StartNode));
			}
            else
            {
				ConnectionArray.Add(AStarManager.PathfindAStar(Goals[i], Goals[i + 1]));
			}			
		}
			
		Debug.Log("Executing");

		foreach (List<Connection> con in ConnectionArray)
        {
			if(con == null || con.Count==0){
				Debug.Log("Empty Array");
			}
			foreach(Connection acon in con){
				ConnectionArrayOnly.Add(acon);
 
			}
		}
 
	}
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        if (MyRoute.Count > 0)
        {
            //ACO Route
            foreach (Connection aConnection in MyRoute)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine((aConnection.GetFromNode().transform.position + OffSet),
                (aConnection.GetToNode().transform.position + OffSet));
            }
            //A* path
            foreach (Connection aConnection in ConnectionArrayOnly)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine((aConnection.GetFromNode().transform.position + new Vector3(0, 1f, 0)),
                (aConnection.GetToNode().transform.position + new Vector3(0, 1f, 0)));
            }
        }
    }

    //On Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            collectedbox += 1;
            text();
            text1();

            if (speed > (speed - (0.9f * speed)))
            {
                speed = speed - (0.1f * speed);
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
        if (count < ConnectionArrayOnly.Count)
        {
            if (!reverse)
            {

                if (transform.position != ConnectionArrayOnly[count].GetToNode().transform.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, ConnectionArrayOnly[count].GetToNode().transform.position, Time.deltaTime * speed);

                    Vector3 relativePos = (ConnectionArrayOnly[count].GetToNode().transform.position) - transform.position;
                    transform.rotation = Quaternion.LookRotation(relativePos);
                }
                else
                {
                    count++;
                }
            }

        }
    }
    //Text box for item counting
    void text()
    {
        WoodenLogs.text = "Wooden Logs: " + collectedbox;
    }

    void text1()
    {
        distance.text = "Speed:" + speed;
    }
}