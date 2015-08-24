using UnityEngine;
using System.Collections;
using Assets;

public class HelicopterScript : MonoBehaviour {

    public GameController gameController;
    public GameObject playerGameObject;
    public PlayerScript playerController;

    private Rigidbody2D helicopterRigidBody;

    private SpriteRenderer helicopterSprite;

        private float hitTimer;
    private float hitTime = 1f;
    private int life;

    private float BulletTime = .5f;
    private float BulletTimer;

    private float MissileTime = 5f;
    private float MissileTimer;



	// Use this for initialization
	void Start () {
        life = 50;
        BulletTimer = BulletTime;
        MissileTimer = MissileTime;

        Init();
	}

    private void Init()
    {
        if (playerGameObject == null)
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerController = playerGameObject.GetComponent<PlayerScript>();
        }

        if (gameController == null)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        helicopterSprite = gameObject.GetComponent<SpriteRenderer>();
        helicopterRigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Hurt(int damage)
    {
        if (hitTimer <= 0)
        {
            gameController.playCrashSound();
            gameController.CameraShake(.1f, GameConfig.CameraShakeAmountSmall);

            hitTimer = hitTime;
            life -= damage;
            if (life <= 0)
            {
                gameController.KillHelicopter(this.gameObject);
            }
        }
    }

	
	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {

            if (hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
                helicopterSprite.color = Color.red;
            }
            else
            {
                helicopterSprite.color = Color.white;
            }

            UpdateFly();
            UpdateFire();
        }
	}

    private void UpdateFire()
    {
        BulletTimer -= Time.deltaTime;
        if (BulletTimer < 0)
        {
            BulletTimer = BulletTime;
            var direction = playerGameObject.transform.position - gameObject.transform.position;
            direction.Normalize();
            var collider = gameObject.GetComponent<Collider2D>();
            gameController.spawnBullet(collider, this.transform.position, direction, false);
        }

        MissileTimer -= Time.deltaTime;
        if (MissileTimer < 0)
        {
            MissileTime = gameController.r.Next(5) +1;
            MissileTimer = MissileTime;
           
            gameController.SpawnMissile(this.transform.position);
        }
    }

    private void UpdateFly()
    {
        float forceX = 0;
        float forceY = 0;
        if(playerController.transform.position.x < this.transform.position.x){

            transform.rotation = Quaternion.Euler(0, 180, 0);
            forceX = -10;
        }
        else{

            transform.rotation = Quaternion.Euler(0, 0, 0);
            forceX = 10;
        }

        if (playerController.transform.position.y > this.transform.position.y)
        {
            forceY = 10;
        }
        else if (playerController.transform.position.y < this.transform.position.y)
        {
            forceY = -10;
        }

        if (this.transform.position.y > 20)
        {
            forceY = -10;
        }
        else if (this.transform.position.y < 7)
        {
            forceY = 10;
        }

        helicopterRigidBody.AddForce(new Vector3(forceX, forceY, 0));
       
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.gameObject.tag == "Player")
        {
            playerController.onGround = true;
            if (playerController.isDiving)
            {
                Hurt(playerController.damage);
            }
        }

        if (coll.gameObject.tag == "TankBullet")
        {
                Hurt(50);
        }

    }
}
