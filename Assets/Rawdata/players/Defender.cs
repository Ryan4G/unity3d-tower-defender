using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public enum TileState
    {
        DEAD = 0,
        ROAD = 1,
        GUARD = 2,
    }

    public float m_attackArea = 2.0f;

    public int m_power = 1;

    public float m_attackInterval = 2.0f;

    protected Enemy m_targetEnemy;

    protected bool m_isFaceEnemy;

    protected GameObject m_model;

    protected Animator m_ani;

    public static T Create<T>(Vector3 pos, Vector3 angle) where T: Defender
    {
        GameObject go = new GameObject("defender");

        go.transform.position = pos;
        go.transform.eulerAngles = angle;

        T d = go.AddComponent<T>();
        d.Init();

        TileObject.Instance.setDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileState.DEAD);

        return d;
    }

    protected virtual void Init()
    {
        m_attackArea = 2.0f;

        m_power = 2;

        m_attackInterval = 2.0f;

        CreateModel("swordman");

        StartCoroutine(Attack());
    }

    protected virtual void CreateModel(string myname)
    {
        GameObject model = Resources.Load<GameObject>(myname);

        m_model = Instantiate(model, this.transform.position, this.transform.rotation, this.transform);

        m_ani = m_model.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindEnemy();
        RotateTo();
        Attack();
    }

    protected virtual IEnumerator Attack()
    {
        while (m_targetEnemy == null || !m_isFaceEnemy)
        {
            yield return 0;
        }

        // play attack animation
        m_ani.CrossFade("attack", 0.1f);

        while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            yield return 0;
        }

        float ani_length = m_ani.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(ani_length * 0.5f);

        if (m_targetEnemy != null)
        {
            m_targetEnemy.SetDamage(m_power);
        }

        yield return new WaitForSeconds(ani_length * 0.5f);

        m_ani.CrossFade("idle", 0.1f);

        yield return new WaitForSeconds(m_attackInterval);

        StartCoroutine(Attack());
    }

    protected void RotateTo()
    {
        if (m_targetEnemy == null)
        {
            return;
        }

        var targetDir = m_targetEnemy.transform.position - transform.position;

        targetDir.y = 0;

        Vector3 rot_delta = Vector3.RotateTowards(
            this.transform.forward,
            targetDir,
            20.0f * Time.deltaTime, 0.0f);

        Quaternion targetRotation = Quaternion.LookRotation(rot_delta);

        float angle = Vector3.Angle(targetDir, transform.forward);

        m_isFaceEnemy = angle < 1.0f;

        transform.rotation = targetRotation;
    }

    protected void FindEnemy()
    {
        if (m_targetEnemy != null)
        {
            return;
        }

        m_targetEnemy = null;

        int miniLife = 0;

        foreach(Enemy enemy in GameManager.Instance.m_enemyList)
        {
            if (enemy.m_life == 0)
            {
                continue;
            }

            Vector3 pos1 = this.transform.position;
            pos1.y = 0;

            Vector3 pos2 = enemy.transform.position;
            pos2.y = 0;

            float dist = Vector3.Distance(pos1, pos2);

            Debug.Log($"{dist} -> {m_attackArea}");
            // out of attack area
            if (dist > m_attackArea)
            {
                continue;
            }

            // find the lowest life enemy
            if (miniLife == 0 || miniLife > enemy.m_life)
            {
                m_targetEnemy = enemy;
                miniLife = enemy.m_life;
            }
        }
    }
}
