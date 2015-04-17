using UnityEngine;
using System.Collections;

public class CharacterControllerAuto : MonoBehaviour {
	
	public float maxSpeed; //multiplicateur de vitesse
	public bool facingRight=true;
	Animator anim;
	
	
	bool grounded = false;//est-il au sol?
	bool wall = false; // vérification du mur
	public Transform groundCheck; //composant pour vérifier le sol
	public Transform playerCheckOne;
	public Transform playerCheckTwo;//composant pour vérifier la position du joueur
	public float groundRadius = 0.2f; //à quelle distance on vérifie le sol
	public float playerRadius = 0.5f;//vérification du joueur
	public float hazardRadius = 0.1f;
	public LayerMask whatIsGroundAndWall; //masque "Qu'est-ce que le sol?"

	bool alreadyFlip=false;
	bool alreadyJump=false;
	bool alreadySlide=false;
	bool jumpSignal=false;
	float offsetY=0f;
	float offsetX=0f;

	public float jumpForce;

	bool dead=false;
	bool end = false;
	[HideInInspector] public float move=0f;
	private bool canIMove;

	private Vector2 playerOrigin;

	CircleCollider2D circleCol;
	BoxCollider2D boxCol;


	
	void Start ()
	{

		playerOrigin=gameObject.transform.position;
		anim=GetComponent<Animator>();//on récupère l'animator de l'objet

		circleCol = GetComponent<CircleCollider2D>();
		boxCol = GetComponent<BoxCollider2D>();

	}
		
	void FixedUpdate () 
	{
		if (dead || end)
			return;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGroundAndWall); 		
		//wall=Physics2D.OverlapCircle(playerCheck.position, playerRadius, whatIsGroundAndWall);
		wall=Physics2D.OverlapArea(playerCheckOne.position,playerCheckTwo.position,whatIsGroundAndWall);


		if (wall && !alreadyFlip)
			Flip ();

		anim.SetBool("Ground", grounded); //envoyer l'info à l'animator

		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);//vitesse verticale envoyer à l'animator

		anim.SetFloat("Speed", Mathf.Abs(move)); //on met la variable speed de l'animator à la valeur absolu du mouvement
		
		rigidbody2D.velocity = new Vector2 (maxSpeed * move, rigidbody2D.velocity.y);

	}
	
	void Update()
	{
		if (canIMove && move == 0f)
			move=1f;

		if (grounded && !canIMove)
		{
			//pour qu'il suive la plateforme en idle (utile pour le système de pause)
			Collider2D SolTemp = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGroundAndWall);
			if (SolTemp !=null)
			{
				if (offsetX==0)
					offsetX=transform.position.x-SolTemp.transform.position.x;

				offsetY=transform.position.y-SolTemp.transform.position.y;

				if(SolTemp.gameObject.CompareTag("PlatformSprite"))
				{
					transform.position=new Vector3(SolTemp.transform.position.x+offsetX,SolTemp.transform.position.y+offsetY, transform.position.z);	
				}
			}
		}
	}

	public void Jump()
	{
		if (grounded && !alreadyJump)
		{
			anim.SetBool("Ground", false);
			rigidbody2D.AddForce(new Vector2 (0, jumpForce));
			alreadyJump=true;
			StartCoroutine(waitForJump());
		}		
	}
	
	public void Flip()
	{
		facingRight = !facingRight;
		//retourner selon l'axe des x (utiliser l'échelle)
		Vector3 theScale = transform.localScale;
		theScale.x *=-1;
		transform.localScale = theScale;
		//courir dans l'autre sens
		move *=-1;
		alreadyFlip=true;
		StartCoroutine(waitForFlip());
	}

	public void ReturnToPosition()
	{
		if (!facingRight) //Regarde toujours vers la droite au début
			Flip ();
		gameObject.transform.position=playerOrigin;
		canIMove=false;
		alreadyJump=false;
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
		if (!end){
			anim.SetFloat("Speed", 0f);
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x/2, rigidbody2D.velocity.y);
		}
		end=true;
	}

	//type== 0 pour une mort normale, 1 pour une noyade
	public void Dead(int type)
	{
		resetAnim();
		if (!dead)
			anim.SetTrigger ("isDeadTrigger");
		dead=true;
		rigidbody2D.velocity = new Vector2 (0f, rigidbody2D.velocity.y);
		if (type !=1)
		{
			rigidbody2D.isKinematic=true;
			circleCol.enabled=false;
			boxCol.enabled=false;
		}
	}

	void Reanim()
	{
		dead=false;
		anim.SetTrigger ("isAliveTrigger");
	}

	IEnumerator waitForFlip()
	{
		yield return new WaitForSeconds(0.1f);
		alreadyFlip=false;
	}
	
	IEnumerator waitForJump()
	{
		yield return new WaitForSeconds(0.1f);
		alreadyJump=false;
	}

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
		move=0f;
		if(alreadySlide)
			Debout ();
	}

	public void GoWait()
	{
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
		anim.SetTrigger("endSlideTrigger");
		circleCol.radius=0.7f;
		playerCheckOne.transform.localPosition=new Vector2(-0.68f, 1.94f);
		playerCheckTwo.transform.localPosition=new Vector2(0.79f, -1.2f);
		boxCol.center = new Vector2 (boxCol.center.x, 0.43f);
		boxCol.size = new Vector2 (1.4f,3.15f);
		alreadySlide=false;
	}
	
}
