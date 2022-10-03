using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class CardInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool isCardSelected = false;
    Camera orthoCamera;

    float moveSpeed = .05f; //smaller = faster

    private void Start()
    {
        orthoCamera = this.GetComponentInParent<Camera>();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        isCardSelected = true;
        StartCoroutine(MoveCard(moveSpeed));
    }

    IEnumerator MoveCard(float moveSpeed)
    {
        float currentMoveTime = 0f;

        while (isCardSelected && currentMoveTime < 1)
        {
            float locX = orthoCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x;
            float locY = orthoCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;
            float locZ = transform.parent.position.z;

            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(locX, locY, locZ), currentMoveTime);

            currentMoveTime = Time.deltaTime / moveSpeed;

            yield return null;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isCardSelected = false;
        StopCoroutine(MoveCard(moveSpeed));
    }

}
