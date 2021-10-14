using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor
{
    protected bool editMode = false;

    protected TileObject tileObject;

    private void OnEnable()
    {
        tileObject = (TileObject)target;
    }

    public void OnSceneGUI()
    {
        if (editMode)
        {
            // cancel editor function
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            tileObject.debug = true;

            // catch input events
            Event e = Event.current;

            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 2000, tileObject.tileLayer))
                {
                    tileObject.setDataFromPosition(hitInfo.point.x, hitInfo.point.z, tileObject.dataID);
                }
            }
        }

        HandleUtility.Repaint();
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tile Editor");
        editMode = EditorGUILayout.Toggle("Edit", editMode);
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);

        string[] editDataStr = { "Dead", "Road", "Guard" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);

        EditorGUILayout.Separator();

        if (GUILayout.Button("Reset"))
        {
            tileObject.Reset();
        }

        DrawDefaultInspector();
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
