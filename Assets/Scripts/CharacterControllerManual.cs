using UnityEngine;
using System.Collections;

public class CharacterControllerManual : MonoBehaviour {
	
	public float maxSpeed; //multiplicateur de vitesse
	bool facingRight=true;
	Animator anim;


	bool grounded = false;//est-il au sol?
	bool wall = false; // vérification du saut sur un mur
	public Transform groundCheck; //composant pour vérifier le sol
	public Transform playerCheck; //composant pour vérifier la position du joueur
	float groundRadius = 0.2f; //à quelle distance on vérifie le sol
	float playerRadius= 0.5f;//vérification du joueur
	public LayerMask whatIsGround; //masque "Qu'est-ce que le sol?"
	public LayerMask whatIsHazard; //masque "Qu'est ce qui est dangereux?"
	public LayerMask whatIsEnd; //masque "Qu'est ce qui est la fin?"
	public float jumpForce;
	bool alreadyJump=false;
	public float waitForWallJump;
	bool hazarded = false;
	bool dead=false;
	bool end = false;
	private LevelController levelController;

	void Start ()
	{
		//récupération du gamecontroller
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
		{
			levelController = levelControllerObject.GetComponent <LevelController>();
		}
		if (levelController == null)
		{
			Debug.Log ("Cannot find 'LevelController' script");
		}

		anim=GetComponent<Animator>();//on récupère l'animator de l'objet
	}
	

	void FixedUpdate () 
	{
		if (dead || end)
			return;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround); //vérification du sol (Vector2 de position, rayon de vérification, masque)
		anim.SetBool("Ground", grounded); //envoyer l'info à l'animator

		hazarded = Physics2D.OverlapCircle(groundCheck.position, 0.0f, whatIsHazard);

		if (!grounded)
			wall=Physics2D.OverlapCircle(playerCheck.position, playerRadius, whatIsGround);
								
		anim.SetFloat("vSpeed", rigidbody2D.velocity.y);//vitesse verticale envoyer à l'animator

		float move = Input.GetAxis("Horizontal"); //quantité de mouvement donné sur l'axe horizontal
		anim.SetFloat("Speed", Mathf.Abs(move)); //on met la variable speed de l'animator à la valeur absolu du mouvement

		rigidbody2D.velocity = new Vector2 (maxSpeed * move, rigidbody2D.velocity.y);

		if (move>0 && !facingRight)
			Flip (); //si on avance à droite et qu'on est orienté à gauche, on se retourne
		else if (move<0 && facingRight)
			Flip (); //si on avance à gauche et qu'on est orienté à droite, on se retourne

		end = Physics2D.OverlapCircle(playerCheck.position, 0.5f, whatIsEnd);
		
		if (end)
			End ();
	}

	void Update()
	{
		if (hazarded && !dead)
		{
			Dead ();
		}


		if (!alreadyJump &&(grounded || wall) && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))) 
		{
			anim.SetBool("Ground", false);
			rigidbody2D.AddForce(new Vector2 (0, jumpForce));
			if (wall)
			{
				alreadyJump = true;
				StartCoroutine (waitForJump());
			}
		}


	}

	void Flip()
	{
		facingRight = !facingRight;

		//retourner selon l'axe des x (utiliser l'échelle)
		Vector3 theScale = transform.localScale;
		theScale.x *=-1;
		transform.localScale = theScale;
	}

	void End()
	{
		anim.SetFloat("Speed", 0f);
		rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x/2, rigidbody2D.velocity.y);
		levelController.GameOver(0);
	}
	
	void Dead()
	{
		dead=true;
		anim.SetTrigger ("isDeadTrigger");
		levelController.GameOver(1);
	}

	IEnumerator waitForJump () 
	{
		yield return new WaitForSeconds(waitForWallJump);
		alreadyJump=false;
	}


	
}
