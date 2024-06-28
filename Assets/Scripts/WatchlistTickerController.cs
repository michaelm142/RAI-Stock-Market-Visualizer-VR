using System;
using System.Xml;
using FinanceModule;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class WatchlistTickerController : MonoBehaviour
{
    public List<GameObject> tickers = new List<GameObject>();

    private VirtualKeyboard virtualKeyboard;

    public Transform Addbutton;

    public GameObject tickerPrefab;

    public float TickerSpacing = 0;

    private string tickerLayout = "Last Price\tChange\t% Change\t\n${0}\t{1}<color=\"{2}\">${3}{4}\t<color=\"{5}\">{6}%";

    // Start is called before the first frame update
    void Start()
    {
        virtualKeyboard = FindObjectOfType<VirtualKeyboard>();
        LoadTickers();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // called from button in unity
    public void AddTicker()
    {
        AddTicker(null);
    }

    public GameObject AddTicker(string stockSymbol = null)
    {
        Rect rect = GetComponent<RectTransform>().rect;
        Rect prefabRect = tickerPrefab.GetComponent<RectTransform>().rect;
        GameObject ticker = Instantiate(tickerPrefab, transform);
        ticker.name = "Ticker #" + tickers.Count.ToString();
        ticker.transform.localPosition = Vector3.down * (prefabRect.height + TickerSpacing) * tickers.Count;
        ticker.GetComponent<StockTickerDragDrop>().stockSymbol = stockSymbol;
        tickers.Add(ticker);
        //tickers.ForEach(t => t.transform.SetParent(null));
        //Addbutton.transform.SetParent(null);
        GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0.0f, rect.height + (prefabRect.height + TickerSpacing));
        //tickers.ForEach(t => t.transform.SetParent(transform));
        //Addbutton.transform.SetParent(transform);

        //Addbutton.transform.localPosition = Vector3.zero;

        ticker.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(delegate () { RemoveTicker(ticker); });
        TextMeshProUGUI stockSymbolInputField = ticker.transform.Find("StockSymbol/Text").GetComponent<TextMeshProUGUI>();
        stockSymbolInputField.GetComponent<EventTrigger>().triggers.Find(t => t.eventID == EventTriggerType.PointerClick).callback
            .AddListener(delegate (BaseEventData data) { OpenKeyboard(stockSymbolInputField); });
        if (stockSymbol != null)
            stockSymbolInputField.text = stockSymbol;
        transform.localPosition = Vector3.zero;

        return ticker;
    }

    public void RemoveTicker(GameObject ticker)
    {
        Vector3 startPoint = ticker.transform.localPosition;

        int index = tickers.IndexOf(ticker);
        Destroy(ticker);
        tickers.RemoveAt(index);
        for (int i = index; i < tickers.Count; i++)
        {
            tickers[i].transform.localPosition = startPoint + Vector3.down * (tickerPrefab.GetComponent<RectTransform>().rect.height + TickerSpacing) * (i - index);
        }
        Rect rect = GetComponent<RectTransform>().rect;
        Rect prefabRect = tickerPrefab.GetComponent<RectTransform>().rect;
        GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 50.0f, rect.height - (prefabRect.height + TickerSpacing));
    }

    void UpdateTicker(string symbol, GameObject ticker)
    {
        Debug.Log("Downloading data from: " + symbol);
       
        // sanatize input
        symbol = symbol.Split(new char[] { (char)0x0b, (char)0x0a }, StringSplitOptions.RemoveEmptyEntries)[0];
        // download stock data
        Stonk stonk = Stonks.Get(symbol.ToUpper());
        if (stonk == null)
        {
            Debug.LogError("Failed to get stock data for: " + symbol);
            return;
        }

        Debug.Log("Downloaded data for: " + stonk.ToString());

        // download stock data
        var twoDayData = Stonks.GetTwoDayStockData(stonk, DateTime.Now, DateTime.Now.AddDays(-1));
        Stonket yesterdayStonket = twoDayData[0];
        Stonket todayStonket = twoDayData[1];

        decimal closePrice = todayStonket.AdjustedClosingPrice;
        decimal priceChange = todayStonket.AdjustedClosingPrice - yesterdayStonket.AdjustedClosingPrice;
        decimal percentChange = System.Math.Round(priceChange / yesterdayStonket.AdjustedClosingPrice * 100, 2);
        closePrice = Math.Round(closePrice, 2);
        priceChange = Math.Round(priceChange, 2);

        ticker.GetComponent<StockTickerDragDrop>().stockSymbol = stonk.Symbol;

        ticker.transform.Find("Ticker/Text").GetComponent<TextMeshProUGUI>().text =
            string.Format(tickerLayout, 
            /*0*/closePrice, 
            /*1*/closePrice.ToString().Length < 5 ? "\t" : "", 
            /*2*/priceChange > 0 ? "green" : "red", 
            /*3*/Math.Abs(priceChange), 
            /*4*/Math.Abs(priceChange).ToString().Length < 5 ? "\t" : "",
            /*5*/percentChange > 0 ? "green" : "red", 
            /*6*/percentChange);
    }

    void OpenKeyboard(TextMeshProUGUI target)
    {
        virtualKeyboard.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.127f;
        virtualKeyboard.target = target;
        virtualKeyboard.onSubmit.AddListener(delegate () { UpdateTicker(target.text, target.transform.parent.parent.gameObject); });
        virtualKeyboard.GetComponent<Animator>().SetBool("Active", true);
    }

    private void OnDestroy()
    {
        SaveTickers();
    }

    void SaveTickers()
    {
        // save all tickers to a local file
        XmlDocument doc = new XmlDocument();
        XmlElement rootElement = doc.CreateElement("TickerRoot");
        doc.AppendChild(rootElement);
        foreach (var ticker in tickers)
        {
            XmlElement tickerElement = rootElement.AppendChild(doc.CreateElement("Ticker")) as XmlElement;
            XmlAttribute attr = tickerElement.Attributes.Append(doc.CreateAttribute("Symbol"));
            attr.Value = ticker.GetComponent<StockTickerDragDrop>().stockSymbol;
        }

        doc.Save("Tickers.xml");
    }


    void LoadTickers()
    {
        // ignore this function if the ticker file does not exist
        FileInfo file = new FileInfo("Tickers.xml");
        if (!file.Exists)
            return;

        // load in tickers from file
        XmlDocument doc = new XmlDocument();
        doc.Load("Tickers.xml");
        XmlElement root = doc.SelectSingleNode("TickerRoot") as XmlElement;
        foreach (XmlElement tickerElement in root.ChildNodes)
        {
            string symbol = tickerElement.Attributes["Symbol"].Value;
            var ticker = AddTicker(symbol);
            UpdateTicker(symbol, ticker);
        }
    }
}
