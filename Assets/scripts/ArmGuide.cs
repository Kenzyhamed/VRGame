using UnityEngine;
using System.Collections;
using Oculus.Interaction;

public class ArmGuideCubeToItsSlot : MonoBehaviour
{
    [Header("What moves")]
    public Transform armRoot;

    [Header("Reference point (existing fingertip child transform)")]
    public Transform fingerTip;

    [Header("Cubes in order (Cube 0 -> Slot 0, etc.)")]
    public Transform[] cubes;

    [Header("Slots in order (must match cubes order)")]
    public Transform[] slots;

    [Header("Movement")]
    public float moveSpeed = 0.45f;          // meters/sec
    public float settleDistance = 0.002f;    // 2mm

    [Header("Finger distance from target point (0 = exactly on it)")]
    public float fingerDistance = 0.003f;

    [Header("Hover motion")]
    public float hoverSide = 0.01f;
    public float hoverUp = 0.005f;
    public float hoverSpeed = 2f;

    [Header("Placed detection (CORRESPONDING SLOT ONLY)")]
    public float placedRadius = 0.03f;

    int index = 0;

    void Start()
    {
        if (armRoot == null) { Debug.LogError("Assign armRoot."); enabled = false; return; }
        if (fingerTip == null) { Debug.LogError("Assign fingerTip."); enabled = false; return; }
        if (cubes == null || cubes.Length == 0) { Debug.LogError("Assign cubes[]."); enabled = false; return; }
        if (slots == null || slots.Length != cubes.Length)
        {
            Debug.LogError("Slots array must exist and match cubes array length.");
            enabled = false;
            return;
        }

        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (index < cubes.Length)
        {
            Transform cube = cubes[index];
            Transform slot = slots[index];

            if (cube == null || slot == null)
            {
                index++;
                continue;
            }

            Grabbable g = GetGrabbable(cube);
            if (g == null)
            {
                index++;
                continue;
            }

            // 🔁 Retry loop until THIS cube is placed in ITS slot
            while (true)
            {
                bool grabbed = IsGrabbed(g);

                // ✅ SUCCESS: released AND in corresponding slot
                if (!grabbed && IsCubeInSlot(cube, slot))
                    break;

                if (!grabbed)
                {
                    // Not grabbed → go to cube (prompt user to grab again)
                    Vector3 p = cube.position;
                    Vector3 hover = ComputeHover(p, cube);
                    MoveArmSoFingerReaches(ComputeDesiredFingerPos(p + hover));
                }
                else
                {
                    // Grabbed → guide toward corresponding slot
                    Vector3 p = slot.position;
                    Vector3 hover = ComputeHover(p, slot);
                    MoveArmSoFingerReaches(ComputeDesiredFingerPos(p + hover));
                }

                yield return null;
            }

            // Move on to next cube/slot pair
            index++;
        }

        Debug.Log("ArmGuideCubeToItsSlot: finished all cubes.");
    }

    // --- Meta grab state ---
    bool IsGrabbed(Grabbable g)
    {
        return g.GrabPoints != null && g.GrabPoints.Count > 0;
    }

    // --- Placed in CORRESPONDING slot only ---
    bool IsCubeInSlot(Transform cube, Transform slot)
    {
        return Vector3.Distance(cube.position, slot.position) <= placedRadius;
    }

    // --- Grabbable lookup ---
    Grabbable GetGrabbable(Transform t)
    {
        Grabbable g =
            t.GetComponentInParent<Grabbable>() ??
            t.GetComponentInChildren<Grabbable>() ??
            t.GetComponent<Grabbable>();

        if (g == null)
            Debug.LogError($"No Oculus.Interaction.Grabbable found on {t.name}.");

        return g;
    }

    // --- Fingertip-driven movement ---
    Vector3 ComputeDesiredFingerPos(Vector3 targetPoint)
    {
        if (fingerDistance <= 0.0001f) return targetPoint;

        Vector3 dir = (fingerTip.position - targetPoint);
        if (dir.sqrMagnitude < 0.000001f) dir = Vector3.back;

        return targetPoint + dir.normalized * fingerDistance;
    }

    Vector3 ComputeHover(Vector3 aroundPoint, Transform basis)
    {
        float t = Time.time * hoverSpeed;
        float side = Mathf.Sin(t) * hoverSide;
        float up = Mathf.Sin(t * 1.7f) * hoverUp;

        return basis.right * side + basis.up * up;
    }

    void MoveArmSoFingerReaches(Vector3 desiredFingerPos)
    {
        Vector3 delta = desiredFingerPos - fingerTip.position;
        Vector3 targetArmPos = armRoot.position + delta;
        armRoot.position = Vector3.MoveTowards(
            armRoot.position,
            targetArmPos,
            moveSpeed * Time.deltaTime
        );
    }
}
