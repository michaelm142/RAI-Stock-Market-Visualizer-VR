using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphLineMesh : MonoBehaviour
{
    public string fileName;

    private MeshFilter filter;
    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        List<float> fileData = new List<float>();
        using (StreamReader file = new StreamReader(new FileStream(fileName, FileMode.Open)))
        {
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                line = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                float value = Convert.ToSingle(line);
                fileData.Add(value);
            }
        }

        Texture2D texture = new Texture2D(fileData.Count, 1, TextureFormat.RFloat, false);
        for (int i = 0; i < fileData.Count; i++)
        {
            texture.SetPixel(i, 0, new Color(fileData[i] / 255.0f, 0, 0, 0));
        }
        texture.Apply();

        Vector3[] vertecies = new Vector3[8]
        {
            Vector3.right + Vector3.up + Vector3.forward,
            Vector3.left + Vector3.up + Vector3.forward,
            Vector3.left + Vector3.down + Vector3.forward,
            Vector3.right + Vector3.down + Vector3.forward,
            Vector3.right + Vector3.down + Vector3.back,
            Vector3.right + Vector3.up + Vector3.back,
            Vector3.left + Vector3.up + Vector3.back,
            Vector3.left + Vector3.down + Vector3.back
        };
        mesh.vertices = vertecies;
        mesh.uv = new Vector2[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right, Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
        mesh.triangles = new int[] { 0,1,2, 0,2,3,  0,3,4, 0,4,5,  0,5,6, 0,6,1,
                                     1,6,7, 1,7,2,  7,4,3, 7,3,2,  4,7,6, 4,6,5 };
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        filter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
