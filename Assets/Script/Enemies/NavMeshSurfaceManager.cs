using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceManager : MonoBehaviour
{
    public List<NavMeshSurface> surfaces;

    public float updateCountdown;

    /// <summary>
    /// This script will automatically update the NavMesh once on Start to ensure that it is up to date.
    /// </summary>
    void Start()
    {
        UpdateNavSurface();
    }

    void Update()
    {
        FindBarriers();
    }

    public void UpdateNavSurface()
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            Debug.Log("NavMesh updated.");
            surfaces[i].BuildNavMesh();
        }
    }

    private void FindBarriers()
    {
        GameObject[] barriers;
        barriers = GameObject.FindGameObjectsWithTag("UnlockBarrier");

        for (int i = 0; i < barriers.Length - 1; i++)
        {
            WorldArea area = barriers[i].GetComponentInParent<WorldArea>();
            if (area.isUnlocked && !area.navUpdated)
            {
                UpdateNavSurface();
                area.navUpdated = true;
            }
        }
    }
}
