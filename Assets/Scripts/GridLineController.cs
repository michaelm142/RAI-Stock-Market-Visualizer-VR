using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridLineController : MonoBehaviour
{
    public Vector2 MinValue;
    public Vector2 MaxValue = Vector2.one * 10;

    private Vector2 minValuePrev;
    private Vector2 maxValuePrev;

    private Vector3 scalePrev;

    public float LineWidth;
    private float widthPrev;
    private float heightPrev;
    public float WidthDifference
    {
        get { return MaxValue.x - MinValue.x; }
    }
    public float HeightDifference
    {
        get { return MaxValue.y - MinValue.y; }
    }

    public UnityEvent Validating { get; private set; } = new UnityEvent();

    public float WidthStep
    {
        get
        {
            Rect rect = GetComponent<RectTransform>().rect;
            return rect.width / (MaxValue.x - MinValue.x);
        }
    }
    List<LineRenderer> horizontalLines = new List<LineRenderer>();
    List<LineRenderer> verticalLines = new List<LineRenderer>();

    public Material LineMaterial;

    private Transform root;

    private bool dirty;

    // Start is called before the first frame update
    void Start()
    {
        Validate();

        Rect rect = GetComponent<RectTransform>().rect;
        widthPrev = rect.width;
        heightPrev = rect.height;
        minValuePrev = MinValue;
        maxValuePrev = MaxValue;

        // find the object at the scene root
        root = FindRoot(transform);
        scalePrev = root.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //float deltaScale = root.localScale.magnitude - scalePrev.magnitude;
        //MinValue.x -= deltaScale;
        //MinValue.y -= deltaScale;
        //MaxValue.x += deltaScale;
        //MaxValue.y += deltaScale;

        Rect rect = root.GetComponent<RectTransform>().rect;
        if (rect.width != widthPrev)
            dirty = true;
        if (rect.height != heightPrev)
            dirty = true;
        if (MinValue != minValuePrev)
            dirty = true;
        if (MaxValue != maxValuePrev)
            dirty = true;

        //if (scalePrev.x != root.localScale.x || scalePrev.y != root.localScale.y || scalePrev.z != root.localScale.z)
        //    dirty = true;

        if (dirty)
            Validate();

        minValuePrev = MinValue;
        maxValuePrev = MaxValue;
        widthPrev = rect.width;
        heightPrev = rect.height;
        scalePrev = root.localScale;
    }

    private void Validate()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        float widthDifference = rect.width / (MaxValue.x - MinValue.x);
        float heightDifference = rect.height / (MaxValue.y - MinValue.y);

        // clear existing objects
        horizontalLines.ForEach(line => Destroy(line.gameObject));
        verticalLines.ForEach(line => Destroy(line.gameObject));
        verticalLines.Clear();
        horizontalLines.Clear();

        // create horizontal line
        int index = 0;
        for (float x = -rect.width / 2.0f; x <= rect.width / 2.0f; x += widthDifference)
        {
            GameObject line = new GameObject("Horizontal Line " + index.ToString());
            line.transform.parent = transform;
            line.transform.localRotation = Quaternion.identity;
            line.transform.localPosition = Vector3.right * x;

            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] { Vector3.zero, rect.height * Vector3.up });
            lr.startWidth = lr.endWidth = LineWidth;
            lr.useWorldSpace = false;
            lr.material = LineMaterial;

            horizontalLines.Add(lr);

            index++;
        }

        // create vertical lines
        index = 0;
        for (float y = 0; y <= rect.height; y += heightDifference)
        {
            GameObject line = new GameObject("Vertical Line " + index.ToString());
            line.transform.parent = transform;
            line.transform.localRotation = Quaternion.identity;
            line.transform.localPosition =
                Vector3.right * (rect.width / 2) + 
                Vector3.up * y;

            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] { Vector3.zero, -rect.width * Vector3.right });
            lr.startWidth = lr.endWidth = LineWidth;
            lr.useWorldSpace = false;
            lr.material = LineMaterial;

            verticalLines.Add(lr);

            index++;
        }

        Validating.Invoke();
        dirty = false;
    }

    private Transform FindRoot(Transform t)
    {
        if (t.GetComponent<Canvas>() != null)
            return t;

        return FindRoot(t.parent);
    }
}
