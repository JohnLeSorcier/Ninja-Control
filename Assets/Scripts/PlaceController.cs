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

	public float panelRadius;
	public LayerMask whatIsPanel;

	public float playerRadius;
	public LayerMask whatIsPlayer;

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
		transform.position= new Vector3 (transform.position.x, transform.position.y, -2);//rester devant le menu

		originPoint=gameObject.transform.position;
		posePoint=originPoint;

		
		GameObject levelControllerObject = GameObject.FindWithTag ("LevelController");
		if (levelControllerObject != null)
			levelController = levelControllerObject.GetComponent <LevelController>();
		else
			Debug.Log ("Cannot find 'LevelController' script");

		CerclePlacement=GetComponent<CircleCollider2D>();

		CerclePlacement.enabled=true;



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

		onGround=false;
		
		if (curPosition.y!=originPoint.y && !newbie)
			Prendre();
	}

	
	void OnMouseUp()
	{
		Collider2D solTemp = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		Collider2D[] panelCollider = Physics2D.OverlapCircleAll(groundCheck.position, panelRadius, whatIsPanel);
		bool playerCollider = Physics2D.OverlapCircle(groundCheck.position, playerRadius, whatIsPlayer);

		foreach (Collider2D panelC in panelCollider)
		{
			if (!onGround && panelC.gameObject != gameObject)
				otherPanel=true;
		}

		if(solTemp == null || otherPanel || playerCollider)
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
		}
	}

	void Prendre()
	{
		levelController.TakePanel(gameObject.tag);
		if (levelController.VerifPanel(gameObject.tag))
		{
			GameObject newFO=(GameObject) GameObject.Instantiate(gameObject,originPoint,transform.rotation);
			newFO.transform.parent=gameObject.transform.parent;
		}
		newbie=true;
	}
	
	void Enlever()
	{
		if (!levelController.VerifPanel(gameObject.tag))
		{
			newbie=false;
			transform.position= new Vector3(transform.position.x, transform.position.y, -2);//remetrte l'obejet devant
			CerclePlacement.enabled=true;
			Instantiate(gameObject, originPoint,transform.rotation);
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
