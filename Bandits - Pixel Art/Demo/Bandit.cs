using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour {

    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 2.0f;
    [SerializeField] float      m_scaleX = 10.0f;
    [SerializeField] float      m_scaleY = 10.0f;
    [SerializeField] float      m_scaleZ = 10.0f;

    private enum State
    {
        NONE,

        WAIT,
        RUN,
        JUMP,
        ATTACK,
        HIT,

        NUM_STATE,
    };

    private enum Direction
    {
        RIGHT,
        LEFT,

        NUM_DIRECTION,
    }

    private const float         m_attackTime = 1.0f * 25.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_isDead = false;
    private bool                m_isDamaged = false;
    private bool                m_isAttacked = false;
    private float               m_attackCount = m_attackTime;
    private State               m_state;
    private Direction           m_direction;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }
	
	// Update is called once per frame
	void Update () {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            transform.localScale = new Vector3(-m_scaleX, m_scaleY, m_scaleZ);
            m_direction = Direction.LEFT;
        }
        else if (inputX < 0)
        {
            transform.localScale = new Vector3(m_scaleX, m_scaleY, m_scaleZ);
            m_direction = Direction.RIGHT;
        }
        // Move
        if (m_isAttacked == false)
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        //if (Input.GetKeyDown("e")) {
        //    if(!m_isDead)
        //        m_animator.SetTrigger("Death");
        //    else
        //        m_animator.SetTrigger("Recover");

        //    m_isDead = !m_isDead;
        //}

        ////Hurt
        //else if (Input.GetKeyDown("q"))
        //    m_animator.SetTrigger("Hurt");

        //Attack
        if (Input.GetKeyDown("z") && m_isAttacked == false)
        {
            m_animator.SetTrigger("Attack");
            m_isAttacked = true;
            m_state = State.ATTACK;
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            m_state = State.JUMP;
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_animator.SetInteger("AnimState", 2);
            m_state = State.RUN;
        }
        //Idle
        else
        {
            m_animator.SetInteger("AnimState", 0);
            m_state = State.WAIT;
        }

        if (m_isAttacked)
        {
            m_attackCount--;
            if (m_attackCount == 0)
            {
                m_isAttacked = false;
                m_attackCount = m_attackTime;
            }
        }
    }

    public bool IsAttackState()
    {
        if(m_state==State.ATTACK)
        {
            return true;
        }
        return false;
    }
}
