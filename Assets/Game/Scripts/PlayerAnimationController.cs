
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private CharacterMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<CharacterMovement>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado!", this);
            enabled = false;
            return;
        }

        if (playerMovement == null)
        {
            Debug.LogError("CharacterMovement não encontrado!", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        animator.SetBool("Walk", playerMovement.Walk);
    }
}
