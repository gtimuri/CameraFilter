using System.Collections.Generic;
using UnityEngine;

public enum RenderShape
{
    Polygon,
    Circle
}

public class ShapeCameraRenderer : MonoBehaviour
{
    public LayerMask shapeLayer;
    public RenderShape renderingShape = RenderShape.Polygon;

    public Material shapeMat;
    public Material borderMat;


    public RectTransform quad1, quad2, quad3, quad4;
    Vector3 oldQuad1, oldQuad2, oldQuad3, oldQuad4;

    public Camera viewCamera;
    public LayerMask intersectionLayer;

    public RectTransform circlePoint;
    public float circleRadius = 300f;
    Vector3 oldCirclePoint;

    protected virtual void OnEnable()
    {
        shapeMat.SetVector("_Resolution", new Vector4(Screen.width, Screen.height, 0, 0));
        borderMat.SetVector("_Resolution", new Vector4(Screen.width, Screen.height, 0, 0));
        shapeMat.SetFloat("_ShapeSelector", (int)renderingShape);
        borderMat.SetFloat("_ShapeSelector", (int)renderingShape);
        shapeMat.SetFloat("_CircleRadius", circleRadius);
        borderMat.SetFloat("_CircleRadius", circleRadius);

        oldQuad1 = quad1.position;
        oldQuad2 = quad2.position;
        oldQuad3 = quad3.position;
        oldQuad4 = quad4.position;
    }
    
    protected virtual void Update()
    {
        shapeMat.SetFloat("_ShapeSelector", (int)renderingShape);
        borderMat.SetFloat("_ShapeSelector", (int)renderingShape);
        shapeMat.SetFloat("_CircleRadius", circleRadius);
        borderMat.SetFloat("_CircleRadius", circleRadius);

        UpdateCameraShape();
        UpdateBorders();
    }

    protected virtual void UpdateCameraShape()
    {
        if (renderingShape == 0) //QUAD SHAPE MODE
        {
            shapeMat.SetVector("_Quad1", quad1.position);
            shapeMat.SetVector("_Quad2", quad2.position);
            shapeMat.SetVector("_Quad3", quad3.position);
            shapeMat.SetVector("_Quad4", quad4.position);
        }
        else //CIRCLE SHAPE MODE
        {
            shapeMat.SetVector("_CircleV", circlePoint.position);
        }
    }

    protected virtual void UpdateBorders()
    {
        if (renderingShape == 0)
        {
            borderMat.SetVector("_Quad4", quad4.position);
            borderMat.SetVector("_Quad1", quad1.position);
            borderMat.SetVector("_Quad2", quad2.position);
            borderMat.SetVector("_Quad3", quad3.position);
        }
        else
        {
            borderMat.SetVector("_CircleV", circlePoint.position);
        }
    }

    //more performant version | 
    //private void LateUpdate()
    //{
    //    if (a.position != oldQuad1 || 
    //        b.position != oldQuad2 ||
    //        c.position != oldQuad3 || 
    //        d.position != oldQuad4 ||
    //        circlePoint != oldCirclePoint)
    //    {
    //        UpdateCameraShape();
    //        UpdateBorders();
    //    }


    //    oldQuad1 = a.position;
    //    oldQuad2 = b.position;
    //    oldQuad3 = c.position;
    //    oldQuad4 = d.position;
    //    oldCirclePoint = circlePoint.position;
    //}

    public virtual bool isPointInBoundary(Vector3 p)
    {

        Vector3 circleRayPoint = Vector3.zero;
        if (renderingShape == RenderShape.Polygon)
        {

            List<Vector2> quad = new List<Vector2> { quad1.position, quad2.position, quad3.position, quad4.position};
            var clippedPoly = ClipPolygonToViewport(quad);
            if (clippedPoly == null || clippedPoly.Count < 3)
            {
                //Debug.LogWarning("Clipped polygon is invalid!");
                return false;
            }

            List<Vector3> worldPolygon = new List<Vector3>();

            foreach (Vector2 screenPoint in clippedPoly)
            {
                Ray ray = viewCamera.ScreenPointToRay(screenPoint);
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, intersectionLayer))
                {
                    worldPolygon.Add(hit.point);
                }
                else
                {
                    Debug.LogWarning($"Raycast failed for screen point: {screenPoint}");
                    return false; // if any vertex fails, the polygon is incomplete
                }
            }

            return IsPointInPolygon(p, worldPolygon);


            //First attempt! it works but only if the quad is always inside the camera frustum
            // Vector3 aRayPoint = Vector3.zero, bRayPoint = Vector3.zero, cRayPoint = Vector3.zero, dRayPoint = Vector3.zero;
            //var aRay = viewCamera.ScreenPointToRay(quad1.position);
            //if (Physics.Raycast(aRay, out RaycastHit aRaycastHit, float.MaxValue, intersectionLayer))
            //    aRayPoint = aRaycastHit.point;
            //var bRay = viewCamera.ScreenPointToRay(quad2.position);
            //if (Physics.Raycast(bRay, out RaycastHit bRaycastHit, float.MaxValue, intersectionLayer))
            //    bRayPoint = bRaycastHit.point;
            //var cRay = viewCamera.ScreenPointToRay(quad3.position);
            //if (Physics.Raycast(cRay, out RaycastHit cRaycastHit, float.MaxValue, intersectionLayer))
            //    cRayPoint = cRaycastHit.point;
            //var dRay = viewCamera.ScreenPointToRay(quad4.position);
            //if (Physics.Raycast(dRay, out RaycastHit dRaycastHit, float.MaxValue, intersectionLayer))
            //    dRayPoint = dRaycastHit.point;

