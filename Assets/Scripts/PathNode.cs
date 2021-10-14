﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode m_parent;

    public PathNode m_next;

    public void SetNext(PathNode node)
    {
        if (m_next != null)
        {
            m_next.m_parent = null;
        }
        m_next = node;
        node.m_parent = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Node.tif");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
