using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    MeoshiGage meoshiGage;

    const float MOVE_POWER = 500;

    /// <summary>
    /// ������
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        meoshiGage = GetComponent<MeoshiGage>();
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
    public void Move(float force)
    {
        rb.AddForce(force * MOVE_POWER * transform.up);
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
        //if (context.started) Move();
        if(context.canceled) meoshiGage.StopBar();
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
