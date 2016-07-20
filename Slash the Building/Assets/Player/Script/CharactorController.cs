using UnityEngine;
using System.Collections;



public class CharactorController : MonoBehaviour
{
	public Rigidbody2D rigidBody = null;
	public BoxCollider2D collider_Defense = null;
	public float jumpPower = 2500;
	public BuildingScript building;

	private bool isInAir = false;
	public AudioSource audioSource = null;
	public AudioClip jumpSound;
	Animator anim;



	IEnumerator ExecuteAfterTime(float time)
	{
		yield return new WaitForSeconds(time);

		rigidBody.AddForce(new Vector2(0, jumpPower));
		// Code to execute after the delay
	}
	void Start()
	{
		anim = GetComponent<Animator>();
	}
	void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow) && isInAir == false)
		{
			//0.2초 딜레이
			anim.SetTrigger("Jump");
			audioSource.PlayOneShot(jumpSound, 1);
			StartCoroutine("ExecuteAfterTime", 0.15f);
			isInAir = true;
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			anim.SetTrigger("Attack");
			building.TryAttack();
		}
		if(Input.GetKeyUp(KeyCode.Z))
		{
			building.StopAttack();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			anim.SetBool("isDefense", true);
			collider_Defense.enabled = true;
		}
		if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			anim.SetBool("isDefense", false);
			collider_Defense.enabled = false;
		}

	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Floor")
		{
			Debug.Log("바닥에 닿았다.");
			isInAir = false;
			anim.SetBool("isOnFloor", true);
			gameObject.layer = 8;

		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Floor")
		{
			Debug.Log("바닥에서 떨어졌다.");
			isInAir = true;
			anim.SetBool("isOnFloor", false);
			gameObject.layer = 9;
		}
	}
}