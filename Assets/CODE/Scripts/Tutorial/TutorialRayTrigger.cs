using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TutorialRayTrigger : MonoBehaviour
{
    [Header("On Trigger Event")]
    [FormerlySerializedAs("todoEventIN")]
    [SerializeField] private UnityEvent onPlayerEnter;

    [Header("Off Trigger Event")]
    [FormerlySerializedAs("todoEventOut")]
    [SerializeField] private UnityEvent onPlayerExit;

    public float spacing = 0.75f;
    public float rayDistance = 10f;

    private Collider _collider;
    private bool _triggered;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player"))
            return;

        FireEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        onPlayerExit?.Invoke();
    }

    void Update()
    {
        if (_triggered)
            return;

        Vector3 poleDirection = transform.right;
        float poleLength = transform.localScale.x;
        int numRays = Mathf.FloorToInt(poleLength / spacing) + 1;

        for (int i = 0; i < numRays; i++)
        {
            float offset = -poleLength / 2f + i * spacing;
            Vector3 rayOrigin = transform.position + poleDirection * offset;

            if (Physics.Raycast(rayOrigin, Vector3.up, out RaycastHit hit, rayDistance))
            {
                Debug.DrawRay(rayOrigin, Vector3.up * hit.distance, Color.red);
                if (hit.collider.CompareTag("Player"))
                    FireEnter();
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector3.up * rayDistance, Color.green);
            }
        }
    }

    private void FireEnter()
    {
        if (_triggered)
            return;

        _triggered = true;
        onPlayerEnter?.Invoke();
        if (_collider != null)
            _collider.enabled = false;
    }
}
