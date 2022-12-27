using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;
    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    private void Update()
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(characterController.velocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(characterController.velocity).x);
    }

}
