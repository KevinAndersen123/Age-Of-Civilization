using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudValues : MonoBehaviour
{
    [SerializeField]
    Text m_gold;

    [SerializeField]
    Text m_horses;

    [SerializeField]
    Text m_iron;

    [SerializeField]
    Text m_wood;

    public void SetHudValues(Player t_player)
    {
        m_gold.text = t_player.GetResourceAmount("Gold").ToString();
        m_horses.text = t_player.GetResourceAmount("Horses").ToString();
        m_iron.text = t_player.GetResourceAmount("Iron").ToString();
        m_wood.text = t_player.GetResourceAmount("Wood").ToString();
    }

}
