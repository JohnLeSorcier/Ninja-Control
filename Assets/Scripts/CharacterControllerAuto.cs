using UnityEngine;
using System.Collections;

public class CharacterControllerAuto : MonoBehaviour {
	
	public float maxSpeed; //multiplicateur de vitesse
	public bool facingRight=true;
	Animator anim;
	
	
	bool grounded = false;//est-il au sol?
	bool wall = false; // vérification du mur
	public Transform groundCheck; //composant pour vérifier le sol
	public Transform playerCheck; //composant pour vérifier la position du joueur
	public float groundRadius = 0.2f; //à quelle distance on vérifie le sol
	public float playerRadius = 0.5f;//vérification du joueur
	public float hazardRadius = 0.1f;
	public LayerMask whatIsGroundAndWall; //masque "Qu'est-ce que le sol?"

	bool alreadyFlip=false;
	bool alreadyJump=false;
	float offsetY=0f;
	float offsetX=0f;

	public float jumpForce;

	bool dead=false;
	bool end = false;
	[HideInInspector] public float move=0f;
	private bool canIMove;

	private Vector2 playerOrigin;


	
	void Start ()
	{

		playerOrigin=gameObject.transform.position;
		anim=GetComponent<Animator>();//on récupère l'animator de l'objet

	}
		
	void FixedUpdate () 
	{
		if (dead || end)
			return;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGroundAndWall); 		
		wall=Physics2D.OverlapCircle(playerCheck.position, playerRadius, whatIsGroundAndWall);


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
	
	public void Dead()
	{
		resetAnim();
		if (!dead)
			anim.SetTrigger ("isDeadTrigger");
		dead=true;
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
		
}
