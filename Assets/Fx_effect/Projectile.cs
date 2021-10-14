using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    System.Action<Enemy> onAttack;

    Transform m_target;

    Bounds m_targetCenter;

    public static void Create(Transform target, Vector3 spawnPos, System.Action<Enemy> onAttack)
    {
        GameObject prefab = Resources.Load<GameObject>("arrow");
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.LookRotation(target.position - spawnPos));

        Projectile arrowModel = go.AddComponent<Projectile>();
        arrowModel.m_target = target;
        arrowModel.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        arrowModel.onAttack = onAttack;

        Destroy(go, 3.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target != null)
        {
            this.transform.LookAt(m_targetCenter.center);
        }

        this.transform.Translate(new Vector3(0, 0, 10 * Time.deltaTime));

        if (m_target != null)
        {
            if (Vector3.Distance(this.transform.position, m_targetCenter.center) < 0.5f)
            {
                onAttack(m_target.GetComponent<Enemy>());
                Destroy(this.gameObject);
            }
        }
    }
}
