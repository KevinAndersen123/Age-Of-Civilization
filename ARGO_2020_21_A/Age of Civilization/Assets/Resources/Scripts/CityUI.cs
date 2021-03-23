using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityUI : MonoBehaviour
{
    [SerializeField]
    GameObject m_cityCanvas = null;

    GameObject m_hud = null;

    [SerializeField]
    Text m_cityNameText = null;

    [SerializeField]
    Text m_populationText = null;

    [SerializeField]
    Text m_goldText = null;

    [SerializeField]
    Text m_foodText = null;

    [SerializeField]
    Text m_productionText = null;

    [SerializeField]
    Text m_newPopText = null;

    [SerializeField]
    Text m_nextExpansionText = null;

    public Slider m_foodBasket = null;

    bool m_isActive = false;

    City m_selectedCity;

    [SerializeField]
    public List<GameObject> m_resourceButtons;

    [SerializeField]
    public List<GameObject> m_buildingButtons;

    [SerializeField]
    public List<GameObject> m_unitButtons;

    [SerializeField]
    GameObject m_queueDisplay;

    public void SetValues(City t_city)
    {
        m_hud = GameObject.FindGameObjectWithTag("HUD");

        m_selectedCity = t_city;
        m_cityNameText.text = m_selectedCity.m_name;

        SetPopulationTexts();
        SetProduceTexts();
        SetExpansionText();
        SetButtons();
        ToggleCanvas();
        SetButtonTurnTime();

        if (m_selectedCity.m_itemInQueue)
        {
            ToggleButtons(false);
        }
        else
        {
            ToggleButtons(true);
            SetButtons();
        }
    }

    void SetPopulationTexts()
    {
        m_populationText.text = m_selectedCity.m_population.ToString();

        m_foodBasket.maxValue = m_selectedCity.m_maxFood;
        m_foodBasket.value = m_selectedCity.m_foodBasket;

        if (m_selectedCity.m_foodTurns != -1)
        {
            m_newPopText.text = m_selectedCity.m_foodTurns.ToString() + " turns until next Population Growth";
        }
        else
        {
            m_newPopText.text = "Your Population is not growing!";
        }
    }

    void SetProduceTexts()
    {
        m_goldText.text = m_selectedCity.m_gold.ToString();
        m_foodText.text = m_selectedCity.m_food.ToString();
        m_productionText.text = m_selectedCity.m_production.ToString();
    }

    void SetExpansionText()
    {
        m_nextExpansionText.text = m_selectedCity.m_borderExpansionCount.ToString() + " / " + m_selectedCity.m_nextExpansion.ToString() + " until city expansion";
    }

    public void ToggleCanvas()
    {
        if (m_isActive == false)
        {
            m_isActive = true;
            m_cityCanvas.SetActive(true);
            m_hud.SetActive(false);
        }
        else
        {
            m_isActive = false;
            m_cityCanvas.SetActive(false);
            m_hud.SetActive(true);
        }
    }

    public void AddBuildingToQueue(int t_type)
    {
        if (t_type <= 3)
        {
            SetButtonTurnTime();
            m_selectedCity.SetBuildItemStats("Resource Building", t_type, m_queueDisplay);
            m_selectedCity.RemoveResource(m_resourceButtons[t_type].GetComponent<QueueButton>().m_resourceUsed, m_resourceButtons[t_type].GetComponent<QueueButton>().m_costToBuild);
        }
        else
        {
            SetButtonTurnTime();
            m_selectedCity.SetBuildItemStats("Building", t_type, m_queueDisplay);
            m_selectedCity.RemoveResource(m_buildingButtons[t_type - 4].GetComponent<QueueButton>().m_resourceUsed, m_buildingButtons[t_type - 4].GetComponent<QueueButton>().m_costToBuild);
        }
        SetButtons();
    }

    public void AddUnitToQueue(int t_unitID)
    {
        SetButtonTurnTime();
        m_selectedCity.SetBuildItemStats("Unit", t_unitID,m_queueDisplay);
        m_selectedCity.RemoveResource(m_unitButtons[t_unitID].GetComponent<QueueButton>().m_resourceUsed, m_unitButtons[t_unitID].GetComponent<QueueButton>().m_costToBuild);
        SetButtons();
    }

    public void SetButtons()
    {
        if (!m_selectedCity.m_itemInQueue)
        {
            m_queueDisplay.GetComponent<QueueDisplay>().SetText(0, "", 0);
            CheckIfEnoughMaterial();
            CheckIfEnabledBuildings();
            foreach (GameObject resourceButton in m_resourceButtons)
            {
                resourceButton.SetActive(false);
            }

            foreach (Tile tile in m_selectedCity.m_resourceTiles)
            {
                m_resourceButtons[(int)tile.GetResourceTile().GetResourceType()].SetActive(true);
            }

            Vector3 position = new Vector3(0, -50, 0);

            foreach (GameObject resourceButton in m_resourceButtons)
            {
                if (resourceButton.activeSelf == true)
                {
                    resourceButton.GetComponent<RectTransform>().localPosition = position;
                    position.y -= 55;
                }
            }

            foreach (GameObject buildingButton in m_buildingButtons)
            {
                if (buildingButton.activeSelf == true)
                {
                    buildingButton.GetComponent<RectTransform>().localPosition = position;
                    position.y -= 55;
                }
            }

            position.y = -50;
            foreach (GameObject unitButton in m_unitButtons)
            {
                if (unitButton.activeSelf == true)
                {
                    unitButton.GetComponent<RectTransform>().localPosition = position;
                    position.y -= 40;
                }
            }
        }
        else
        {
            m_queueDisplay.GetComponent<QueueDisplay>().SetText(m_selectedCity.m_maxTimeToBuild, m_selectedCity.m_itemInqueueName, m_selectedCity.m_queueCounter);
            ToggleButtons(false);
        }
    }

    public void CheckIfEnoughMaterial()
    {
        foreach (GameObject buildingButton in m_buildingButtons)
        {
            int cost = buildingButton.GetComponent<QueueButton>().m_costToBuild;

            if (m_selectedCity.GetOwner().GetResourceAmount("Wood") >= cost)
            {
                buildingButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                buildingButton.GetComponent<Button>().interactable = false;
            }
        }

        foreach (GameObject resourceButton in m_resourceButtons)
        {
            int cost = resourceButton.GetComponent<QueueButton>().m_costToBuild;
            if (m_selectedCity.GetOwner().GetResourceAmount("Wood") >= cost)
            {
                resourceButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                resourceButton.GetComponent<Button>().interactable = false;
            }
        }

        foreach (GameObject unitButon in m_unitButtons)
        {
            int cost = unitButon.GetComponent<QueueButton>().m_costToBuild;
            if (m_selectedCity.GetOwner().GetResourceAmount(unitButon.GetComponent<QueueButton>().m_resourceUsed) >= cost)
            {
                unitButon.GetComponent<Button>().interactable = true;
            }
            else
            {
                unitButon.GetComponent<Button>().interactable = false;
            }
        }
    }

    public bool GetIsActive()
    {
        return m_isActive;
    }

    public void CheckIfEnabledBuildings()
    {
        if (m_selectedCity.m_enabledBarracks)
        {
            m_buildingButtons[0].SetActive(false);
            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "Spearman" || button.name == "Archer" || button.name == "Warrior")
                {
                    button.SetActive(true);
                }
            }
        }
        else
        {
            m_buildingButtons[0].SetActive(true);
            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "Spearman" || button.name == "Archer" || button.name == "Warrior")
                {
                    button.SetActive(false);
                }
            }
        }

        if (m_selectedCity.m_enabledWorkshop)
        {
            m_buildingButtons[1].SetActive(false);

            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "BatteringRam" || button.name == "Catapult")
                {
                    button.SetActive(true);
                }
            }
        }
        else
        {
            m_buildingButtons[1].SetActive(true);
            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "BatteringRam" || button.name == "Catapult")
                {
                    button.SetActive(false);
                }
            }
        }

        if (m_selectedCity.m_enabledStables)
        {
            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "Horseman" || button.name == "ArchHorse")
                {
                    button.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject button in m_unitButtons)
            {
                if (button.name == "Horseman" || button.name == "ArchHorse")
                {
                    button.SetActive(false);
                }
            }
        }
    }
    public void ToggleButtons(bool t_toggle)
    {
        foreach (GameObject button in m_buildingButtons)
        {
            button.GetComponent<Button>().interactable = t_toggle;
        }

        foreach (GameObject button in m_resourceButtons)
        {
            button.GetComponent<Button>().interactable = t_toggle;
        }
        foreach (GameObject button in m_unitButtons)
        {
            button.GetComponent<Button>().interactable = t_toggle;
        }
    }

    public List<GameObject> GetResourceButtons()
    {
        return m_resourceButtons;
    }

    public List<GameObject> GetBuildingButtons()
    {
        return m_buildingButtons;
    }
    public List<GameObject> GetUnitButtons()
    {
        return m_unitButtons;
    }

    public void SetButtonTurnTime()
    {
        foreach (GameObject button in m_resourceButtons)
        {
            button.GetComponent<QueueButton>().SetTimeText(CaluculateProductionTime(button));
        }
        foreach (GameObject button in m_buildingButtons)
        {
            button.GetComponent<QueueButton>().SetTimeText(CaluculateProductionTime(button));
        }
        foreach (GameObject button in m_unitButtons)
        {
            button.GetComponent<QueueButton>().SetTimeText(CaluculateProductionTime(button));
        }
    }

    public int CaluculateProductionTime(GameObject t_button)
    {
        int result = Mathf.CeilToInt(t_button.GetComponent<QueueButton>().m_productionCost / m_selectedCity.m_production);
        return Mathf.Min(result, 9);
    }
}
