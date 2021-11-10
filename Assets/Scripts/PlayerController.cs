using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{

    [Header("littleFox Data")]
    public float speed;
    public float upDownSpeed;
    public float jumpForce;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D collTop, collBottom, collLadder;//������
    private CapsuleCollider2D collPlayer;

    private Collider2D LadderObj;//����

    private bool isHurt;//�Ƿ�����


  //  private static PlayerController instance;
  //ʹ���Զ�����
    public static PlayerController Instance { get; set; }
    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        collPlayer = GetComponent<CapsuleCollider2D>();
        collTop = transform.GetChild(0).GetComponent<Collider2D>();
        collBottom = transform.GetChild(1).GetComponent<Collider2D>();
        collLadder = transform.GetChild(2).GetComponent<Collider2D>();
        LadderObj = GameObject.FindGameObjectWithTag("Ladder").GetComponent<Collider2D>();

        cherry = PlayerPrefs.GetInt("Cherry");
        gem = PlayerPrefs.GetInt("Gem");
    }

    private void Update()
    {
        Jump();
        Crouch();
        Ladder();

        if (Input.GetKeyDown(KeyCode.L) && nextScene)
        {
            if (SceneManager.GetActiveScene().buildIndex < 2)
            {
                PlayerPrefs.SetInt("Cherry", PlayerController.Instance.cherry);
                PlayerPrefs.SetInt("Gem", PlayerController.Instance.gem);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            //�ص���һ��
            else
            {
                PlayerPrefs.SetInt("Cherry", PlayerController.Instance.cherry);
                PlayerPrefs.SetInt("Gem", PlayerController.Instance.gem);

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }



        }

        //��Ϸʤ��
        if (cherry + gem >30)
        {
            Time.timeScale = 0;
            UIShowController.Instance.gameObject.transform.GetChild(6).gameObject.SetActive(true);
        }


    }

    private void FixedUpdate()
    {
        SwitchAnimation();//������������Ҫ���ӳ٣���ֹ������jumping�ı�false��Ҫ��Ȼ����

        if (!isHurt)
            MoveHorizontal();

        UIShowController.Instance.cherrynum.text = string.Format("{0:D2}", cherry);//$"{cherry:D2}";����һ��д��
        UIShowController.Instance.gem.text = $"{gem:D2}";
    }


    /// <summary>
    /// �����ƶ�
    /// </summary>

    void MoveHorizontal()
    {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");

        anim.SetFloat("running", Mathf.Abs(horizontal));//����float��������0.5f��runing,С�ھͷ���idle


        //ʩ�ӵ������������
        rb.velocity = new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);

        if (horizontal != 0)
        {
            transform.localScale = new Vector3(horizontal, 1, 1);//���ҳ���

        }


        //���������¥�ݵ�ʱ��ʧȥ�����ƣ�������ԭ���� rb.velocity.y,�������︲������Ӱ�죬Ҫ��Ȼû����������С�ӻ�����



    }


    /// <summary>
    /// ſ��
    /// </summary>
    void Crouch()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            collPlayer.size = new Vector2(1f, 0.5f);
            collPlayer.offset = new Vector2(0, -0.48f);
            anim.SetBool("crouch", true);
        }





        else if (!Input.GetKey(KeyCode.Space) && !collTop.IsTouchingLayers(1 << 6))
        {
            collPlayer.size = new Vector2(0.6f, 1.35f);
            collPlayer.offset = new Vector2(0, -0.3f);

            anim.SetBool("crouch", false);
        }


    }


    /// <summary>
    /// ���Ӳ���
    /// </summary>
    void Ladder()
    {
        //���������ӵ�ʱ��״̬
        if (collLadder.IsTouchingLayers(1 << 7))
        {

            LadderObj.isTrigger = true;

            int vertical = (int)Input.GetAxisRaw("Vertical");


            //���ͬʱ����������
            if (collLadder.IsTouchingLayers(1 << 6))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            //ֻ�����Ķ���ʱ��ſ��������ƶ����������y
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("climb"))
            {

                rb.velocity = new Vector2(rb.velocity.x, vertical * upDownSpeed);

                //ʵ����������������������
                if (Mathf.Abs(vertical) == 1f)
                {
                    anim.speed = 1;//��������
                }
                else
                    anim.speed = 0;//��ͣ����
            }




            if (!collLadder.IsTouchingLayers(1 << 6) && !anim.GetBool("climb"))
            {
                rb.gravityScale = 3;
            }
            else rb.gravityScale = 0;


        }


        //û�д��������ӵ�ʱ��״̬
        else if (!collLadder.IsTouchingLayers(1 << 7))
        {
            rb.gravityScale = 3;
            LadderObj.isTrigger = false;
            anim.speed = 1;//��������
        }



    }

    bool isJump;
    int jumpCount;//Ĭ����һ����

    /// <summary>
    /// ����ʩ����
    /// </summary>
    void Jump()
    {

        if (collBottom.IsTouchingLayers(1 << 6 | 1 << 7))
        {

            if (cherry >= 5 && gem >= 5)
            {
                jumpCount = 2;
                UIShowController.Instance.isTwoJump.gameObject.SetActive(true);
            }
            else jumpCount = 1;


        }

        isJump = collBottom.IsTouchingLayers(1 << 6 | 1 << 7) || (jumpCount > 1);


        GameObject clickUIEvent = EventSystem.current.currentSelectedGameObject;
        if (Input.GetMouseButtonDown(0) && !clickUIEvent)
        {


            if (isJump && !isHurt)
            {
                //����¥�ݻ��ߵ����ʱ��ͱ������
                if (!anim.GetBool("climb") && !anim.GetBool("crouch"))
                {

                    //ʩ�ӵ������������
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    SoundManager.Instance.JumpSound();
                }

            }

            if (!collBottom.IsTouchingLayers(1 << 6 | 1 << 7))
                jumpCount--;


            anim.SetBool("jumping", true);


        }




    }



    /// <summary>
    /// �����л�
    /// </summary>
    void SwitchAnimation()
    {



        //����״̬�������л������䶯�������ܲ�״̬ʱ�������
        if (rb.velocity.y > 0) { anim.SetBool("falling", false); anim.SetBool("jumping", true); }
        if (rb.velocity.y < 0) { anim.SetBool("falling", true); anim.SetBool("jumping", false); }


        if (isHurt)
        {
            anim.SetBool("hurt", true);
            if (Mathf.Abs(rb.velocity.x) <= 1f)
            {
                anim.SetBool("hurt", false);
                isHurt = false;
                rb.sharedMaterial = Resources.Load<PhysicsMaterial2D>("PlayerMaterial");
            }
            else
            {

                rb.sharedMaterial = null;

            }

        }




        //����¥��

        //ֻ����Ծ�����л�����¥������
        if (collLadder.IsTouchingLayers(1 << 7))
        {
            if (anim.GetBool("jumping"))
            {
                anim.SetBool("climb", true);

                anim.SetBool("jumping", false);
            }



        }
        else
        {
            anim.SetBool("climb", false);//û������¥�ݵ�ʱ��          
        }




        if (collBottom.IsTouchingLayers(1 << 6 | 1 << 7))//LayerMask.GetMask("Ground")�൱��1<<6
        {
            anim.SetBool("falling", false); anim.SetBool("jumping", false);
        }





    }




    public int cherry = 0;//ӣ��
    public int gem = 0;//��ʯ
    bool nextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {



        if (collision.CompareTag("Enemy"))
        {

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger("dead");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                SoundManager.Instance.DownAttackSound();

            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-10, 5);
                isHurt = true;
                SoundManager.Instance.HitSound();
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {

                rb.velocity = new Vector2(10, 5);
                isHurt = true;
                SoundManager.Instance.HitSound();
            }


        }




        if (collision.tag == "House")
        {
            UIShowController.Instance.dialog.SetActive(true);
            nextScene = true;
        }

        if (collision.tag == "DeadLine")
        {


            Invoke("Restart", 2f);


        }

    
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "House")
        {
            UIShowController.Instance.dialog.SetActive(false);
            nextScene = false;
        }
       



    }

    //���ص�ǰ
    void Restart()
    {
        SceneManager.LoadScene(0);
    }





}
