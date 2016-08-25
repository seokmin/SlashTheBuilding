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
	public ParticleSystem particleSys = null;

	bool attackChecker = false;

	public Animator slashAnim;
	public Animator defenseAnim;

	int currentMaxHeight = 0;
	BoxCollider2D myCollider = null;
	int hp = 0;

	class Block
	{
		public GameObject obj;
		public ParticleSystem particleSys;

	};
	Queue<Block> blockQueue;

	void Start()
	{
		hp = Random.Range(1, 3 + 1);
		myCollider = gameObject.GetComponent<BoxCollider2D>();
		Init(5,rigidBody.mass,rigidBody.gravityScale);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Sword_Defense")
		{
			audioSource.PlayOneShot(defenseSound, 1);
			defenseAnim.SetTrigger("defense");
			rigidBody.velocity = new Vector2(0, tingSpeed);
			playerBody.isKinematic = true;
			playerBody.isKinematic = false;
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

	void Init(int height, float mass, float gravityScale)
	{
		currentMaxHeight = height;
		// 블록 생성하는 부분
		blockQueue = new Queue<Block>();
		for (int i = 0; i < height; ++i)
		{
			Block newBlock = new Block();
			int spriteNum = 0;
			if (i == 0) spriteNum = 4;
			else if (i == height - 1) spriteNum = 0;
			else spriteNum = Random.Range(1, 4);

			// 오브젝트 셋팅
			newBlock.obj = new GameObject();
			newBlock.obj.AddComponent<SpriteRenderer>();
			newBlock.obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprite/Building")[spriteNum];
			newBlock.obj.transform.SetParent(transform);
			newBlock.obj.transform.localPosition = new Vector3(0, 120 * i, 0);

			// 파티클시스템 셋팅
			newBlock.particleSys = Instantiate<ParticleSystem>(particleSys);

			blockQueue.Enqueue(newBlock);
		}

		// 하늘위로 올려놓는 부분
		gameObject.transform.position = new Vector3(0, 1100, 0);
		rigidBody.mass = mass;
		// 충돌박스 초기화
		myCollider.size = new Vector2(480, 120 * height);
		myCollider.offset = new Vector2(0, 60 * height);
	}
	void Slashed()
	{
		if (blockQueue.Count <= 0)
			return;

		var slashedBlock = blockQueue.Dequeue();
		// 파티클 뿌리는 부분
		slashedBlock.particleSys.Play(true);
		slashedBlock.particleSys.transform.position = slashedBlock.obj.transform.position;
		// 소리
		audioSource.PlayOneShot(breakSound);


		// 한칸씩 잘리는 경우. 충돌박스를 수정해준다.
		DestroyObject(slashedBlock.obj);
		myCollider.size = myCollider.size - new Vector2(0, 120);
		myCollider.offset = myCollider.offset + new Vector2(0, 60);
		hp = Random.Range(1, 3  + 1);

		if (blockQueue.Count <= 0)
			Init(currentMaxHeight + 1, rigidBody.mass + 2.0f,rigidBody.gravityScale + 0.5f);
	}
}