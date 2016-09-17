using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingScript : MonoBehaviour
{
	public Rigidbody2D rigidBody = null;
	public AudioSource audioSource = null;
	public AudioClip defenseSound = null;
	public AudioClip breakSound = null;
	public AudioClip tubureSound = null;
	public AudioClip wazaSound = null;
	public AudioClip[] slashSounds;
	public Rigidbody2D playerBody = null;
	public float tingSpeed = 10.0f;
	public float playerBouncePower = 100;
	public ParticleSystem particleSys = null;

	public GameObject wazaEffect;

	bool attackChecker = false;

	public Animator slashAnim;
	public Animator defenseAnim;
	public Animator tutiAnim;

	public UIManager uiManager;

	int currentMaxHeight = 0;
	BoxCollider2D myCollider = null;
	int hp = 0;

	int playerHp = 3;

	bool wazaIng = false;

	class Block
	{
		public GameObject obj;
		public ParticleSystem particleSys;

	};
	Queue<Block> blockQueue;
	Queue<Block> blockPool;

	void CreateBlockPool(int maxBlockCount)
	{
		blockPool = new Queue<Block>();
		for (int i = 0; i < maxBlockCount; ++i)
		{
			Block newBlock = new Block();

			// 오브젝트 셋팅
			newBlock.obj = new GameObject();
			newBlock.obj.AddComponent<SpriteRenderer>().sortingLayerName = "Building";
			newBlock.obj.transform.SetParent(transform);
			newBlock.obj.transform.localPosition = new Vector3(0, 120 * i, 0);

			// 파티클시스템 셋팅
			newBlock.particleSys = Instantiate<ParticleSystem>(particleSys);
			newBlock.particleSys.gameObject.SetActive(false);

			blockPool.Enqueue(newBlock);
			newBlock.obj.SetActive(false);
		}
	}

	void Start()
	{
		uiManager.SetScore(0);
		hp = Random.Range(1, 3 + 1);
		myCollider = gameObject.GetComponent<BoxCollider2D>();
		CreateBlockPool(100);
		blockQueue = new Queue<Block>();
		Init(5, rigidBody.mass, rigidBody.gravityScale);
		uiManager.SetHeart(3);
		uiManager.SetWazaGage(0.0f);

	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Floor")
		{
			uiManager.SetHeart(--playerHp);
			audioSource.PlayOneShot(tubureSound);
			tutiAnim.SetTrigger("Tuti");
			isTuti = true;
		}

	}

	public bool GetTuti() {return isTuti; }
	bool isTuti = false;
	void OnCollisionExit2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Floor")
		{
			isTuti = false;
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Sword_Defense" && !isTuti)
		{
			audioSource.PlayOneShot(defenseSound, 1);
			defenseAnim.SetTrigger("defense");
			rigidBody.velocity = new Vector2(0, tingSpeed);
			playerBody.isKinematic = true;
			playerBody.isKinematic = false;
			playerBody.AddForce(new Vector2(0, -1 * playerBouncePower));
			uiManager.SetDifenseGage(uiManager.GetDifenseGage() - 10.0f);
		}
	}

	void Update()
	{
		if (isTuti && attackChecker)
		{
			audioSource.PlayOneShot(slashSounds[Random.Range(0, 3)]);
			slashAnim.SetTrigger("slash");
			--hp;
			if (hp <= 0 || isTuti)
				Slashed();
			isTuti = false;
			attackChecker = false;
		}
		// 필살기
		if(Input.GetKeyDown(KeyCode.X))
		{
			if(uiManager.GetWazaGage() >= 100.0f)
			{
				StartCoroutine( Wazaaaaa());
			}
		}
	}

	IEnumerator Wazaaaaa()
	{
		uiManager.SetWazaGage(0.0f);
		audioSource.PlayOneShot(wazaSound);
		wazaEffect.SetActive(true);
		yield return new WaitForSeconds(0.65f);
		wazaIng = true;
		rigidBody.velocity = new Vector2(0, tingSpeed);
		while (wazaIng == true)
		{
			Debug.Log("와자 한번");
			Slashed();
			yield return new WaitForSeconds(0.25f);
		}
		wazaEffect.SetActive(false);
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
				if (hp <= 0 || isTuti)
					Slashed();
				isTuti = false;
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
		wazaIng = false;
		currentMaxHeight = height;
		// 블록 생성하는 부분
		for (int i = 0; i < height; ++i)
		{
			Block newBlock = blockPool.Dequeue();
			newBlock.obj.SetActive(true);
			newBlock.particleSys.gameObject.SetActive(true);

			int spriteNum = 0;
			if (i == 0) spriteNum = 4;
			else if (i == height - 1) spriteNum = 0;
			else spriteNum = Random.Range(1, 4);

			// 오브젝트 셋팅
			newBlock.obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprite/Building")[spriteNum];
			newBlock.obj.transform.SetParent(transform);
			newBlock.obj.transform.localPosition = new Vector3(0, 120 * i, 0);

			blockQueue.Enqueue(newBlock);
		}

		// 하늘위로 올려놓는 부분
		gameObject.transform.position = new Vector3(0, 1100, 0);
		rigidBody.mass = mass;
		rigidBody.gravityScale = gravityScale;
		// 충돌박스 초기화
		myCollider.size = new Vector2(480, 120 * height);
		myCollider.offset = new Vector2(0, 60 * height);
	}

	uint score = 0;
	void Slashed()
	{
		if (blockQueue.Count <= 0)
			return;

		score += 1;
		uiManager.SetScore(score);

		if (wazaIng == false)
			uiManager.SetWazaGage(Mathf.Min(100.0f,uiManager.GetWazaGage() + 17.0f));

		var slashedBlock = blockQueue.Dequeue();
		// 파티클 뿌리는 부분
		slashedBlock.particleSys.Play(true);
		slashedBlock.particleSys.transform.position = slashedBlock.obj.transform.position;
		// 소리
		audioSource.PlayOneShot(breakSound);


		// 한칸씩 잘리는 경우. 충돌박스를 수정해준다.
		slashedBlock.obj.SetActive(false);
		StartCoroutine(DisableAfterTime(slashedBlock.particleSys.gameObject, 1.2f + slashedBlock.particleSys.gameObject.transform.position.y / 500.0f));
		blockPool.Enqueue(slashedBlock);

		myCollider.size = myCollider.size - new Vector2(0, 120);
		myCollider.offset = myCollider.offset + new Vector2(0, 60);
		hp = Random.Range(1, 3 + 1);

		if (blockQueue.Count <= 0)
			Init(currentMaxHeight + 1, rigidBody.mass, rigidBody.gravityScale + 0.5f);
	}
	IEnumerator DisableAfterTime(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		obj.SetActive(false);
	}
}