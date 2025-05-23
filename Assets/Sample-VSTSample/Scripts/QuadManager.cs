using System;
using System.Collections.Generic;
using UnityEngine;

namespace YVR.VST.Sample
{
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
        [HideInInspector]public bool isSettingPoint;
        
        private List<Vector3> m_CurrentQuadPoints = new List<Vector3>();
        private List<GameObject> m_DrawMarker = new List<GameObject>();

        private void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
        }

        private void Update()
        {
            SettingPoint();
        }
        
        public void SettingPoint()
        {
            if(m_DrawMarker.Count == 1 && isSettingPoint)
            {
                m_CurrentQuadPoints[0] = RightRayOrigin.position + RightRayOrigin.forward * pointSpawnDistance;
                m_DrawMarker[0].transform.position = m_CurrentQuadPoints[0];
            }
            else if (m_DrawMarker.Count == 3 && isSettingPoint)
            {
                LineRenderer lr = m_DrawMarker[1].GetComponent<LineRenderer>();
                m_CurrentQuadPoints[1] = RightRayOrigin.position + RightRayOrigin.forward * pointSpawnDistance;
                m_DrawMarker[2].transform.position = m_CurrentQuadPoints[1];
                lr.SetPosition(1, m_CurrentQuadPoints[1]);
            }
            else if (m_DrawMarker.Count == 8 && isSettingPoint)
            {
                LineRenderer lr = m_DrawMarker[3].GetComponent<LineRenderer>();
                m_CurrentQuadPoints[2] = RightRayOrigin.position + RightRayOrigin.forward * pointSpawnDistance;
                m_DrawMarker[4].transform.position = m_CurrentQuadPoints[2];
                lr.SetPosition(1, m_CurrentQuadPoints[2]);
                
                lr = m_DrawMarker[5].GetComponent<LineRenderer>();
                m_CurrentQuadPoints[3]
                    = CalculateParallelogramFourthPoint(m_CurrentQuadPoints[0], m_CurrentQuadPoints[1],
                                                        m_CurrentQuadPoints[2]);
                m_DrawMarker[6].transform.position = m_CurrentQuadPoints[3];
                lr.SetPosition(0, m_CurrentQuadPoints[2]);
                lr.SetPosition(1, m_CurrentQuadPoints[3]);
                
                lr = m_DrawMarker[7].GetComponent<LineRenderer>();
                lr.SetPosition(0, m_CurrentQuadPoints[3]);
            }
        }
        public void AddQuadPoint()
        {
            if (!isSettingPoint)
            {
                if(m_CurrentQuadPoints.Count == 4)
                {
                    DrawQuadrilateral();
                }
                else
                {
                    Vector3 point = RightRayOrigin.position + RightRayOrigin.forward * pointSpawnDistance;
                    m_CurrentQuadPoints.Add(point);
                    // 根据点数绘制不同内容
                    switch (m_CurrentQuadPoints.Count)
                    {
                        case 1:
                            DrawSphere(point); // 绘制第一个点
                            break;
                        case 2:
                            DrawLine(m_CurrentQuadPoints[0], m_CurrentQuadPoints[1]); // 两点连线
                            DrawSphere(point);//绘制第二个点
                            break;
                        case 3:
                            DrawLine(m_CurrentQuadPoints[1], m_CurrentQuadPoints[2]); // 两点连线
                            DrawSphere(point); //绘制第三个点
                            Vector3 calculatePoint = CalculateParallelogramFourthPoint(m_CurrentQuadPoints[0], m_CurrentQuadPoints[1], m_CurrentQuadPoints[2]);
                            m_CurrentQuadPoints.Add(calculatePoint);
                            DrawLine(m_CurrentQuadPoints[2], m_CurrentQuadPoints[3]); // 两点连线
                            DrawSphere(calculatePoint);// 绘制第四个点
                            DrawLine(m_CurrentQuadPoints[3], m_CurrentQuadPoints[0]); // 两点连线
                            break;
                    }
                    isSettingPoint = true;
                }
            }
        }

        // 计算平行四边形的第四个点
        private Vector3 CalculateParallelogramFourthPoint(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return p0 + p2 - p1;
        }

        // 绘制小球
        private void DrawSphere(Vector3 position)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one*0.05f;
            sphere.GetComponent<MeshRenderer>().material = pointMaterial;
            m_DrawMarker.Add(sphere);
        }

        // 绘制线段
        private void DrawLine(Vector3 start, Vector3 end)
        {
            GameObject lineObj = new GameObject("Line");
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            Vector3[] lrPoint = { start, end };
            lr.startWidth = lr.endWidth = 0.05f;
            lr.material = lineMaterial;
            lr.SetPositions(lrPoint);
            m_DrawMarker.Add(lineObj);
        }

        private void DrawQuadrilateral()
        {
            // 存入字典并生成四边形
            m_QuadsDic.Add(creatQuadIndex, new List<Vector3>(m_CurrentQuadPoints));
            CreateQuad();
            creatQuadIndex++;
            ClearTemporaryPoints();
        }

        public void ClearTemporaryPoints()
        {
            isSettingPoint = false;
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
            if (m_QuadsDic[creatQuadIndex].Count != 4)
            {
                for (int i = 0; i < m_QuadsDic[creatQuadIndex].Count; i++)
                {
                    Debug.Log(m_QuadsDic[creatQuadIndex][i]);
                }
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
    }
}