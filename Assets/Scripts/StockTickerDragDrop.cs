using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StockTickerDragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public string stockSymbol { get; set; }

    private GameObject currentGraph;

    public void SetSymbol(TextMeshProUGUI text)
    {
        stockSymbol = text.text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentGraph != null)
            return;
        OVRPointerEventData data = eventData as OVRPointerEventData;
        currentGraph = FindObjectOfType<StockGraphManager>().CreateStockGraph(stockSymbol, data.pointer.position);
        currentGraph.transform.SetParent(data.pointer.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentGraph.transform.SetParent(null);
        currentGraph = null;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
