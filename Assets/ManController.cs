using UnityEngine;
using System.Collections;

public class ManController : MonoBehaviour {

    public GameController gameController;

    public static float speed = 5;

    private System.Random r;
    private float runTime;
    private float runTimer;

    private bool isLeft = true;

	// Use this for initialization
	void Start () {
        this.gameController = GameObject.FindObjectOfType<GameController>();

        this.r = new System.Random();
        runTime = r.Next(5);
        isLeft = r.Next() % 2 == 0? true : false;
	}
	
	// Update is called once per frame
	void Update () {

        if (!gameController.isGameOver)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0)
            {
                runTime = r.Next(5);
                runTimer = runTime;
                isLeft = r.Next() % 2 == 0 ? true : false;
            }

            UpdateRunaway();
        }
       
	}

    private void UpdateRunaway()
    {
        if (isLeft)
        {
            gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0);
        }
        else
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (gameController != null)
            {
                gameController.killMan(this.gameObject);
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
