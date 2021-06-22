using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Unit : NetworkBehaviour
{
    public float movementSpeed = 2f;
    public float timeToNextNode = .3f;
    public float moveTimer = 0f;
    public List<Node> path;
    public GameObject target;
    Grid gridObject;
    Node[,] grid;
    
    Vector3 targetPos;

    Pathfinding pathfinder;
    float nodeDiameter;

    public void Init(ref GameObject pathGameObject, ref GameObject baseTarget)
    {
        pathfinder = pathGameObject.GetComponent<Pathfinding>();
        target = baseTarget;
        Debug.Log(pathfinder);
        //this.pathfinder = pathfinder;
        gridObject = pathfinder.gameObject.GetComponent<Grid>();
        grid = gridObject.grid;
        nodeDiameter = gridObject.nodeDiameter;
        path = pathfinder.FindPath(this.transform.position, target.transform.position);
        Debug.Log(path);
        path.Reverse();
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				if (path != null)
					if (path.Contains(n))
						Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}  
    }

    private void Update() 
    {

            float dt = Time.deltaTime;
            Debug.Log(path);
            Debug.Log(pathfinder);
            Debug.Log(target);
            if (moveTimer >= timeToNextNode && path.Count > 0)
            {
                //MoveUnit(this.transform.position, path[path.Count - 1]);
                targetPos = path[path.Count - 1].worldPosition;
                path.RemoveAt(path.Count - 1);
                moveTimer = 0f;
            }
            else
            {
                if (this.transform.position != targetPos)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, targetPos, movementSpeed * dt);
                }
            }
            moveTimer += dt;
        
    }

    void MoveUnit(Vector3 currentPos, Node targetPos)
    {
        this.transform.position = Vector3.Lerp(currentPos, targetPos.worldPosition, movementSpeed);
    }


}
