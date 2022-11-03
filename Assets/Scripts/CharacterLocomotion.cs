using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public PlayerMovement player;
    Animator animator;
    Vector3 input;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Velocity", player.playerSpeed);
        animator.SetFloat("InputX", player.moveDirection.x);
        animator.SetFloat("InputY", player.moveDirection.z);

        if (Input.GetKeyDown(player.jumpKey) && player.grounded) animator.SetTrigger("Jump");

        if (player.state.ToString() == "aiming") animator.SetBool("Aiming", true);
        else animator.SetBool("Aiming", false);

        if (player.state.ToString() == "air") animator.SetBool("OnAir", true);
        else animator.SetBool("OnAir", false);
    }
}
