using UnityEngine;
using System.Collections;

public class BloodController : MonoBehaviour {

    public float bloodTime;
    public float bloodTimer;
    public System.Random r;

    public GameController gameController;
	// Use this for initialization
	void Start () {
        r = new System.Random();
        bloodTime = (float)r.NextDouble() + 1;
        bloodTimer = bloodTime;

        gameController = GameObject.FindObjectOfType<GameController>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!gameController.isGameOver)
        {
            bloodTimer -= Time.deltaTime;
            if (bloodTimer < 0)
            {
                gameController.removeBloodSplatter(this.gameObject);
            }
        }
       
	}
}
