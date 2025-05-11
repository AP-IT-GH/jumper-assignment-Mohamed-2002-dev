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

        // Springen als de jumpCommand groter is dan 0.5f en het agent is op de grond
        if (jumpCommand > 0.5f && isGrounded)
        {
            agentRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;

            // Controleer of het agent over een obstakel springt en geef een reward
            if (obstacleInstance != null && transform.position.z > obstacleInstance.transform.position.z - 1f && transform.position.z < obstacleInstance.transform.position.z + 1f)
            {
                AddReward(0.5f); // Rewards wanneer het agent succesvol over het obstakel springt
                Debug.Log("Succesvol gesprongen: +0.5f");
            }
        }
        // Onnodig springen op een afstand van meer dan 4 een negatieve reward
        else if (jumpCommand > 0.5f && Vector3.Distance(transform.position, obstacleInstance.transform.position) > 4f)
        {
            AddReward(-0.5f); // Straf voor onnodig springen op grote afstand van het obstakel
            Debug.Log("Onnodig springen met positie: " + transform.position);
        }

        // Eindig de episode als het agent valt
        if (transform.position.y < -1f)
        {
            AddReward(-1f); // Negatieve reward voor vallen
            Debug.Log("Agent gevallen: -1f");
            EndEpisode();
        }

        // Geef een kleine negatieve straf voor niets doen, bijvoorbeeld als het agent te lang stil blijft
        if (jumpCommand <= 0.5f)
        {
            AddReward(-0.001f); // Negatieve beloning voor niet springen
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
            SpawnObstacle();
        }
    }
}
