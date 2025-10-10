using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    public float dragSensitivity = 1.0f;
    public bool invertDrag = true;
    public float minZoom = 3f;
    public float maxZoom = 12f;
    public float pinchZoomSpeed = 0.01f;
    public bool autoBoundsFromBackground = true;
    public SpriteRenderer background;
    public Rect worldBounds = new Rect(-25, -15, 50, 30);

    Camera cam;
    Vector2 lastTouchWorld;
    bool hasLastTouch = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        if (autoBoundsFromBackground && background != null)
        {
            var b = background.bounds;
            worldBounds = new Rect(b.min.x, b.min.y, b.size.x, b.size.y);
        }
    }

    void Update()
    {
        HandleTouchPan();
        HandlePinchZoom();
        ClampToBounds();
    }

    void HandleTouchPan()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            Vector3 touchWorld = cam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, -cam.transform.position.z));
            touchWorld.z = transform.position.z;

            if (t.phase == TouchPhase.Began)
            {
                lastTouchWorld = touchWorld;
                hasLastTouch = true;
            }
            else if (t.phase == TouchPhase.Moved && hasLastTouch)
            {
                Vector2 delta = (Vector2)touchWorld - lastTouchWorld;
                if (invertDrag) delta = -delta;
                float zoomFactor = cam.orthographicSize / ((minZoom + maxZoom) * 0.5f);
                transform.position += (Vector3)(delta * dragSensitivity * Mathf.Lerp(0.6f, 1.4f, zoomFactor));
                lastTouchWorld = touchWorld;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                hasLastTouch = false;
            }
        }
        else
        {
            hasLastTouch = false;
        }
    }

    void HandlePinchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;
            float prevMag = (t0Prev - t1Prev).magnitude;
            float currMag = (t0.position - t1.position).magnitude;
            float delta = currMag - prevMag;
            float newSize = cam.orthographicSize - delta * pinchZoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            Vector2 pinchCenter = (t0.position + t1.position) * 0.5f;
            Vector3 before = cam.ScreenToWorldPoint(new Vector3(pinchCenter.x, pinchCenter.y, -cam.transform.position.z));
            ClampToBounds();
            Vector3 after = cam.ScreenToWorldPoint(new Vector3(pinchCenter.x, pinchCenter.y, -cam.transform.position.z));
            transform.position += before - after;
        }
    }

    void ClampToBounds()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float minX = worldBounds.xMin + halfW;
        float maxX = worldBounds.xMax - halfW;
        float minY = worldBounds.yMin + halfH;
        float maxY = worldBounds.yMax - halfH;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        transform.position = p;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(worldBounds.center.x, worldBounds.center.y, 0);
        Vector3 size = new Vector3(worldBounds.width, worldBounds.height, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
