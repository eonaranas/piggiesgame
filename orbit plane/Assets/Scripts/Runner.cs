using UnityEngine;
using System.Collections;

public class Runner : MonoBehaviour {
	
	public static Runner Instance = null;
	
	public enum AnimState { RUNNING = 0, HURT };
	public AnimState animState = AnimState.RUNNING;
	
	public AnimationClip[] animClip;
	
	public OrbitPlanet myOrbit;
	
	void OnEnable()
	{
		if(Instance == null)	Instance = this;
	}
	
	void OnDisable()
	{
		if(Instance == this)	Instance = null;
	}
	
	// Use this for initialization
	void Start () {
	
		myOrbit = this.GetComponent<OrbitPlanet>();
		PlayMe();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(animState == AnimState.RUNNING)
			myOrbit.inputMag = myOrbit.dampenForwardSpeed*myOrbit.translationSpeed*Time.deltaTime;
	}
	
	void OnTriggerEnter(Collider col)
	{
		Obstacle obs = col.GetComponent<Obstacle>();
		if(obs != null)
		{
			GotHurt();
		}
	}
	
	void PlayMe()
	{
		animState = AnimState.RUNNING;
		animation.clip = animClip[(int)animState];
		animation.Play();
	}
	
	void GotHurt()
	{
		animState = AnimState.HURT;
		animation.Stop();
		animation.clip = animClip[(int)animState];
		animation.Play();
		StartCoroutine("GetBackToRunning");
	}
	
	IEnumerator GetBackToRunning()
	{
		OnReleaseUp();
		yield return new WaitForSeconds(animClip[(int)animState].length);
		
		PlayMe();
	}
	
	void Idle()
	{
		if(!animation.isPlaying)
		{
			animState = AnimState.RUNNING;
			animation.Stop();
			animation.clip = animClip[(int)animState];
		}
	}
	
	void OnPressUp()
	{
		
	}
	
	void OnReleaseUp()
	{
		StopCoroutine("DampenForwardRelease");
		StartCoroutine("DampenForwardRelease");
	}
	
	IEnumerator DampenForwardRelease()
	{
		while(myOrbit.inputMag > 0.0f)
		{
			myOrbit.inputMag -= (myOrbit.dampenForwardSpeed / 6.0f) * Time.deltaTime;
			yield return 0;
		}
		myOrbit.inputMag = 0.0f;
	}
	
	void OnPressRight()
	{
		myOrbit.headingDeltaAngle = myOrbit.dampenHorizontalSpeed*myOrbit.translationSpeed*Time.deltaTime;
	}
	
	void OnPressLeft()
	{
		myOrbit.headingDeltaAngle = -1.0f * myOrbit.dampenHorizontalSpeed*myOrbit.translationSpeed*Time.deltaTime;
	}
	
	void OnHorizontalUp()
	{
		StopCoroutine("DampenHorizontalRelease");
		StartCoroutine("DampenHorizontalRelease");
	}
	
	IEnumerator DampenHorizontalRelease()
	{
		while(myOrbit.headingDeltaAngle > 0.0f)
		{
			myOrbit.headingDeltaAngle  -= (myOrbit.dampenHorizontalSpeed / 6.0f) * Time.deltaTime;
			yield return 0;
		}
		myOrbit.headingDeltaAngle  = 0.0f;
	}
	
	
}
