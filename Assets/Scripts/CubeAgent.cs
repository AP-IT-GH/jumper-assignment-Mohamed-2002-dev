using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    public float jumpForce = 5f;
    private Rigidbody agentRb;
    private bool isGrounded = true;

    public GameObject obstaclePrefab; // Prefab voor nieuwe obstakels
    private GameObject obstacleInstance; // De huidige instantie van het obstakel

    public override void OnEpisodeBegin()
    {
        Initialisatie();

        transform.position = new Vector3(0f, 1f, 0f); // Zet de agent terug naar de startpositie
        agentRb.velocity = Vector3.zero;
        transform.SetParent(null);
        isGrounded = true;

        // Verwijder oude obstakel als die nog bestaat
        if (obstacleInstance != null)
        {
            Destroy(obstacleInstance);
        }

        // Spawn een nieuwe obstacle
        SpawnObstacle();
    }

    private void Initialisatie()
    {
        if (agentRb == null)
        {
            agentRb = GetComponent<Rigidbody>();
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefab != null)
        {
            obstacleInstance = Instantiate(obstaclePrefab, new Vector3(-4f, 0.5f, Random.Range(-4f, 4f)), Quaternion.identity);
            obstacleInstance.transform.localScale = new Vector3(1f, 1f, Random.Range(3f, 10f));
            obstacleInstance.tag = "Obstacle"; // Zorg dat de tag klopt
        }
        else
        {
            Debug.LogError("Obstacle Prefab niet toegewezen! Sleep een prefab in de inspector.");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(agentRb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpCommand = actions.ContinuousActions[0];

        // Hier checken we of de agent te vroeg springt op basis van een minimum afstand (bijv. 2.7)
        if (jumpCommand > 0.5f && isGrounded && transform.position.y < 2.7f)
        {
            agentRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
            AddReward(1f); // Beloning voor springen
            Debug.Log("Beloning voor springen: +1");
        }

        // Controleer of de agent onder de drempel valt
        if (transform.position.y < -1f)
        {
            // Zet de agent terug naar de beginpositie
            transform.position = new Vector3(0f, 1f, 0f);
            agentRb.velocity = Vector3.zero;
            isGrounded = true;

            // Spawnen van nieuwe obstakels
            if (obstacleInstance != null)
            {
                Destroy(obstacleInstance);
            }
            SpawnObstacle();

            AddReward(-1f); // Beloning voor vallen
            Debug.Log("Beloning voor vallen van platform: -1");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.SetParent(other.transform);
            // Deze beloning is verwijderd
            // AddReward(-0.5f); // Beloning voor raken van obstakel
            Debug.Log("Raakte obstakel, maar geen beloning meer.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.SetParent(null);
        }
    }

    // BONUS: automatisch een nieuwe obstacle als huidige vernietigd wordt
    private void Update()
    {
        if (obstacleInstance == null)
        {
            SpawnObstacle(); // Spawnen van een nieuw obstakel als de oude vernietigd is
        }

        // Geen actie (niets doen)
        AddReward(-0.001f); // Beloning voor niks doen
        // Geen debug log voor niks doen
    }
}
