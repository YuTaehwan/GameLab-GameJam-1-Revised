using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StarMove : MonoBehaviour
{
    public GameObject starStatObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        StartCoroutine(Move());
    }

    IEnumerator Move() {
        yield return new WaitForSeconds(1.0f);

        float time = 0f;
        Vector3 initPos = GetComponent<RectTransform>().anchoredPosition;
        Vector3 targetPos = new Vector3(570, 460, 0);
        while (time < 1f) {
            time += Time.deltaTime;
            GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(initPos, targetPos, time);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