            //return IsPointInQuad(p, aRayPoint, bRayPoint, cRayPoint, dRayPoint);
        }
        else
        {
            var circleRay = viewCamera.ScreenPointToRay(circlePoint.position);
            if (Physics.Raycast(circleRay, out RaycastHit circleRaycastHit, float.MaxValue, intersectionLayer))
                circleRayPoint = circleRaycastHit.point;
            return IsPointInCircle(p, circleRayPoint);
        }
    }

    public List<Vector2> ClipPolygonToViewport(List<Vector2> poly)
    {
        // Convert to viewport (0..1)
        List<Vector2> viewportPoly = new List<Vector2>();
        foreach (var p in poly)
        {
            Vector3 vp = viewCamera.ScreenToViewportPoint(p);
            viewportPoly.Add(new Vector2(vp.x, vp.y));
        }

        // Clip each side of [0,1]
        List<Vector2> output = new List<Vector2>(viewportPoly);

        output = ClipEdgeViewport(output, 0, true);  // Left (x >= 0)
        output = ClipEdgeViewport(output, 1, false); // Right (x <= 1)
        output = ClipEdgeViewportY(output, 0, true); // Bottom (y >= 0)
        output = ClipEdgeViewportY(output, 1, false);// Top (y <= 1)

        // Convert back to screen space
        List<Vector2> clipped = new List<Vector2>();
        foreach (var vp in output)
        {
            Vector3 sp = viewCamera.ViewportToScreenPoint(new Vector3(vp.x, vp.y, 0));
            clipped.Add(new Vector2(sp.x, sp.y));
        }

        //Debug.Log($"Clipped polygon count: {clipped.Count}");
        return clipped;
    }

    // Axis-aligned clip for X
    List<Vector2> ClipEdgeViewport(List<Vector2> input, float x, bool isMin)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 prev = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = isMin ? current.x >= x : current.x <= x;
            bool prevInside = isMin ? prev.x >= x : prev.x <= x;

            if (currentInside)
            {
                if (!prevInside)
                    output.Add(IntersectX(prev, current, x));
                output.Add(current);
            }
            else if (prevInside)
            {
                output.Add(IntersectX(prev, current, x));
            }
        }
        return output;
    }

    // Axis-aligned clip for Y
    List<Vector2> ClipEdgeViewportY(List<Vector2> input, float y, bool isMin)
    {
        List<Vector2> output = new List<Vector2>();
        for (int i = 0; i < input.Count; i++)
        {
            Vector2 current = input[i];
            Vector2 prev = input[(i - 1 + input.Count) % input.Count];

            bool currentInside = isMin ? current.y >= y : current.y <= y;
            bool prevInside = isMin ? prev.y >= y : prev.y <= y;

            if (currentInside)
            {
                if (!prevInside)
                    output.Add(IntersectY(prev, current, y));
                output.Add(current);
            }
            else if (prevInside)
            {
                output.Add(IntersectY(prev, current, y));
            }
        }
        return output;
    }

    // Intersect with vertical line x = x
    Vector2 IntersectX(Vector2 a, Vector2 b, float x)
    {
        float t = (x - a.x) / (b.x - a.x);
        return new Vector2(x, Mathf.Lerp(a.y, b.y, t));
    }

    // Intersect with horizontal line y = y
    Vector2 IntersectY(Vector2 a, Vector2 b, float y)
    {
        float t = (y - a.y) / (b.y - a.y);
        return new Vector2(Mathf.Lerp(a.x, b.x, t), y);
    }
    protected virtual bool IsPointInCircle(Vector3 p, Vector3 centerWorld)
    {
        float distToCenter = Vector3.Distance(p, centerWorld);

        float pixelRadius = circleRadius * Screen.height * 0.5f;

        Vector3 edgeScreenPos = Vector3.one;
        if (circlePoint.position.x > Screen.width / 2)
            edgeScreenPos = circlePoint.position - new Vector3(pixelRadius, 0, 0);
        else
            edgeScreenPos = circlePoint.position + new Vector3(pixelRadius, 0, 0);

        if (viewCamera == null)
        {
            Debug.LogWarning("No view camera!");
            return false;
        }

        Ray edgeRay = viewCamera.ScreenPointToRay(edgeScreenPos);
        if (Physics.Raycast(edgeRay, out RaycastHit hit, float.MaxValue, intersectionLayer))
        {
            float worldRadius = Vector3.Distance(centerWorld, hit.point);
            return distToCenter <= worldRadius;
        }
        else
        {
            //circle is bigger than screen, always true. 
            if (circleRadius > 1.5f)
                return true;
            return false;
        }
    }

    protected virtual bool IsPointInQuad(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {

        Vector3 normal = Vector3.Cross(b - a, c - a).normalized;

        bool side1 = Vector3.Dot(Vector3.Cross(b - a, p - a), normal) >= 0f;
        bool side2 = Vector3.Dot(Vector3.Cross(c - b, p - b), normal) >= 0f;
        bool side3 = Vector3.Dot(Vector3.Cross(d - c, p - c), normal) >= 0f;
        bool side4 = Vector3.Dot(Vector3.Cross(a - d, p - d), normal) >= 0f;

        return side1 && side2 && side3 && side4;
    }

    protected virtual bool IsPointInPolygon(Vector3 p, List<Vector3> polygon)
    {
        if (polygon == null || polygon.Count < 3) return false;

        Vector3 normal = Vector3.Cross(polygon[1] - polygon[0], polygon[2] - polygon[0]).normalized;

        int count = polygon.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 a = polygon[i];
            Vector3 b = polygon[(i + 1) % count];
            Vector3 edge = b - a;
            Vector3 toPoint = p - a;

            if (Vector3.Dot(Vector3.Cross(edge, toPoint), normal) < 0f)
                return false;
        }
        return true;
    }
}





