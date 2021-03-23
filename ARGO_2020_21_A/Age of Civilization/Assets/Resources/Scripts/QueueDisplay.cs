using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDisplay : MonoBehaviour
{
    [SerializeField]
    Text m_timeText;

    [SerializeField]
    Text m_itemNameText;
    string m_itemName = "";

    private void Start()
    {
        m_timeText.text = "";
        m_itemNameText.text = "";
    }
    public void SetText(int t_maxTimeTObuild,string t_name, int t_currentTurnVal)
    {
        m_itemName = t_name;

        m_timeText.text = t_currentTurnVal.ToString() + "/" + t_maxTimeTObuild.ToString();

        m_itemNameText.text = t_name;
    }

    public string GetName()
    {
        return m_itemName;
    }
}
