using UnityEngine;
using System.Collections;
using Perception;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    // The character's running speed
    [SerializeField]
    float m_RunSpeed = 5.0f;

    // The gravity strength
    [SerializeField]
    float m_Gravity = 60.0f;

    // The maximum speed the character can fall
    [SerializeField]
    float m_MaxFallSpeed = 20.0f;

    // The character's jump height
    [SerializeField]
    float m_JumpHeight = 4.0f;

    // --------------------------------------------------------------

    // The charactercontroller of the player
    CharacterController m_CharacterController;

    // The current movement direction in x & z.
    Vector3 m_MovementDirection = Vector3.zero;

    // The current movement speed
    float m_MovementSpeed = 0.0f;

    // The current vertical / falling speed
    float m_VerticalSpeed = 0.0f;

    // The current movement offset
    Vector3 m_CurrentMovementOffset = Vector3.zero;

    // The starting position of the player
    Vector3 m_SpawningPosition = Vector3.zero;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // The time it takes to respawn
    const float MAX_RESPAWN_TIME = 1.0f;
    float m_RespawnTime = MAX_RESPAWN_TIME;

    // The force added to the player (used for knockbacks)
    Vector3 m_Force = Vector3.zero;

    // The characters Animator
    public Animator animator;

    // The characters rigidbody
    public Rigidbody rigidbody;

    // character Roll timer
    public float rollReset;
    public float rollDelay;
    public bool rollAvailable = true;

    public bool isMoving;
    public bool rolling;
    public bool sneaking;

     float punchDelay;
     bool punching;
     bool punchAvailable = true;
     float audioDelayReset;

    public BoxCollider punchCol;

    [HideInInspector]
    public float audioDelay;

    // Foot step audio clip
    [SerializeField]
    AudioClip m_footStep;
    float footstepVolume;
    float footstepNoiseDistance;

    // roll audio clip
    [SerializeField]
    public AudioClip m_roll;

    // The AudioSource to play Sounds for this object
    AudioSource m_AudioSource;

    public Health health;

    GameController gameController;
    PlayerProgressController progController;
    // --------------------------------------------------------------

    PerceptionManager pm;
    
    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        pm = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<PerceptionManager>();
        progController = FindObjectOfType<PlayerProgressController>();

        rollReset = progController.rollCooldown;
        rollDelay = rollReset;
        footstepVolume = progController.footstepVolume;
        footstepNoiseDistance = progController.footstepNoise;
    }

    // Use this for initialization
    void Start()
    {
        m_SpawningPosition = transform.position;

    }

    void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_Gravity);
    }

    void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_Gravity * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeed);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeed);
    }

    void UpdateMovementState()
    {
        // Get Player's movement input and determine direction and set run speed
        float horizontalInput = Input.GetAxisRaw("Horizontal_P1");
        float verticalInput = Input.GetAxisRaw("Vertical_P1");

        m_MovementDirection = new Vector3(horizontalInput, 0, verticalInput);
        m_MovementSpeed = m_RunSpeed;
    }

    void UpdateJumpState()
    {
        // Character can jump when standing on the ground
        if (Input.GetButtonDown("Jump_P1") && m_CharacterController.isGrounded)
        {
            Jump();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            return;
        }
        if (!gameController.gameStarted)
        {
            return;
        }
        if (health.isDead != true)
        {
            gameController.failedText.SetActive(false);
            gameController.retryButton.SetActive(false);
            // If the player is dead update the respawn timer and exit update loop
            if (!m_IsAlive)
            {
                UpdateRespawnTime();
                return;
            }

            // Update movement input
            UpdateMovementState();

            // Update jumping input and apply gravity
            UpdateJumpState();
            ApplyGravity();

            // Calculate actual motion
            m_CurrentMovementOffset = (m_MovementDirection * m_MovementSpeed + m_Force + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

            m_Force *= 0.95f;

            // Move character
            //m_CharacterController.Move(m_CurrentMovementOffset); 

            CheckMovementDirection();
            if (isMoving)
            {
                if (rolling != true && punching != true)
                {
                    if (sneaking)
                    {
                        PlaySneakFootsteps();
                    }
                    else
                    {
                        PlayFootSteps();
                    }
                }
                if (Input.GetAxis("Roll_P1") > 0 && rollAvailable == true) Roll();
                else
                {
                    audioDelayReset = 0.4f;
                    //animator.SetBool("Sneak", false);
                }
            }
            else
            {
                audioDelay = 0f;
            }
            if (!m_AudioSource.isPlaying)
            {
                rolling = false;
                punching = false;
            }
            // Checks if there is 0 input on vertical axis
            if (Input.GetAxisRaw("Vertical_P1") == 0)
            {
                animator.SetBool("MoveForward", false);
                animator.SetBool("MoveBackward", false);
            }
            // Checks if forward input is being used
            else if (Input.GetAxisRaw("Vertical_P1") > 0)
            {
                MoveForward();
            }
            // Checks if Backward input is being used
            else if (Input.GetAxisRaw("Vertical_P1") < 0)
            {
                MoveBackward();
            }
            //Checks if there is 0 input on horizontal axis
            if (Input.GetAxisRaw("Horizontal_P1") == 0)
            {
                animator.SetBool("MoveRight", false);
                animator.SetBool("MoveLeft", false);
            }
            // Checks if right input is being used
            else if (Input.GetAxisRaw("Horizontal_P1") > 0)
            {
                MoveRight();
            }
            // Checks if left input is being used
            else if (Input.GetAxisRaw("Horizontal_P1") < 0)
            {
                MoveLeft();
            }
            // Checks if the player is pressing shift to sneak 
            if (Input.GetAxisRaw("Sneak_P1") > 0)
            {
                if (!rolling && !punching)
                {
                    Sneak();
                }
            }
            else
            {
                sneaking = false;
                animator.SetBool("Sneak", false);
            }

            // If the roll isnt available, start reset timer
            if (rollAvailable == false)
            {
                rollDelay -= Time.deltaTime;
                if (rollDelay <= 0)
                {
                    rollDelay = rollReset;
                    rollAvailable = true;
                }
            }
            if (punchAvailable == false)
            {
                punchDelay -= Time.deltaTime;
                if (punchDelay <= 0)
                {
                    punchDelay = 1;
                    punchAvailable = true;
                }
            }

            if (Input.GetAxisRaw("Punch_P1") > 0 && punchAvailable == true && !rolling)
            {
                Punch();
            }
            // Rotate the character towards the mouse cursor
            RotateCharacterTowardsMouseCursor();
        }
        else if(health.isDead)
        {
            gameController.failedText.SetActive(true);
            gameController.retryButton.SetActive(true);
        }
    }
    

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }

    void RotateCharacterTowardsMouseCursor()
    {
        Vector3 mousePosInScreenSpace = Input.mousePosition;
        Vector3 playerPosInScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 directionInScreenSpace = mousePosInScreenSpace - playerPosInScreenSpace;

        float angle = Mathf.Atan2(directionInScreenSpace.y, directionInScreenSpace.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 90.0f, Vector3.up);
    }

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
    }

    void UpdateRespawnTime()
    {
        m_RespawnTime -= Time.deltaTime;
        if (m_RespawnTime < 0.0f)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        m_IsAlive = true;
        transform.position = m_SpawningPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    public void AddForce(Vector3 force)
    {
        m_Force += force;
    }
    public void CheckMovementDirection()
    {
        // play footstep if moving
        if (Input.GetAxisRaw("Vertical_P1") != 0 || Input.GetAxisRaw("Horizontal_P1") != 0)
        {
            isMoving = true;           
        }
        else
        {
            isMoving = false;
        }       
    }
    public void MoveForward()
    {
        animator.SetBool("MoveForward", true);
    }
    public void MoveBackward()
    {
        animator.SetBool("MoveBackward", true);
    }
    public void MoveLeft()
    {
        animator.SetBool("MoveLeft", true);
    }
    public void MoveRight()
    {
        animator.SetBool("MoveRight", true);
    }
    public void Sneak()
    {
        sneaking = true;
        animator.SetBool("Sneak",true);
        audioDelayReset = 0.6f;
       // if (m_AudioSource.volume != 0.025f) m_AudioSource.volume = 0.025f;
    }
    public void Roll()
    {
        animator.SetTrigger("Roll");
        rolling = true;
        rollAvailable = false;
    }
    public void PlayFootSteps()
    {
        audioDelay -= Time.deltaTime;
        if (audioDelay <= 0)
        {
            m_AudioSource.volume = footstepVolume;
            m_AudioSource.PlayOneShot(m_footStep);
            pm.AcceptStimulus(new Stimulus(Stimulus.StimulusTypes.AudioMovement, this.gameObject, this.transform.position, transform.forward, footstepNoiseDistance, null));
            audioDelay = audioDelayReset;
        }
    }
    public void PlaySneakFootsteps()
    {
        audioDelay -= Time.deltaTime;
        if (audioDelay <= 0)
        {
            m_AudioSource.volume = 0.025f;
            m_AudioSource.PlayOneShot(m_footStep);
            audioDelay = audioDelayReset;
        }
    }
    public void Punch()
    {
        animator.SetTrigger("Punch");
        punching = true;
        punchAvailable = false;
    }
}
