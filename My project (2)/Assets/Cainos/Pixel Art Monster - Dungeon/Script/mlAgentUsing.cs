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

    //���Ǽҵ�(�н�����)�� �����Ҷ����� ȣ��
    public override void OnEpisodeBegin()
    {
        rb2d.velocity = Vector3.zero;

        tr.localPosition = new Vector2(Random.Range(-4.0f, 4.0f), 0.05f);
        target.localPosition = new Vector2(Random.Range(-4.0f, 4.0f), 0.55f);
    }

    //ȯ�� ������ ���� �� ������ ��å ������ ���� �극�ο� �����ϴ� �޼ҵ�
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(rb2d.velocity.x);           //1 (x)
    }

    //�극��(��å)���� ���� ���� ���� �ൿ�� �����ϴ� �޼ҵ�
    public override void OnActionReceived(float[] vectorAction)
    {
        float h = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
        Vector3 dir = (Vector3.right * h);
        rb2d.AddForce(dir.normalized * 50.0f);
        SetReward(-0.001f);
    }

    //������(�����)�� ���� ����� ������ ȣ���ϴ� �޼ҵ�(�ַ� �׽�Ʈ�뵵 �Ǵ� ����н��� ���)
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("DEAD_ZONE"))
        {
            //�߸��� �ൿ�� �� ���̳ʽ� ������ �ش�.
            SetReward(-1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }

        if (coll.collider.CompareTag("TARGET"))
        {
            //�ùٸ� �ൿ�� �� �÷��� ������ �ش�.
            SetReward(+1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }
    }
}
