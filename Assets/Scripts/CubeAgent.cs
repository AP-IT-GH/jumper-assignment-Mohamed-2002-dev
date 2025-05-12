using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    public float jumpForce = 5f;
    private Rigidbody agentRb;
    private bool isGrounded = true;

    public GameObject obstaclePrefab;
    private GameObject obstacleInstance;

    private int successfulJumps = 0;
    private const int maxJumps = 5;

    public override void OnEpisodeBegin()
    {
        if (agentRb == null)
            agentRb = GetComponent<Rigidbody>();

        transform.position = new Vector3(0f, 1f, 0f);
        agentRb.velocity = Vector3.zero;
        isGrounded = true;
        successfulJumps = 0;

        if (obstacleInstance != null)
            Destroy(obstacleInstance);

        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("Obstacle Prefab niet toegewezen!");
            return;
        }

        Vector3 pos = new Vector3(-4f, 0.5f, Random.Range(-4f, 4f));
        Vector3 scale = new Vector3(1f, 1f, Random.Range(3f, 10f));

        obstacleInstance = Instantiate(obstaclePrefab, pos, Quaternion.identity);
        obstacleInstance.transform.localScale = scale;
        obstacleInstance.tag = "Obstacle";
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(agentRb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float jumpCommand = actions.ContinuousActions.Array[0];

        if (jumpCommand > 0.5f && isGrounded)
        {
            agentRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;

            if (obstacleInstance != null)
            {
                float zDistance = Mathf.Abs(transform.position.z - obstacleInstance.transform.position.z);
                if (zDistance < 1f)
                {
                    AddReward(0.5f); 
                    Debug.Log("Beloning +0.5 voor succesvolle sprong, Z afstand");
                    successfulJumps++;

                    if (successfulJumps >= maxJumps)
                    {
                        AddReward(1f); 
                        Debug.Log("Bonus Beloning +1 voor het bereiken van het maximum aantal sprongen");
                        EndEpisode();
                    }
                }
            }
        }
        else if (jumpCommand > 0.5f && obstacleInstance != null)
        {
            float distance = Vector3.Distance(transform.position, obstacleInstance.transform.position);
            if (distance > 4f)
            {
                AddReward(-0.25f); 
                Debug.Log("Straf -0.25 voor onnodige sprong, Afstand: " + distance);
            }
        }

        if (transform.position.y < -1f)
        {
            AddReward(-1f); 
            Debug.Log("Straf -1 voor vallen van de grond");
            EndEpisode();
        }

        if (jumpCommand <= 0.5f)
        {
            AddReward(-0.001f); 
            Debug.Log("Straf -0.001 voor niet springen");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void Update()
    {
        if (obstacleInstance == null)
            SpawnObstacle();
    }
}
