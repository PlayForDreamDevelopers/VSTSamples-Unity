using System.Collections.Generic;
using UnityEngine;

public class QuadManager : MonoBehaviour
{
    private static QuadManager m_Instance;
    public static QuadManager instance => m_Instance;
    private Dictionary<int, List<Vector3>> m_QuadsDic = new Dictionary<int, List<Vector3>>();
    [SerializeField] private int m_QuadCount;
    public int creatQuadIndex;
    public Material quadMaterial;
    public Material pointMaterial;
    public Material lineMaterial;

    public Transform RightRayOrigin;
    public float pointSpawnDistance;
    
    private List<Vector3> m_CurrentQuadPoints = new List<Vector3>();
    private List<GameObject> m_DrawMarker = new List<GameObject>();

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
    }

    public void AddQuadPoint()
    {
        // 当未完成当前四边形时处理临时点
        if (m_CurrentQuadPoints.Count < 3)
        {
            Vector3 point = RightRayOrigin.position + RightRayOrigin.forward * pointSpawnDistance;
            m_CurrentQuadPoints.Add(point);

            // 根据点数绘制不同内容
            switch (m_CurrentQuadPoints.Count)
            {
                case 1:
                    DrawSphere(point); // 第一个点绘制小球
                    break;
                case 2:
                    DrawLine(m_CurrentQuadPoints[0], m_CurrentQuadPoints[1]); // 两点连线
                    break;
                case 3:
                    DrawQuadrilateral(m_CurrentQuadPoints[0], m_CurrentQuadPoints[1], m_CurrentQuadPoints[2]);//绘制四边形
                    break;
            }
        }
    }

    // 计算平行四边形的第四个点
    private Vector3 CalculateParallelogramFourthPoint(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 通过向量计算：p3 = p0 + (p1 - p0) + (p2 - p0) = p1 + p2 - p0
        return p0 + p2 - p1;
    }

    // 绘制小球（第一个点）
    private void DrawSphere(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one*0.05f;
        sphere.GetComponent<MeshRenderer>().material = pointMaterial;
        m_DrawMarker.Add(sphere);
    }

    // 绘制线段（第二个点）
    private void DrawLine(Vector3 start, Vector3 end)
    {
        DrawSphere(end);
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        Vector3[] lrPoint = { start, end };
        lr.startWidth = lr.endWidth = 0.05f;
        lr.material = lineMaterial;
        lr.SetPositions(lrPoint);
        m_DrawMarker.Add(lineObj);
    }

    private void DrawQuadrilateral(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 计算第四个点完成平行四边形
        Vector3 p3 = CalculateParallelogramFourthPoint(p0, p1, p2);
        m_CurrentQuadPoints.Add(p3);

        // 存入字典并生成四边形
        m_QuadsDic.Add(creatQuadIndex, new List<Vector3>(m_CurrentQuadPoints));
        CreateQuad();
        creatQuadIndex++;
        ClearTemporaryPoints();
    }

    public void ClearTemporaryPoints()
    {
        m_CurrentQuadPoints.Clear(); // 重置临时点
        //销毁绘制标识
        foreach (GameObject marker in m_DrawMarker)
        {
            Destroy(marker);
        }
        m_DrawMarker.Clear();
    }
    public void CreateQuad()
    {
        if (creatQuadIndex >= m_QuadsDic.Count || m_QuadsDic[creatQuadIndex].Count != 4)
        {
            Debug.LogWarning("四边形数据不完整");
            return;
        }

        List<Vector3> quadPoints = m_QuadsDic[creatQuadIndex];
        GameObject quad = new GameObject("Parallelogram " + creatQuadIndex);
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();
        meshRenderer.material = quadMaterial;

        // 顶点直接使用四个点
        Vector3[] vertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = quadPoints[i];
        }

        // 三角形索引（两个三角形组成四边形）
        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        // UV映射（简单拉伸）
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(0, 1);

        // 更新网格
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Debug.Log("平行四边形生成完成");
    }

    // 保留原有ProjectPointToPlane方法（未使用但避免报错）
    private Vector3 ProjectPointToPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {
        float distance = Vector3.Dot(point - planePoint, planeNormal);
        return point - distance * planeNormal;
    }
}