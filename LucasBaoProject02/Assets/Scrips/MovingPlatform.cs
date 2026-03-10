using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 5f;

    private Vector3 startPos;
    private int direction = 1;

    public Vector3 platformVelocity;

    private Vector3 lastPosition;

    void Start()
    {
        startPos = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        // 左右移动
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (transform.position.x > startPos.x + moveDistance)
            direction = -1;

        if (transform.position.x < startPos.x - moveDistance)
            direction = 1;

        // 计算平台速度
        platformVelocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;
    }
}