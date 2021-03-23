using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMovement : MonoBehaviour
{
    public float speed;
    public int m_endPos;

    [SerializeField]
    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, -750, 0);
    }
    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            rectTransform.localPosition = new Vector3(0,m_endPos,0);
        }
        if (transform.position.y < m_endPos)
        {
            rectTransform.localPosition = rectTransform.localPosition + new Vector3(0, 10, 0) * Time.deltaTime * speed;
        }
        else
        {
           SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        } 
    }
}
