using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float moveInput;
    public float maxSpeed;
    public float jumpSpeed;
    public bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask groundLayer;
    public bool isJumping;
    public float jumpTimeCounter;
    public float jumpTime;
    private bool isFacingRight = true;
    public bool isCoyoteTime;
    private float dirtCreateDelay = 0f;
    private bool isConveyor;
    private bool isMoveToStartPosition = false;
    private bool isDied = false;

    public bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = .2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = .4f;
    private Vector2 wallJumpingSpeed = new Vector2(8f, 13f);
    Rigidbody2D rb;
    public Transform wallCheck;
    public LayerMask wallLayer;
    public Animator squashStretchAnimator;
    [SerializeField] private GameObject testParticleSystem = default;
    [SerializeField] private GameObject dirtParticle = default;

    [SerializeField] private GameManager gameManager = default;

    public AudioClip jumpSound;
    public AudioSource audioSource;
    public SpriteRenderer spr, eye1, eye2;
    private float startX, startY;
    private float curTime;
    private Color newColor;
    private bool canMove;

    private float cameraMoveTime = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startX = rb.transform.position.x;
        startY = rb.transform.position.y;
        newColor = spr.color;

        gameManager = DontDestroyObject.gameManager;
    }

    void Update()
    {
        canMove = DontDestroyObject.Instance.IsPlayMode();
        if (canMove) moveInput = Input.GetAxisRaw("Horizontal");
        else moveInput = 0;
        if (moveInput != 0) gameObject.transform.SetParent(null);
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, groundLayer) || Physics2D.OverlapCircle(feetPos.position, checkRadius, wallLayer);
        if (dirtCreateDelay > 0f)
        {
            dirtCreateDelay -= Time.deltaTime;
        }
        
        if (isGrounded && moveInput != 0 && dirtCreateDelay <= 0 && Mathf.Abs(rb.velocity.x) > 1.1f)
        {
            dirtCreateDelay = 0.2f;
            PlayDirtParticle();
        }
        
        if (canMove && (isGrounded || isCoyoteTime) && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            audioSource.PlayOneShot(jumpSound);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpSpeed;
            squashStretchAnimator.SetTrigger("Jump");
        }

        if (canMove && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space)) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpSpeed;
                jumpTimeCounter -= Time.deltaTime * 2;
            }
        }

        if (canMove && (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space)))
        {
            isJumping = false;
        }

        WallSlide();
        WallJump();
        if (!isWallJumping) Flip();
        if (isMoveToStartPosition)
        {
            curTime += Time.deltaTime;
            gameObject.transform.position =
                Vector3.Lerp(gameObject.transform.position, new Vector3(startX, startY, 0), curTime / cameraMoveTime);
        }
    }


    void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
        if (!isWallJumping)
        {
            float conveyorMultipleValue = 1;
            if (isConveyor)
            {
                if (moveInput > 0) conveyorMultipleValue = 1.2f;
                else if (moveInput < 0) conveyorMultipleValue = .5f;
                else conveyorMultipleValue = 1;
            }
            
            rb.velocity = new Vector2(conveyorMultipleValue * (moveInput * maxSpeed) + (isConveyor ? 2 : 0), rb.velocity.y);
        }
    }

    #region WallJump
    
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, .2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded && moveInput != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && wallJumpingCounter > 0f)
        {
            GameObject particle = Instantiate(dirtParticle, rb.transform.position + new Vector3(isFacingRight ? .5f : -.5f, -.5f, 0), transform.rotation);
            ParticleSystem particlesys = particle.GetComponent<ParticleSystem>();
            var main = particlesys.main;
            main.startSize = 1.0f; 
            particlesys.Play();
            audioSource.PlayOneShot(jumpSound);
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingSpeed.x, wallJumpingSpeed.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
            canMove = false;
        }
    }

    private void StopWallJumping()
    {
        canMove = true;
        isWallJumping = false;
    }
   
    #endregion
    
    private void Flip()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Arrow"))
        {
            if (!isDied) HandlingDie();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            squashStretchAnimator.SetTrigger("Landing");
        }
        
        if (collision.collider.CompareTag("Spike"))
        {
            if (!isDied) HandlingDie();
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.tag == "ConveyorBelt")
        {
            isConveyor = true;
        }
        else
        {
            isConveyor = false;
        }
    }
    
    #region Die

    public void Die() {
        HandlingDie();
    }

    void HandlingDie()
    {
        isDied = true;
        GameObject particle = Instantiate(testParticleSystem, rb.transform.position + new Vector3(0f, 0f, 0f), transform.rotation);
        ParticleSystem particlesys = particle.GetComponent<ParticleSystem>();
        particlesys.Play();
        //GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().VibrateForTime(.2f);
        
        //gameObject.SetActive(false);
        newColor.a = 0f;
        spr.color = newColor;
        eye1.color = new Color(1f, 1f, 1f, 0f);
        eye2.color = new Color(1f, 1f, 1f, 0f);
        StartCoroutine("MoveToStartPosition");
        //Destroy(this.gameObject);
    }
    IEnumerator MoveToStartPosition()
    {
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(0.5f);
        curTime = 0;
        isMoveToStartPosition = true;
        yield return new WaitForSeconds(1.5f);
        newColor.a = 1f;
        spr.color = newColor;
        eye1.color = new Color(1f, 1f, 1f, 1f);
        eye2.color = new Color(1f, 1f, 1f, 1f);
        isDied = false;
        isMoveToStartPosition = false;

        /*
         * Handling Editing Mode
         */

        gameManager.EditMode();
        //yield return new
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("OilPress"))
        {
            if (!isDied) HandlingDie();
        }

        if (collision.CompareTag("Spring"))
        {
            rb.velocity = Vector2.up * jumpSpeed * 3;
            audioSource.PlayOneShot(jumpSound);
        }
    }

    #endregion
    
    
    #region Particle

    void PlayDirtParticle()
    {
        GameObject particle = Instantiate(dirtParticle, rb.transform.position + new Vector3(isFacingRight ? -.5f : .5f, -.5f, 0), transform.rotation);
        ParticleSystem particlesys = particle.GetComponent<ParticleSystem>();
        particlesys.Play();
    }
    
    
    #endregion
}