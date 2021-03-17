using UnityEngine;
using System.Collections;

public static class CameraExt
{
    #region FRUSTUM GENERATION
    /**основа взята из: http://answers.unity3d.com/questions/143025/more-questions-on-generating-camera-frustrum.html
     *Расположение врешин в Frustum 
         v[5]________________v[6]
            |\              /|
            | \v[1]____v[2]/ | 
            |  |          |  |
            |  |          |  |  
            |  |__________|  |
            | /v[0]    v[3]\ |
            |/______________\|
         v[4]                v[7]     
    */

    private static int[] m_VertOrder = new int[24]
    {
        0,1,2,3, // near
        6,5,4,7, // far
        0,4,5,1, // left
        3,2,6,7, // right
        1,5,6,2, // top
        0,3,7,4  // bottom
    };       

    private static int[] m_Indices = new int[36]
    {
         0,  1,  2,  3,  0,  2, // near
         4,  5,  6,  7,  4,  6, // far
         8,  9, 10, 11,  8, 10, // left
        12, 13, 14, 15, 12, 14, // right
        16, 17, 18, 19, 16, 18, // top
        20, 21, 22, 23, 20, 22, // bottom
    }; //              |______|---> shared vertices


    /// <summary>
    /// Генерация меша для визуализации пирамиды отсечения камеры.
    /// </summary>
    //пример использования (в GO должны быть Mesh filter и Mesh renderer): GetComponent<MeshFilter>().mesh = Camera.main.GenerateFrustumMesh();
    public static Mesh GenerateFrustumMesh(this Camera cam)
    {
        Vector3[] v = new Vector3[8];
        v[0] = v[4] = new Vector3(0, 0, 0);
        v[1] = v[5] = new Vector3(0, 1, 0);
        v[2] = v[6] = new Vector3(1, 1, 0);
        v[3] = v[7] = new Vector3(1, 0, 0);
        v[0].z = v[1].z = v[2].z = v[3].z = cam.nearClipPlane;
        v[4].z = v[5].z = v[6].z = v[7].z = cam.farClipPlane;
        
        //Transform viewport --> world
        for (int i = 0; i < 8; i++)
            v[i] = cam.ViewportToWorldPoint(v[i]);


        Vector3[] vertices = new Vector3[24];
        Vector3[] normals = new Vector3[24];

        // Split vertices for each face (8 vertices --> 24 vertices)
        for (int i = 0; i < 24; i++)
            vertices[i] = v[m_VertOrder[i]];
        
        // Calculate facenormal
        for (int i = 0; i < 6; i++)
        {
            Vector3 faceNormal = Vector3.Cross(vertices[i * 4 + 2] - vertices[i * 4 + 1], vertices[i * 4 + 0] - vertices[i * 4 + 1]);
            normals[i * 4 + 0] = normals[i * 4 + 1] = normals[i * 4 + 2] = normals[i * 4 + 3] = faceNormal;
        } 
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = m_Indices;
        return mesh;
    }

