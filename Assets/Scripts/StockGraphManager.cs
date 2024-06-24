using FinanceModule;
using Meta.WitAi.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StockGraphManager : MonoBehaviour
{
    public GameObject stockGraphPrefab;
    public GameObject watchlistPrefab;
    public GameObject canvasPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Stonks.Load();
        CreateStockTicker(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private GameObject CreateCanvas(string name, float width, float height)
    {
        GameObject stockGraph = Instantiate(canvasPrefab);
        stockGraph.name = name;
        Canvas canvas = stockGraph.GetComponent<Canvas>();
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canvas.referencePixelsPerUnit = 0.01f;
        canvas.sortingOrder = -100;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.referencePixelsPerUnit = 1;
        BoxCollider box = stockGraph.GetComponentInChildren<BoxCollider>();
        box.size = Vector3.up * height + Vector3.right * width;

        return stockGraph;
    }

    public void CreateStockTicker(Vector3 position)
    {
        GameObject canvas = CreateCanvas("StockTicker", 2.032f, 2.794f);
        canvas.transform.position = position;
        canvas.transform.localScale = Vector3.one * 0.1f;

        GameObject watchList = Instantiate(watchlistPrefab, canvas.transform);
    }

    public GameObject CreateStockGraph(string symbol, Vector3 position)
    {
        Stonk stonk = Stonks.Get(symbol.ToUpper());
        if (stonk == null)
        {
            Debug.LogError("Unable to get stock from Yahoo");
            return null;
        }
        Debug.Log("Downloaded data for " + stonk.CompanyName);

        // create canvas
        GameObject stockGraph = CreateCanvas(string.Format("Stock Graph for {0}", symbol), 1.0f, 1.0f);
        stockGraph.transform.position = position;

        // create graph element
        GameObject graphElement = Instantiate(stockGraphPrefab, stockGraph.transform);
        graphElement.transform.Find("Controls/StockSymbol").GetComponent<TMPro.TMP_InputField>().text = stonk.Symbol;
        graphElement.transform.Find("Controls/StockName").GetComponent<TextMeshProUGUI>().text = stonk.CompanyName;

        // download historical data
        GridLineController grid = graphElement.transform.Find("StockInfo/Graph/GridLines").GetComponent<GridLineController>();


        StartCoroutine("DownloadDataForStockGraphCoroutine", new object[] { stonk, System.DateTime.Now.AddDays(-(grid.MaxValue.x - grid.MinValue.x)), System.DateTime.Now, grid, graphElement });

        return stockGraph.gameObject;
    }

    IEnumerator DownloadDataForStockGraphCoroutine(object[] prams)
    {
        Stonk stonk = prams[0] as Stonk; 
        System.DateTime start = (System.DateTime)prams[1];
        System.DateTime end = (System.DateTime)prams[2];
        GridLineController grid = prams[3] as GridLineController;
        GameObject graphElement = prams[4] as GameObject;
        foreach (int i in stonk.DownloadStockData(start, end))
        {
            yield return new WaitForSeconds(1);
        }    

        Debug.Log(string.Format("Downloading data from {0} to {1}", System.DateTime.Now.AddDays(-(grid.MaxValue.x - grid.MinValue.x)), System.DateTime.Now));
        List<float>[] boxAndWhiskerData = new List<float>[4]
        {
            new List<float>(),
            new List<float>(),
            new List<float>(),
            new List<float>()
        };
        List<float> lineData = new List<float>();
        for (int i = 0; i < stonk.HistoricalData.Count; i++)
        {
            Stonket stonket = stonk.HistoricalData[i];
            boxAndWhiskerData[0].Add((float)stonket.OpeningPrice);
            boxAndWhiskerData[1].Add((float)stonket.LowPrice);
            boxAndWhiskerData[2].Add((float)stonket.HighPrice);
            boxAndWhiskerData[3].Add((float)stonket.ClosingPrice);
            lineData.Add((float)stonket.ClosingPrice);
        }

        graphElement.transform.Find("StockInfo/Graph/BoxAndWhiskerPlot").GetComponent<BoxAndWhiskerLineController>().SetData(boxAndWhiskerData);
        graphElement.transform.Find("StockInfo/Graph/Line").GetComponent<GraphLineController>().SetData(lineData);
        yield return null;
    }
}
