using UnityEngine;
using System.Collections;
using Assets;

public class TankController : MonoBehaviour {

    public GameController gameController;
    public GameObject playerGameObject;
    public PlayerScript playerController;
         

    private SpriteRenderer tankSprite;

    private float speed = 5;

    private float FireTime = 5f;
    private float FireTimer;

    private int life;

    private float hitTimer;
    private float hitTime = 1f;

    System.Random r = new System.Random();

	// Use this for initialization
	void Start () {
        life = 100;
        FireTimer = FireTime;
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

        tankSprite = gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {
            if (hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
                tankSprite.color = Color.red;
            }
            else
            {
                tankSprite.color = Color.white;
            }

            UpdateMove();
            UpdateFire();
        }
       
	}

    private void UpdateMove()
    {
        if (playerGameObject.transform.position.x-5 < gameObject.transform.position.x)
        {
            gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (playerGameObject.transform.position.x +5 > gameObject.transform.position.x)
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
       
    }

    private void UpdateFire()
    {
        FireTimer -= Time.deltaTime;
        if (FireTimer < 0)
        {
            FireTime = r.Next(5) +1;
            FireTimer = FireTime;

            var direction = playerGameObject.transform.position - gameObject.transform.position;
            direction.Normalize();
            var collider = gameObject.GetComponent<Collider2D>();
            gameController.spawnBullet(collider, this.transform.position, direction, true);
        }
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
                gameController.KillTank(this.gameObject);
            }
        }
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
