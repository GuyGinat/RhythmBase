using UnityEngine;
using UnityEditor;

public class MeshSaver : MonoBehaviour
{
    // Call this function to save the mesh (for example from another script)
    [MenuItem("Tools/Mesh/Save Mesh")]
    public static void SaveMesh()
    {
        GameObject go = Selection.activeObject as GameObject;
        
        // Create the directory if it doesn't exist
        if (!System.IO.Directory.Exists("Assets/Models"))
        {
            System.IO.Directory.CreateDirectory("Assets/Models");
        }

        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh meshToSave = mf.sharedMesh;        

        // if (optimizeMesh)
        // {
        //     // This optimizes the mesh for GPU access
        //     MeshUtility.Optimize(meshToSave);
        // }

        AssetDatabase.CreateAsset(meshToSave, "Assets/Models/RingMeshes/" + go.name + ".asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Mesh/Save Multiple Meshes")]
    public static void SaveMeshes()
    {
        GameObject[] gos = Selection.gameObjects;

        foreach (GameObject go in gos)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null) continue;
            Mesh meshToSave = mf.sharedMesh;
            AssetDatabase.CreateAsset(meshToSave, "Assets/Models/RingMeshesWithUVs/" + go.name + ".asset");
        }
        AssetDatabase.SaveAssets();
    }
}