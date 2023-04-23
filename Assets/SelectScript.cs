using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScript : MonoBehaviour
{
    public Button b2x2;
    public Button b3x2;
    public Button b3x3;
    public Button b5x2;
    public Button b4x3;
    public Button b4x4;
    public Button b5x5;
    public Button b6x6;
    public GameObject canvas2x2;
    public GameObject canvas3x2;
    public GameObject canvas3x3;
    public GameObject canvas5x2;
    public GameObject canvas4x3;
    public GameObject canvas4x4;
    public GameObject canvas5x5;
    public GameObject canvas6x6;
    // Start is called before the first frame update
    void Start()
    {
        b2x2.onClick.AddListener(listen2x2);
        b3x2.onClick.AddListener(listen3x2);
        b3x3.onClick.AddListener(listen3x3);
        b5x2.onClick.AddListener(listen5x2);
        b4x3.onClick.AddListener(listen4x3);
        b4x4.onClick.AddListener(listen4x4);
        b5x5.onClick.AddListener(listen5x5);
        b6x6.onClick.AddListener(listen6x6);
        canvas2x2.SetActive(false);
        canvas3x2.SetActive(false);
        canvas3x3.SetActive(false);
        canvas5x2.SetActive(false);
        canvas4x3.SetActive(false);
        canvas4x4.SetActive(false);
        canvas5x5.SetActive(false);
        canvas6x6.SetActive(false);
    }

    // Update is called once per frame
    void listen2x2()
    {
        canvas2x2.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen3x2()
    {
        canvas3x2.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen3x3()
    {
        canvas3x3.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen5x2()
    {
        canvas5x2.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen4x3()
    {
        canvas4x3.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen4x4()
    {
        canvas4x4.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen5x5()
    {
        canvas5x5.SetActive(true);
        gameObject.SetActive(false);
    }
    void listen6x6()
    {
        canvas6x6.SetActive(true);
        gameObject.SetActive(false);
    }
}
