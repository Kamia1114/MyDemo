using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(MeshFilter))]
public class GameManager : MonoBehaviour
{
    // 要生成的Cube预制体
    public GameObject cubePrefab;

    public ProBuilderMesh proBuilderMesh;
    
    // 网格位置参数（X和Z方向的索引）
    public int targetGridX = 2;
    public int targetGridZ = 3;
    
    // Cube的大小
    public float cubeScale = 0.8f;
    
    // 存储平面的网格数据
    private Mesh planeMesh;
    private Vector3[] vertices;
    private int xSegments;
    private int zSegments;

    void Start()
    {
        // 获取平面的网格组件
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("平面物体上没有有效的MeshFilter组件！");
            return;
        }
        
        // 存储网格数据
        planeMesh = meshFilter.sharedMesh;
        vertices = planeMesh.vertices;
        
        // 计算平面的细分数量
        CalculateSegments();
        
        // 检查目标网格位置是否有效
        if (IsValidGridPosition(targetGridX, targetGridZ))
        {
            // 在指定网格位置创建Cube
            CreateCubeAtGrid(targetGridX, targetGridZ);
        }
        else
        {
            Debug.LogError($"无效的网格位置！最大范围 X: 0-{xSegments}, Z: 0-{zSegments}");
        }
    }
    
    // 计算平面的细分数量
    private void CalculateSegments()
    {
        // 假设平面是规则网格，顶点数量为 (xSegments+1) * (zSegments+1)
        int vertexCount = vertices.Length;
        xSegments = (int)Mathf.Sqrt(vertexCount) - 1;
        zSegments = xSegments; // 假设是正方形网格
        
        // 处理非正方形网格的情况
        if ((xSegments + 1) * (zSegments + 1) != vertexCount)
        {
            // 尝试找到合适的分段比例
            for (int i = 1; i < vertexCount; i++)
            {
                if (vertexCount % i == 0)
                {
                    xSegments = i - 1;
                    zSegments = (vertexCount / i) - 1;
                    break;
                }
            }
        }
    }
    
    // 检查网格位置是否有效
    private bool IsValidGridPosition(int x, int z)
    {
        return x >= 0 && x <= xSegments && z >= 0 && z <= zSegments;
    }
    
    // 在指定网格位置创建Cube
    public void CreateCubeAtGrid(int x, int z)
    {
        if (!IsValidGridPosition(x, z)) return;
        
        // 计算对应顶点的索引
        int vertexIndex = x + z * (xSegments + 1);
        
        // 将本地顶点坐标转换为世界坐标
        Vector3 worldPosition = transform.TransformPoint(vertices[vertexIndex]);
        
        // 调整Y轴位置，使Cube底面贴合平面
        worldPosition.y += (cubeScale / 2f);
        
        // 实例化Cube
        if (cubePrefab != null)
        {
            GameObject newCube = Instantiate(cubePrefab, worldPosition, Quaternion.identity);
            newCube.transform.localScale = Vector3.one * cubeScale;
            newCube.name = $"Cube_{x}_{z}";
        }
        else
        {
            // 如果没有预制体，创建一个默认Cube
            GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCube.transform.position = worldPosition;
            newCube.transform.localScale = Vector3.one * cubeScale;
            newCube.name = $"Cube_{x}_{z}";
        }
    }
    
    // 批量创建网格上的Cube
    public void CreateCubeGrid(int startX, int endX, int startZ, int endZ)
    {
        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                CreateCubeAtGrid(x, z);
            }
        }
    }
    
    // 在编辑器中绘制网格点辅助线
    void OnDrawGizmosSelected()
    {
        if (vertices == null) return;
        
        Gizmos.color = Color.green;
        foreach (Vector3 vertex in vertices)
        {
            Vector3 worldPos = transform.TransformPoint(vertex);
            Gizmos.DrawWireSphere(worldPos, 0.1f);
        }
        
        // 高亮显示目标网格点
        if (IsValidGridPosition(targetGridX, targetGridZ))
        {
            int vertexIndex = targetGridX + targetGridZ * (xSegments + 1);
            Vector3 targetPos = transform.TransformPoint(vertices[vertexIndex]);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPos, 0.2f);
        }
    }
}
    