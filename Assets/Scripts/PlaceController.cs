using UnityEngine;
using System.Collections;

public class PlaceController : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 originPoint;
	private Vector3 offset;
	private Vector3 posePoint;
	
	public float groundRadius;
	public LayerMask whatIsGround;
	public Transform groundCheck;
	public Transform playerCheck;

	public float panelRadius;
	public LayerMask whatIsPanel;

	public float playerRadius;
	public LayerMask whatIsPlayer;
	
	public float invRadius;
	public LayerMask whatIsInv;

	private float hazardRadius=0.3f;
	public LayerMask whatIsHazard;
	private float waterRadius=0.1f;
	public LayerMask whatIsWater;

	private bool newbie=false;
	
	private bool onGround= false;
	private bool otherPanel=false;
	private BoxCollider2D sol; 
	private float solX;
	private float solY;
	private float offsetY;

	private CircleCollider2D CerclePlacement;
	
	private LevelController levelController;

	
	void Start()
	{
		transform.localPosition= new Vector3 (transform.localPosition.x, transform.localPosition.y,-50);//rester devant le menu
		
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");

		CerclePlacement=GetComponent<CircleCollider2D>();

		CerclePlacement.enabled=true;
		originPoint=gameObject.transform.position;
		posePoint=originPoint;
	}

	void Update()
	{
		if (onGround && sol && !levelController.panelCanMove)
		{
			solX= sol.gameObject.transform.position.x;
			float surface=sol.transform.position.y+sol.size.y/2;
			solY= surface+offsetY;
			transform.position=new Vector3(solX,solY,transform.position.z);
		}

		if (CerclePlacement.enabled && !levelController.panelCanMove)
			CerclePlacement.enabled=false;
		else if (!CerclePlacement.enabled && levelController.panelCanMove)
			CerclePlacement.enabled=true;

	}
	
	void OnMouseDown()
	{
		if (!levelController.panelCanMove)
			return;		

		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		originPoint=gameObject.transform.position;
	}
	
	void OnMouseDrag()
	{
		if (!levelController.panelCanMove)
			return;

		if (!onGround && !levelController.VerifPanel(gameObject.tag) && !newbie)
			return;

		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
		transform.position= new Vector3 (transform.position.x, transform.position.y,-2);

		onGround=false;
		
		//if (curPosition.y!=originPoint.y && !newbie)
		if(!newbie)
			Prendre();
	}

	
	void OnMouseUp()
	{
		if (!levelController.panelCanMove)
			return;
			
		Collider2D solTemp = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		Collider2D[] panelCollider = Physics2D.OverlapCircleAll(groundCheck.position, panelRadius, whatIsPanel);
		bool playerCollider = Physics2D.OverlapCircle(playerCheck.position, playerRadius, whatIsPlayer);
		bool hazardCollider =Physics2D.OverlapCircle(groundCheck.position, hazardRadius, whatIsHazard);
		bool waterCollider =Physics2D.OverlapCircle(groundCheck.position, waterRadius, whatIsWater) || Physics2D.OverlapCircle(playerCheck.position, waterRadius, whatIsWater);
		bool invCollider;
		
		foreach (Collider2D panelC in panelCollider)
		{
			if (panelC.gameObject != gameObject)
				otherPanel=true;
		}		

		if(solTemp == null || otherPanel || playerCollider || hazardCollider || waterCollider)
			Enlever();
		else 
		{
			onGround=true;
			//Pour que ca colle au sol
			sol = solTemp.GetComponent<BoxCollider2D>();
			float surface=sol.transform.position.y+sol.size.y/2;
			offsetY = GetComponent<BoxCollider2D>().size.y/2;
			solX= sol.transform.position.x;
			solY= surface+offsetY;
			transform.position = new Vector3 (solX, solY, 1); //z=1 pour rester derrière le décor
			posePoint=transform.position;
			//revérifier el placement derrière le ninja ou un bouton ou un piège
			playerCollider = Physics2D.OverlapCircle(playerCheck.position, playerRadius, whatIsPlayer);
			hazardCollider =Physics2D.OverlapCircle(playerCheck.position, hazardRadius, whatIsHazard);
			panelCollider = Physics2D.OverlapCircleAll(groundCheck.position, panelRadius, whatIsPanel);
			invCollider= Physics2D.OverlapCircle(playerCheck.position, invRadius,whatIsInv);
			
			if (!invCollider)
				Enlever();
							
			foreach (Collider2D panelC in panelCollider)
			{
				if (panelC.gameObject != gameObject)
					otherPanel=true;
			}
			if (playerCollider || hazardCollider || otherPanel)
				Enlever ();
		}
	}

	void Prendre()
	{
		levelController.TakePanel(gameObject.tag);
		if (levelController.VerifPanel(gameObject.tag))
		{
			GameObject newFO=(GameObject) GameObject.Instantiate(gameObject);
			newFO.transform.rotation=gameObject.transform.rotation;
			newFO.transform.position=originPoint;
			newFO.transform.parent=gameObject.transform.parent;
			newFO.transform.localScale=gameObject.transform.localScale;
		}
		newbie=true;
	}
	
	void Enlever()
	{
		if (!levelController.VerifPanel(gameObject.tag))
		{
			newbie=false;
			GameObject newFO=(GameObject) GameObject.Instantiate(gameObject);
			newFO.transform.rotation=gameObject.transform.rotation;
			newFO.transform.position=originPoint;
			newFO.transform.parent=gameObject.transform.parent;
			newFO.transform.localScale=gameObject.transform.localScale;
		}
		Destroy(gameObject);
		levelController.AddPanel(gameObject.tag);
	}

	public void Replace()
	{
		transform.position=posePoint;
	}

	public bool isOnGround()
	{
		return onGround;
	}
}
