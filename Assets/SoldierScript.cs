using UnityEngine;
using System.Collections;

public class SoldierScript : MonoBehaviour {

    public GameObject playerGameObject;
    public GameController gameController;

    public System.Random r = new System.Random();
    private float fireTimer;
    private float fireTime;
	// Use this for initialization
	void Start () {
        Init();
        fireTime = (float)(r.NextDouble() + r.NextDouble());
        fireTimer = fireTime;
	}

    private void Init()
    {
        if(playerGameObject == null){
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (gameController == null)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer < 0)
            {
                fireTime = (float)(r.NextDouble() + r.NextDouble());
                fireTimer = fireTime;
                Fire();
            }
        }

        if (playerGameObject.transform.position.x < this.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

	}

    private void Fire()
    {
        var direction = playerGameObject.transform.position - gameObject.transform.position;
        direction.Normalize();
        var collider = gameObject.GetComponent<Collider2D>();
        gameController.spawnBullet(collider, this.transform.position, direction,false);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (gameController != null)
            {
                gameController.killSoldier(this.gameObject);
            }
        }

        if (coll.gameObject.tag == "Bullet" || coll.gameObject.tag == "TankBullet")
        {
            if (gameController != null)
            {
                gameController.killMan(this.gameObject);
            }
        }
    }

   
}
