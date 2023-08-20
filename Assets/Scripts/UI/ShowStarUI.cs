using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowStarUI : MonoBehaviour
{
    public GameObject[] stars = new GameObject[3];

    private float[] startX = new float[3] { 0f, -100f, -200f };

    private bool showStar = false;
    private int targetStarNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        showStar = false;
        for (int i = 0; i < 3; i++) {
            stars[i].SetActive(false);
        }
        DontDestroyObject.gameManager.OnGiveStarCallbacks += ShowStars;
    }

    // Update is called once per frame
    void Update()
    {
        if (showStar) {
            showStar = false;
            StartCoroutine(ShowStarCoroutine());
        }
    }

    void ShowStars(int originalStar, int newStar) {
        showStar = true;
        targetStarNum = newStar;
        for (int i = newStar; i < 3; i++) {
            stars[i].SetActive(false);
        }
    }

    IEnumerator ShowStarCoroutine() {
        for (int i = 0; i < targetStarNum; i++) {
            stars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(startX[targetStarNum - 1] + (200 * i), 0f, 0f);
            stars[i].SetActive(true);
            stars[i].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
