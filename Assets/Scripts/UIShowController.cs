using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowController : MonoBehaviour
{
    private static UIShowController instance;
    public static UIShowController Instance { get { return instance; } }

   public Text cherrynum,gem;
    public Image isTwoJump;
    public GameObject dialog;//¶Ô»°¿ò


    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        cherrynum = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        gem= transform.GetChild(1).GetChild(0).GetComponent<Text>();
        isTwoJump = transform.GetChild(2).GetChild(1).GetComponent<Image>();
        dialog = transform.GetChild(3).gameObject;
    }

}
