using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LabelDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Material matchingColorMat;

    private CanvasGroup canvasGroup;
    private LayoutElement layoutElement;

    private void Start()
    {
        layoutElement = GetComponent<LayoutElement>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // canvas'ın "scalefactor"ünü unutma
        transform.position = Input.mousePosition;

        //Touch touch = Input.GetTouch(0);
        //transform.position = touch.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        layoutElement.ignoreLayout = false;
    }
}