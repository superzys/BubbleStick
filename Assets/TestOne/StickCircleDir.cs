using UnityEngine;
using System.Collections;

public class StickCircleDir : MonoBehaviour
{
    //最大距离
    private const float MaxDis = 3f;

    //用来检测玩家点击的球
    public GameObject StartObj;
    //玩家移动的球
    public GameObject EndObj;
    //用来显示mesh的Filter
    public MeshFilter MFilter;
    //点的数组，用来存放Start的上下两点和End的上下两点
    public GameObject[] PointArray;

    //自建的mesh
    private Mesh m_Mesh;
    //是否点击到
    private bool m_IsClick;
    //是否在移动
    private bool m_IsMove;
    //链接的mesh的点
    private Vector3[] m_Vertices;

    //初始化
    private void Awake()
    {
        m_Mesh = new Mesh();
        MFilter.mesh = m_Mesh;

        m_Vertices = new Vector3[] { Vector3.zero, Vector3.zero, 
            Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        m_Mesh.vertices = m_Vertices;
    }

    //每帧检测
    private void Update()
    {
        //当点击到
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                m_IsClick = true;
            }
        }

        //如果已经被点击
        if (m_IsClick)
        {
            //转换鼠标坐标到世界坐标
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            EndObj.transform.position = pos;

            //让两个球互相看向对方（为了保持mesh上的点一直在球自身的本地坐标的上下方）
            EndObj.transform.LookAt(StartObj.transform);
            StartObj.transform.LookAt(EndObj.transform);

            //小优化，为了做根据两个球的距离做缩放的逻辑
            float dis = Vector3.Distance(EndObj.transform.position, StartObj.transform.position);
           
                float scale = dis / MaxDis;

                PointArray[0].transform.localPosition = new Vector3(0f, 0.5f - 0.08f * scale, 0f);
                PointArray[1].transform.localPosition = new Vector3(0f, -0.5f + 0.08f * scale, 0f);

                PointArray[2].transform.localPosition = new Vector3(0f, 0.5f - 0.15f * scale, 0f);
                PointArray[3].transform.localPosition = new Vector3(0f, -0.5f + 0.15f * scale, 0f);

			if(scale > 1f)
			{
				scale = 0.9f;
			}
				EndObj.transform.localScale = Vector3.one * (0.6f * (1f - scale));
          
            //mesh的信息加入
            m_Vertices[0] = PointArray[0].transform.position;
            m_Vertices[1] = PointArray[1].transform.position;
            m_Vertices[2] = PointArray[2].transform.position;

            m_Vertices[3] = PointArray[1].transform.position;
            m_Vertices[4] = PointArray[2].transform.position;
            m_Vertices[5] = PointArray[3].transform.position;

            m_Mesh.vertices = m_Vertices;
            m_Mesh.triangles = new int[] { 0, 1, 2, 5, 4, 3 };
        }

        //当鼠标抬起的时候恢复状态
        if (Input.GetMouseButtonUp(0))
        {
            m_Mesh = new Mesh();
            MFilter.mesh = m_Mesh;

            if (m_IsMove)
            {
                StartObj.SetActive(true);
                
                MFilter.gameObject.SetActive(true);
                m_IsMove = false;
            }

            m_IsClick = false;

            StartObj.transform.localScale = Vector3.one;
            StartObj.transform.position = EndObj.transform.position;
            //EndObj.transform.position = StartObj.transform.position;
            EndObj.transform.localScale = Vector3.one;
        }
    }
}