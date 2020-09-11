using UnityEngine;
using UnityEngine.Events;

public class PillDragDrop : MonoBehaviour
{
    private Rigidbody rb;

    public float bottlePosZ = 0;    // Şişelerin pozisyonu
    [HideInInspector] public bool isDragging;
    [HideInInspector] public bool canDrag = true;

    public UnityEvent onBeginDrag;
    public UnityEvent onEndDrag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {//touch olacak
        Drag();

        if (transform.position.y < 0 && LevelManager.instance.currentPart == LevelManager.PARTS.PART2)
        {
            rb.velocity = Vector3.zero;
            LevelManager.instance.petri.transform.GetChild(0).GetComponent<PillSpawner>().RespawnPill(gameObject);
        }
    }

    private void Drag()
    {
        if (isDragging && canDrag)
        {
            Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, bottlePosZ));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
                transform.position = ray.GetPoint(distance);

            if (transform.position.y <= 0.32f)
                EndDrag();
        }
    }

    private void OnMouseDown()
    {
        if (!isDragging && canDrag)
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

        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void EndDrag()
    {
        onEndDrag.Invoke();

        isDragging = false;
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottles"))
        {
            canDrag = false;
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            EndDrag();
        }
    }
}