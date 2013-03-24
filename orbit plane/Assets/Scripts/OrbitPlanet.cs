using UnityEngine;

using System.Collections;

 

public class OrbitPlanet : MonoBehaviour {

    public float rotationSpeed = 120.0f;

    public float translationSpeed = 10.0f;  

    public float height = 2.0f;             //height from ground level 

    private Transform centre;               //transform for planet

    private float radius;                   //calculated radius from collider

    public SphereCollider planet;           //collider for planet

 	public Vector3 camPos;

    void Start () {

        //consider scale applied to planet transform (assuming uniform, just pick one)

        radius = planet.radius * planet.transform.localScale.y;

        centre = planet.transform;

        //starting position at north pole

        transform.position = centre.position + new Vector3(0,radius+height,0);
		
		Vector3 newPos = this.transform.position;
		newPos.x = Random.Range(-100.0f, 100.0f);
		newPos.z = Random.Range(-100.0f, 100.0f);
		newPos.y = radius+height;
		transform.position = newPos;

    }

    
	public float inputMag  = 0.0f;
	public float headingDeltaAngle = 0.0f;
	public float dampenForwardSpeed = 1.0f;
	public float dampenHorizontalSpeed = 10.0f;
	
	
    void Update () {

        //translate based on input      

       
		

        transform.position += transform.forward * inputMag;

        //snap position to radius + height (could also use raycasts)

        Vector3 targetPosition = transform.position - centre.position;

        float ratio = (radius + height) / targetPosition.magnitude;

        targetPosition.Scale( new Vector3(ratio, ratio, ratio) );

        transform.position = targetPosition + centre.position;

        //calculate planet surface normal                       

        Vector3 surfaceNormal = transform.position - centre.position;

        surfaceNormal.Normalize();

        //GameObject's heading

        Quaternion headingDelta = Quaternion.AngleAxis(headingDeltaAngle, transform.up);

        //align with surface normal

        transform.rotation = Quaternion.FromToRotation( transform.up, surfaceNormal) * transform.rotation;

        //apply heading rotation

        transform.rotation = headingDelta * transform.rotation;

    }
	
	
}