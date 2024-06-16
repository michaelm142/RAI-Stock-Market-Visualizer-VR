using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoxAndWhiskerLineController : MonoBehaviour
{
    public AnimationCurve openDataCurve;
    public AnimationCurve highDataCurve;
    public AnimationCurve lowDataCurve;
    public AnimationCurve closeDataCurve;

    public GridLineController grid;

    public Material material;

    List<BoxAndWhiskerDataPoint> points = new List<BoxAndWhiskerDataPoint>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetData(List<float>[] data)
    {
        List<float> openData =          data[0];
        List<float> lowDataData =       data[1];
        List<float> highDataData =      data[2];
        List<float> closeDataData =     data[3];

        int i = 0;
        openData.ForEach(delegate (float f) { openDataCurve.AddKey(i, openData[i]); i++; });
        i = 0;
        highDataData.ForEach(delegate (float f) { highDataCurve.AddKey(i, highDataData[i]); i++; });
        i = 0;
        lowDataData.ForEach(delegate (float f) { lowDataCurve.AddKey(i, lowDataData[i]); i++; });
        i = 0;
        closeDataData.ForEach(delegate (float f) { closeDataCurve.AddKey(i, closeDataData[i]); i++; });

        grid.Validating.AddListener(Validate);
        Validate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Validate()
    {
        if (grid.WidthStep < 0)
            return;

        // clear existing data points
        points.ForEach(p => Destroy(p.gameObject));
        points.Clear(); 

        // create new data points based on grid data
        Rect rect = GetComponent<RectTransform>().rect;
        for (float x = 0, i = grid.MinValue.x; x < rect.width; x += grid.WidthStep, i++)
        {
            GameObject dataPointObj = new GameObject(String.Format("Data Point {0}", i), typeof(BoxAndWhiskerDataPoint));
            dataPointObj.transform.SetParent(transform);

            // calculate data points for this point in time 0 1 2 3
            List<float> data = SampleCurveData(x);
            float lowerExtreme = data[0];
            float lowerQuartile = data[1];
            float median = Mathf.Lerp(data[1], data[2], 0.5f);
            float upperQuartile = data[2];
            float upperExtreme = data[3];

            dataPointObj.transform.localPosition = Vector3.up * median + Vector3.right * x;

            BoxAndWhiskerDataPoint dataPoint = dataPointObj.GetComponent<BoxAndWhiskerDataPoint>();
            dataPoint.Width = grid.WidthStep;
            dataPoint.LowerExtreme = lowerExtreme - median;
            dataPoint.Median = median;
            dataPoint.UpperQuartile = upperQuartile - median;
            dataPoint.UpperExtreme = upperExtreme - median;
            dataPoint.LowerQuartile = lowerQuartile - median;
            dataPoint.material = material;
            points.Add(dataPoint);
        }
    }

    private List<float> SampleCurveData(float x)
    {
        Rect rect = GetComponent<RectTransform>().rect;
        List<float> data = new List<float>();
        //float value = (Mathf.Clamp(openDataCurve.Evaluate(i), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height;
        data.Add((Mathf.Clamp(openDataCurve.Evaluate(x), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height);
        data.Add((Mathf.Clamp(highDataCurve.Evaluate(x), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height);
        data.Add((Mathf.Clamp(lowDataCurve.Evaluate(x), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height);
        data.Add((Mathf.Clamp(closeDataCurve.Evaluate(x), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height);
        data.Sort();

        return data;
    }
}
