using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 20f;

    [Header("Despawn Settings")]
    public float despawnYThreshold = -10f;
    public float lifetime = 10f;

    private bool _hasHitPlayer = false;
    private float _lifetimeTimer = 0f;
    private StaminaManager _manager;

    private void Start()
    {
        _manager = GameObject.FindAnyObjectByType<StaminaManager>();
    }

    void Update()
    {
        if (transform.position.y < despawnYThreshold)
        {
            ReturnToPool();
            return;
        }

        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer >= lifetime)
        {
            ReturnToPool();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasHitPlayer) return;

        if (IsPlayer(collision.gameObject))
        {
            _hasHitPlayer = true;
            ApplyDamage();
            ReturnToPool();
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj.CompareTag("Player")) return true;

        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null) return true;

        Transform parent = obj.transform.parent;
        if (parent != null)
        {
            if (parent.CompareTag("Player")) return true;
            if (parent.GetComponent<CharacterController>() != null) return true;
        }

        return false;
    }

    private void ApplyDamage()
    {
        if (_manager != null)
        {
            _manager.ApplyDamageToBothHands(damageAmount);
        }

        if (DamageOverlayUI.Instance != null)
        {
            DamageOverlayUI.Instance.TriggerDamageFlash();
        }
    }

    private void ReturnToPool()
    {
        if (FallingObjectSpawner.Instance != null)
        {
            FallingObjectSpawner.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        _hasHitPlayer = false;
        _lifetimeTimer = 0f;
    }
}
