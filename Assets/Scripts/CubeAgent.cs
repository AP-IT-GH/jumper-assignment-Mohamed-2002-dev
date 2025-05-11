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

        transform.position = new Vector3(0f, 1f, 0f);
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
            float obstacleZ = transform.position.z; // zelfde Z als agent
            obstacleInstance = Instantiate(obstaclePrefab, new Vector3(-4f, 0.5f, obstacleZ), Quaternion.identity);
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

        if (jumpCommand > 0.5f && isGrounded)
        {
            agentRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
        }

        if (transform.position.y < -1f)
        {
            AddReward(-1f);
            Debug.Log("Agent gevallen: -1 reward");
            EndEpisode();
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

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
            Debug.Log("Botst tegen obstakel: -0.5 reward");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.SetParent(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.SetParent(null);

            if (transform.position.y > 1.2f) // controleer of agent sprong over obstakel
            {
                AddReward(0.5f);
                Debug.Log("Succesvolle sprong over obstakel: +0.5 reward");
            }
        }
    }

    // BONUS: automatisch een nieuwe obstacle als huidige vernietigd wordt
    private void Update()
    {
        if (obstacleInstance == null)
        {
            SpawnObstacle();
        }
    }
}
