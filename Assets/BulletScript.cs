using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    private float bulletTimer;
    private bool isDead;

    public GameController gameController;
	// Use this for initialization
	void Start () {
        this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        bulletTimer = 5;
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {
            bulletTimer -= Time.deltaTime;
            if (bulletTimer < 0)
            {
                gameController.RemoveBullet(this.gameObject);
            }
            if (isDead)
            {
                gameController.RemoveBullet(this.gameObject);
            }
        }
        
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            isDead = true;
        }

    }
}
