using UnityEngine;
using UnityEngine.Events;

public class BottleDragDrop : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 startingPos;

    public Touch touch;

    private Collider lastHolderCollider = null;

    [HideInInspector] public UnityEvent onBeginDrag;
    [HideInInspector] public UnityEvent onEndDrag;

    private bool isDragging;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startingPos = transform.position;
    }

    private void Update()
    {
        if (LevelManager.instance.currentPart == LevelManager.PARTS.PART3)
        {
            //Drag_Touch();
            Drag_Mouse();
        }
    }

    private void Drag_Touch()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
        }

        if (touch.phase == TouchPhase.Began)
        {
            if (!isDragging && LevelManager.instance.currentPart == LevelManager.PARTS.PART3)
                BeginDrag();
        }
        if (touch.phase == TouchPhase.Ended)
        {
            EndDrag();
        }

        if (isDragging)
        {
            Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, .9f));
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (plane.Raycast(ray, out float distance))
                transform.position = ray.GetPoint(distance);
        }
    }

    private void Drag_Mouse()
    {
        if (isDragging)
        {
            Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, .9f));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
                transform.position = ray.GetPoint(distance);
        }
    }

    private void OnMouseDown()
    {
        if (!isDragging && LevelManager.instance.currentPart == LevelManager.PARTS.PART3)
            BeginDrag();
    }

    private void OnMouseUp()
    {
        EndDrag();
    }

    public void BeginDrag()
    {
        onBeginDrag.Invoke();

        isDragging = true;
        rb.isKinematic = true;
    }

    public void EndDrag()
    {
        onEndDrag.Invoke();

        isDragging = false;
        rb.isKinematic = false;

        PutBottleOnShelf();
    }

    private void PutBottleOnShelf()
    {
        if (LevelManager.instance.currentPart == LevelManager.PARTS.PART3)
        {
            if (lastHolderCollider != null)
            {
                for (int i = 0; i < lastHolderCollider.transform.childCount; i++)
                {
                    if (lastHolderCollider.transform.GetChild(i).GetComponent<BottleHolder>().isEmpty)
                    {
                        transform.SetParent(lastHolderCollider.transform.GetChild(i));
                        lastHolderCollider.transform.GetChild(i).GetComponent<BottleHolder>().isEmpty = false;
                        transform.position = lastHolderCollider.transform.GetChild(i).position + new Vector3(0, GetComponent<Bottle>().offsetY, 0);

                        // doğru renk mi kontrol et
                        if (lastHolderCollider.GetComponent<ShelfHolders>().colorMat.color == GetComponent<Bottle>().colorMat.color)
                            GameManager.instance.AddToScore(5);
                        else
                            GameManager.instance.AddToScore(-3);

                        GameManager.instance.Part3TOFinish();

                        return;
                    }
                }
                // Raf doluysa geri yerine gidecek
                transform.position = startingPos;
            }
            else
                transform.position = startingPos;
        }
    }

    private void OnTriggerEnter(Collider other) => lastHolderCollider = other;
}