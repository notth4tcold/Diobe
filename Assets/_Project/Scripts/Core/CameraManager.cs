using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    private Transform target;
    private Vector3 followSpeed = Vector3.zero;
    private float dampTime = 0.001f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 point = cam.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, point.z));
        Vector3 destination = cameraTransform.position + delta;
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, destination, ref followSpeed, dampTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Shake()
    {
        animator.SetTrigger("Shake");
    }
}
