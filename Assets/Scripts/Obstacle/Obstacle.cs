using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleData data;
    [SerializeField] private float despawnOffset = 5f;

    private Transform playerTransform;
    private Health health;
    private Rigidbody rb;
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        if (data != null && health != null)
        {
            health.Init(data.maxHealth);
            health.OnDeath += HandleDeath;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTransform != null && transform.position.z < playerTransform.position.z - despawnOffset)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 launchDirection)
    {
        if (launched || rb == null || data == null)
            return;

        launched = true;

        Vector3 launch = (launchDirection + Vector3.up * 0.5f).normalized * data.launchForce;
        rb.AddForce(launch, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Player player = collision.gameObject.GetComponent<Player.Player>();
            if (player != null)
            {
                player.PlayerTakeDamage(data.damage);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("EnemyMelee") || other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }
}