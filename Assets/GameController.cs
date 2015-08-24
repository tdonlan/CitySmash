using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets;

public class GameController : MonoBehaviour {

    public System.Random r;

    public float GameTimer;
    
    public Camera mainCamera;
    public Vector2 cameraDest;

    private float CameraShakeAmount=1;
    private float CameraShakeTimer;
    private float CameraShakeTime = .2f;


    public GameObject playerObject;
    public PlayerScript playerController;

    public Slider playerLifeSlider;
    public Slider playerXPSlider;
    public Text PowerText;
    public Text TitleText;
    public Text DebugText;

    public GameObject manPrefab;
    public GameObject bloodPrefab;
    public GameObject soldierPrefab;
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public GameObject tankBulletPrefab;
    public GameObject tankPrefab;
    public GameObject helicopterPrefab;
    public GameObject shockwavePrefab;

    public GameObject explosionPrefab;
    public List<GameObject> objectList = new List<GameObject>();

    public Text ScoreText;

    public long score;
    public long kills;

    private float spawnTime;
    private float spawnTimer;

    public bool isGameOver = false;
    public GameObject gameOverPopup;
    public Text GameOverScoreText;


    public AudioSource audio;
    private List<AudioClip> deathSoundList;
    private AudioClip monsterSound;
    private AudioClip monsterSound2;
    private AudioClip monsterSound3;
    private AudioClip shockwaveSound;
    private AudioClip bloodSound;
    private AudioClip gunSound;
    private AudioClip radioSound;
    private AudioClip bigGunSound;
    private AudioClip explosionSound;
    private AudioClip helicopterSound;
    private AudioClip crashSound;


    private AudioClip song1;

	// Use this for initialization
	void Start () {
        r = new System.Random();
        InitPrefab();
        InitSounds();

        PlaySong();
	}

    
    private void PlaySong()
    {
        audio.loop = true;
        audio.clip = song1;
        audio.volume = .25f;
        audio.Play();
    }

    private void InitPrefab()
    {

        manPrefab = Resources.Load<GameObject>("ManPrefab");
        bloodPrefab = Resources.Load<GameObject>("BloodSplatterPrefab");
        soldierPrefab = Resources.Load<GameObject>("SoldierPrefab");
        bulletPrefab = Resources.Load<GameObject>("BulletPrefab");
        tankBulletPrefab = Resources.Load<GameObject>("TankBulletPrefab");
        tankPrefab = Resources.Load<GameObject>("TankPrefab");
        helicopterPrefab = Resources.Load<GameObject>("HelicopterPrefab");
        explosionPrefab = Resources.Load<GameObject>("ExplosionPrefab");
        missilePrefab = Resources.Load<GameObject>("MissilePrefab");
        shockwavePrefab = Resources.Load<GameObject>("ShockwavePrefab");
    }

    private void InitSounds()
    {
        audio = GameObject.FindObjectOfType<AudioSource>();
        deathSoundList = new List<AudioClip>();
        for (int i = 1; i <= 7; i++)
        {
            deathSoundList.Add(Resources.Load<AudioClip>("Sound/Death" + i));
        }
        monsterSound = Resources.Load<AudioClip>("Sound/MonsterGrowl");
        bloodSound = Resources.Load<AudioClip>("Sound/bloodSplatter");
        gunSound = Resources.Load<AudioClip>("Sound/Gun1");
        radioSound = Resources.Load<AudioClip>("Sound/Radio1");

        bigGunSound = Resources.Load<AudioClip>("Sound/BigGun");
        explosionSound = Resources.Load<AudioClip>("Sound/Explosion");
        helicopterSound = Resources.Load<AudioClip>("Sound/Helicopter");

        monsterSound2= Resources.Load<AudioClip>("Sound/MonsterGrowl2");
        monsterSound3 = Resources.Load<AudioClip>("Sound/MonsterGrowl3");
        shockwaveSound= Resources.Load<AudioClip>("Sound/Shockwave");
        crashSound = Resources.Load<AudioClip>("Sound/Crash");

        song1 = Resources.Load<AudioClip>("Sound/Song1");
    }
	
