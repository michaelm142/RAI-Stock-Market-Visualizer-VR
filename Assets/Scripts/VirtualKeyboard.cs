using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class VirtualKeyboard : MonoBehaviour
{
    public TextMeshProUGUI target;

    public UnityEvent onSubmit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterKey(string key)
    {
        if (target == null)
            return;

        target.text += key;
        Debug.Log("Entered key: " + key);
    }

    public void Delete()
    {
        if (target == null || target.text.Length == 0)
            return;

        target.text = target.text.Remove(target.text.Length - 1);
    }

    public void Submit()
    {
        if (target == null)
            return;

        GetComponent<Animator>().SetBool("Active", false);
        onSubmit.Invoke();
    }
}
