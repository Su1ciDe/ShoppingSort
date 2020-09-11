using UnityEngine;
using UnityEngine.EventSystems;

public class LabelDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Bottle bottle = gameObject.GetComponent<Bottle>();
        GameObject labelGo = eventData.pointerDrag;

        if (bottle.colorMat == labelGo.GetComponent<LabelDrag>().matchingColorMat)
        {
            if (bottle.transform.GetChild(1).GetComponent<MeshRenderer>().material.color != bottle.glassColorMat.color)
            {
                bottle.ChangeGlassColor();

                GameManager.instance.AddToScore(5);

                GameManager.instance.Part1TOPart2();
            }
        }
        else
        {
            GameManager.instance.AddToScore(-3);
            //animation X
        }
    }
}