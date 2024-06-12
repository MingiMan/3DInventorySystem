using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region ����
    [SerializeField] Transform followTarget;

    // ȸ���ӵ�
    [SerializeField] float rotationSpeed = 2f;


    // �ּ� ���� ����
    [SerializeField] float minVerticalAngle = -20;

    // �ִ� ���� ����
    [SerializeField] float maxVerticalAngle = 90;

    // ����� �߽����� �� ȭ���� ������
    [SerializeField] Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [SerializeField] LayerMask collsionLayer;


    [SerializeField] public float minDistance = 3f;

    [SerializeField] public float maxDistance = 5f;

    Camera mainCam;


    // float fixDistance = 4f;

    float finalDistance;

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    #endregion

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        CameraFollowTarget();
    }


    private void CameraFollowTarget()
    {
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPostion = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);


        transform.position = focusPostion - targetRotation * new Vector3(0, 0, finalDistance);
        transform.rotation = targetRotation;
    }
}