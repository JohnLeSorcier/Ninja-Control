using UnityEngine;
using System.Collections;

public class CharacterControllerAuto : MonoBehaviour {
	
	public float maxSpeed; //multiplicateur de vitesse
	public bool facingRight=true;
	bool facingRightInit;
	Animator anim;
	
	
	bool grounded = false;//est-il au sol?
	bool wall = false; // vérification du mur
	bool knocked;
	public Transform groundCheck; //composant pour vérifier le sol
	public Transform playerCheckOne;
	public Transform playerCheckTwo;//composant pour vérifier la position du joueur
	public Transform playerCheckThree;
	public float groundRadius = 0.2f; //à quelle distance on vérifie le sol
	public float playerRadius = 0.5f;//vérification du joueur
	public float hazardRadius = 0.1f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;
	
	float circleRadius;
	Vector2 circleCenter;
	Vector3 playerCheckOneOrigin;
	Vector3 playerCheckTwoOrigin;
	Vector3 playerCheckThreeOrigin;
	Vector2 boxColCenter;
	Vector2 boxColSize;
	
	CircleCollider2D circleOrigin;

	bool alreadyFlip=false;
	bool wasSliding=false;
	//bool alreadyJump=false;
	bool alreadySlide=false;
	int nbSlide=0;
	float offsetY=0f;
	float offsetX=0f;
	
	float enregMove;

	public float jumpForce;

	bool dead=false;
	bool end = false;
	[HideInInspector] public float move=0f;
	private bool canIMove;

	private Vector3 playerOrigin;
	
	public CircleCollider2D touchCircle;

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
				
		circleRadius=circleCol.radius;
		circleCenter=circleCol.center;
		playerCheckOneOrigin=playerCheckOne.localPosition;
		playerCheckTwoOrigin=playerCheckTwo.localPosition;
		playerCheckThreeOrigin=playerCheckThree.localPosition;
		boxColCenter=boxCol.center;
		boxColSize=boxCol.size;

	}
		
	void FixedUpdate () 
	{
		if (dead || end)
			return;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); 		
		wall=Physics2D.OverlapArea(playerCheckOne.position,playerCheckTwo.position,whatIsWall);
		knocked=(wasSliding && Physics2D.OverlapArea(playerCheckOne.position,playerCheckThree.position,whatIsWall));
		
		
		if (knocked)
		{
			Assome();
			return;
		}
			

		if (wall && !alreadyFlip)
			Flip ();

		anim.SetBool("Ground", grounded); //envoyer l'info à l'animator

		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);//vitesse verticale envoyer à l'animator

		anim.SetFloat("Speed", Mathf.Abs(move)); //on met la variable speed de l'animator à la valeur absolu du mouvement

		if (canIMove)
			rigidbody2D.velocity = new Vector2 (maxSpeed * move, rigidbody2D.velocity.y);

	}


	void Update()
	{
		if (canIMove && move == 0f && !dead)
		{
			if (facingRight)
				move=1f;
			else
				move=-1f;
		}

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
		if (canIMove && !dead)
		{
			if (alreadySlide)
				Debout();
			
			rigidbody2D.velocity=new Vector2(rigidbody2D.velocity.x,0);
			anim.SetBool("Ground", false);
			rigidbody2D.AddForce(new Vector2 (0, jumpForce));
		}
	
	}
	
	public void Flip()
	{
		if (canIMove && !dead)
		{
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *=-1;
			transform.localScale = theScale;
			move *=-1;
			alreadyFlip=true;
			StartCoroutine(waitForFlip());
		}
	}

	public void ReturnToPosition()
	{
		move=0f;
		if (dead)
			Reanim ();
		if (facingRight != facingRightInit)
		{
			canIMove=true; //pour éviter un bug avec le timer
			Flip ();
		}
		canIMove=false;
		wasSliding=false;
		rigidbody2D.isKinematic=false;
		rigidbody2D.velocity = new Vector2 (0, 0);
		circleCol.radius=circleRadius;
		circleCol.center=circleCenter;
		boxCol.enabled=true;
		boxCol.size=boxColSize;
		boxCol.center=boxColCenter;		
		playerCheckOne.localPosition=playerCheckOneOrigin;
		playerCheckTwo.localPosition=playerCheckTwoOrigin;
		playerCheckThree.localPosition=playerCheckThreeOrigin;		
		gameObject.transform.position=playerOrigin;
		if(alreadySlide)
			Debout ();
		end=false;
		touchCircle.enabled=true;
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
			boxCol.enabled=false;
			circleCol.radius=0.0f;
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
		if(alreadySlide)
			Debout ();
		canIMove=false;
		rigidbody2D.velocity = new Vector2 (0f, 0f);
		enregMove=move;
		move=0f;
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
			circleCol.center=new Vector2 (boxCol.center.x, -1.35f);
			circleCol.radius=0.1f;
			boxCol.center = new Vector2 (boxCol.center.x, -0.35f);
			boxCol.size = new Vector2 (2.7f,2.2f);
			playerCheckOne.transform.localPosition=new Vector2(-1.33f, 0.76f);
			playerCheckTwo.transform.localPosition=new Vector2(1.67f, -1.2f);
			playerCheckThree.transform.localPosition=new Vector2(0.6f, 0.34f);
			alreadySlide=true;
			wasSliding=true;
		}
		else
		{
			//continuer le slide plus longtemps
			nbSlide++;
		}
		StartCoroutine(WaitForDebout());

	}

	IEnumerator WaitForDebout()
	{
		int nbSlideInit;
		nbSlideInit=nbSlide;
		yield return new WaitForSeconds(0.7f);
		if (alreadySlide && nbSlideInit==nbSlide)
		{
			Debout();
		}
	}
	

	void Debout()
	{
		anim.SetBool("endSlide", true);
		circleCol.radius=circleRadius;
		circleCol.center=circleCenter;
		boxCol.size=boxColSize;
		boxCol.center=boxColCenter;		
		nbSlide=0;
		alreadySlide=false;
		StartCoroutine(WaitForCheck());
		StartCoroutine(WaitForWasSlide());
	}
	
	IEnumerator WaitForWasSlide()
	{
		yield return new WaitForSeconds(0.2f);
		wasSliding=false;
	}
	
	IEnumerator WaitForCheck()
	{
		yield return new WaitForSeconds(0.1f);
		anim.SetBool("endSlide", false);
		playerCheckOne.localPosition=playerCheckOneOrigin;
		playerCheckTwo.localPosition=playerCheckTwoOrigin;
		playerCheckThree.localPosition=playerCheckThreeOrigin;		
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
		{
			levelController.ChangeMove ();
			touchCircle.enabled=false;
		}
	}
	
}
