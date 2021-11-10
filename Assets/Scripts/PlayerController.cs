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
    private Collider2D collTop, collBottom, collLadder;//触发器
    private CapsuleCollider2D collPlayer;

    private Collider2D LadderObj;//梯子

    private bool isHurt;//是否受伤


  //  private static PlayerController instance;
  //使用自动属性
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
            //回到上一关
            else
            {
                PlayerPrefs.SetInt("Cherry", PlayerController.Instance.cherry);
                PlayerPrefs.SetInt("Gem", PlayerController.Instance.gem);

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }



        }

        //游戏胜利
        if (cherry + gem >30)
        {
            Time.timeScale = 0;
            UIShowController.Instance.gameObject.transform.GetChild(6).gameObject.SetActive(true);
        }


    }

    private void FixedUpdate()
    {
        SwitchAnimation();//动画跳起来需要有延迟，防止被覆盖jumping的变false，要不然不跳

        if (!isHurt)
            MoveHorizontal();

        UIShowController.Instance.cherrynum.text = string.Format("{0:D2}", cherry);//$"{cherry:D2}";另外一种写法
        UIShowController.Instance.gem.text = $"{gem:D2}";
    }


    /// <summary>
    /// 左右移动
    /// </summary>

    void MoveHorizontal()
    {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");

        anim.SetFloat("running", Mathf.Abs(horizontal));//动画float条件大于0.5f就runing,小于就返回idle


        //施加的向量方向的力
        rb.velocity = new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);

        if (horizontal != 0)
        {
            transform.localScale = new Vector3(horizontal, 1, 1);//左右朝向

        }


        //这个力在走楼梯的时候失去了限制，保持了原来的 rb.velocity.y,所以这里覆盖限制影响，要不然没了重力，这小子还下落



    }


    /// <summary>
    /// 趴着
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
    /// 梯子部分
    /// </summary>
    void Ladder()
    {
        //触碰到梯子的时候状态
        if (collLadder.IsTouchingLayers(1 << 7))
        {

            LadderObj.isTrigger = true;

            int vertical = (int)Input.GetAxisRaw("Vertical");


            //与此同时还碰到地面
            if (collLadder.IsTouchingLayers(1 << 6))
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            //只有爬的动作时候才可以上下移动覆盖上面的y
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("climb"))
            {

                rb.velocity = new Vector2(rb.velocity.x, vertical * upDownSpeed);

                //实现上下爬根据输入来控制
                if (Mathf.Abs(vertical) == 1f)
                {
                    anim.speed = 1;//继续动画
                }
                else
                    anim.speed = 0;//暂停动画
            }




            if (!collLadder.IsTouchingLayers(1 << 6) && !anim.GetBool("climb"))
            {
                rb.gravityScale = 3;
            }
            else rb.gravityScale = 0;


        }


        //没有触碰到梯子的时候状态
        else if (!collLadder.IsTouchingLayers(1 << 7))
        {
            rb.gravityScale = 3;
            LadderObj.isTrigger = false;
            anim.speed = 1;//继续动画
        }



    }

    bool isJump;
    int jumpCount;//默认是一段跳

    /// <summary>
    /// 向上施加力
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
                //在爬楼梯或者地面的时候就别给力了
                if (!anim.GetBool("climb") && !anim.GetBool("crouch"))
                {

                    //施加的向量方向的力
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
    /// 动画切换
    /// </summary>
    void SwitchAnimation()
    {



        //下落状态都可以切换至下落动画，如跑步状态时候的下落
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




        //关于楼梯

        //只有跳跃可以切换到爬楼梯条件
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
            anim.SetBool("climb", false);//没有碰到楼梯的时候          
        }




        if (collBottom.IsTouchingLayers(1 << 6 | 1 << 7))//LayerMask.GetMask("Ground")相当于1<<6
        {
            anim.SetBool("falling", false); anim.SetBool("jumping", false);
        }





    }




    public int cherry = 0;//樱桃
    public int gem = 0;//宝石
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

    //加载当前
    void Restart()
    {
        SceneManager.LoadScene(0);
    }





}
