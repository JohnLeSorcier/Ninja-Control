using UnityEngine;
using System.Collections;

public class CharacterControllerAuto : MonoBehaviour {
	
	public float maxSpeed; //multiplicateur de vitesse
	public bool facingRight=true;
	bool facingRightInit;
	Animator anim;
	
	
	bool grounded = false;//est-il au sol?
	bool wall = false; // vérification du mur
	public Transform groundCheck; //composant pour vérifier le sol
	public Transform playerCheckOne;
	public Transform playerCheckTwo;//composant pour vérifier la position du joueur
	public float groundRadius = 0.2f; //à quelle distance on vérifie le sol
	public float playerRadius = 0.5f;//vérification du joueur
	public float hazardRadius = 0.1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;

	bool alreadyFlip=false;
	//bool alreadyJump=false;
	bool alreadySlide=false;
	bool jumpSignal=false;
	bool justSlide=false;
	float offsetY=0f;
	float offsetX=0f;
	
	float enregMove;

	public float jumpForce;

	bool dead=false;
	bool end = false;
	[HideInInspector] public float move=0f;
	private bool canIMove;

	private Vector3 playerOrigin;

	CircleCollider2D circleCol;
	BoxCollider2D boxCol;

	LevelController levelController;


	
	void Start ()
	{
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");

		playerOrigin=gameObject.transform.position;
		anim=GetComponent<Animator>();//on récupère l'animator de l'objet

		circleCol = GetComponent<CircleCollider2D>();
		boxCol = GetComponent<BoxCollider2D>();
		facingRightInit = facingRight;

	}
		
	void FixedUpdate () 
	{
		if (dead || end)
			return;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); 		
		wall=Physics2D.OverlapArea(playerCheckOne.position,playerCheckTwo.position,whatIsWall);

		if (wall && !alreadyFlip)
			Flip ();

		anim.SetBool("Ground", grounded); //envoyer l'info à l'animator

		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);//vitesse verticale envoyer à l'animator

		anim.SetFloat("Speed", Mathf.Abs(move)); //on met la variable speed de l'animator à la valeur absolu du mouvement

		if (canIMove)
			rigidbody2D.velocity = new Vector2 (maxSpeed * move, rigidbody2D.velocity.y);

		if (wall && justSlide)
			Assome();
	}


	void Update()
	{
		if (canIMove && move == 0f && !dead)
			move=1f;

		if (grounded && !canIMove)
		{	//pour qu'il suive la plateforme en idle (utile pour le système de pause)
			Collider2D SolTemp = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
			float OldPosition;
			if (SolTemp !=null && SolTemp.gameObject.CompareTag("PlatformSprite"))
			{
				OldPosition=SolTemp.transform.position.x;
				offsetX=SolTemp.transform.position.x-OldPosition;

				offsetY=transform.position.y-SolTemp.transform.position.y;
				transform.position=new Vector3(SolTemp.transform.position.x+offsetX,SolTemp.transform.position.y+offsetY, transform.position.z);	
			}
		}
	}

	public void Jump()
	{
		rigidbody2D.velocity=new Vector2(rigidbody2D.velocity.x,0);

		anim.SetBool("Ground", false);
		rigidbody2D.AddForce(new Vector2 (0, jumpForce));
	
	}
	
	public void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *=-1;
		transform.localScale = theScale;
		move *=-1;
		alreadyFlip=true;
		StartCoroutine(waitForFlip());
	}

	public void ReturnToPosition()
	{
		rigidbody2D.isKinematic=false;
		rigidbody2D.velocity = new Vector2 (0, 0);
		circleCol.enabled=true;
		boxCol.enabled=true;
		if (facingRight != facingRightInit)
			Flip ();
		gameObject.transform.position=playerOrigin;
		canIMove=false;
		//alreadyJump=false;
		if(alreadySlide)
			Debout ();
		end=false;
		move=0f;
		if (dead)
			Reanim ();
	}
	
	public void End()
	{
		resetAnim();
		if (!end)
		{
			anim.SetFloat("Speed", 0f);
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x/2, rigidbody2D.velocity.y);
		}
		end=true;
	}

	//type== 0 pour une mort normale, 1 pour une noyade, 2 une mort écrasée
	public void Dead(int type)
	{
		resetAnim();
		if (!dead)
			anim.SetTrigger ("isDeadTrigger");
		dead=true;
		rigidbody2D.velocity = new Vector2 (0f, 0f);
		if (type == 2)
		{
			rigidbody2D.isKinematic=true;
			circleCol.enabled=false;
			boxCol.enabled=false;
		}
	}

	void Reanim()
	{
		anim.SetBool("isAlive", true);
		dead=false;
		StartCoroutine(WaitForDeath());
	}

	IEnumerator waitForFlip()
	{
		yield return new WaitForSeconds(0.1f);
		alreadyFlip=false;
	}
	
/*	IEnumerator waitForJump()
	{
		yield return new WaitForSeconds(0.1f);
		alreadyJump=false;
	}*/

	void resetAnim()
	{
		anim.SetBool("Ground", true);		
		anim.SetFloat("vSpeed", 0f);		
		anim.SetFloat("Speed", 0f); 
	}

	public void PlayerMove()
	{
		canIMove=true;
	}

	public void StopWait()
	{
		canIMove=false;
		rigidbody2D.velocity = new Vector2 (0f, 0f);
		enregMove=move;
		move=0f;
		if(alreadySlide)
			Debout ();
	}

	public void GoWait()
	{
		move=enregMove;
		canIMove=true;
	}

	public void Slide()
	{

		if (!alreadySlide)
		{
			anim.SetTrigger("isSlideTrigger");
			circleCol.radius=0.01f;
			boxCol.center = new Vector2 (boxCol.center.x, -0.35f);
			boxCol.size = new Vector2 (2.7f,2.2f);
			playerCheckOne.transform.localPosition=new Vector2(-1.33f, 0.76f);
			playerCheckTwo.transform.localPosition=new Vector2(1.37f, -1.2f);
			alreadySlide=true;
			StartCoroutine(WaitForSlide());
		}
		else
		{
			//continuer le slide plus longtemps
			jumpSignal=true;
			StartCoroutine(WaitForSlide());
		}

	}

	IEnumerator WaitForSlide()
	{
		yield return new WaitForSeconds(0.6f);
		if (!jumpSignal)
		{
			Debout();
		}
		jumpSignal=false;
	}

	void Debout()
	{
		anim.SetBool("endSlide", true);
		circleCol.radius=0.7f;
		playerCheckOne.transform.localPosition=new Vector2(-0.68f, 1.94f);
		playerCheckTwo.transform.localPosition=new Vector2(0.79f, -1.2f);
		boxCol.center = new Vector2 (boxCol.center.x, 0.43f);
		boxCol.size = new Vector2 (1.4f,3.15f);
		alreadySlide=false;
		justSlide=true;
		StartCoroutine(WaitForVerifSlide());
	}

	IEnumerator WaitForVerifSlide()
	{
		yield return new WaitForSeconds(0.1f);
		anim.SetBool("endSlide", false);
		justSlide=false;
	}

	IEnumerator WaitForDeath()
	{
		yield return new WaitForSeconds(0.1f);
		anim.SetBool("isAlive", false);
	}

	void Assome()
	{
		levelController.GameOver(5);
	}

	void OnMouseUp()
	{
		if (!canIMove)
			levelController.ChangeMove ();
	}
	
}
