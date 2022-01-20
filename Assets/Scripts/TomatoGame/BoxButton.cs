using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxButton : MonoBehaviour
{
   public bool isTomato;
    [HideInInspector] public RawImage image;
    [SerializeField] TomatoGameMain main;

    private void Start()
    {
        image = GetComponent<RawImage>();
    }

    public void Clicked()
    {
        GetComponent<Button>().interactable = false;

        if (isTomato)
        {
            main.ChangeScore(1);
        }
        else
        {
            main.ChangeScore(-1);
        }
    }
}
