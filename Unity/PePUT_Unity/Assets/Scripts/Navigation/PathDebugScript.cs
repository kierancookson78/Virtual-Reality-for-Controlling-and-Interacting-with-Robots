using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathDebugScript : MonoBehaviour
{
    public LineRenderer renderer;
    public NavMeshAgent agent;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (playerController.navigationOption != NavigationOption.MouseClick)
            {
                renderer.enabled = false;
            }
            else
            {
                RenderLine();
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("There is no playerController.navigationOption.");
            if (!agent.hasPath)
            {
                renderer.enabled = false;
            }
            else
            {
                RenderLine();
            }
        }   
        
    }

    private void RenderLine()
    {
        renderer.positionCount = agent.path.corners.Length;
        renderer.SetPositions(agent.path.corners);
        renderer.enabled = true;
    }
}
