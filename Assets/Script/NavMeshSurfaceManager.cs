using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceManager : MonoBehaviour
{
    public List<NavMeshSurface> surfaces;

    void Start()
    {
        UpdateNavSurface();
    }

    void Update()
    {
        
    }

    public void UpdateNavSurface()
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}
