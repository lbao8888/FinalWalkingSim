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

    private float horizontal;
    private float vertical;

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

        // 锁定玩家
        player.SetControlsLocked(true);

        // 关闭玩家CharacterController
        player.GetComponent<CharacterController>().enabled = false;

        // 隐藏玩家模型
        player.gameObject.SetActive(false);

        // 摄像机移动到车
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

        // 恢复玩家
        player.gameObject.SetActive(true);

        // 传送玩家
        player.transform.position = exitPoint.position;

        // 恢复玩家控制
        player.SetControlsLocked(false);

        // 开启CharacterController
        player.GetComponent<CharacterController>().enabled = true;

        // 摄像机回玩家
        Camera.main.transform.SetParent(player.cameraTransform);

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
}