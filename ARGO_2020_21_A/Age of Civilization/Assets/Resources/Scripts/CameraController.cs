using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraBoundry
{
    public float m_minX, m_minY;
    public float m_maxX, m_maxY;
}

public class CameraController : MonoBehaviour
{
    Vector2 m_halfScreenSize;                       //Pixels contained in camera  
    Vector2 m_mousePosition = new Vector2(0, 0);
    Vector2 m_worldSize;                            //Size of the map
    Vector2 m_halfViewSize;

    [HideInInspector]
    public const int m_MAX_ZOOM = 5;

    [HideInInspector]
    public const int m_MIN_ZOOM = 25;

    int m_screenMargin = 5;

    float m_halfTileWidth;
    float m_halfTileHeight;

    public float m_speed;

    [SerializeField]
    Camera m_camera;

    [HideInInspector]
    public CameraBoundry m_cameraBoundry;

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            Zoom(-1);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f) 
        {
            Zoom(1);
        }

        if (Input.anyKey)
        {
            Vector2 move = new Vector2(0, 0);

            if (Input.GetKey(KeyCode.W))
            {
               move.y = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
               move.y = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                move.x = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                move.x = 1;
            }

            //Purposely letting player move camera diagonal.
            MoveCamera(move);
        }
       
        else
        {
            m_mousePosition = Input.mousePosition;

            if (IsMouseOnScreenEdge(m_screenMargin, m_mousePosition))
            {
                m_mousePosition -= m_halfScreenSize;
                MoveCamera(m_mousePosition);
            }
        }
    }

    public bool IsMouseOnScreenEdge(int t_edgeSize, Vector2 t_mousePos)
    {
        t_mousePos -= m_halfScreenSize;

        // convert mouseposition to positive so that only one side needs to be checked as screen is symmetrical 
        if (Mathf.Abs(t_mousePos.x) > m_halfScreenSize.x - t_edgeSize || Mathf.Abs(t_mousePos.y) > m_halfScreenSize.y - t_edgeSize)
        {           
            return true;
        }
        
        return false;
    }

    public void Zoom(float t_zoomValue)
    {
        m_camera.orthographicSize += t_zoomValue;

        if (m_MAX_ZOOM > m_camera.orthographicSize)
        {
            m_camera.orthographicSize = m_MAX_ZOOM;
        }

        else if (m_MIN_ZOOM < m_camera.orthographicSize)
        {
            m_camera.orthographicSize = m_MIN_ZOOM;
        }

        CalculateHalfViewSize();
        CalculateBoundry();
        CheckOutOfBounds();
    }

    public void MoveCamera(Vector2 t_move)
    {
        t_move = t_move.normalized * m_speed * Time.fixedDeltaTime;

        transform.position += new Vector3(t_move.x, t_move.y, 0);

        CheckOutOfBounds();
    }

    void CalculateBoundry()
    {
        m_cameraBoundry.m_minX = m_halfViewSize.x + m_halfTileWidth;
        m_cameraBoundry.m_minY = m_halfViewSize.y;
        m_cameraBoundry.m_maxX = m_worldSize.x - m_halfViewSize.x;
        m_cameraBoundry.m_maxY = m_worldSize.y - m_halfTileHeight - m_halfViewSize.y;
    }

    void CheckOutOfBounds()
    {
        Vector3 pos = transform.position;

        if(pos.x < m_cameraBoundry.m_minX)
        {
            pos.x = m_cameraBoundry.m_minX;
        }
        else if(pos.x > m_cameraBoundry.m_maxX)
        {
            pos.x = m_cameraBoundry.m_maxX;
        }

        if (pos.y < m_cameraBoundry.m_minY)
        {
            pos.y = m_cameraBoundry.m_minY;
        }
        else if (pos.y > m_cameraBoundry.m_maxY)
        {
            pos.y = m_cameraBoundry.m_maxY;
        }

        transform.position = pos;
    }

    /// <summary>
    /// Set up the dimensions for the camera so that we can use it within a map.
    /// </summary>
    /// <param name="t_width"> amount of tiles in a row</param>
    /// <param name="t_height"> amount of tiles in a column </param>
    /// <param name="t_tileWidth"> width of tile sprite </param>
    /// <param name="t_tileHeight"> heigh of tile sprite </param>
    public void Setup(int t_width, int t_height, float t_tileWidth, float t_tileHeight)
    {
        m_halfTileWidth = t_tileWidth / 2;      
        m_halfTileHeight = t_tileHeight / 2;

        m_halfScreenSize.x = Screen.width / 2;
        m_halfScreenSize.y = Screen.height / 2;

        CalculateHalfViewSize();

        m_worldSize.x = t_width * t_tileWidth;
        m_worldSize.y = t_tileHeight / 4 + t_height * (t_tileHeight * 0.75f);

        CalculateBoundry();
    }

    void CalculateHalfViewSize()
    {
        m_halfViewSize.y = 2f * m_camera.orthographicSize;
        m_halfViewSize.x = m_halfViewSize.y * m_camera.aspect;
        m_halfViewSize /= 2;
    }

    public void PlaceCameraAtWorldCenter()
    {
        transform.position = m_worldSize / 2;
    }

    public Vector2 GetGridSize()
    {
        return m_worldSize;
    }

    public void SetPosition(Vector3 t_positon)
    {
        t_positon.z = transform.position.z;

        transform.position = t_positon;

        CheckOutOfBounds();
    }
}