    /// <summary>
    /// Генерация меша для визуализации пирамиды видимости выбранной на экране прямоугольной области
    /// </summary>
    public static Mesh GenerateFrustumMesh(this Camera cam, float x1, float y1, float x2, float y2)
    {
        Vector3[] vScreen = new Vector3[4];
        vScreen[0] = cam.ScreenToViewportPoint(new Vector3(x1, y1));
        vScreen[1] = cam.ScreenToViewportPoint(new Vector3(x1, y2));
        vScreen[2] = cam.ScreenToViewportPoint(new Vector3(x2, y2));
        vScreen[3] = cam.ScreenToViewportPoint(new Vector3(x2, y1));

        Vector3[] v = new Vector3[8];
        for (int i = 0; i < 4; i++)
            v[i] = v[i + 4] = vScreen[i];
        v[0].z = v[1].z = v[2].z = v[3].z = cam.nearClipPlane;
        v[4].z = v[5].z = v[6].z = v[7].z = cam.farClipPlane;

        // Transform viewport --> world
        for (int i = 0; i < 8; i++)
            v[i] = cam.ViewportToWorldPoint(v[i]);


        Vector3[] vertices = new Vector3[24];
        Vector3[] normals = new Vector3[24];

        // Split vertices for each face (8 vertices --> 24 vertices)
        for (int i = 0; i < 24; i++)
            vertices[i] = v[m_VertOrder[i]];

        // Calculate facenormal
        for (int i = 0; i < 6; i++)
        {
            Vector3 faceNormal = Vector3.Cross(vertices[i * 4 + 2] - vertices[i * 4 + 1], vertices[i * 4 + 0] - vertices[i * 4 + 1]);
            normals[i * 4 + 0] = normals[i * 4 + 1] = normals[i * 4 + 2] = normals[i * 4 + 3] = faceNormal;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = m_Indices;
        return mesh;
    }

    //Выдается ошибка "!IsNormalized" (только в Debug режиме), если выделить слишком маленькую область. Как ее пофиксить написано в коде функции.
    /// <summary>
    /// Генерация 6 плоскостей пирамиды отсечения для выбранной в камере прямоугольной области.
    /// Полученные плоскости можно использовать в методе GeometryUtility.TestPlanesAABB(). 
    /// </summary>
    public static Plane[] GenerateFrustumPlanes(this Camera cam, float x1, float y1, float x2, float y2)
    {
        DoCorrectCoordinates(ref x1, ref y1, ref x2, ref y2);

        Vector3[] vScreen = new Vector3[4];
        vScreen[0] = cam.ScreenToViewportPoint(new Vector3(x1, y1));
        vScreen[1] = cam.ScreenToViewportPoint(new Vector3(x1, y2));
        vScreen[2] = cam.ScreenToViewportPoint(new Vector3(x2, y2));
        vScreen[3] = cam.ScreenToViewportPoint(new Vector3(x2, y1));

        Vector3[] v = new Vector3[8];
        for (int i = 0; i < 4; i++)
            v[i] =v[i+4]= vScreen[i];
        v[0].z = v[1].z = v[2].z = v[3].z = cam.nearClipPlane;
        v[4].z = v[5].z = v[6].z = v[7].z = cam.farClipPlane;

        // Transform viewport --> world
        for (int i = 0; i < 8; i++)
            v[i] = cam.ViewportToWorldPoint(v[i]) *-1.0f;

        Plane[] planes = new Plane[6];
        planes[0] = new Plane(v[0], v[1], v[2]);//near
        planes[1] = new Plane(v[6], v[5], v[4]);//far
        planes[2] = new Plane(v[0], v[4], v[5]);//left
        planes[3] = new Plane(v[3], v[2], v[6]);//right
        planes[4] = new Plane(v[1], v[5], v[6]);//top
        planes[5] = new Plane(v[0], v[3], v[7]);//bottom

        for (int i = 0; i < 6; i++)
            planes[i].normal *= -1.0f;

        #region отображение Frustum в редакторе

        #if UNITY_EDITOR
        //for (int i = 0; i < 8; i++)
        //    v[i] *= -1f;
        //for (int i = 0; i < 4; i++)
        //{
        //    int j = (i != 3) ? i : -1;
        //    Debug.DrawLine(v[m_VertOrder[i]], v[m_VertOrder[4 + 2 - j]], Color.blue, 10.0f);
        //    Debug.DrawLine(v[m_VertOrder[i]], v[m_VertOrder[j + 1]], Color.blue, 10.0f);
        //    Debug.DrawLine(v[m_VertOrder[i + 4]], v[m_VertOrder[j + 5]], Color.blue, 10.0f);
        //}
        #endif

        #endregion

        return planes;
    }

    #endregion

    private static void DoCorrectCoordinates(ref float x1, ref float y1, ref float x2, ref float y2)
    {
        //ели координаты равны, то увеличиваем размер области, чтобы пирамида генерировалась корректно
        if (x1 == x2)
            x2++;
        if (y1 == y2)
            y2++;

        //устанавливаем x1,y1 как минимумы, а  x2,y2 как максимумы
        if (x1 > x2)
        {
            float temp = x1;
            x1 = x2;
            x2 = temp;
        }
        if (y1 > y2)
        {
            float temp = y1;
            y1 = y2;
            y2 = temp;
        }
    }
}