	// Update is called once per frame
	void Update () {


        if (!isGameOver)
        {
            GameTimer += Time.deltaTime;
            
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                spawnTimer = r.Next(5) + r.Next(5);
                SpawnStuff();
            }

            UpdateCamera();
            UpdateUI();
        }

	}

    private void UpdateUI()
    {
        ScoreText.text = score.ToString();
        playerLifeSlider.value = (float)playerController.life / (float)playerController.lifeTotal;

        playerXPSlider.value = (float)playerController.xp / (float)playerController.nextXP;
        PowerText.text = playerController.powerLevel.ToString();

        var titlePos = TitleText.gameObject.transform.localPosition;

        if (GameTimer > 1)
        {
            if (titlePos.y > -5000)
            {
                TitleText.gameObject.transform.localPosition = new Vector3(titlePos.x, titlePos.y - 1, titlePos.z);
            }
        }

        //DebugText.text = mainCamera.transform.position.ToString();

        //DebugText.text = playerController.xp + "/" + playerController.nextXP;
       
    }

    public void addKills(long num)
    {
        kills += num;
        score += Mathf.RoundToInt(num * GameTimer);

        playerController.addXP(num);
    }

    private void SpawnStuff()
    {
        for (int i = 0; i < r.Next(10); i++)
        {
            SpawnMan();
        }

        if (GameTimer > 5 && r.NextDouble() < GameConfig.soldierSpawnPercent)
        {
            for (int i = 0; i < r.Next(3); i++)
            {
                SpawnSoldier();
            }
        }

        if (GameTimer > 20 && r.NextDouble() < GameConfig.tankSpawnPercent)
        {
            SpawnTank();
        }

        if (GameTimer > 40 && r.NextDouble() < GameConfig.heliSpawnPercent)
        {
            SpawnHelicopter();
        }
    }

    private void SpawnMan()
    {
        var tempMan = (GameObject)Instantiate(manPrefab);
        var sprite = tempMan.GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("Man" + r.Next(6));

        tempMan.transform.position = new Vector3(r.Next(-20, 20), 0, 0);
        objectList.Add(tempMan);
    }

    public void killMan(GameObject manObject)
    {
        if (playerController.isDiving)
        {
            addKills(GameConfig.manXP);
            playerController.Heal(5);
            playScream();
            Destroy(manObject);
            SpawnBloodSplatter(manObject.transform.position);
            objectList.Remove(manObject);
        }   
    }

    public void playScream()
    {
        if (r.NextDouble() <= GameConfig.screamPercent)
        {
            audio.PlayOneShot(deathSoundList[r.Next(deathSoundList.Count - 1)], (float)r.NextDouble());
        }
       
    }

    public void playCrashSound()
    {
        audio.PlayOneShot(crashSound, 1);
    }

    public void playMonsterGrowl(int num)
    {
        switch (num)
        {
            case 1:
                audio.PlayOneShot(monsterSound, (float)r.NextDouble());
                break;
            case   2:
                audio.PlayOneShot(monsterSound2, (float)r.NextDouble());
                break;
            case 3:
                audio.PlayOneShot(monsterSound3, (float)r.NextDouble());
                break;
            default: break;
        }
       
    }

    private void SpawnBloodSplatter(Vector3 pos)
    {
        audio.PlayOneShot(bloodSound, (float)r.NextDouble());
        var blood = Instantiate(bloodPrefab);
        blood.transform.position = pos;
    }

    private void SpawnExplosion(Vector3 pos)
    {

        audio.PlayOneShot(explosionSound, 1f);

        var explosion = Instantiate(explosionPrefab);
        objectList.Add(explosion);

        explosion.transform.position = pos;
        var eplosionRigidBody = explosion.GetComponent<Rigidbody2D>();
      

        //Explode outward
        Collider2D[] explosionArray = Physics2D.OverlapCircleAll(pos, explosion.GetComponent<SpriteRenderer>().bounds.extents.x);
        for (int i = 0; i < explosionArray.Length; i++)
        {
            Rigidbody2D rbody = explosionArray[i].gameObject.GetComponent<Rigidbody2D>();
            if (rbody != null)
            {
                Vector2 forceDirection = rbody.transform.position - pos;
                forceDirection.Normalize();
                rbody.AddForceAtPosition(forceDirection * 1500, pos);
            }
        }
    }

    public void SpawnShockwave(Vector3 pos)
    {
        var shockwave = Instantiate(shockwavePrefab);

        shockwave.transform.position = pos;
        CameraShake(.5f, GameConfig.CameraShakeAmountMed);
        audio.PlayOneShot(shockwaveSound, (float)r.NextDouble());
    }

    public void removeBloodSplatter(GameObject bloodSplatter)
    {
        Destroy(bloodSplatter);
    }

    public void SpawnSoldier()
    {
        var tempSoldier = (GameObject)Instantiate(soldierPrefab);
        audio.PlayOneShot(radioSound, (float)r.NextDouble());

        tempSoldier.transform.position = new Vector3(r.Next(-20, 20), 0, 0);
        objectList.Add(tempSoldier);
       
    }

    public void killSoldier(GameObject soldier)
    {
        if (playerController.isDiving)
        {
            addKills(GameConfig.soldierXP);
            playerController.Heal(10);
            playScream();
            Destroy(soldier);
            SpawnBloodSplatter(soldier.transform.position);
            objectList.Remove(soldier);
        }   
    }

    public void SpawnTank()
    {
        var tempTank = Instantiate(tankPrefab);
        tempTank.transform.position = new Vector3(r.Next(-20, 20), 0, 0);
        objectList.Add(tempTank);
    }

    public void KillTank(GameObject tank)
    {
        addKills(GameConfig.tankXP);
        SpawnExplosion(tank.transform.position);
        Destroy(tank);
        objectList.Remove(tank);

        CameraShake(.5f, GameConfig.CameraShakeAmountLarge);
    }

    public void SpawnHelicopter()
    {
        audio.PlayOneShot(helicopterSound, .5f);
        var tempHeli = Instantiate(helicopterPrefab);
        tempHeli.transform.position = new Vector3(r.Next(-20, 20), r.Next(7, 20), 0);
        objectList.Add(tempHeli);
    }

    public void KillHelicopter(GameObject heli)
    {
        addKills(GameConfig.heliXP);
        SpawnExplosion(heli.transform.position);
        Destroy(heli);
        objectList.Remove(heli);

        CameraShake(.2f, GameConfig.CameraShakeAmountMed);
    }

  

    public void spawnBullet(Collider2D shooterCollider, Vector3 pos, Vector3 direction, bool isTank)
    {

        GameObject tempBullet;
        if (isTank)
        {
            tempBullet = Instantiate(tankBulletPrefab);
        }
        else
        {
             tempBullet = Instantiate(bulletPrefab);
        }
        
        if (direction.x > 0)
        {
            pos = new Vector3(pos.x + 1, pos.y , pos.z);
        }
        else
        {
            pos = new Vector3(pos.x - 1, pos.y, pos.z);
        }
        tempBullet.transform.position = pos;

    
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        tempBullet.transform.Rotate(0, 0, angle);

        var bulletRigidBody = tempBullet.GetComponent<Rigidbody2D>();

        if (isTank)
        {
            bulletRigidBody.AddForce(new Vector2(direction.x * 10000, direction.y * 10000));
            audio.PlayOneShot(bigGunSound, (float)r.NextDouble());
        }
        else
        {
            bulletRigidBody.AddForce(new Vector2(direction.x * 1000, direction.y * 1000));

            if (r.NextDouble() < GameConfig.bulletPercent)
            {
                audio.PlayOneShot(gunSound, (float)r.NextDouble());
            }
        }

        var bulletCollider = tempBullet.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(shooterCollider, bulletCollider);

        objectList.Add(tempBullet);
      
       
    }

    public void RemoveBullet(GameObject bullet)
    {
        Destroy(bullet);
        objectList.Remove(bullet);
    }

    public void SpawnMissile(Vector3 pos)
    {

        GameObject tempMissile = Instantiate(missilePrefab);
        tempMissile.transform.position = pos;
        objectList.Add(tempMissile);

    }

    public void RemoveMissile(GameObject missile)
    {
        SpawnExplosion(missile.transform.position);
        Destroy(missile);
        objectList.Remove(missile);
    }

    public void CameraShake(float time, float amount)
    {

        CameraShakeTimer = time;
        CameraShakeAmount = amount;
    }

    private void UpdateCamera()
    {
        cameraDest = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, mainCamera.transform.position.z);
       var curPos = mainCamera.transform.position;
       var cameraPosX = Mathf.Lerp(curPos.x,cameraDest.x,  5* Time.deltaTime);
       var cameraPosY = Mathf.Lerp(curPos.y,cameraDest.y,  5*Time.deltaTime);


       if (CameraShakeTimer > 0)
       {
           CameraShakeTimer -= Time.deltaTime;

           cameraPosX += -CameraShakeAmount + (CameraShakeAmount + CameraShakeAmount) * (float)r.NextDouble();
           cameraPosY += -CameraShakeAmount + (CameraShakeAmount + CameraShakeAmount) * (float)r.NextDouble();
       }

        mainCamera.transform.position = new Vector3(cameraPosX,cameraPosY,curPos.z);


        var curZoom = mainCamera.orthographicSize;
        float newZoom = 8;
        if (playerController.isDiving)
        {
             newZoom = 5;    
        }

        mainCamera.orthographicSize = Mathf.Lerp(curZoom, newZoom, 5 * Time.deltaTime);


        //mainCamera.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, mainCamera.transform.position.z);
    }

    public void GameOver()
    {
        playMonsterGrowl(1);

        GameOverScoreText.text = "Final Score: " + score.ToString();
        UpdateUI();
        isGameOver = true;
        DisplayGameOverPopup();

    }

    private void DisplayGameOverPopup()
    {

        gameOverPopup.transform.localPosition = playerObject.transform.position;
    }

    public void Restart()
    {

        isGameOver = false;

        gameOverPopup.transform.position = new Vector3(5000, 5000, 0);

        GameTimer = 0;
        score = 0;

        playerController.ResetPlayer();
        foreach (var o in objectList)
        {
            Destroy(o);
        }
        objectList.Clear();

    }

    public void testPlaySound()
    {
       // audio.PlayOneShot(monsterSound, 1);
    }
}
