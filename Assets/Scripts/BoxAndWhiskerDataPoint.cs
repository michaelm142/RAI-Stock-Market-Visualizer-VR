using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BoxAndWhiskerDataPoint : MonoBehaviour
{
    public float Width;
    public float LowerExtreme;
    public float LowerQuartile;
    public float Median;
    public float UpperQuartile;
    public float UpperExtreme;

    private float prev_Width;
    private float prev_UpperExtreme;
    private float prev_LowerExtreme;
    private float prev_Median;
    private float prev_LowerQuartile;
    private float prev_UpperQuartile;

    private LineRenderer box;
    private LineRenderer upperWhisker;
    private LineRenderer lowerWhisker;

    public Material material;

    private bool dirty;

    private Vector3[] boxPoints = new Vector3[]
    {
        Vector3.left + Vector3.down, Vector3.right + Vector3.down, Vector3.one, Vector3.up + Vector3.left,

    };

    // Start is called before the first frame update
    void Start()
    {
        // update monitoring variables
        prev_Width = Width;
        prev_Median = Median;
        prev_UpperExtreme = UpperExtreme;
        prev_UpperQuartile = UpperQuartile;
        prev_LowerExtreme = LowerExtreme;
        prev_LowerQuartile = LowerQuartile;

        Validate();
    }

    // Update is called once per frame
    void Update()
    {
        if (prev_Width != Width || prev_LowerExtreme != LowerExtreme || prev_LowerQuartile != LowerQuartile
            || prev_Median != Median || prev_UpperExtreme != UpperExtreme || prev_UpperQuartile != UpperQuartile)
            dirty = true;

        // validate if dirty
        if (dirty)
            Validate();

        // update values for testing
        prev_Width = Width;
        prev_Median = Median;
        prev_UpperExtreme = UpperExtreme;
        prev_UpperQuartile = UpperQuartile;
        prev_LowerExtreme = LowerExtreme;
        prev_LowerQuartile = LowerQuartile;

    }

    private void Validate()
    {
        // clear existing values
        if (box != null) Destroy(box.gameObject);
        if (upperWhisker != null) Destroy(upperWhisker.gameObject);
        if (lowerWhisker != null) Destroy(lowerWhisker.gameObject);

        // create box
        Vector3[] points = new Vector3[]
        {
            Vector3.left * (Width / 2.0f) + Vector3.up * LowerQuartile,
            Vector3.right * (Width / 2.0f) + Vector3.up * LowerQuartile,
            Vector3.right * (Width / 2.0f) + Vector3.up * UpperQuartile,
            Vector3.left * (Width / 2.0f) + Vector3.up * UpperQuartile,
        };

        GameObject boxObject = new GameObject("Box");
        boxObject.transform.SetParent(transform);
        boxObject.transform.localPosition = Vector3.zero;
        box = boxObject.AddComponent<LineRenderer>();
        box.positionCount = points.Length;
        box.SetPositions(points);
        box.loop = true;
        box.endWidth = box.startWidth = Width * 0.1f;
        box.useWorldSpace = false;
        box.material = material;
        // create upper whisker
        points = new Vector3[]
        {
            Vector3.up * UpperQuartile,
            Vector3.up * UpperExtreme,
        };

        GameObject upperWhiskerObject = new GameObject("UpperWhisker");
        upperWhiskerObject.transform.SetParent(transform);
        upperWhiskerObject.transform.localPosition = Vector3.zero;
        upperWhisker = upperWhiskerObject.AddComponent<LineRenderer>();
        upperWhisker.positionCount = points.Length;
        upperWhisker.SetPositions(points);
        upperWhisker.endWidth = upperWhisker.startWidth = Width * 0.1f;
        upperWhisker.useWorldSpace = false;
        upperWhisker.material = material;

        // create lower whisker
        points = new Vector3[]
        {
            Vector3.up * LowerQuartile,
            Vector3.up * LowerExtreme,
        };

        GameObject lowerWhiskerObject = new GameObject("LowerWhisker");
        lowerWhiskerObject.transform.SetParent(transform);
        lowerWhiskerObject.transform.localPosition = Vector3.zero;
        lowerWhisker = lowerWhiskerObject.AddComponent<LineRenderer>();
        lowerWhisker.positionCount = points.Length;
        lowerWhisker.SetPositions(points);
        lowerWhisker.endWidth = lowerWhisker.startWidth = Width * 0.1f;
        lowerWhisker.useWorldSpace = false;
        lowerWhisker.material = material;

        // clear dirty
        dirty = false;
    }
}
