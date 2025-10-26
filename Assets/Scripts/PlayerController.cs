using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    Vector2 direction = new(0,0);
    const float MOVE_POWER = 500;

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
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
        
    }

    /// <summary>
    /// �O���ɉ���(�ړ�)
    /// </summary>
    void Move()
    {
        rb.AddForce(transform.up * MOVE_POWER);
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

    #region #���͊֐�

    /// <summary>
    /// �ړ�����
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //�{�^���������ꂽ�Ƃ������ړ�
        if (context.started) Move();
    }
    
    /// <summary>
    /// 㩐ݒu����
    /// </summary>
    /// <param name="context"></param>
    public void OnTrap(InputAction.CallbackContext context)
    {
        ////�{�^���������ꂽ�Ƃ�
        //if (context.started) { }
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
