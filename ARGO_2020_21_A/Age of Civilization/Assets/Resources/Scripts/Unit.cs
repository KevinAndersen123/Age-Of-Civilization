using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public int m_speed;
    public int m_maxHealth;
    public int m_attackRange;
    public int m_visionRange;
    public int m_damage;
    public int m_structureDamage;
    public int m_upKeepCost;
    public int m_unitScore;
}

public enum UnitType
{ 
    CombatUnit,
    Settler
}

public class Unit : MonoBehaviour
{   
    public UnitType m_unitType;

    [SerializeField]
    List<TileType> m_traversableTiles;

    [SerializeField]
    protected UnitStats m_stats;

    [SerializeField]
    GameObject m_healthBar;

    SpriteRenderer m_spriteRenderer;

    float m_maxHealthSize;

    bool m_isSelected;

    protected int m_movementLeft = 0;
    protected int m_curHealth;

    protected Tile m_currentTile;

    protected Player m_player;

    protected List<Tile> m_path = new List<Tile>();

    protected bool m_isAnimating;
   
    protected Vector3 m_move;
    protected List<Vector3> m_movementPosition = new List<Vector3>();
    protected int m_smoothMoveStep;
    Color m_baseColour;

    protected bool m_isAI;

    void Awake()
    {
        m_isAnimating = false;
        m_curHealth = m_stats.m_maxHealth;
        m_maxHealthSize = m_healthBar.transform.localScale.x;
    }

    public void Initialize(Player t_player)
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_player = t_player;
        m_baseColour = m_player.GetColor();

        m_spriteRenderer.color = m_baseColour;

        m_movementLeft = m_stats.m_speed;

        m_player.AddUnit(gameObject);
        m_isAI = t_player.m_isAI;
    }

    virtual public void TakeDamage(int t_damage)
    {
        m_curHealth -= t_damage;

        if(m_curHealth <= 0)
        {
            m_curHealth = 0;
            Kill();
        }

        Vector3 scale = m_healthBar.transform.localScale;

        scale.x = m_maxHealthSize * ((float)m_curHealth / m_stats.m_maxHealth);

        m_healthBar.transform.localScale = scale;
    }

    public void TurnStart()
    {
        m_movementLeft = m_stats.m_speed;
       
    }

    virtual public void TurnEnd()
    {
        Move();
       
    }

    public int GetOwnerID()
    {
        return m_player.GetID();
    }

    public void SetCurrentTile(Tile t_tile)
    {
        if(m_currentTile != null)
        {
            m_currentTile.RemoveUnitFromTile(this);
        }

        m_currentTile = t_tile;
        transform.SetParent(m_currentTile.transform);
    }

    public Tile GetCurrentTile()
    {
        return m_currentTile;
    }

    public int GetSpeed()
    {
        return m_stats.m_speed;
    }

    public void SetSelect(bool t_selected)
    {
        m_isSelected = t_selected;

        if (m_isSelected)
        {
            Color highlightColour = Color.Lerp(m_baseColour, Color.white, .5f);
            m_spriteRenderer.color = highlightColour;
        }
        else
        {
            m_spriteRenderer.color = m_baseColour;
        }
    }

    public bool GetIsSelected()
    {
        return m_isSelected;
    }

    virtual public void SetPath(List<Tile> t_path, Tile t_tile)
    {
        m_path = t_path;
        Move();

    }

    public List<Tile> GetPath()
    {
        return m_path;
    }

    virtual public void Move()
    {

        List<Vector3> smoothMovePath = new List<Vector3>();
        smoothMovePath.Add(transform.position);


        if (m_path.Count > 0)
        {
            bool reachedEnd = false;

            while (m_movementLeft != 0 && !reachedEnd)
            {
                if (m_path[m_path.Count - 1].AddUnitToTile(this))
                {
                    smoothMovePath.Add(transform.position);
                    m_path.RemoveAt(m_path.Count - 1);
                    m_movementLeft--;
                }
                else
                {
                    m_path.Clear();

                }

                if (m_path.Count == 0)
                {
                    reachedEnd = true;

                }
            }
        }

        if (!m_isAI)
        {
            SetMovementPositions(smoothMovePath);
        }
        
    }

    public virtual void Kill()
    {
        Debug.Log(gameObject.name + " says 'Kill me Father'");
        
        m_player.RemoveUnit(gameObject);
        m_currentTile.RemoveUnitFromTile(this);

        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return m_curHealth;
    }

    public int GetMovementLeft()
    {
        return m_movementLeft;
    }

    public void SetMovementLeft(int t_movementLeft)
    {
        m_movementLeft = t_movementLeft;
    }

    public UnitType GetUnitType()
    {
        return m_unitType;
    }

    public List<TileType> GetTraversableTiles()
    {
        return m_traversableTiles;
    }

    public Player GetPlayer()
    {
        return m_player;
    }
 
    public void CalculateSmoothMoveStep()
    {
        if (m_movementPosition.Count > 0)
        {
           m_move = m_movementPosition[0] - transform.position;        
           m_move = new Vector2(m_move.x, m_move.y);
           m_move.Normalize();
        }
        else if (m_movementPosition.Count == 1)
        {
            transform.position = m_movementPosition[0];
            m_movementPosition.Clear();
            m_isAnimating = false;
            m_move = new Vector2(0, 0);
        }
        else
        {
            m_movementPosition.Clear();
            m_isAnimating = false;
            m_move = new Vector2(0, 0);
        }
    }

   void Update()
   {
        if (m_isAnimating && !m_isAI)
        {
            SmoothMove();
        }
   }

    virtual public bool SmoothMove()
    {
        if (m_smoothMoveStep != 0 && m_movementPosition.Count>0)
        {
            CalculateSmoothMoveStep();
            transform.position += m_move * 0.1f;
            for (int i = 0; i < m_movementPosition.Count; i++)
            {
                if ((transform.position - m_movementPosition[i]).magnitude < 0.8f)
                {
                    m_smoothMoveStep--;
                    transform.position = m_movementPosition[i];
                    
                    m_movementPosition.RemoveAt(0); 
                    if (m_smoothMoveStep == 0 || m_movementPosition.Count == 0)
                    {
                        m_isAnimating = false;
                    }
                    break;
                }
            }
        }

        if (m_smoothMoveStep == 0|| m_movementPosition.Count == 0)
        {
            m_isAnimating = false;
        }

        return m_isAnimating;
    }

    virtual public void SetMovementPositions(List<Vector3> t_path)
    {
        m_movementPosition.Clear();
        if (t_path.Count > 0)
        {
            for (int i = 0; i < t_path.Count; i++)
            {
                m_movementPosition.Add(t_path[i]);
            }
            transform.position = m_movementPosition[0];
            m_isAnimating = true;
            m_smoothMoveStep = m_stats.m_speed + 1;
        }
    }
    
    public virtual int GetUpkeepCost()
    {
        return m_stats.m_upKeepCost;
    }
    
    public int GetVisionRange()
    {
        return m_stats.m_visionRange;
    }

    public int GetUnitScore()
    {
        return m_stats.m_unitScore;
    }

    public virtual void SetIsAI(bool t_ai)
    {
        m_isAI = t_ai;
    }
}