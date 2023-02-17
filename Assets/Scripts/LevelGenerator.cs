using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{ 
    //Reuse object
    [SerializeField] private Transform[] levelPart;

    //set postion for reused object
    [SerializeField] private Vector3 nextPartPosition;

    //check distance for platform spawn/delete
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;

    //ref player
    [SerializeField] private Transform player;

    void Update()
    {
        DeletePlatform();

        GeneratePlatform();
    }

    private void GeneratePlatform()
    {
        //set up platforms to spawn when at certain distance from player
        while (Vector2.Distance(player.transform.position, nextPartPosition) < distanceToSpawn)
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)];

            //set start and end point generation for next platform
            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0);
            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);

            nextPartPosition = newPart.Find("EndPoint").position;
        }
    }

    private void DeletePlatform()
    {
        //delete child of level generator
        if(transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);

            //check if player is far enough for deletion
            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
                Destroy(partToDelete.gameObject);
        }
    }
}
