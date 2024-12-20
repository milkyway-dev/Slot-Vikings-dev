using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // For DOTween

public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Transform buttonTransform; // Assign the Button's transform in the Inspector.
    private float OrignalScale;
    private void OnValidate() {
        if (buttonTransform == null)
        {
            buttonTransform = transform; // Default to this GameObject's transform.
            OrignalScale=transform.localScale.x;
        }
    }

    private void Start() {
        if (buttonTransform == null)
        {
            buttonTransform = transform; // Default to this GameObject's transform.
            OrignalScale=transform.localScale.x;
        }   
    }

    // Called when the button is pressed.
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
        PressedAnimation(buttonTransform);
    }

    // Called when the button is released.
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
        OnClickedAnimation(buttonTransform);
    }

    void PressedAnimation(Transform transform)
    {
        transform.DOScale(0.8f, 0.2f); // Scale down on press.
    }

    void OnClickedAnimation(Transform transform)
    {
        transform.DOScale(OrignalScale, 0.2f); // Scale back to normal size after release.
    }
}
