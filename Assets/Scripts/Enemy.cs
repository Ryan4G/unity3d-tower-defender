using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PathNode m_currentNode;

    public int m_life = 5;

    public int m_maxlife = 15;

    public float m_speed = 2;

    public System.Action<Enemy> onDeath;

    protected Transform m_lifebarObj;

    protected UnityEngine.UI.Slider m_lifebar;

    // Start is called before the first frame update
    protected void Start()
    {
        GameManager.Instance.m_enemyList.Add(this);

        GameObject prefab = (GameObject)Resources.Load("Canvas3D");

        m_lifebarObj = ((GameObject)Instantiate(prefab, Vector3.zero,
                Camera.main.transform.rotation, this.transform)).transform;

        m_lifebarObj.localPosition = new Vector3(0, 2.0f, 0);
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        m_lifebar = m_lifebarObj.GetComponentInChildren<UnityEngine.UI.Slider>();

        StartCoroutine(UpdateLifebar());
    }

    // Update is called once per frame
    void Update()
    {
        RotateTo();
        MoveTo();
    }

    protected void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;

        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z),
            new Vector2(pos2.x, pos2.z));

        if (dist < 1.0f)
        {
            if (m_currentNode.m_next == null)
            {
                GameManager.Instance.SetDamage(1);
                DestoryMe();
            }
            else
            {
                m_currentNode = m_currentNode.m_next;
            }
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }

    protected void DestoryMe()
    {
        GameManager.Instance.m_enemyList.Remove(this);
        onDeath(this);
        Destroy(this.gameObject);
    }

    protected void RotateTo()
    {
        var position = m_currentNode.transform.position - transform.position;

        position.y = 0;

        var targetRotation = Quaternion.LookRotation(position);

        float next = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime);

        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;

        if (m_life <= 0)
        {
            m_life = 0;

            GameManager.Instance.SetPoint(5);
            DestoryMe();
        }
    }

    protected IEnumerator UpdateLifebar()
    {
        m_lifebar.value = m_life * 1.0f / m_maxlife;

        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;

        yield return 0;

        StartCoroutine(UpdateLifebar());
    }
}
