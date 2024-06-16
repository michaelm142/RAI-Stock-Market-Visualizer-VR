using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GraphLineController : MonoBehaviour
{
    public string filename;

    public AnimationCurve dataCurve;

    public GridLineController grid;

    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();

        grid.Validating.AddListener(Validate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(List<float> data)
    {
        while (dataCurve.keys.Length > 0)
            dataCurve.RemoveKey(0);
        for (int i = 0; i < data.Count; i++)
        {
            dataCurve.AddKey(i, data[i]);
        }

        Validate();
    }

    private void Validate()
    {
        if (grid.WidthStep < 0)
            return;

        line = GetComponent<LineRenderer>();

        Rect rect = GetComponent<RectTransform>().rect;
        if (rect.width < 0.0f)
            return;

        List<Vector3> positions = new List<Vector3>();
        for (float x = 0, i = grid.MinValue.x; x < rect.width; x+=grid.WidthStep, i++)
        {
            float value = (Mathf.Clamp(dataCurve.Evaluate(i), grid.MinValue.y, grid.MaxValue.y) - grid.MinValue.y) / grid.HeightDifference * rect.height;
            positions.Add(Vector3.up * value + Vector3.right * x);
        }
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray()); 
    }
}
