using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public static CameraScaler instance;

    public float zOffset = -10f;
    public float yOffset = 0.5f;
    public float aspectRatio = 0.625f;
    public float padding = 2;

    private void Awake()
    {
        CameraScaler.instance = this;
    }

    private void Start()
    {
        Level info = GameManager.instance.GetLevelInfo();
        if (info != null)
        {
            RepositionCamera(info.width - 1, info.height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, zOffset);
        transform.position = tempPosition;
        if (x >= y)
        {
            Camera.main.orthographicSize = (x / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = y / 2 + padding;
        }
    }
}
