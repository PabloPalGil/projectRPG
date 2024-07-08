using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Rigidbody2D theRB;
    public float moveSpeed;

    public Animator myAnim;

    public static PlayerController instance;

    public string areaTransitionName;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    public bool canMove = true;

    public Vector2 direction;

    public bool canDialog;
    public bool canPickup;
    public GameObject interactableObject;

    //Skills system:
    public bool usingSkill;
    private GameObject skillsContainer;
    private Animator[] skillAnims;
    private CircleCollider2D skillColl;
    private string skillTrigger;
    private Vector2 skillTempPos;
    public int skillEquipped;
    public float skillFreezeDuration;
    public bool electroInverse;//Se activa al detectar un doble tap desde TouchUI, si no, atrae.
    public BoxCollider2D skillBoxColl;//Collider cuando tiene forma de box2D (electro...)

    //Footsteps
    public int footstepSound;

    //Falling
    public bool falling;//Bool que indica que el player se ha caido en un abismo y se está gestionando la caida mediante Fall()
    public Vector3 enterPoint;//Punto de entrada en cualquier escena al que vuelves si te caes por un abismo...
    public Vector2 lastMoveXY;//Los dos valores para determinar la dirección a la que debe mirar el player al reaparecer
    public bool levitating;//Var que indica que el player está levitando y no le afectan los abismos (skill electro)

    //Variable para indicar si el player está en una zona de batalla activa
    public bool inBattleArea;

    //Luces
    //public Light2D viewLight;
    public GameObject viewLight;

    //Correr:
    public float runSpeedMod;

    //Joystick
    public Joystick joystick;

    //Camera
    public CameraController cam;//Referencia a la camara

    //Dirección a la que mira el player:
    public bool lookingUp;
    public bool lookingDown;
    public bool lookingLeft;
    public bool lookingRight;


	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        } else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        //StartUp comprobations:
        //joystick = GameMenu.instance.touchJoystick.transform.GetChild(0).GetComponent<Joystick>();
        StartCoroutine(StartCo());

        DontDestroyOnLoad(gameObject);
	}

    public IEnumerator StartCo()
    {
        StartCoroutine(LoadStartCo());//Este método aplica al cargar el juego (al cambiar entre escenas aplica un método muy parecido de AreaEntrance)
        
        skillAnims = transform.GetChild(0).GetComponentsInChildren<Animator>();
        skillsContainer = transform.GetChild(0).gameObject;
        viewLight = transform.GetChild(1).gameObject;
        runSpeedMod = 1;
        joystick = GameMenu.instance.touchJoystick.transform.GetChild(0).GetComponent<Joystick>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        yield return null;
    }

    // Update is called once per frame
    void Update () {
        if (canMove)
        {
            //Movimiento con joystick:
            //SOLO APLICA EN ANDROID O IOS
            if (/*Application.platform == RuntimePlatform.Android || */Application.platform == RuntimePlatform.IPhonePlayer)
            {
                theRB.velocity = joystick.Direction * moveSpeed * runSpeedMod;
            }
            else// if (Application.isEditor)//En modo editor aplica tanto D-Pad como joystick via mouse (y se suman si se usan a la vez)
            {
                //Movimiento con D-PAD / Joystick / Botones:
                theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") + direction.x, Input.GetAxisRaw("Vertical") + direction.y) * moveSpeed * runSpeedMod;

                theRB.velocity += joystick.Direction * moveSpeed * runSpeedMod;

            }

            myAnim.SetFloat("moveX", theRB.velocity.x);
            myAnim.SetFloat("moveY", theRB.velocity.y);
        }
        else
        {
            theRB.velocity = Vector2.zero;
            direction = Vector2.zero;
        }

        if (!GameManager.instance.storyEvent)
        {
            myAnim.SetFloat("moveX", theRB.velocity.x);
            myAnim.SetFloat("moveY", theRB.velocity.y);
        }

        //Con Joystick:
        if (/*Application.platform == RuntimePlatform.Android || */Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (joystick.Direction != Vector2.zero)
            {
                if (canMove)
                {
                    //myAnim.SetFloat("lastMoveX", Snapping.Snap(joystick.Horizontal, 1));
                    //myAnim.SetFloat("lastMoveY", Snapping.Snap(joystick.Vertical, 1));
                    if (Mathf.Abs(joystick.Direction.x) > Mathf.Abs(joystick.Direction.y))
                    {
                        if (runSpeedMod != 1)
                        {
                            myAnim.speed = 1.4f;
                        }
                        else
                        {
                            myAnim.speed = Mathf.Abs(joystick.Direction.x);
                        }

                        myAnim.SetFloat("lastMoveY", 0);

                        if (joystick.Horizontal > 0)
                        {
                            myAnim.SetFloat("lastMoveX", 1);
                        }
                        else
                        {
                            myAnim.SetFloat("lastMoveX", -1);
                        }
                    }
                    else if (Mathf.Abs(joystick.Direction.x) < Mathf.Abs(joystick.Direction.y))
                    {
                        if (runSpeedMod != 1)
                        {
                            myAnim.speed = 1.4f;
                        }
                        else
                        {
                            myAnim.speed = Mathf.Abs(joystick.Direction.y);
                        }

                        myAnim.SetFloat("lastMoveX", 0);
                        if (joystick.Vertical > 0)
                        {
                            myAnim.SetFloat("lastMoveY", 1);
                        }
                        else
                        {
                            myAnim.SetFloat("lastMoveY", -1);
                        }
                    }
                }
            }
        }
        else// if(Application.isEditor)//En modo editor aplica tanto D-Pad como joystick via mouse (y se suman si se usan a la vez)
        {
            //Con joystick
            if (joystick.Direction != Vector2.zero)
            {
                if (canMove)
                {
                    if (Mathf.Abs(joystick.Direction.x) > Mathf.Abs(joystick.Direction.y))
                    {
                        if (runSpeedMod != 1)
                        {
                            myAnim.speed = 1.4f;
                        }
                        else
                        {
                            myAnim.speed = Mathf.Abs(joystick.Direction.x);
                        }

                        myAnim.SetFloat("lastMoveY", 0);

                        if (joystick.Horizontal > 0)
                        {
                            myAnim.SetFloat("lastMoveX", 1);
                        }
                        else
                        {
                            myAnim.SetFloat("lastMoveX", -1);
                        }
                    }
                    else if (Mathf.Abs(joystick.Direction.x) < Mathf.Abs(joystick.Direction.y))
                    {
                        if (runSpeedMod != 1)
                        {
                            myAnim.speed = 1.4f;
                        }
                        else
                        {
                            myAnim.speed = Mathf.Abs(joystick.Direction.y);
                        }

                        myAnim.SetFloat("lastMoveX", 0);
                        if (joystick.Vertical > 0)
                        {
                            myAnim.SetFloat("lastMoveY", 1);
                        }
                        else
                        {
                            myAnim.SetFloat("lastMoveY", -1);
                        }
                    }
                }
            }
            //Con D-PAD:
            if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1 || direction != Vector2.zero)
            {
                if (canMove)
                {
                    myAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal") + direction.x);
                    myAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical") + direction.y);
                }
            }
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);

        if(viewLight != null)
        {
            UpdateViewLight();
        }
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        bottomLeftLimit = botLeft + new Vector3(.5f, 1f, 0f);
        topRightLimit = topRight + new Vector3(-.5f, -1f, 0f);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Este if es para aplicar lo que hace el boton A tactil:
        if(other.tag == "Chest")
        {

        }
        if(other.GetComponent<DialogActivator>() != null)
        {
            canDialog = true;
            interactableObject = other.gameObject;
            if (other.GetComponent<PickupItem>())
            {
                canPickup = true;
            }
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<DialogActivator>() != null)
        {
            canDialog = false;
            canPickup = false;
            interactableObject = null;
        }
    }

    //Skills
    public void UseSkill(int i)
    {
        skillEquipped = i;

        //Según la skill que sea:
        if (i == 0)//Fire
        {
            skillTrigger = "Fire";
            if (!usingSkill)
            {
                usingSkill = true;
                StartCoroutine("UseSkillFireCo");
            }
        }
        else if (i == 1)//Freeze
        {
            skillTrigger = "Freeze";
            if (!usingSkill)
            {
                usingSkill = true;
                skillFreezeDuration = 3f;

                //Referencio el collider
                skillColl = skillAnims[skillEquipped].gameObject.GetComponent<CircleCollider2D>();

                //Le digo al shader qué color tiene que usar con esta skill:
                skillColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.blue;
                skillAnims[skillEquipped].SetTrigger(skillTrigger);
                skillColl.enabled = true;

                StartCoroutine("UseSkillFreezeCo");
            }
        }
        else if(i == 2)//Electro
        {
            if (!usingSkill)
            {
                usingSkill = true;

                //referencio el collider
                skillBoxColl = skillAnims[skillEquipped].gameObject.GetComponent<BoxCollider2D>();

                GameManager.instance.castingSkill = true;

                //Color del shader
                if (!electroInverse)
                {
                    Debug.Log("Using electro-");
                    skillTrigger = "Electro-";
                    skillBoxColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.yellow;

                    skillAnims[skillEquipped].SetBool(skillTrigger, true);
                    skillBoxColl.enabled = true;

                    StartCoroutine("UseSkillElectroCo");
                }
                else
                {
                    Debug.Log("Using electro+ (double tap)");
                    skillTrigger = "Electro+";
                    skillBoxColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.blue;

                    skillAnims[skillEquipped].SetBool(skillTrigger, true);
                    skillBoxColl.enabled = true;

                    StartCoroutine("UseSkillElectroCo");
                }
            }
        }
        else if (i == 3)//Earthquake
        {
            skillTrigger = "Earthquake";
            if (!usingSkill)
            {
                usingSkill = true;
                StartCoroutine("UseSkillEarthquakeCo");
            }
        }
        else if(i == 4)//Water
        {
            skillTrigger = "Water";
            if (!usingSkill)
            {
                usingSkill = true;
                StartCoroutine("UseSkillWaterCo");
            }
        }
        else if(i == 5)//Wind
        {
            skillTrigger = "Wind";
            if (!usingSkill)
            {
                usingSkill = true;
                StartCoroutine("UseSkillWindCo");
            }
        }
    }

    public IEnumerator UseSkillFireCo()
    {
        skillColl = skillAnims[skillEquipped].gameObject.GetComponent<CircleCollider2D>();

        //Ubico el child Skills (para que las skills se instancien enfrente del player segun donde mire)
        Vector2 skillOffset = new Vector2(0, 0);
        if (myAnim.GetFloat("lastMoveX") == 0 && myAnim.GetFloat("lastMoveY") == 0)
        {
            skillOffset = Vector2.down;
        }
        else
        {
            if (myAnim.GetFloat("lastMoveX") == 1)
            {
                skillOffset = Vector2.right;
            }
            if (myAnim.GetFloat("lastMoveX") == -1)
            {
                skillOffset = Vector2.left;
            }
            if (myAnim.GetFloat("lastMoveY") == 1)
            {
                skillOffset = Vector2.up;
            }
            if (myAnim.GetFloat("lastMoveY") == -1)
            {
                skillOffset = Vector2.down;
            }
        }

        skillTempPos = new Vector2(transform.position.x, transform.position.y) + skillOffset;

        skillColl.radius = 1f;
        //Le digo al shader qué color tiene que usar con esta skill:
        skillColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
        skillAnims[skillEquipped].SetTrigger(skillTrigger);

        GameManager.instance.castingSkill = true;
        StartSkillAnim();

        skillColl.enabled = true;
        skillColl.transform.position = skillTempPos;

        yield return new WaitForSecondsRealtime(0.683f);

        skillColl.enabled = false;
        StopSkillAnim();
        GameManager.instance.castingSkill = false;
        usingSkill = false;
    }

    public IEnumerator UseSkillFreezeCo()
    {
        yield return new WaitForSecondsRealtime(skillFreezeDuration);

        skillColl.enabled = false;

        usingSkill = false;

        yield return null;
    }


    public IEnumerator UseSkillElectroCo()
    {
        while (usingSkill)
        {
            yield return null;
        }

        Debug.Log("Stop electro");
        skillBoxColl.enabled = false;
        skillAnims[skillEquipped].SetBool(skillTrigger, false);
        GameManager.instance.castingSkill = false;
        electroInverse = false;
    }


    public IEnumerator UseSkillEarthquakeCo()
    {
        skillColl = skillAnims[skillEquipped].gameObject.GetComponent<CircleCollider2D>();

        //Le digo al shader qué color tiene que usar con esta skill:
        skillColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.grey;
        skillAnims[skillEquipped].SetTrigger(skillTrigger);

        GameManager.instance.castingSkill = true;
        StartSkillAnim();

        skillColl.enabled = true;

        //Invoco al shake de cámara con una magnitud dada:
        StartCoroutine(cam.ShakeCo(1));

        yield return new WaitForSecondsRealtime(0.5f);

        skillColl.enabled = false;
        StopSkillAnim();
        GameManager.instance.castingSkill = false;
        usingSkill = false;

        yield return null;
    }


    public IEnumerator UseSkillWaterCo()
    {
        //Referencio el collider
        skillColl = skillAnims[skillEquipped].gameObject.GetComponent<CircleCollider2D>();

        skillColl.radius = 0.3f;
        //Le digo al shader qué color tiene que usar con esta skill:
        skillAnims[skillEquipped].gameObject.GetComponent<SpriteRenderer>().material.color = Color.blue;
        skillAnims[skillEquipped].SetTrigger(skillTrigger);

        GameManager.instance.castingSkill = true;
        StartSkillAnim();

        //Ubico el child Skills (para que las skills se instancien enfrente del player segun donde mire)
        Vector2 skillOffset = new Vector2(0, 0);

        //Utilizando las variables directas del animator:
        if (lookingUp)
        {
            skillOffset = Vector2.up;
        }
        else if(lookingRight)
        {
            skillOffset = Vector2.right;
        }
        else if (lookingLeft)
        {
            skillOffset = Vector2.left;
        }
        else
        {
            skillOffset = Vector2.down;
        }

        skillTempPos = new Vector2(transform.position.x, transform.position.y) + skillOffset;

        skillColl.enabled = true;
        skillColl.transform.position = skillTempPos;

        yield return null;
        //Ahora hay que programar la lógica de solidificar la lava en el tile donde ha caído el agua:
        //Llamo a la corutina de SkillWater (le meto un offset ejeY negativo para ajustar el collider al punto donde cae la gota de agua de la animación):
        skillTempPos.y -= 0.7f;
        StartCoroutine(skillAnims[skillEquipped].gameObject.GetComponent<SkillWater>().WaterSkillCo(skillTempPos));

        yield return new WaitForSecondsRealtime(0.63f);

        skillColl.enabled = false;
        StopSkillAnim();
        GameManager.instance.castingSkill = false;
        usingSkill = false;
    }

    public IEnumerator UseSkillWindCo()
    {
        skillColl = skillAnims[skillEquipped].gameObject.GetComponent<CircleCollider2D>();

        //Le digo al shader qué color tiene que usar con esta skill:
        skillColl.gameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
        skillAnims[skillEquipped].SetTrigger(skillTrigger);

        GameManager.instance.castingSkill = true;
        StartSkillAnim();

        skillColl.enabled = true;

        yield return new WaitForSecondsRealtime(1f);

        skillColl.enabled = false;
        StopSkillAnim();
        GameManager.instance.castingSkill = false;
        usingSkill = false;

        yield return null;
    }



    //Puedo poner funciones en las animaciones, como la de los footsteps
    public void Footstep()
    {
        AudioManager.instance.PlayRandomSFX(footstepSound);
    }


    //Activar animación de skill:
    public void StartSkillAnim()
    {
        Debug.Log("Start SkillAnim");
        if (lookingDown)
        {
            myAnim.SetBool("skillDown", true);
            myAnim.SetBool("skillUp", false);
            myAnim.SetBool("skillRight", false);
            myAnim.SetBool("skillLeft", false);
        }
        else if (lookingUp)
        {
            myAnim.SetBool("skillDown", false);
            myAnim.SetBool("skillUp", true);
            myAnim.SetBool("skillRight", false);
            myAnim.SetBool("skillLeft", false);
        }
        else if (lookingRight)
        {
            myAnim.SetBool("skillDown", false);
            myAnim.SetBool("skillUp", false);
            myAnim.SetBool("skillRight", true);
            myAnim.SetBool("skillLeft", false);
        }
        else if (lookingLeft)
        {
            myAnim.SetBool("skillDown", false);
            myAnim.SetBool("skillUp", false);
            myAnim.SetBool("skillRight", false);
            myAnim.SetBool("skillLeft", true);
        }
    }

    public void StopSkillAnim()
    {
        Debug.Log("Stop skillAnim");
        myAnim.SetBool("skillDown", false);
        myAnim.SetBool("skillUp", false);
        myAnim.SetBool("skillRight", false);
        myAnim.SetBool("skillLeft", false);
    }

    public void LookingUp()
    {
        lookingUp = true;
        lookingDown = false;
        lookingRight = false;
        lookingLeft = false;
    }
    public void LookingDown()
    {
        lookingUp = false;
        lookingDown = true;
        lookingRight = false;
        lookingLeft = false;
    }
    public void LookingRight()
    {
        lookingUp = false;
        lookingDown = false;
        lookingRight = true;
        lookingLeft = false;
    }
    public void LookingLeft()
    {
        lookingUp = false;
        lookingDown = false;
        lookingRight = false;
        lookingLeft = true;
    }


    private void UpdateViewLight()
    {
        Vector3 lightEulerAngles;

        if(myAnim.GetFloat("lastMoveX") == 1)
        {
            lightEulerAngles = new Vector3(0, 0, 270);
            viewLight.transform.eulerAngles = lightEulerAngles;
        }
        else if (myAnim.GetFloat("lastMoveX") == -1)
        {
            lightEulerAngles = new Vector3(0, 0, 90);
            viewLight.transform.eulerAngles = lightEulerAngles;
        }
        else if (myAnim.GetFloat("lastMoveY") == 1)
        {
            lightEulerAngles = new Vector3(0, 0, 0);
            viewLight.transform.eulerAngles = lightEulerAngles;
        }
        else if (myAnim.GetFloat("lastMoveX") == -1)
        {
            lightEulerAngles = new Vector3(0, 0, 180);
            viewLight.transform.eulerAngles = lightEulerAngles;
        }
        else
        {
            lightEulerAngles = new Vector3(0, 0, 180);
            viewLight.transform.eulerAngles = lightEulerAngles;
        }

    }

    //Función que siempre devuelve true, es para comprobar que este script ya ha cargado y está funcional desde el resto de scripts
    public bool CheckIfLoaded()
    {
        return true;
    }

    //Función que devuelve true si todos los essential scripts están funcionales o falso si no.
    public bool CheckEverythingIsLoaded()
    {
        int i = 0;//Var para contar scripts esenciales cargados

        if (BattleManager.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("BattleManager loaded");
        }
        if (LevelManager.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("LevelManager loaded");
        }
        if (GameMenu.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("GameMenu loaded");
        }
        if (GameManager.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("GameManager loaded");
        }
        if (QuestManager.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("QuestManager loaded");
        }
        if (AudioManager.instance.CheckIfLoaded())
        {
            i++;
            Debug.Log("AudioManager loaded");
        }


        if (i == 6)
        {
            Debug.Log("Everything is loaded");
            return true;
        }
        else
        {
            Debug.Log("Something is not loaded yet...");
            return false;
        }
    }


    //Corrutina para cargar todo bien al cargar el juego
    public IEnumerator LoadStartCo()
    {
        Debug.Log("Cargando todo via PlayerController");
        bool loaded = false;

        //CheckIfLoaded dará false si el GameManager no ha cargado y true si sí:
        while (!loaded)
        {
            Debug.Log("Esperando al PlayerController");
            if (PlayerController.instance.CheckIfLoaded())
            {
                loaded = PlayerController.instance.CheckIfLoaded();
            }
            yield return null;
        }

        loaded = false;

        //Comprobar que el resto de scripts esenciales ya están listos:
        while (!loaded)
        {
            Debug.Log("Esperando al resto de scripts esenciales...");
            if (PlayerController.instance.CheckEverythingIsLoaded())
            {
                loaded = PlayerController.instance.CheckEverythingIsLoaded();
            }
            yield return null;
        }

        Debug.Log("TODO CARGADO!");

        //Asigno lightView al player:
        StartCoroutine(GameObject.FindGameObjectWithTag("Weather").GetComponent<Weather>().GetWeatherCo());

        yield return new WaitForEndOfFrame();

        if (!UIFade.instance.blackScreen)
        {
            Debug.Log("AreaEntrance fadeFromBlack");
            UIFade.instance.FadeFromBlack();
        }

        //GameManager.instance.fadingBetweenAreas = false;

    }

    public void Fall()
    {
        if (!falling && !levitating)
        {
            falling = true;
            StartCoroutine(FallingCo());
        }
    }

    public IEnumerator FallingCo()
    {
        GameManager.instance.playerFalling = true;

        transform.position = transform.position + new Vector3(0, -0.5f, 0);
        yield return new WaitForEndOfFrame();

        myAnim.SetTrigger("fall");

        AudioManager.instance.PlaySFX(63);

        yield return new WaitForSecondsRealtime(0.7f);

        transform.position = enterPoint;
        myAnim.SetFloat("lastMoveX", lastMoveXY.x);
        myAnim.SetFloat("lastMoveY", lastMoveXY.y);


        yield return new WaitForSecondsRealtime(0.4f);

        yield return new WaitForEndOfFrame();
        falling = false;
        GameManager.instance.playerFalling = false;
    }

}
