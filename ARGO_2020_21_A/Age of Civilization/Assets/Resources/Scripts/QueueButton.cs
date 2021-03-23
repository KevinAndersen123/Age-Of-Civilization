using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QueueButton : MonoBehaviour
{
    public int m_costToBuild = 0;
    public int m_productionCost = 0;

    [SerializeField]
    Text m_resourceUsedText;

    [SerializeField]
    Text m_timeText;

    [SerializeField]
    public string m_resourceUsed;

    int m_timeToBuild;

    private void Start()
    {
        m_resourceUsedText.text = m_costToBuild.ToString();
    }

    public void SetTimeText(int t_val)
    {
        m_timeToBuild = t_val;

        m_timeText.text = m_timeToBuild.ToString();
    }

    public int GetTimeToBuild()
    {
        return m_timeToBuild;
    }
}
