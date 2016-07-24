﻿using UnityEngine;
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
	public AudioClip landingSound;
	public Animator floorEffectAnimator;

	Animator anim;



	public void JumpPhysically()
	{
		rigidBody.AddForce(new Vector2(0, jumpPower));
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
			isInAir = false;
			anim.SetBool("isOnFloor", true);
			gameObject.layer = 8;
			floorEffectAnimator.SetTrigger("FloorEffect");
			audioSource.PlayOneShot(landingSound);
		}
	}

	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Floor")
		{
			isInAir = true;
			anim.SetBool("isOnFloor", false);
			gameObject.layer = 9;
			floorEffectAnimator.SetTrigger("FloorEffect");
		}
	}
}