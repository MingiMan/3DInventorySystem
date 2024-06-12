using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    float runSpeed = 8f;
    [SerializeField]
    float rotationSpeed = 500f;
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    GameObject inventoryUI;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    float ySpeed;

    public static bool isActive = true;
    Quaternion targetRotation;

    Animator animor;
    CharacterController characterController;
    MeeleFighter meeleFighter;


    [System.Serializable]
    public class InputSettings
    {
        public string verticalInput = "Vertical";
        public string horizontalInput = "Horizontal";
        public string sprintInput = "Sprint";
        public string aim = "Fire2";
        public string fire = "Fire1";
    }

    [SerializeField]
    public InputSettings input;

    [Header("Gravity Settings")]
    public float gracityValue = 1.2f;
    public Transform camCenter;
    Transform mainCam;




    private void Awake()
    {
        animor = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        meeleFighter = GetComponent<MeeleFighter>();
        mainCam = Camera.main.transform;
    }


    private void Update()
    {
        if (isActive)
        {
            MoveMent();
            PlayerAttack();
        }
        Inventory();
    }

    private void MoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));


        var moveInput = (new Vector3(h, 0, v)).normalized;

        var moveDir = cameraController.PlanarRotation * moveInput;

        GroundCheck();

        if (isGrounded)
            ySpeed = -0.5f;

        else
            ySpeed += Physics.gravity.y * Time.deltaTime;

        var velocity = moveDir * currentMoveSpeed;

        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
           rotationSpeed * Time.deltaTime);

        animor.SetFloat("moveAmount", moveAmount);
        animor.SetBool("IsRun", Input.GetKey(KeyCode.LeftShift));
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Attack"))
        {
            meeleFighter.TryToAttack();
        }
    }

    private void Inventory()
    {
        if (Input.GetButtonDown("Inventory"))
            inventoryUI.SetActive(!inventoryUI.activeSelf);

        if (inventoryUI.activeSelf)
        {
            isActive = false;
            ResetAnimator();
        }
        else
            isActive = true;
    }

    private void ResetAnimator()
    {
        animor.SetFloat("moveAmount", 0);
        animor.SetBool("IsRun", false);
    }
}
