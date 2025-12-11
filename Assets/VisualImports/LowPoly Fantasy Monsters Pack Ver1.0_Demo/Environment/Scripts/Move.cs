using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private float move = 20;
	private bool stop = false;	
	private float blend;
	private float delay = 0;
	public float AddRunSpeed = 1;
	public float AddWalkSpeed = 1;
	private bool hasAniComp = false;
	private Transform player;
	public float rotationSpeed = 5f; // Vitesse de rotation vers le joueur
	public float detectionRange = 150f; // Distance pour détecter le joueur
	public float followSpeed = 25f; // Vitesse de course vers le joueur (beaucoup plus rapide)
	public float stopDistance = 3f; // Distance d'arrêt avant le joueur
	public float attackDistance = 8f; // Distance pour déclencher le dash/attaque (augmentée)
	public float attackCooldown = 1.5f; // Temps entre les attaques
	private float lastAttackTime = 0f;
	public float dashDistance = 30f; // Distance de dash vers le joueur (beaucoup plus long)
	public float dashSpeed = 50f; // Vitesse du dash (beaucoup plus rapide)
	private bool isDashing = false;
	private Vector3 dashDirection;


	// Use this for initialization
	void Start () 
	{

		if ( null != GetComponent<Animation>() )
		{
			hasAniComp = true;
		}

		// Trouver le joueur au démarrage
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj != null)
		{
			player = playerObj.transform;
		}

	}

	void LookAtPlayer()
	{
		if (player == null) return;

		// Vérifier la distance
		float distance = Vector3.Distance(transform.position, player.position);
		if (distance > detectionRange) return;

		// Calculer la direction vers le joueur
		Vector3 directionToPlayer = (player.position - transform.position).normalized;
		directionToPlayer.y = 0; // Garder la rotation que sur l'axe Y

		// Calculer l'angle cible
		Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

		// Interpoler la rotation pour un mouvement fluide
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
	}

	void FollowPlayer()
	{
		if (player == null) return;
		if (isDashing) return; // Ne pas marcher pendant le dash

		// Vérifier la distance
		float distance = Vector3.Distance(transform.position, player.position);
		if (distance > detectionRange) return;

		// Arrêter s'il est assez près
		if (distance < stopDistance) return;

		// Courir vers le joueur (au lieu de marcher)
		Vector3 directionToPlayer = (player.position - transform.position).normalized;
		Vector3 movement = directionToPlayer * followSpeed * Time.deltaTime;
		transform.Translate(movement, Space.World);

		// Jouer l'animation de course s'il y a un composant Animation
		if (hasAniComp && CheckAniClip("run"))
		{
			GetComponent<Animation>().Play("run");
		}
	}

	void AttackPlayer()
	{
		if (player == null) return;

		// Vérifier la distance d'attaque
		float distance = Vector3.Distance(transform.position, player.position);
		if (distance > attackDistance) return;

		// Vérifier le cooldown d'attaque
		if (Time.time - lastAttackTime < attackCooldown) return;

		// Jouer l'animation d'attaque (utilise "attack01" par défaut)
		if (hasAniComp && CheckAniClip("attack01"))
		{
			GetComponent<Animation>().CrossFade("attack01", 0.2f);
			GetComponent<Animation>().CrossFadeQueued("idle01");
		}

		lastAttackTime = Time.time;

		// Infliger des dégâts au joueur (si tu as un composant Player avec TakeDamage)
		Player playerComponent = player.GetComponent<Player>();
		if (playerComponent != null)
		{
			playerComponent.TakeDamage(10); // 10 dégâts par attaque (à personnaliser)
		}

		// Commencer un dash d'attaque vers le joueur
		StartDash();
	}

	void StartDash()
	{
		if (player == null) return;
		if (isDashing) return; // Déjà en train de dash

		// Calculer la direction vers le joueur
		Vector3 directionToPlayer = (player.position - transform.position).normalized;
		directionToPlayer.y = 0; // Garder le dash au sol

		dashDirection = directionToPlayer;
		isDashing = true;

		// Jouer l'animation de run si disponible
		if (hasAniComp && CheckAniClip("run"))
		{
			GetComponent<Animation>().CrossFade("run");
		}

		// Durée du dash en fonction de la distance
		StartCoroutine(ExecuteDash());
	}

	System.Collections.IEnumerator ExecuteDash()
	{
		float dashDuration = dashDistance / dashSpeed;
		float elapsedTime = 0f;

		while (elapsedTime < dashDuration && isDashing)
		{
			// Vérifier si on est trop proche du joueur (collision pendant le dash)
			if (player != null)
			{
				float distanceToPlayer = Vector3.Distance(transform.position, player.position);
				if (distanceToPlayer < 1f) // Stop si on touche presque le joueur
				{
					isDashing = false;
					break;
				}
			}

			// Avancer dans la direction du dash
			transform.Translate(dashDirection * dashSpeed * Time.deltaTime, Space.World);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		isDashing = false;

		// Revenir à idle après le dash
		if (hasAniComp && CheckAniClip("idle01"))
		{
			GetComponent<Animation>().CrossFade("idle01", 0.3f);
		}
	}

	void MoveActive ()
	{ 
		float speed =0.0f;
		float add =0.0f;

		if ( hasAniComp == true )
		{	
			if ( Input.GetKey(KeyCode.UpArrow) )
			{  	
				move *= 1.015F;

				if ( move>250 && CheckAniClip( "run" )==true )
				{
					{
						GetComponent<Animation>().CrossFade("run");
						add = 20*AddRunSpeed;
					}
				}
				else
				{
					GetComponent<Animation>().Play("walk");
					add = 5*AddWalkSpeed;
				}

				speed = Time.deltaTime*add;

				transform.Translate( 0,0,speed );

			}


			if ( Input.GetKeyUp(KeyCode.UpArrow))
			{
				if ( GetComponent<Animation>().IsPlaying("walk"))
				{	GetComponent<Animation>().CrossFade("idle01",0.3f); }
				if ( GetComponent<Animation>().IsPlaying("run"))
				{	
					GetComponent<Animation>().CrossFade("idle01",0.5f);
					stop = true;
				}	
				move = 20;

			}

			if (stop == true)
			{	
				float max = Time.deltaTime*20*AddRunSpeed;
				blend = Mathf.Lerp(max,0,delay);

				if ( blend > 0 )
				{	
					transform.Translate( 0,0,blend );
					delay += 0.025f; 
				}	
				else 
				{	
					stop = false;
					delay = 0.0f;
				}
			}

		}
		else
		{
			if ( Input.GetKey(KeyCode.UpArrow) )
			{  	
				add = 5*AddWalkSpeed;
				speed = Time.deltaTime*add;
				transform.Translate( 0,0,speed );
			}
		}
	}

	bool CheckAniClip ( string clipname )
	{	
		if( this.GetComponent<Animation>().GetClip(clipname) == null ) 
			return false;
		else if ( this.GetComponent<Animation>().GetClip(clipname) != null ) 
			return true;

		return false;
	}

	// Update is called once per frame
	void Update () 
	{

		MoveActive();

		// Le loup se tourne vers le joueur
		LookAtPlayer();

		// Le loup suit le joueur
		FollowPlayer();

		// Le loup attaque quand il est assez proche
		AttackPlayer();

		if ( hasAniComp == true )
		{	

			if (Input.GetKey(KeyCode.V))
			{	
				if ( CheckAniClip( "damage away" ) == false ) return;

				GetComponent<Animation>().CrossFade("damage away",0.2f);
				GetComponent<Animation>().CrossFadeQueued("idle01");
			} 

			if (Input.GetKey(KeyCode.C))
			{	
				if ( CheckAniClip( "dead away" ) == false ) return;

				GetComponent<Animation>().CrossFade("dead away",0.2f);
			} 

			if (Input.GetKey(KeyCode.E))
			{	
				if ( CheckAniClip( "attack03" ) == false ) return;

				GetComponent<Animation>().CrossFade("attack03",0.2f);
				GetComponent<Animation>().CrossFadeQueued("idle01");
			} 

			if (Input.GetKey(KeyCode.Q))
			{	
				if ( CheckAniClip( "attack01" ) == false ) return;

				GetComponent<Animation>().CrossFade("attack01",0.2f);
				GetComponent<Animation>().CrossFadeQueued("idle01");
			}

			if (Input.GetKey(KeyCode.W))
			{	
				if ( CheckAniClip( "attack02" ) == false ) return;

				GetComponent<Animation>().CrossFade("attack02",0.2f);
				GetComponent<Animation>().CrossFadeQueued("idle01");
			}

			if (Input.GetKey(KeyCode.A))
			{	
				if ( CheckAniClip( "drop down" ) == false ) return;

				GetComponent<Animation>().CrossFade("drop down",0.2f);
			}

			if (Input.GetKey(KeyCode.Z))
			{	
				if ( CheckAniClip( "sit up" ) == false ) return;

				GetComponent<Animation>().CrossFade("sit up",0.2f);
				GetComponent<Animation>().CrossFadeQueued("idle01");
			}

			if (Input.GetKey(KeyCode.S))
			{	
				if ( CheckAniClip( "damage" ) == false ) return;

				GetComponent<Animation>().CrossFade("damage",0.1f);
				GetComponent<Animation>().CrossFadeQueued("idle01");			
			}

			if (Input.GetKey(KeyCode.X))
			{	
				if ( CheckAniClip( "dead" ) == false ) return;

				GetComponent<Animation>().CrossFade("dead",0.1f);
			}			

			if (Input.GetKey(KeyCode.D))
			{	
				if ( CheckAniClip( "idle02" ) == false ) return;

				GetComponent<Animation>().CrossFade("idle02",0.1f);
				GetComponent<Animation>().CrossFadeQueued("idle01");			
			}	
								
		}

		if ( Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate( 0.0f,Time.deltaTime*-100.0f,0.0f);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(0.0f,Time.deltaTime*100.0f,0.0f);
		}

	}
}
