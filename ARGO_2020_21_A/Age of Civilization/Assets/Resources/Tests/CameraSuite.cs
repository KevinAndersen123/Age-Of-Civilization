using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraSuite
{
    CameraController m_camera;
    GameObject m_cameraPrefab;
    GameObject m_cameraObject;

    GameObject m_mapGridPrefab;

    [OneTimeSetUp]
    public void Setup()
    {
        m_cameraPrefab = Resources.Load<GameObject>("Prefabs/Camera");
        m_cameraObject = GameObject.Instantiate(m_cameraPrefab);
        m_camera = m_cameraObject.GetComponent<CameraController>();

        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");
        GameObject mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        mapGridObject.GetComponent<MapGrid>().CreateMap();
    }

    [UnityTest, Order(1)]
    public IEnumerator CameraMovesInAllDirections()
    {
        m_camera.PlaceCameraAtWorldCenter();

        Vector3 initialPos = m_cameraObject.transform.position;
        Assert.True(initialPos == m_cameraObject.transform.position);

        m_camera.MoveCamera(new Vector2(-1, 0));
        Vector3 newPosition = m_cameraObject.transform.position;
        Assert.True(initialPos.x > newPosition.x);

        initialPos = newPosition;
        m_camera.MoveCamera(new Vector2(1, 0));
        newPosition = m_cameraObject.transform.position;
        Assert.True(initialPos.x < newPosition.x);

        initialPos = newPosition;
        m_camera.MoveCamera(new Vector2(0, 1));
        newPosition = m_cameraObject.transform.position;
        Assert.True(initialPos.y < newPosition.y);

        initialPos = newPosition;
        m_camera.MoveCamera(new Vector2(0, -1));
        newPosition = m_cameraObject.transform.position;
        Assert.True(initialPos.y > newPosition.y);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator CanDetectMouseAtScreenBorder()
    {
        m_camera.PlaceCameraAtWorldCenter();
        Vector2 testPoint = m_cameraObject.transform.position;
        testPoint = m_camera.gameObject.GetComponent<Camera>().WorldToScreenPoint(testPoint);
        Assert.True(m_camera.IsMouseOnScreenEdge(5, testPoint) == false);

        testPoint = new Vector2(-1000, -1000);
        testPoint = m_camera.gameObject.GetComponent<Camera>().WorldToScreenPoint(testPoint);
        Assert.True(m_camera.IsMouseOnScreenEdge(50, testPoint) == true);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator DoesNotMoveOutOfBounds()
    {
        m_camera.m_speed = 5000;
        m_camera.PlaceCameraAtWorldCenter();

        for (int i = 0; i < 10; i++)
        {
            m_camera.MoveCamera(new Vector2(-1, 0));
        }

        Vector3 cameraPos = m_cameraObject.transform.position;
        Assert.True(cameraPos.x == m_camera.m_cameraBoundry.m_minX);

        for (int i = 0; i < 10; i++)
        {
            m_camera.MoveCamera(new Vector2(1, 0));
        }

        cameraPos = m_cameraObject.transform.position;
        Assert.True(cameraPos.x == m_camera.m_cameraBoundry.m_maxX);

        for (int i = 0; i < 10; i++)
        {
            m_camera.MoveCamera(new Vector2(0, -1));
        }

        cameraPos = m_cameraObject.transform.position;
        Assert.True(cameraPos.y == m_camera.m_cameraBoundry.m_minY);

        for (int i = 0; i < 10; i++)
        {
            m_camera.MoveCamera(new Vector2(0, 1));
        }

        cameraPos = m_cameraObject.transform.position;
        Assert.True(cameraPos.y == m_camera.m_cameraBoundry.m_maxY);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator ZoomIn()
    {
        float ortographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        m_camera.Zoom(-1);

        float newOrtographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        Assert.True(newOrtographicSize < ortographicSize);

        m_camera.Zoom(-100);

        newOrtographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        Assert.True(newOrtographicSize == CameraController.m_MAX_ZOOM);

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator ZoomOut()
    {
        float ortographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        m_camera.Zoom(1);

        float newOrtographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        Assert.True(newOrtographicSize > ortographicSize);

        m_camera.Zoom(100);

        newOrtographicSize = m_cameraObject.GetComponent<Camera>().orthographicSize;

        Assert.True(newOrtographicSize == CameraController.m_MIN_ZOOM);

        yield return null;
    }
}
