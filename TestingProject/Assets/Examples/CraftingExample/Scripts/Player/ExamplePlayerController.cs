using UnityEngine;

public class ExamplePlayerController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] Transform itemHolder;

    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private KeyCode throwKey;

    [SerializeField] private bool debugMode;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private Color interactionRadiusColor = Color.magenta;

    Item currentItemHeld;
    bool hasItemOnHand;

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Inputs

    private void Inputs()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Interaction();
        }

        if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }
    }

    #endregion

    #region Player Logic

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        rb.velocity = movement.normalized * speed;

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }

    private void Interaction()
    {
        Collider table = GetNearestTable();

        if (table != null)
        {
            table.GetComponent<IInteractable>().Interact();
        }
    }

    private void Throw()
    {
        DropItem();
    }

    #endregion

    #region Utility Methods

    private Collider GetNearestTable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);

        Collider nearestTable = null;
        float minDistance = float.MaxValue;

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<IInteractable>() != null)
            {
                if (collider.GetComponent<Item>() && hasItemOnHand)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTable = collider;
                }
            }
        }

        return nearestTable;
    }

    private void DrawRayTowardsNearestTable()
    {
        Collider table = GetNearestTable();

        if (table != null)
        {
            Debug.DrawRay(transform.position, table.transform.position - transform.position, highlightColor);
        }
    }

    public void HoldItem(Item item)
    {
        currentItemHeld = item;
        hasItemOnHand = true;
        SetItemTransform(currentItemHeld);
        currentItemHeld.GetComponent<Rigidbody>().useGravity = false;
        currentItemHeld.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void DropItem()
    {
        currentItemHeld.transform.SetParent(null);
        currentItemHeld.GetComponent<Rigidbody>().useGravity = true;
        currentItemHeld.GetComponent<Rigidbody>().isKinematic = false;
        currentItemHeld = null;
        hasItemOnHand = false;
    }

    private void SetItemTransform(Item item)
    {
        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void TransferItem()
    {
        currentItemHeld = null;
        hasItemOnHand = false;
    }

    public bool HasItemOnHand()
    {
        return hasItemOnHand;
    }

    public Item GetCurrentItemHeld()
    {
        return currentItemHeld;
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if (!debugMode) return;

        Gizmos.color = interactionRadiusColor;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        DrawRayTowardsNearestTable();
    }

    #endregion
}
