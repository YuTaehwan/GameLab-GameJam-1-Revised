using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeScore : MonoBehaviour
{
    public TextMeshProUGUI oldScore;
    public TextMeshProUGUI newScore;

    private bool needPlay;
    private bool isPlaying;
    float timeStamp;

    private float playTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;
        needPlay = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (needPlay)
        {
            needPlay = false;
            StartCoroutine(PlayScoreAnim());
        }
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            timeStamp += Time.deltaTime;
            //Debug.Log(Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 100, 0), timeStamp / playTime));
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 100, 0), timeStamp / playTime);
            if (timeStamp > playTime)
            {
                isPlaying = false;
            }
        }
    }

    public void PlayScore(int oldScore, int newScore)
    {
        this.oldScore.text = "" + oldScore;
        this.newScore.text = "" + newScore;

        isPlaying = true;
    }

    IEnumerator PlayScoreAnim()
    {
        isPlaying = true;
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        timeStamp = 0;
        yield return new WaitForSeconds(playTime);
        isPlaying = false;
    }
}
