using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TomatoGameMain : MonoBehaviour
{
    public delegate void OnImageLoadedHandler();
    public OnImageLoadedHandler OnImageLoaded;

    [Header("UI Items")]
    [SerializeField] GameObject introTitle;
    [SerializeField] GameObject gameBox;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    [SerializeField] List<BoxButton> images;

    [Header("URLs")]
    [SerializeField] List<string> potatoURLS;
    [SerializeField] string tomatoURL;

    Queue<int> loadingImages;

    int currentURL;
    int currentScore;

    void Start()
    {
        currentURL = 0;

        loadingImages = new Queue<int>();

        OnImageLoaded += NextImage;

        StartCoroutine(StartGame());
    }

    void NextImage()
    {
        if (loadingImages.Count > 0)
        {
            StartCoroutine(GetImage(loadingImages.Dequeue()));
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2);

        LeanTween.scale(introTitle, Vector3.zero, 0.5f).setEaseOutBack();
        LeanTween.scale(gameBox, Vector3.one, 0.5f);

        for (int i = 0; i < images.Count; i++)
        {
            loadingImages.Enqueue(i);
        }

        NextImage();
    }

    IEnumerator GetImage(int imagePlace)
    {
        bool isTomato = false;
        UnityWebRequest request = null;

        if (Random.Range(0, 100) > 25)
        {
            currentURL++;

            if (currentURL == potatoURLS.Count)
            {
                currentURL = 0;
            }

            request = UnityWebRequestTexture.GetTexture(potatoURLS[currentURL]);
        }
        else
        {
            request = UnityWebRequestTexture.GetTexture(tomatoURL);
            isTomato = true;
        }

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (images[imagePlace].image.texture != null)
            {
                Destroy(images[imagePlace].image.texture);
            }

            yield return new WaitForSeconds(0.1f);
            images[imagePlace].image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            images[imagePlace].isTomato = isTomato;
            images[imagePlace].GetComponent<Button>().interactable = true;
        }

        loadingImages.Enqueue(imagePlace);
        OnImageLoaded();
    }

    public void ChangeScore(int amount)
    {
        currentScore += amount;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        scoreText.text = "Score: " + currentScore;

        if (!LeanTween.isTweening(scoreText.gameObject))
        {
            LeanTween.scale(scoreText.gameObject, Vector3.one * 2, 0.5f).setEasePunch();
        }
    }
}
