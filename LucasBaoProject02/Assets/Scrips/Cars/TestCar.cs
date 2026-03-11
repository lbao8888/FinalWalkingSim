using UnityEngine;

public class TestCar : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider FLWheelColl;
    public WheelCollider FRWheelColl;
    public WheelCollider RLWheelColl;
    public WheelCollider RRWheelColl;

    [Header("Car Settings")]
    public float motorTorque = 1500f;
    public float brakeTorque = 3000f;
    public float maxSteeringAngle = 30f;

    [Header("Player Exit")]
    public Transform exitPoint;

    [Header("Camera Point")]
    public Transform cameraPoint;

    public bool isDriving = false;

    private CCPlayer player;

    private Transform originalCameraParent;

    private float horizontal;
    private float vertical;

    void Start()
    {
        SetupWheelFriction();
    }

    void Update()
    {
        if (!isDriving) return;

        // 获取输入
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // 下车
        if (Input.GetKeyDown(KeyCode.F))
        {
            ExitCar();
        }
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        Move();
        Steer();
        Brake();
    }

    // =========================
    // 玩家进入车
    // =========================

    public void EnterCar(CCPlayer p)
    {
        player = p;
        isDriving = true;

        Debug.Log("Player Entered Car");

        player.SetControlsLocked(true);
        player.EnablePlayerColliders(false);

        player.GetComponent<CharacterController>().enabled = false;

        if (player.playerMesh != null)
            player.playerMesh.SetActive(false);

        // 记录原始摄像机父物体
        originalCameraParent = Camera.main.transform.parent;

        // 摄像机切到车
        Camera.main.transform.SetParent(cameraPoint);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }

    // =========================
    // 玩家下车
    // =========================

    void ExitCar()
    {
        Debug.Log("Player Exit Car");

        isDriving = false;

        if (player.playerMesh != null)
            player.playerMesh.SetActive(true);

        player.transform.position = exitPoint.position;

        player.SetControlsLocked(false);

        player.GetComponent<CharacterController>().enabled = true;
        player.EnablePlayerColliders(true);

        // 摄像机回玩家
        Camera.main.transform.SetParent(originalCameraParent);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }

    // =========================
    // 车辆移动
    // =========================

    void Move()
    {
        RLWheelColl.motorTorque = motorTorque * vertical;
        RRWheelColl.motorTorque = motorTorque * vertical;
    }

    // =========================
    // 转向
    // =========================

    void Steer()
    {
        float steer = maxSteeringAngle * horizontal;

        FLWheelColl.steerAngle = steer;
        FRWheelColl.steerAngle = steer;
    }

    // =========================
    // 刹车
    // =========================

    void Brake()
    {
        float brake = Input.GetKey(KeyCode.Space) ? brakeTorque : 0;

        FLWheelColl.brakeTorque = brake;
        FRWheelColl.brakeTorque = brake;
        RLWheelColl.brakeTorque = brake;
        RRWheelColl.brakeTorque = brake;
    }

    void SetupWheelFriction()
    {
        // 前轮
        FLWheelColl.forwardFriction = CopyFriction(FLWheelColl.forwardFriction, 1.5f);
        FRWheelColl.forwardFriction = CopyFriction(FRWheelColl.forwardFriction, 1.5f);
        FLWheelColl.sidewaysFriction = CopyFriction(FLWheelColl.sidewaysFriction, 1.4f);
        FRWheelColl.sidewaysFriction = CopyFriction(FRWheelColl.sidewaysFriction, 1.4f);

        // 后轮
        RLWheelColl.forwardFriction = CopyFriction(RLWheelColl.forwardFriction, 2f);
        RRWheelColl.forwardFriction = CopyFriction(RRWheelColl.forwardFriction, 2f);
        RLWheelColl.sidewaysFriction = CopyFriction(RLWheelColl.sidewaysFriction, 1.8f);
        RRWheelColl.sidewaysFriction = CopyFriction(RRWheelColl.sidewaysFriction, 1.8f);
    }

    // 辅助函数，用来快速修改 stiffness
    WheelFrictionCurve CopyFriction(WheelFrictionCurve original, float newStiffness)
    {
        WheelFrictionCurve f = original;
        f.stiffness = newStiffness;
        return f;
    }
}