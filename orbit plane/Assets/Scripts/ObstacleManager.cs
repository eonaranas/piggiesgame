using UnityEngine;
using System.Collections;

public class ObstacleManager : MonoBehaviour {
	
	public GameObject obstacles;
	public int obstacleCount;
	
	// Use this for initialization
	void Start () {
		
		StartCoroutine(InstantiateObstacles());	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator InstantiateObstacles()
	{
		int count = 0;
		while(count < obstacleCount)
		{
			Instantiate(obstacles, Vector3.zero, Quaternion.identity);
			
			count++;
			yield return 0;
		}
	}
}
