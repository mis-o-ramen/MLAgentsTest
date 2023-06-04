using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BallBalanceAgent : Agent
{
    [SerializeField]
    private GameObject Ball;

    private Rigidbody BallRB;

    /// <summary>
    /// 初期化処理　Start()のようなもの
    /// </summary>
    public override void Initialize()
    {
        this.BallRB = this.Ball.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 現在の状態(入力)を取得
    ///1. Ballの速度(x)
    ///2. Ballの速度(y)
    ///3. Ballの速度(z)
    ///4. Ballの位置(x)
    ///5. Ballの位置(y)
    ///6. Ballの位置(z)
    ///7. Cubeの角度(z)
    ///8. Cubeの角度(x)
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.BallRB.velocity);
        sensor.AddObservation(this.Ball.transform.position);
        sensor.AddObservation(this.transform.rotation.z);
        sensor.AddObservation(this.transform.rotation.x);
    }

    /// <summary>
    /// Action実行
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        //出力層の値を-1~1の範囲に収める
        var angleZ = actions.ContinuousActions.Array[0];
        var angleX = actions.ContinuousActions.Array[1];

        //回転
        this.gameObject.transform.Rotate(new Vector3(angleX, 0, angleZ));

        if (Ball.transform.position.y < transform.position.y)
        {
            //ボールが落ちたらマイナス報酬 & エピソード終了
            this.SetReward(-1f);
            this.EndEpisode();
        }
        else
        {
            //継続中は常に報酬0.1を与える
            this.SetReward(.1f);
        }
    }

    /// <summary>
    /// 各エピソード開始時に呼び出される処理
    /// </summary>
    public override void OnEpisodeBegin()
    {
        var maxCubeRotation = 10f;
        var maxBallPosition = .15f;

        //Cubeの角度をランダムに初期化
        this.gameObject.transform.rotation = Quaternion.Euler(Random.Range(-maxCubeRotation, maxCubeRotation), 0, Random.Range(-maxCubeRotation, maxCubeRotation));

        //Ballを停止 & 位置をランダムに初期化
        this.BallRB.velocity = Vector3.zero;
        this.Ball.transform.position = new Vector3(Random.Range(-maxBallPosition, maxBallPosition), .6f, Random.Range(-maxBallPosition, maxBallPosition)) + this.transform.position;
    }
}
