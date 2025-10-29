using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    MoveGage moveGage;

    Rigidbody2D rb;
    [SerializeField] Transform cameraTf;

    const float MOVE_POWER = 500;

    [SerializeField] float moveSpeed = 1;

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        moveGage = GetComponent<MoveGage>();
    }

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 pos = transform.position;
        cameraTf.position = new Vector3(pos.x, pos.y, -10);
    }

    /// <summary>
    /// �O���ɉ���(�ړ�)
    /// </summary>
    public void Move(float force)
    {
        rb.AddForce(force * moveSpeed * MOVE_POWER * transform.up);
    }

    /// <summary>
    /// �w��̌����ɕύX
    /// </summary>
    void ChangeDirection(Vector2 dire)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.up, dire);
    }

    /// <summary>
    /// 㩂�ݒu
    /// </summary>
    void Trap()
    {

    }

    #region #�O������g���֐�

    /// <summary>
    /// �ړ������̔{�����w��
    /// </summary>
    /// <param name="speedRatio"></param>
    public void SetMoveSpeedRatio(float speedRatio = 1)
    {
        moveSpeed = speedRatio;
    }


    #endregion

    #region #���͊֐�

    /// <summary>
    /// �ړ�����
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ�Ƃ������ړ�
        if(context.canceled) moveGage.StopBar();
    }
    
    /// <summary>
    /// 㩐ݒu����
    /// </summary>
    /// <param name="context"></param>
    public void OnTrap(InputAction.CallbackContext context)
    {
        ////�{�^���������ꂽ�Ƃ�
        if (context.started) { }
        //context.canceled
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="context"></param>
    public void OnDirection(InputAction.CallbackContext context)
    {
        //���͒��Ȃ玩�g�̌�������͕����ɕύX
        if (context.performed)
        {
            Vector2 dire = context.ReadValue<Vector2>();
            ChangeDirection(dire);
        }
    }

    #endregion
}
