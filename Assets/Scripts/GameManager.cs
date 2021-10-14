using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public LayerMask m_groundLayer;

    public int m_wave = 1;

    public int m_waveMax = 10;

    public int m_life = 10;

    public int m_point = 30;

    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;

    Button m_but_try;

    bool m_isSelectedButton = false;

    public bool m_debug = true;

    public List<PathNode> m_PathNodes;

    public List<Enemy> m_enemyList = new List<Enemy>();

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        UnityAction<BaseEventData> downAction =
            new UnityAction<BaseEventData>(OnButCreateDefenderDown);

        UnityAction<BaseEventData> upAction =
            new UnityAction<BaseEventData>(OnButCreateDefenderUp);

        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("wave") == 0)
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if (t.name.CompareTo("point") == 0)
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_point.text = $"Point: <color=yellow>{m_point}</color>";
            }
            else if (t.name.CompareTo("life") == 0)
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = $"Life: <color=yellow>{m_life}</color>";
            }
            else if (t.name.CompareTo("but_try") == 0)
            {
                m_but_try = t.GetComponent<Button>();
                m_but_try.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });

                m_but_try.gameObject.SetActive(false);
            }
            else if (t.name.Contains("but_player"))
            {
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }
    }

    public void SetWave(int v)
    {
        m_wave = v;
        m_txt_wave.text = $"Wave: <color=yellow>{m_wave}/{m_waveMax}</color>";
    }

    public void SetDamage(int v)
    {
        m_life -= v;

        if (m_life <= 0)
        {
            m_life = 0;
            m_but_try.gameObject.SetActive(true);
        }

        m_txt_life.text = $"Life: <color=yellow>{m_life}</color>";
    }

    public bool SetPoint(int v)
    {
        if (m_point + v < 0)
        {
            return false;
        }

        m_point += v;
        m_txt_point.text = $"Point: <color=yellow>{m_point}</color>";

        return true;
    }
    private void OnButCreateDefenderUp(BaseEventData arg0)
    {
        //Debug.Log("Button Up");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundLayer))
        {
            var state = TileObject.Instance.getDataFromPosition(hitinfo.point.x, hitinfo.point.z);
            var index = TileObject.Instance.getIndexFromPosition(hitinfo.point.x, hitinfo.point.z);

            //Debug.Log($"{index} : {state}");

            if (state == (int)Defender.TileState.GUARD)
            {
                Vector3 hitpos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);

                Vector3 gridPos = TileObject.Instance.transform.position;

                float tileSize = TileObject.Instance.tileSize;

                hitpos.x = gridPos.x + (int)((hitpos.x - gridPos.x) / tileSize) * tileSize + tileSize * 0.5f;
                hitpos.z = gridPos.z + (int)((hitpos.z - gridPos.z) / tileSize) * tileSize + tileSize * 0.5f;

                GameObject go = arg0.selectedObject;

                if (go.name.CompareTo("but_player1") == 0)
                {
                    if (SetPoint(-15))
                    {
                        Defender.Create<Defender>(hitpos, new Vector3(0, 180, 0));
                    }
                }
                else if (go.name.CompareTo("but_player2") == 0)
                {
                    if (SetPoint(-20))
                    {
                        Defender.Create<Archer>(hitpos, new Vector3(0, 180, 0));
                    }
                }
            }
        }

        m_isSelectedButton = false;
    }

    private void OnButCreateDefenderDown(BaseEventData arg0)
    {
        //Debug.Log("Button Down");
        m_isSelectedButton = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isSelectedButton)
        {
            return;
        }

#if(UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        bool press = Input.touches.Length > 0;

        float mx = 0;

        float my = 0;

        if (press){
            if (Input.GetTouch(0).phase == TouchPhase.Moved){
                mx = Input.GetTouch(0).deltaPositions.x * 0.01f;
                my = Input.GetTouch(0).deltaPositions.y * 0.01f;
            }
        }
#else
        bool press = Input.GetMouseButton(0);
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
#endif

        GameCamera.Instance.Control(press, mx, my);
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new List<PathNode>();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("PathNode");

        for(int i = 0; i < objs.Length; i++)
        {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());
        }
    }
    private void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null)
        {
            return;
        }

        Gizmos.color = Color.blue;

        foreach (PathNode node in m_PathNodes)
        {
            if (node.m_next != null)
            {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
