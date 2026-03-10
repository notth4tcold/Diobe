using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerWeapon weapon;

    private Animator anim;
    private Player player;

    private float cooldown;

    void Awake() {
        anim = GetComponent<Animator>();

        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
        input.OnAttackPressed += TryAttack;
        input.OnAttackReleased += ReleaseAttack;
    }

    void Update() {
        cooldown -= Time.deltaTime;
    }

    void TryAttack() {
        if (player == null || UIManager.Instance.IsUIBlocking || cooldown > 0) return;

        float attackSpeed = player.combat.AttackSpeed;
        cooldown = 1f / attackSpeed;
        anim.SetFloat("AttackSpeed", attackSpeed);
        anim.SetBool("Attack", true);
    }

    void ReleaseAttack() {
        anim.SetBool("Attack", false);
        weapon.ReleaseAttack();
    }

    void OnDestroy() {
        input.OnAttackPressed -= TryAttack;
        input.OnAttackReleased -= ReleaseAttack;
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        player = p;
    }
}
