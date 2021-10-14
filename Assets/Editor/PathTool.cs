using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTool : ScriptableObject
{
    static PathNode m_parent = null;

    [MenuItem("Path Tool/Create PathNode")]
    static void CreatePathNode()
    {
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "PathNode";

        go.tag = "PathNode";

        Selection.activeTransform = go.transform;
    }

    [MenuItem("Path Tool/Set Parent %q")]
    static void SetParent()
    {
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1)
        {
            return;
        }

        if (Selection.activeTransform.tag.CompareTo("PathNode") == 0)
        {
            m_parent = Selection.activeGameObject.GetComponent<PathNode>();
        }
    }

    [MenuItem("Path Tool/Set Next %w")]
    static void SetNextChild()
    {
        if (!Selection.activeGameObject || 
            m_parent == null || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1)
        {
            return;
        }

        if (Selection.activeTransform.tag.CompareTo("PathNode") == 0)
        {
            m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_parent = null;
        }
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
