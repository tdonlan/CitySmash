using UnityEngine;
using System.Collections;

public class ShockwaveScript : MonoBehaviour {

    private GameObject playerGameObject;
    public PlayerScript playerController;

    private float size = 1f;
    private float endSize = 3f;
    private float timer;
    private float time = .5f;

    private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        timer = 0;

        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGameObject.GetComponent<PlayerScript>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.position = playerGameObject.transform.position;

        if (timer <= time) {
            timer += Time.deltaTime;
            size = Mathf.Lerp(size, endSize, timer / time);
        }
        else
        {
            Destroy(this.gameObject);
        }

        sprite.transform.localScale = new Vector3(size, size, 1);
        
        UpdateForce();

	}

    private void UpdateForce()
    {
        var pos = this.transform.position;
        Collider2D[] explosionArray = Physics2D.OverlapCircleAll(pos, sprite.bounds.extents.x);
        for (int i = 0; i < explosionArray.Length; i++)
        {
            if (!explosionArray[i].Equals(playerController.playerCollider))
            {
                Rigidbody2D rbody = explosionArray[i].gameObject.GetComponent<Rigidbody2D>();
                if (rbody != null)
                {
                    Vector2 forceDirection = rbody.transform.position - pos;
                    forceDirection.Normalize();
                    rbody.AddForceAtPosition(forceDirection * 5000, pos);
                }
            }
        }
    }
}
