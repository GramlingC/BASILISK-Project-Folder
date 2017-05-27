using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{

    public float speedMultiplier = 2f;
    public int numberOfRays = 30;
    public float light_length = 7f;
    public LayerMask layerMask;
    int floorMask;
    Rigidbody player_rb;
    Light player_light;
    public Animator player_sprite;
    public float lightDiminishPerSecond = 5f;
    public float lightCrankedPerSecond = 15f;
    public float secondsUntilStartDiminish = 3f;
    public float maxLightAngle = 90;
    private float currentSecondsBeforeDiminish;

    private float crankTime;
    private float secondsCrankUntilDiminish;
    private KeyCode crankKeyPress;
    public bool crankingLight;
    public bool lightIsOn;
    private KeyCode lightSwitching = KeyCode.Mouse1;

    public float maxStamina = 15;
    private float stamina;
    public float TirePerSecond = 5f;
    public float RestPerSecond = 5f;

    private GUIStyle LightBoxStyle;
    private GUIStyle StaminaBoxStyle;

    public GameObject loss_screen;
    private bool loss;
    //Used to access methods from A_Pathfinding to constrain movement
    private A_Pathfinding pathfinder;
    private float leftBound;
    private float rightBound;
    private float bottomBound;
    private float topBound;

    private GameObject player;
    //Used for Gameover condition

    // Use this for initialization
    void Start()
    {
        player = gameObject;
        Game_Controller journals_script = GameObject.Find("GameController").GetComponent<Game_Controller>();
        journals_script.player = player;
        player_rb = GetComponent<Rigidbody>();
        crankingLight = false;
        currentSecondsBeforeDiminish = secondsUntilStartDiminish;
        maxLightAngle = Mathf.Max(player_light.spotAngle, maxLightAngle);
        crankTime = 0;
        secondsCrankUntilDiminish = 1;
        stamina = maxStamina;
        crankKeyPress = KeyCode.Mouse0;
        loss = false;
        InitStyle();

        //Sets boundaries
        pathfinder = GameObject.Find("PathfindingObj").GetComponent<A_Pathfinding>();
        leftBound = pathfinder.GetLeftBound();
        rightBound = pathfinder.GetRightBound();
        bottomBound = pathfinder.GetBottomBound();
        topBound = pathfinder.GetTopBound();
    }

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        player_light = GetComponentInChildren<Light>();
    }
    void FixedUpdate()
    {
        Turning();
    }
    void Update()
    {
        
        if (lightIsOn)
        {
            //Call CastLight
            CastLight();
            crankLight();
            gradualLightDiminish();
        }
        //Player Movement
        restMovement();
        playerActivateDeactivateLight();
        playerMovement();
        
        //player_rb.velocity = new Vector3(0, 0, 0);

    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Guard" || col.gameObject.tag == "Bat")
        {
            loss_screen.transform.Translate(0f, 0f, .5f);
            loss = true;
            Time.timeScale = 0;
            //print("Collided");

        }

    }
    void OnGUI()
    {
        if (loss == true)
        {
            GUI.Label(new Rect(200, 200, 300, 400), "Game Over press R to restart");
        }

        GUI.Box(new Rect(10, Screen.height - 20, (stamina / maxStamina) * Screen.width / 6, 20), "STAMINA " + (int)(stamina * 100 / maxStamina) + "/" + 100, StaminaBoxStyle);
        GUI.Box(new Rect(10, Screen.height - 50, ((player_light.spotAngle - 1) / (maxLightAngle - 1)) * Screen.width / 6, 20), "LIGHT " + (int)(player_light.spotAngle - 1) * 100 / (89) + "/" + 100, LightBoxStyle);


    }
    private void InitStyle()
    {
        LightBoxStyle = new GUIStyle();
        StaminaBoxStyle = new GUIStyle();
        Texture2D ltexture = new Texture2D(1, 1);
        Texture2D stexture = new Texture2D(1, 1);
        ltexture.SetPixel(0, 0, new Color(1f, 1f, 0.5f, 0.5f));
        stexture.SetPixel(0, 0, new Color(0.8f, 0f, 0f, 0.5f));
        ltexture.Apply();
        stexture.Apply();
        LightBoxStyle.normal.background = ltexture;
        StaminaBoxStyle.normal.background = stexture;
    }

    //Functions dealing wih diminishing light. Public functions are so that other scripts can call it also
    public void diminishLight(float angleAmount)
    {
        player_light.spotAngle -= angleAmount;
    }

    private void gradualLightDiminish()
    {
        currentSecondsBeforeDiminish -= Time.deltaTime;
        if (currentSecondsBeforeDiminish > 0)
            return;

        activatateOrDisableLight();
        if (crankingLight)
            return;
        if (player_light.spotAngle > 1)
            diminishLight(lightDiminishPerSecond * Time.deltaTime);
    }

    //Light's spotAngle can't go lower than 1, so if it is 1, then turn the light object off
    private void playerActivateDeactivateLight()
    {
        if (Input.GetKeyDown(lightSwitching))
        {
            lightIsOn = !lightIsOn;
            player_light.enabled = !player_light.enabled;
        }
    }

    private void activatateOrDisableLight()
    {
        player_light.gameObject.SetActive(player_light.spotAngle <= 1 ? false : true);
    }

    public bool lightIsDisabled()
    {
        return !player_light.gameObject.activeSelf;
    }

    public void increaseLight(float angleAmount)
    {
        if (player_light.spotAngle < maxLightAngle)
            player_light.spotAngle += angleAmount;
    }

    private void crankLight()
    {
        if (Input.GetKey(crankKeyPress))
        {
            crankingLight = true;
            crankTime = Mathf.Clamp(crankTime += Time.deltaTime, 0, secondsCrankUntilDiminish);
            increaseLight(lightCrankedPerSecond * Time.deltaTime);
            player_rb.velocity = Vector3.zero;
        }
        else
        {
            crankingLight = false;
        }
        if (Input.GetKeyUp(crankKeyPress))
        {
            currentSecondsBeforeDiminish = crankTime * secondsCrankUntilDiminish;
            crankTime = 0;
        }
    }

    //For controlling player movement
    private void playerMovement()
    {

        float mov_V = Input.GetAxis("Vertical") * getSpeed();
        float mov_H = Input.GetAxis("Horizontal") * getSpeed();

        if (mov_V == 0 & mov_H == 0)
        {
            player_sprite.SetBool("isWalking", false);
            player_rb.velocity = Vector3.zero;
        }
        else
        {
            player_sprite.SetBool("isWalking", true);
            Vector3 move = new Vector3(mov_H, 0, mov_V);
            player_rb.velocity = Vector3.ClampMagnitude(move, speedMultiplier * speedKeys());


            if (Mathf.Abs(mov_V) > Mathf.Abs(mov_H))
            {
                if (mov_V > 0)
                    player_sprite.SetInteger("Direction", 3);
                else
                    player_sprite.SetInteger("Direction", 1);
            }
            else if (Mathf.Abs(mov_V) <= Mathf.Abs(mov_H))
            {
                if (mov_H > 0)
                    player_sprite.SetInteger("Direction", 2);
                else
                    player_sprite.SetInteger("Direction", 4);
            }
            else { }        //If vertical and horizontal speeds are equal, keep the previous sprite
        }

    }

    // Update is called once per frame
    void Turning()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit cursor;

        if (Physics.Raycast(ray, out cursor, 20f, floorMask))
        {
            Vector3 player_ray = cursor.point - transform.position;

            player_ray.y = 0.0f;

            Quaternion newRotation = Quaternion.LookRotation(player_ray);

            player_rb.MoveRotation(newRotation);

        }

    }
    //Prototype method for player light casting: Raycasts in a fan, Raycasts trigger enemy behavior
    //It doesn't seem to be working well, though...
    void CastLight()
    {
        //Only when light is enabled
        if (lightIsDisabled())
            return;
        //To simplify later code
        float angle_radius = player_light.spotAngle / 2;
        float angle_increment = player_light.spotAngle / numberOfRays;
        //float light_length = player_light.range;
        //Iterate through angles
        for (float degree = -angle_radius; degree < angle_radius; degree += angle_increment)
        {
            //Code to calculate direction based on degree and Vector3.forward
            Vector3 LightDirection = Quaternion.AngleAxis(degree, Vector3.up) * transform.forward;

            //Cast ray
            RaycastHit hit;
            if (Physics.Raycast(transform.position, LightDirection, out hit, light_length,layerMask))
            {
                //Trigger enemy behavior
                if (hit.transform.gameObject.tag == "Guard")
                {
                    Guard_Controller_v2 enemy = (Guard_Controller_v2)hit.transform.gameObject.GetComponent(typeof(Guard_Controller_v2));
                    enemy.LightTrigger();
                }
                else if (hit.transform.gameObject.tag == "Bat")
                {
                    Bat_Controller enemy = (Bat_Controller)hit.transform.gameObject.GetComponent(typeof(Bat_Controller));
                    enemy.LightTrigger(gameObject);
                }
            }
        }

    }

    public void increaseStamina(float amount)
    {
        if (stamina < maxStamina)
            stamina += amount;
    }
    public void decreaseStamina(float amount)
    {
        if (stamina > 0)
            stamina -= amount;
    }
    private void restMovement()
    {
        if (Input.GetAxis("Run") > 0)
        {
            decreaseStamina(TirePerSecond * Time.deltaTime);
        }
        else
        {
            increaseStamina(RestPerSecond * Time.deltaTime);
        }
    }

    private float getSpeed()
    {
        float speed = speedMultiplier;
        speed *= speedKeys();
        if (crankingLight)
            speed *= .5f;
        return speed;
    }
    private float speedKeys()
    {
        if (Input.GetAxis("Run") > 0 && stamina > 0)
            return 1.5f * (stamina + 50) / 50;
        else if (Input.GetAxis("Run") < 0)
            return 0.5f;
        else
            return 1f;
    }
}
