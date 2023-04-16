using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
public class mlAgentUsing : Agent
{
    private Rigidbody2D rb2d;
    private Transform tr;
    private Transform target;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        rb2d.velocity = Vector3.zero;

        tr.localPosition = new Vector2(Random.Range(-4.0f, 4.0f), 0.05f);
        target.localPosition = new Vector2(Random.Range(-4.0f, 4.0f), 0.55f);
    }

    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(rb2d.velocity.x);           //1 (x)
    }

    //브레인(정책)으로 부터 전달 받은 행동을 실행하는 메소드
    public override void OnActionReceived(float[] vectorAction)
    {
        float h = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
        Vector3 dir = (Vector3.right * h);
        rb2d.AddForce(dir.normalized * 50.0f);
        SetReward(-0.001f);
    }

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("DEAD_ZONE"))
        {
            //잘못된 행동일 때 마이너스 보상을 준다.
            SetReward(-1.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }

        if (coll.collider.CompareTag("TARGET"))
        {
            //올바른 행동일 때 플러스 보상을 준다.
            SetReward(+1.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
    }
}
