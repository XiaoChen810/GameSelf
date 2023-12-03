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

    void Update()
    {
        if (!Lock)
        {
            MoveCameraWithMouse();

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

