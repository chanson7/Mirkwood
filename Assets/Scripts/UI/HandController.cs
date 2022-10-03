using UnityEngine;
using UnityEngine.Splines;

public class HandController : MonoBehaviour
{
    [SerializeField] PlayerHand localPlayerHand;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Camera orthoCamera;
    [SerializeField] Spline spline;
    float frustumHeight;
    float frustumWidth;

    void Start()
    {
        frustumHeight = orthoCamera.orthographicSize * 2f;
        frustumWidth = orthoCamera.aspect * frustumHeight;

        SetScreenPosition();
        InstantiateStartingHand();
    }

    void SetScreenPosition()
    {
        //set to the bottom of the frustum
        transform.localPosition = new Vector3(0,                                                                    //horizontal placement on screen
                                              0 - ((frustumHeight / 2)),                                            //vertical placement on screen
                                              (orthoCamera.farClipPlane + orthoCamera.nearClipPlane) / 2);          //ensures the object is always in view of the camera
    }

    void InstantiateStartingHand()
    {
        int numCards = localPlayerHand.cardsInHand.Count;

        for (int i = 0; i < numCards; i++)
        {
            var newCard = Instantiate(cardPrefab, this.transform, false);
            newCard.GetComponent<Canvas>().sortingOrder = i; //so that cards will render in front of each other

            newCard.transform.localPosition = spline.EvaluatePosition<Spline>((float)(i + 1) / (numCards + 1));
            newCard.transform.Rotate(Vector3.up, 10f * i / numCards, Space.Self);
        }
    }

}
