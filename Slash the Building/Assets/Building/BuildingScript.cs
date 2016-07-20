using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingScript : MonoBehaviour
{
	public Rigidbody2D rigidBody = null;
	public AudioSource audioSource = null;
	public AudioClip defenseSound = null;
	public AudioClip breakSound = null;
	public AudioClip[] slashSounds;
	public Rigidbody2D playerBody = null;
	public float tingSpeed = 10.0f;
	public float playerBouncePower = 100;

	bool attackChecker = false;

	public Animator slashAnim;

	Queue<GameObject> blockQueue;
	int currentMaxHeight = 0;
	BoxCollider2D collider = null;
	int hp = 0;

	void Start()
	{
		hp = Random.Range(1, 3 + 1);
		collider = gameObject.GetComponent<BoxCollider2D>();
		Init(5,5);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Sword_Defense")
		{
			Debug.Log("방어!");
			audioSource.PlayOneShot(defenseSound, 1);
			rigidBody.velocity = new Vector2(0, tingSpeed);
			Debug.Log(rigidBody.velocity);
			playerBody.AddForce(new Vector2(0, -1 * playerBouncePower));
		}
	}
	
	void OnTriggerStay2D(Collider2D coll)
	{
		if (attackChecker)
		{
			if (coll.gameObject.name == "Sword_Attack")
			{
				audioSource.PlayOneShot(slashSounds[Random.Range(0, 3)]);
				slashAnim.SetTrigger("slash");
				Debug.Log("썰기!");
				--hp;
				if (hp <= 0)
					Slashed();
				attackChecker = false;
			}
		}
	}

	public void TryAttack()
	{
		attackChecker = true;
	}
	public void StopAttack()
	{
		attackChecker = false;
	}

	void Init(int height, float mass, float gravityScale = 3)
	{
		currentMaxHeight = height;
		// 블록 생성하는 부분
		blockQueue = new Queue<GameObject>();
		for (int i = 0; i < height; ++i)
		{
			int spriteNum = 0;
			if (i == 0) spriteNum = 4;
			else if (i == height - 1) spriteNum = 0;
			else spriteNum = Random.Range(1, 4);

			GameObject newObj = new GameObject();
			newObj.AddComponent<SpriteRenderer>();
			newObj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprite/Building")[spriteNum];
			newObj.transform.SetParent(transform);
			newObj.transform.localPosition = new Vector3(0, 120 * i, 0);

			blockQueue.Enqueue(newObj);
		}

		// 하늘위로 올려놓는 부분
		gameObject.transform.position = new Vector3(0, 1100, 0);
		rigidBody.mass = mass;
		// 충돌박스 초기화
		collider.size = new Vector2(480, 120 * height);
		collider.offset = new Vector2(0, 60 * height);
	}
	void Slashed()
	{
		audioSource.PlayOneShot(breakSound);

		if (blockQueue.Count <= 0)
			return;

		// 한칸씩 잘리는 경우. 충돌박스를 수정해준다.
		var slashedObj = blockQueue.Dequeue();
		DestroyObject(slashedObj);
		collider.size = collider.size - new Vector2(0, 120);
		collider.offset = collider.offset + new Vector2(0, 60);
		hp = Random.Range(1, 3  + 1);

		if (blockQueue.Count <= 0)
			Init(currentMaxHeight + 1, rigidBody.mass + 2.0f);
	}
}