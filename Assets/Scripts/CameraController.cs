using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float baseScrollSpeed = 5f; // �������ӽ��ƶ��ٶ�
    public float scrollSpeedMultiplier = 2f; // �����ٶȵı�������
    public float scrollBoundary = 10f; // ������괥����Ե�ķ�Χ
    public float minX = -10f; // X�����Сֵ
    public float maxX = 10f;  // X������ֵ

    public CinemachineVirtualCamera virtualCamera;
    public GameObject player;
    public bool Lock;

    // �ֻ��ƶ���ͷ��ʽ
    private Vector2 touchStartPos; // ��¼������ʼλ��
    public float dragSpeed = 2f; // �����϶��ƶ����ٶ�

    void Update()
    {
        if (!Lock)
        {
            MoveCameraWithMouse();
            HandleTouchInput();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // �ƶ�����ɫλ��
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

        // �������Ƿ�����Ļ���Ե
        if (mousePosition.x < scrollBoundary && transform.position.x > minX)
        {
            moveDirection += Vector3.left;
        }
        // �������Ƿ�����Ļ�ұ�Ե
        else if (mousePosition.x > Screen.width - scrollBoundary && transform.position.x < maxX)
        {
            moveDirection += Vector3.right;
        }

        // ���㶯̬�ƶ��ٶ�
        float dynamicScrollSpeed = baseScrollSpeed + Mathf.Abs(mousePosition.x - Screen.width / 2) / Screen.width * scrollSpeedMultiplier;

        // �ƶ����
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

        // �����������X���ϵ��ƶ���Χ
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // �ƶ������
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

