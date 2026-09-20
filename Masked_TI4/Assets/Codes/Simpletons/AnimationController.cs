using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController instance;
    public Animator anim;


    private void Start()
    {
        instance = this;
    }

    public void ControlarWalk(int num)
    {
        anim.SetInteger("WalkingSpeed", num);
    }

    public void ControlarJump(float num)
    {
        anim.SetFloat("Jump", num);
    }
}
