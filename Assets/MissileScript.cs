using UnityEngine;
using System.Collections;

public class MissileScript : MonoBehaviour {

    public GameController gameController;
    public GameObject playerGameObject;
    public PlayerScript playerController;

    private SpriteRenderer missileSprite;

    public Rigidbody2D missileRigidBody;

    private float speed = 20;

    private float explodeTimer;
    private float explodeTime = .5f;

	// Use this for initialization
	void Start () {

        if (playerGameObject == null)
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerController = playerGameObject.GetComponent<PlayerScript>();
        }

        if (gameController == null)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        missileSprite = gameObject.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {


        if (explodeTimer > 0)
        {
            explodeTimer -= Time.deltaTime;
            if (explodeTimer <= 0)
            {
                gameController.RemoveMissile(this.gameObject);
            }

            float colorFlash = (float)gameController.r.NextDouble();

            if (colorFlash < .3f)
            {
                missileSprite.color = Color.red;
            }
            else if(colorFlash < .6f){
                 missileSprite.color = Color.yellow;
            }
            else{
                missileSprite.color = Color.white;
            }
        }

        UpdateFly();
    }

    private void UpdateFly()
    {
        var dest = playerGameObject.transform.position;

        var dir = dest - this.transform.position;
        dir.Normalize();

        float velocityX = Mathf.Lerp(missileRigidBody.velocity.x, dir.x * speed, .5f);
        float velocityY = Mathf.Lerp(missileRigidBody.velocity.y, dir.y * speed, .5f);

        missileRigidBody.velocity = new Vector2(velocityX, velocityY);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.gameObject.tag == "Player")
        {
            playerController.onGround = true;
            explodeTimer = explodeTime;

        }

    }
}
