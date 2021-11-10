using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;
    public static SoundManager Instance { get => instance; }

    public AudioSource audioSource;
    public AudioClip jump, hit, attack, cherry;



    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        instance = this;
    }



    public void JumpSound() { audioSource.clip = jump; audioSource.Play(); }
    public void HitSound() { audioSource.clip = hit; audioSource.Play(); }
    public void DownAttackSound() { audioSource.clip = attack; audioSource.Play(); }
    public void CherrySound() { audioSource.clip = cherry; audioSource.Play(); }





}
