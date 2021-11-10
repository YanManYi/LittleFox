using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;

    [Header("Frog")]
    public float jumpForce;
    Rigidbody2D rb;

    Vector3 leftPoint, rightPoint;
    bool faceLeft = true;
    bool isGround;
    Collider2D coll;
    Animator anim;

    private void Start()
    {
        leftPoint = transform.GetChild(0).position;
        rightPoint = transform.GetChild(1).position;

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        isGround = coll.IsTouchingLayers(1 << 6);//layer ������6
        Movement();
    }


    /// <summary>
    /// �����ܱȽ�����,��������
    /// </summary>
    void Movement()
    {

        //�����ʱ��
        if (faceLeft)
        {
            //���������
            if (this.gameObject.layer == 9)
            {

                AnimatorStateInfo stateinfo = anim.GetCurrentAnimatorStateInfo(0);
                //�ڵ��棬������Event�¼����������˹�������
                if (isGround && stateinfo.IsName("Idle") && stateinfo.normalizedTime > 1.0f)
                {
                    anim.SetBool("jump", true);
                    rb.velocity = new Vector2(-speed, jumpForce);
                }
                else if (rb.velocity.y <= 0)
                {
                    anim.SetBool("down", true);
                    anim.SetBool("jump", false);
                }


                if (coll.IsTouchingLayers(1 << 6))
                {
                    anim.SetBool("down", false);
                }
                //��ը���������ƶ�
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead")) rb.velocity = Vector2.zero;


            }

            else
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead")) rb.velocity = Vector2.zero;
            }



            //�ı䳯��
            //���������
            if (this.gameObject.layer == 9)
            {
                if (transform.position.x <= leftPoint.x && isGround)//���˸��ȵ��������
                {
                    faceLeft = false;
                    transform.localScale = new Vector3(-1, 1, 1);

                }
            }
            else
            {

                if (transform.position.x <= leftPoint.x)
                {
                    faceLeft = false;
                    transform.localScale = new Vector3(-1, 1, 1);

                }
            }


        }

        else//���ұ���ʱ
        {

            //���������
            if (this.gameObject.layer == 9)
            {
                AnimatorStateInfo stateinfo = anim.GetCurrentAnimatorStateInfo(0);
                //�ڵ��棬������Event�¼����������˹�������
                if (isGround && stateinfo.IsName("Idle") && stateinfo.normalizedTime > 1.0f)
                {

                    anim.SetBool("jump", true);
                    rb.velocity = new Vector2(speed, jumpForce);
                }
                else if (rb.velocity.y <= 0)
                {
                    anim.SetBool("down", true);
                    anim.SetBool("jump", false);
                }

                if (coll.IsTouchingLayers(1 << 6))
                {
                    anim.SetBool("down", false);
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead")) rb.velocity = Vector2.zero;

            }
            else
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead")) rb.velocity = Vector2.zero;
            }




            //�ı䳯��
            //���������
            if (this.gameObject.layer == 9)
            {
                if (transform.position.x >= rightPoint.x && isGround)//���˸��ȵ��������
                {
                    faceLeft = true;
                    transform.localScale = new Vector3(1, 1, 1);

                }
            }
            else
            {

                if (transform.position.x >= rightPoint.x)
                {
                    faceLeft = true;
                    transform.localScale = new Vector3(1, 1, 1);


                }
            }
        }



    }


    //Event�¼���ը������������
    public void Dead()
    {
        Destroy(gameObject);
    }

}
