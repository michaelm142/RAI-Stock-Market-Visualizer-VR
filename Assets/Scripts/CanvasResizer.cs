using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResizer : MonoBehaviour
{
    public BoxCollider target;

    private RectTransform canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float width = target.size.x * target.transform.parent.transform.localScale.x;
        float height = target.size.y * target.transform.parent.transform.localScale.y;

        float deltaWidth = width - canvas.rect.width;
        float deltaHeight = height - canvas.rect.height;

        canvas.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, deltaWidth / 2.0f);
        canvas.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, deltaWidth / 2.0f);
        canvas.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, deltaHeight / 2.0f);
        canvas.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, deltaHeight / 2.0f);
    }
}
