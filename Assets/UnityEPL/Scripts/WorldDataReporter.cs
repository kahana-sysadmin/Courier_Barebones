using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UnityEPL/Reporters/World Data Reporter")]
public class WorldDataReporter : DataReporter
{
    public bool reportTransform = true;
    public int framesPerTransformReport = 60;
    public bool reportView = true;

    public int framesPerViewReport = 60;

    public int threshold = 1;

    public int layerMask =  6;// 0b0110 choose layers NOT to report, with bits set in positions corresponding
                                   // to ignored layers

    void Update()
    {
        if (reportTransform) CheckTransformReport();
        if (reportView) CheckViewReport();
    }

    void Start()
    {
        if (reportView && GetComponent<BoxCollider>() == null)
        {
            reportView = false;
            throw new UnityException("You have selected enter/exit viewfield reporting for " + gameObject.name + " but there is no box collider on the object." +
                                      "  This feature uses collision detection to compare with camera bounds and other objects.  Please add a collider or " +
                                      "unselect viewfield enter/exit reporting.");
        }
    }

    public void DoTransformReport(System.Collections.Generic.Dictionary<string, object> extraData = null)
    {
        if (extraData == null)
            extraData = new Dictionary<string, object>();
        System.Collections.Generic.Dictionary<string, object> transformDict = new System.Collections.Generic.Dictionary<string, object>(extraData);
        transformDict.Add("positionX", transform.position.x);
        transformDict.Add("positionY", transform.position.y);
        transformDict.Add("positionZ", transform.position.z);
        transformDict.Add("rotationX", transform.position.x);
        transformDict.Add("rotationY", transform.position.y);
        transformDict.Add("rotationZ", transform.position.z);
        transformDict.Add("scaleX", transform.position.x);
        transformDict.Add("scaleY", transform.position.y);
        transformDict.Add("scaleZ", transform.position.z);
        transformDict.Add("object reporting id", reportingID);
        eventQueue.Enqueue(new DataPoint(gameObject.name + " transform", RealWorldFrameDisplayTime(), transformDict));
    }

    private void CheckTransformReport()
    {
        if (Time.frameCount % framesPerTransformReport == 0)
        {
            DoTransformReport();
        }
    }

    private void CheckViewReport()
    {
        if (Time.frameCount % framesPerViewReport == 0)
        {
            DoViewReport();
        }
    }

    //untested accuraccy, requires collider
    private void DoViewReport()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();

        foreach (Camera thisCamera in cameras)
        {
            Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(thisCamera);
            Collider objectCollider = GetComponent<Collider>();

            if(!GeometryUtility.TestPlanesAABB(frustrumPlanes, objectCollider.bounds)) {
                continue;
            }

            RaycastHit lineOfSightHit;
            Vector3[] vertices = GetColliderVertexPositions(gameObject);
            int hits = 0;

            // raycast to center mass
            foreach (Vector3 vertex in vertices) {
                bool hit = Physics.Linecast(thisCamera.transform.position, vertex, out lineOfSightHit, layerMask);

                Collider gameBox = gameObject.GetComponent<Collider>();
                if(hit && lineOfSightHit.collider.Equals(gameBox)) {
                    hits++;
                }
            }

            Debug.Log(hits);

            bool inView = hits >= threshold;

            string eventName = "";
            Dictionary<string, object> dataDict = new Dictionary<string, object>();
            dataDict.Add("camera", thisCamera.name);
            dataDict.Add("in view", inView);
            dataDict.Add("ray hits", hits); // provide data to rethreshold
            eventName = gameObject.name + "object in view";
            eventQueue.Enqueue(new DataPoint(eventName, RealWorldFrameDisplayTime(), dataDict));
        }
    }

    private Vector3[] GetColliderVertexPositions(GameObject obj) {
        Vector3[] vertices = new Vector3[9];
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();

        Vector3 colliderCenter  = boxCollider.center;
        Vector3 colliderExtents = boxCollider.size/2.0f;
        Vector3 pointOffset = new Vector3(.02f, .02f, .02f);

        for (int i = 0; i < 8; i++)
        {
            Vector3 extents = colliderExtents;
            Vector3 offset = pointOffset;
            extents.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));
            offset.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));

            Vector3 vertexPosLocal = colliderCenter + extents - offset;

            Vector3 vertexPosGlobal = boxCollider.transform.TransformPoint(vertexPosLocal);

            // display vector3 to six decimal places
            vertices[i] = vertexPosGlobal;
            Debug.Log ("Vertex " + i + " @ " + vertexPosGlobal.ToString("F6"));
        }
        vertices[8] = boxCollider.transform.TransformPoint(colliderCenter);
        /*
        Debug.Log("$$$####$$$$");
        
        Debug.Log(extents);

        thisCollider.transform.rotation = Quaternion.identity;
        float insetDist = .1f;
    
        extents = thisCollider.bounds.size;
        Debug.Log(extents);
        vertices[0] = thisMatrix.MultiplyPoint3x4(extents - new Vector3(insetDist, insetDist, insetDist));
        vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x + insetDist, extents.y - insetDist, extents.z - insetDist));
        vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x - insetDist, extents.y - insetDist, -extents.z + insetDist));
        vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x + insetDist, extents.y - insetDist, -extents.z + insetDist));
        vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x - insetDist, -extents.y + insetDist, extents.z - insetDist));
        vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x + insetDist, -extents.y + insetDist, extents.z - insetDist));
        vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x - insetDist, -extents.y + insetDist, -extents.z + insetDist));
        vertices[7] = thisMatrix.MultiplyPoint3x4(-extents + new Vector3(insetDist, insetDist, insetDist));
        vertices[8] = thisMatrix.MultiplyPoint3x4(new Vector3(0.0f, 0.0f, 0.0f));
        */
        //obj.transform.rotation = storedRotation;
        return vertices;
    }
}