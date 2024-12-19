using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // For DOTween

public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Transform buttonTransform; // Assign the Button's transform in the Inspector.

    private void OnValidate() {
        if (buttonTransform == null)
        {
            buttonTransform = transform; // Default to this GameObject's transform.
        }
    }

    // Called when the button is pressed.
    public void OnPointerDown(PointerEventData eventData)
    {
        PressedAnimation(buttonTransform);
    }

    // Called when the button is released.
    public void OnPointerUp(PointerEventData eventData)
    {
        OnClickedAnimation(buttonTransform);
    }

    // Optionally handle the click event.
    public void OnPointerClick(PointerEventData eventData)
    {
        // Add any additional logic if needed.
        Debug.Log("Button clicked!");
    }

    void PressedAnimation(Transform transform)
    {
        transform.DOScale(0.8f, 0.2f); // Scale down on press.
    }

    void OnClickedAnimation(Transform transform)
    {
        transform.DOScale(1f, 0.2f); // Scale back to normal size after release.
    }
}
