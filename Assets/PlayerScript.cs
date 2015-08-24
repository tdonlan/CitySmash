using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets;

public class PlayerScript : MonoBehaviour {

    public GameController gameController;

    public Rigidbody2D playerRigidBody;

    public Collider2D playerCollider;

    private SpriteRenderer playerSprite;

    private Sprite jumpSprite;
    private Sprite landSprite;
    private Sprite idleSprite;
    private Sprite dodgeSprite;

    public static float speed = 300f;
    public static float slowdown = .85f;

    private Vector3 velocity;
    private Vector3 acceleration;

    public bool isDoubleJump = false;
    public bool isDiving = false;
    public bool onGround = true;

    private float diveTimer;
    private float diveTime = .5f;

    public float hitTimer;
    private float hitTime = .2f;

    private float shockwaveTimer;
    private float shockwaveTime = 2f;

    public Camera mainCamera;

    //Monster stats
    public long lifeTotal;
    public long life;
    public long xp;
    public long nextXP;
    public int powerLevel;
    public int damage;

	// Use this for initialization
	void Start () {

        init();

        ResetPlayer();
	}

    public void ResetPlayer()
    {
        this.transform.position = new Vector3(0, 0, 0);
        this.playerRigidBody.velocity = Vector3.zero;
        this.playerRigidBody.angularVelocity = 0;

        this.powerLevel = 1;
        this.damage = 20;
        this.lifeTotal = 100;
        this.life = 100;
        this.xp = 0;
        this.nextXP = 25;
    }

    private void init()
    {
        jumpSprite = Resources.Load<Sprite>("MonsterJump");
        landSprite = Resources.Load<Sprite>("MonsterLand");
        idleSprite = Resources.Load<Sprite>("MonsterIdle");
        dodgeSprite = Resources.Load<Sprite>("MonsterDodge");

        playerSprite = gameObject.GetComponent<SpriteRenderer>();
    }

	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {
            UpdateControlPhysics();
        }
       
	}

    private void UpdateControlPhysics()
    {

        if (shockwaveTimer > 0)
        {
            shockwaveTimer -= Time.deltaTime;
        }

        if (hitTimer >= 0)
        {
            hitTimer -= Time.deltaTime;
            playerSprite.color = Color.red;
        }
        else
        {
            playerSprite.color = Color.white;
        }
      

        if (isDiving)
        {

            diveTimer -= Time.deltaTime;
            if (diveTimer <= 0)
            {
                isDiving = false;
                playerSprite.sprite = idleSprite;
            }
        }

        //if we get blown into the sky, still able to slam
        if (transform.position.y > 2)
        {
            onGround = false;
        }

        //Double Jump
        if (Input.GetKeyDown(KeyCode.Space) && !onGround && !isDoubleJump)
        {
            playerSprite.sprite = jumpSprite;
            isDoubleJump = true;

            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0f;

            playerRigidBody.AddForce(new Vector2(0, 1000));
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            playerSprite.sprite = jumpSprite;
            onGround = !onGround;
            isDoubleJump = false;
      
            playerRigidBody.AddForce(new Vector2(0, 1000));
        }

        if (Input.GetKeyDown(KeyCode.E) && !isDiving)
        {

            playerSprite.sprite = dodgeSprite;
            playerSprite.transform.localScale *= 1;

            isDiving = true;
            diveTimer = diveTime;
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0;

            playerRigidBody.AddForce(new Vector2(2000, 0));

            transform.localRotation = Quaternion.Euler(0, 0, 0);
            gameController.playMonsterGrowl(2);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isDiving)
        {

            playerSprite.sprite = dodgeSprite;

            isDiving = true;
            diveTimer = diveTime;
            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0;

            playerRigidBody.AddForce(new Vector2(-2000, 0));

            transform.rotation = Quaternion.Euler(0, 180, 0);
            gameController.playMonsterGrowl(2);
        }

        if (Input.GetMouseButtonDown(1) && !isDiving && shockwaveTimer <= 0)
        {
            shockwaveTimer = shockwaveTime;
            gameController.SpawnShockwave(gameObject.transform.position);
        }


        if ( Input.GetMouseButtonDown(0) && !onGround && !isDiving)
        {
          
            isDiving = true;
            diveTimer = diveTime;
            playerSprite.sprite = landSprite;

            playerRigidBody.velocity = Vector2.zero;
            playerRigidBody.angularVelocity = 0;

            playerRigidBody.AddForce(new Vector2(0, -3000));

            gameController.CameraShake(.5f, GameConfig.CameraShakeAmountSmall);
            gameController.playMonsterGrowl(3);
        }

        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");

        if (!isDiving)
        {
            
            acceleration = new Vector3(moveX * speed, 0, 0);
            velocity += acceleration * Time.deltaTime;

            playerRigidBody.velocity = new Vector2(velocity.x, playerRigidBody.velocity.y);

            velocity *= slowdown;

            if (moveX == 0 && onGround)
            {
                playerSprite.sprite = idleSprite;
            }

            if (moveX < 0)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

    }

    public void Hurt(int damage)
    {
        life -= damage;
        gameController.CameraShake(.2f, GameConfig.CameraShakeAmountSmall);

        if (life <= 0)
        {
            gameController.GameOver();
        }
    }

    public void Heal(int num)
    {
        life += num;
        if (life > lifeTotal){
            life = lifeTotal;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Tank")
        {
            onGround = true;
        }

        if (coll.gameObject.tag == "Bullet")
        {
            if (hitTimer <= 0)
            {
                hitTimer = hitTime;
                Hurt(GameConfig.bulletDamage);
            }

        }

        if (coll.gameObject.tag == "TankBullet")
        {
            if (hitTimer <= 0)
            {
                hitTimer = hitTime;
                Hurt(GameConfig.tankBulletDamage);
            }

        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        playerSprite.sprite = landSprite;
        onGround = true;
    }

    public void addXP(long xp)
    {
        this.xp += xp;
        if (this.xp >= nextXP)
        {
            damage += 10;
            lifeTotal = (long)Mathf.RoundToInt(lifeTotal * 1.1f);
            life = lifeTotal;
            powerLevel++;
            xp -= nextXP;
            nextXP += Mathf.RoundToInt(nextXP * 1.1f);
        }
    }
}
