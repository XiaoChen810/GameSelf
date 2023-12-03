using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float baseScrollSpeed = 5f; // 基础的视角移动速度
    public float scrollSpeedMultiplier = 2f; // 滑动速度的倍增因子
    public float scrollBoundary = 10f; // 定义鼠标触碰边缘的范围
    public float minX = -10f; // X轴的最小值
    public float maxX = 10f;  // X轴的最大值

    public CinemachineVirtualCamera virtualCamera;
    public GameObject player;
    public bool Lock;

    // 手机移动镜头方式
    private Vector2 touchStartPos; // 记录触摸开始位置
    public float dragSpeed = 2f; // 调整拖动移动的速度

    void Update()
    {
        if (!Lock)
        {
            MoveCameraWithMouse();
            HandleTouchInput();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 移动到角色位置
                transform.position = new Vector3(player.transform.position.x, 0, 0);
            }

            if (Player.Instance.isIndoor)
            {
                Lock = true;
                transform.position = new Vector2(-10, 16);
                virtualCamera.m_Lens.OrthographicSize = 4;
            }
        }
        else
        {
            if (!Player.Instance.isIndoor)
            {
                Lock = false;
                transform.position = new Vector2(-10, 0);
                virtualCamera.m_Lens.OrthographicSize = 6.5f;
            }
        }
    }

    void MoveCameraWithMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // 检测鼠标是否在屏幕左边缘
        if (mousePosition.x < scrollBoundary && transform.position.x > minX)
        {
            moveDirection += Vector3.left;
        }
        // 检测鼠标是否在屏幕右边缘
        else if (mousePosition.x > Screen.width - scrollBoundary && transform.position.x < maxX)
        {
            moveDirection += Vector3.right;
        }

        // 计算动态移动速度
        float dynamicScrollSpeed = baseScrollSpeed + Mathf.Abs(mousePosition.x - Screen.width / 2) / Screen.width * scrollSpeedMultiplier;

        // 移动相机
        transform.position += moveDirection.normalized * dynamicScrollSpeed * Time.deltaTime;
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    Vector2 deltaPos = touch.position - touchStartPos;
                    MoveCamera(-deltaPos.x);
                    touchStartPos = touch.position;
                    break;
            }
        }
    }

    void MoveCamera(float deltaHorizontal)
    {
        Vector3 moveDirection = new Vector3(deltaHorizontal, 0f, 0f);
        Vector3 newPosition = transform.position + moveDirection * dragSpeed * Time.deltaTime;

        // 限制摄像机在X轴上的移动范围
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // 移动摄像机
        transform.position = newPosition;
    }

    private void OnEnable()
    {
        Statistics.Instance.OnDayDark += ResetTransform;
    }

    private void ResetTransform()
    {
        transform.position = Vector3.zero;
    }

    private void OnDestroy()
    {
        Statistics.Instance.OnDayDark -= ResetTransform;
    }
}

