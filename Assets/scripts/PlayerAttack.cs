using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : NetworkBehaviour
{
    public bool canAttack;
    public Camera cam;

    public float attackSpeed;
    float timer;
    public int damage;
    [SerializeField]
    Image attackCD;
    [SerializeField]
    float radius;

    Vector3 lastAttack;

    private void Start()
    {
        if (IsLocalPlayer)
            cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;
        if (!canAttack) return;
        timer += Time.deltaTime;
        if (Input.GetMouseButton(0) && timer >= attackSpeed)
        {
            timer = 0;
            AttackRpc();
        }
        attackCD.fillAmount = timer / attackSpeed;
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AttackRpc()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int fieldMask = 1 << 9;//field layer for sphere center
        int unitMask = 1 << 6;//unit layer for dealing damage
        if (Physics.Raycast(ray, out hit, 100, fieldMask))
        {
            Collider[] units = Physics.OverlapSphere(hit.point, radius, unitMask);
            lastAttack = hit.point;
            foreach (Collider unit in units)
                unit.GetComponent<Unit>().GetDamageRpc(damage, OwnerClientId);
            print("owner "+OwnerClientId);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(lastAttack, radius);
    }
}
