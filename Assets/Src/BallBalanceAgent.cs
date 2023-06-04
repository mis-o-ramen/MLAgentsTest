using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BallBalanceAgent : Agent
{
    [SerializeField]
    private GameObject Ball;

    private Rigidbody BallRB;
    private EnvironmentParameters DefaultParameters;

    public override void Initialize()
    {
        this.BallRB = this.Ball.GetComponent<Rigidbody>();
        this.DefaultParameters = Academy.Instance.EnvironmentParameters;
        this.ResetScene();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.BallRB.velocity);
        sensor.AddObservation(this.Ball.transform.position);
        sensor.AddObservation(this.transform.rotation.z);
        sensor.AddObservation(this.transform.rotation.x);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var angleZ = 2f * Mathf.Clamp(actions.ContinuousActions.Array[0], -1f, 1f);
        var angleX = 2f * Mathf.Clamp(actions.ContinuousActions.Array[1], -1f, 1f);

        if ((this.gameObject.transform.rotation.z < .25f && angleZ > 0f) ||
            (this.gameObject.transform.rotation.z > -.25 && angleZ < 0f))
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0, 1), angleZ);
        }

        if ((this.gameObject.transform.rotation.x < .25f && angleX > 0f) ||
            (this.gameObject.transform.rotation.x > -.25 && angleX < 0f))
        {
            this.gameObject.transform.Rotate(new Vector3(1, 0, 0), angleX);
        }

        if (Ball.transform.position.y < transform.position.y)
        {
            this.SetReward(-1f);
            this.EndEpisode();
        }
        else
        {
            this.SetReward(.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0] = -Input.GetAxis("Horizontal");
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        this.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        this.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        this.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        this.BallRB.velocity = Vector3.zero;
        this.Ball.transform.position = new Vector3(Random.Range(-.15f, .15f), .6f, Random.Range(-.15f, .15f)) + this.transform.position;

        this.ResetScene();


    }

    private void ResetScene()
    {
        this.BallRB.mass = DefaultParameters.GetWithDefault("mass", 1.0f);
        var scale = DefaultParameters.GetWithDefault("scale", 0.1f);
        Ball.transform.localScale = new Vector3(scale, scale, scale);
    }
}
