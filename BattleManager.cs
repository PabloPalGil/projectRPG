using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{

    public static BattleManager instance;

    private bool battleActive;

    public GameObject battleScene;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;

    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;
    public GameObject highlightAttackerEffect;

    public DamageNumber theDamageNumber;

    public GameObject[] playerStatsHolder;
    public Text[] playerName, playerHP, playerMP;
    public Slider[] playerSliderHP, playerSliderMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    //Esta var se usa para almacenar el move que se ha seleccionado y se desea hacer, y pillar sus propiedades para unificar todo:
    public BattleMove activeMove;

    //Este bool se usa para que lo que hay en el update sólo se ejecute una vez:
    private bool turnJustStarted = true;

    //Bool para saber si estoy en medio de un multitarget:
    private bool doingMultiTarget;

    //Testing items
    public GameObject itemsMenu;
    public ItemButton[] battleItemButtons;
    public string battleSelectedItem;
    public Item battleActiveItem;
    public Text itemName, itemDescription;
    public GameObject useItemButton;
    public Text useButtonText;

    public GameObject itemBattleChoiceMenu;
    public Text[] itemCharChoiceNames;

    public BattleNotification battleNotice;
    public BattleNotification battleMessage;

    public int chanceToFlee = 35;
    private bool fleeing;
    private bool fleeFailed;

    public string gameOverScene;

    public int rewardEXP;
    //public string[] rewardItems;
    public List<string> rewardItems;
    public int rewardGold;
    public bool isEventBattle;

    public bool cannotFlee;

    //Weakness system
    public Sprite iconSword, iconAxe, iconBow, iconStaff, iconFire, iconIce, iconThunder, iconWind, iconWater, iconEarth, iconDark, iconLight;
    public Sprite[] weaknessSprites;//Aquí guardo los 12 sprites de los iconos de debilidades.
    public List<string> weaknessStringList;//Aquí guardo los strings de las debilidades utiles de cada enemigo
    public List<Sprite> weaknessSpriteList;//Aquí guardo los sprites de las debilidades utiles de cada enemigo
    public Sprite unknownIcon;//Sprite de debilidad de tipo desconocido
    public Sprite iconSelectPlayer, iconSelectEnemy, iconPunch;
    public string typeOfAttack;
    public bool shouldWeakenShield;
    private List<string> tempEnemyWeaknessList;//Obsoleto
    private List<Sprite> weaknessSpritesList;//Obsoleto

    public List<string> equippeddWeaponsList;
    public string usingWeapon;
    public Item activeWeapon;//Referencia del Item arma actualmente equipada y activa
    public int weaponPositionInList;//int para indicar qué posición de la lista de armas equipadas usa el player actual
    public GameObject changeWeaponButton;
    public Sprite shieldSprite;
    public Sprite shieldBrokenSprite;



    //Sistema de energy / focus
    public GameObject[] energyHolder;
    public GameObject itemButton;
    public GameObject fleeButton;
    public float focusMultiplier = 1.8f;
    public GameObject focus1Vfx, focus2Vfx, focus3Vfx;

    //Select target and back buttons
    public GameObject targetBackButton;
    public GameObject targetAttackButton;
    public int currentSelectedTarget;
    private bool currentMoveIsMultiTarget;
    public string currentMoveName;
    private int spellCostDebt;
    private List<int> Targets;

    //Sistema cambios de estado:
    public GameObject[] statsHolder;//Arrastro aquí los stat icons holder de cada player
    public GameObject[] statPosHolder;//Arrastro el parent de las Posiciones de los statIcons
    //public GameObject[] statsIcons;//Arrastro aquí cada icono de cambio de estado (el gameobject entero)

    //Sistema de orden de turnos
    public List<BattleChar> currentRoundList;//Aquí se ordenan los battlers según ronda actual
    public List<BattleChar> nextRoundList;//Aquí se ordenan los battlers según siguiente ronda
    public GameObject currentRoundHolder;//Arrastro aquí el listado con las posiciones de la ronda actual
    public GameObject nextRoundHolder;//Arrastro aquí el listado con las posiciones de la ronda siguiente
    public bool turnVerified = false;//Uso esta bool para comprobar que el currentTurn se aplica a quien debe.

    public List<BattleChar> checkRepeatedBattlers;//Uso esta lista para comprobar que si hay enemigos duplicados,
    //les asigno un nombre diferente para diferenciarlos.


    //Background
    public SpriteRenderer backgroundRenderer;


    //tutorials/especialBattles
    public bool specialBattle;
    public string eventBattleRef;//Nombre de la corrutina que gestiona la batalla (Ej: "Tutorial1Co")
    public string[] eventObjectNames;
    //Events triggered by round number:
    public string[] eventRoundMessages;
    public int[] eventTriggerRounds;
    public string[] eventCharTriggerRounds;
    //Events triggered by hp conditions:
    public string[] eventHpMessages;//Mensajes a mostrar según condiciones por hp
    public int[] eventHpConditions;//Hp a partir del cual se activa el evento de batalla (si aplica)
    public string[] eventCharHpConditions;//Chars en que aplican los hp conditions (en orden)

    public Button[] buttons;

    public bool forceLoseBattle;//Si la batalla termina al perder por exigencias del guion


    //Variable que cuenta las rondas (desde 1)
    public int currentRound;

    //UI
    public Color buttonsColor = new Color(27, 20, 100);
    public Color buttonsColorFaded = new Color(27, 20, 100, 0.3f);
    public Sprite[] usingWeaponIcons;
    public Image[] lightButtonsUI;
    public Image[] darkButtonsUI;
    public Image[] lightBgUI;
    public Image[] darkBgUI;



    //Active Player position:
    public Transform activePos;
    private bool playerMoving;//Bool para indicar si algún player está moviendose hacia o desde la ActivePos
    private BattleChar activePosPlayer;//Esta posición debe ocuparla el player que va a la active position y sólo 1 a la vez

    //Almaceno la musica que estaba sonando cuando empezó el combate (para retormarlo luego)
    public int pausedMusic;
    //public float pausedAtTime;

    //VFX
    public AttackEffect playerAttacksVfx;
    public AttackEffect enemyAttacksVfx;
    public AttackEffect playerHitVfx;
    public AttackEffect physicalHitVfx;
    public AttackEffect specialHitVfx;
    public AttackEffect shieldBreakVfx;
    public AttackEffect monsterkillVfx;
    public AttackEffect monsterkillVfx2;

    //Colores de los shaders emission para los vfx según enemigo/player/poison, etc:
    public Color emissionColorDefaultPlayers;
    public Color emissionColorDefaultEnemy;
    public Color emissionColorPoison;
    public Color emissionColorBloody;
    public Color emissionColorDark;

    //Que los battlers no puedan hacer nada hasta que se haga la luz:
    public bool readyForBattle;

    //Lista de players
    public List<BattleChar> battlePlayers;


    

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.T))
        //      {
        //          BattleStart(new string[] {"Bad Weed", "Bad Weed", "Bad Weed", "Bad Weed"}, false);
        //      }

        if (battleActive)
        {
            if (turnWaiting && readyForBattle)
            {
                if (!turnVerified)
                {
                    Debug.Log("Turn verification");
                    turnVerified = true;

                    //Comprobar que a quien le toca no ha actuado ya (las posiciones de los chars
                    //se modifican a menudo) y actualizar el currentTurn
                    for (int i = 0; i < activeBattlers.Count; i++)
                    {
                        if (!activeBattlers[i].turnDone)
                        {
                            currentTurn = i;
                            activeBattlers[i].turnDone = true;
                            Debug.Log("Current turn is " + currentTurn + " and is for " + activeBattlers[i].tempName);
                            if (activeBattlers[i].hasDied)
                            {
                                Debug.Log(activeBattlers[i].tempName + " is dead");
                                continue;//Termino esta iteración ya y paso a la siguiente iteracion
                            }
                            else
                            {
                                break;//Salgo y me salto el resto del bucle for
                            }
                        }
                    }
                }

                if (!activeBattlers[currentTurn].justUsedFocus)//Para ocultar el menu entre ataques reiterados por focus
                {
                    if (activeBattlers[currentTurn].isPlayer)
                    {
                        //lo siguiente sólo debe ejecutarse una vez cada vez que le toca a algun player:
                        if (turnJustStarted)
                        {
                            GetEquippedWeapons();
                            SetWeaponButtonSprite();
                            Debug.Log("comienza turno del player " + activeBattlers[currentTurn].tempName);
                            //Comprobar si algún estado alterado me impide actuar:
                            if (activeBattlers[currentTurn].hasDied)
                            {
                                Debug.Log(activeBattlers[currentTurn].charName + " is dead...");
                                NextTurn();
                            }
                            else if (activeBattlers[currentTurn].turnsSleep > 0)
                            {
                                Debug.Log(activeBattlers[currentTurn].charName + " is sleeping...");
                                NextTurn();
                            }
                            else if (fleeFailed)
                            {
                                Debug.Log(activeBattlers[currentTurn].charName + " failed to escape...");
                                NextTurn();
                            }
                            else
                            {
                                //Muevo al player a la Active Position con una corrutina (solo si hay más de 1 player en el grupo):
                                if(GetPlayers() > 1)
                                {
                                    StartCoroutine(MoveToActivePositionCo(activeBattlers[currentTurn]));
                                }

                                uiButtonsHolder.SetActive(true);
                                for (int i = 0; i < buttons.Length; i++)
                                {
                                    buttons[i].gameObject.SetActive(true);
                                }
                                //Si estoy en una batalla tutorial triggereo aquí las instrucciones:
                                if (specialBattle)
                                {
                                    //LLamo a la Corutina indicada por el propio BattleStarter:
                                    StartCoroutine(eventBattleRef);
                                }
                            }
                        }
                    }
                    else//Si no es player
                    {
                        if (turnJustStarted)
                        {
                            Debug.Log("comienza turno del enemy " + activeBattlers[currentTurn].tempName);
                            turnJustStarted = false;
                            uiButtonsHolder.SetActive(false);
                            

                            //Si estoy en una batalla tutorial triggereo aquí las instrucciones:
                            if (specialBattle)
                            {
                                Debug.Log("Enemy special event");
                                //LLamo a la Corutina indicada por el propio BattleStarter:
                                StartCoroutine(eventBattleRef);
                            }
                            else if (!activeBattlers[currentTurn].hasDied)
                            {
                                Debug.Log("Enemy should attack");
                                //enemy should attack
                                StartCoroutine(EnemyMoveCo());
                            }
                            else
                            {
                                Debug.Log("Enemy " + activeBattlers[currentTurn].charName + " is dead.");
                                NextTurn();
                            }
                        }
                    }

                }

                //if (activeBattlers[currentTurn].isPlayer)
                //{
                //    if (!activeBattlers[currentTurn].justUsedFocus)//Para ocultar el menu entre ataques reiterados por focus
                //    {
                //        //lo siguiente sólo debe ejecutarse una vez cada vez que le toca a algun player:
                //        if (turnJustStarted)
                //        {
                //            GetEquippedWeapons();
                //            SetWeaponButtonSprite();

                //            //Comprobar si algún estado alterado me impide actuar:
                //            if (activeBattlers[currentTurn].turnsSleep > 0)
                //            {
                //                Debug.Log(activeBattlers[currentTurn].charName + " is sleeping...");
                //                NextTurn();
                //            }
                //            else
                //            {
                //                uiButtonsHolder.SetActive(true);
                //                for (int i = 0; i < buttons.Length; i++)
                //                {
                //                    buttons[i].gameObject.SetActive(true);
                //                }
                //                //Si estoy en una batalla tutorial triggereo aquí las instrucciones:
                //                if (specialBattle)
                //                {
                //                    //LLamo a la Corutina indicada por el propio BattleStarter:
                //                    StartCoroutine(eventBattleRef);
                //                }

                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    uiButtonsHolder.SetActive(false);
                //    Debug.Log("Enemy should attack");
                //    //enemy should attack
                //    StartCoroutine(EnemyMoveCo());
                //}
            }
        }
    }

    /*Secuencia lógica:
    El botón Atacar -> OpenAttackTargetMenu: Se muestran los targets enemigos, al seleccionar y confirmar ataque -> AtackTargetMenu
    El botón Magic -> OpenMagicMenu lo mismo que OpenAttackTargetMenu pero con los ataques especiales -> AttackTargetMenu
    -> AtackTargetMenu: pone a 0 el coste magico y envía la referencia del BattleChar target seleccionado a PlayerAttack
    -> PlayerAttack: actualiza la UI (desactiva iconos seleccion y menu de targets) y envía la ref del target a ApplyMoveCo
    -> ApplyMoveCo: secuencia las animaciones del ataque (resaltar player, ataque y efecto sobre el target), llama a DealEffect (pasandole la
       referencia del target) para calcular el daño a aplicar y detecta el focus (se itera a sí misma para gestionar ataques múltiples)
        -> DealEffect: gestion de calculo de daño y aplicacion por tipo de ataque y su focus correspondiente.

    */


    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee)
    {
        if (!battleActive)
        {
            cannotFlee = setCannotFlee;

            battleActive = true;

            activeBattlers = new List<BattleChar>();
            battlePlayers = new List<BattleChar>();

            GameManager.instance.battleActive = true;

            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);

            //Desactivo los playerUIStats para solo llenar los de los players activos en UpdateUIStats:
            for (int h = 0; h < playerStatsHolder.Length; h++)
            {
                playerStatsHolder[h].SetActive(false);
            }


            Debug.Log("Invocar players");

            //Lista temporal para ir descartando a los players que ya he añadido al combate
            List<BattleChar> playersAdded = new List<BattleChar>();

            for (int i = 0; i < playerPositions.Length; i++)
            {
                for (int j = 0; j < playerPrefabs.Length; j++)
                {
                    if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName && !playersAdded.Contains(playerPrefabs[j]))
                    {
                        if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)//Sólo lo añado si está desbloqueado en el juego
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];//Para Player prefab (no UI)
                                                                            //newPlayer.transform.SetParent(playerPositions[i], false);//Para elementos en un UI canvas

                            activeBattlers.Add(newPlayer);

                            //Añado el player a la lista de players activos de este combate:
                            battlePlayers.Add(newPlayer);

                            playersAdded.Add(playerPrefabs[j]);

                            newPlayer.focusLevel = 1;

                            //Recabar los stats de cada Player
                            CharStats thePlayer = GameManager.instance.playerStats[i];

                            //Check lack of weapons and add makeshifted weapons:
                            if (thePlayer.canUseSword && thePlayer.equippedSword == "")
                            {
                                thePlayer.equippedSword = "Espada improvisada";
                                thePlayer.swordPwr = 1;
                            }
                            if (thePlayer.canUseAxe && thePlayer.equippedAxe == "")
                            {
                                thePlayer.equippedAxe = "Hacha improvisada";
                                thePlayer.axePwr = 1;
                            }
                            if (thePlayer.canUseBow && thePlayer.equippedBow == "")
                            {
                                thePlayer.equippedBow = "Arco improvisado";
                                thePlayer.bowPwr = 1;
                            }
                            if (thePlayer.canUseStaff && thePlayer.equippedStaff == "")
                            {
                                thePlayer.equippedStaff = "Báculo improvisado";
                                thePlayer.staffPwr = 1;
                            }

                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].magicStr = thePlayer.magicStr;
                            activeBattlers[i].magicDef = thePlayer.magicDef;
                            activeBattlers[i].speed = thePlayer.speed;
                            activeBattlers[i].evasion = thePlayer.evasion;
                            activeBattlers[i].swordPwr = thePlayer.swordPwr;
                            activeBattlers[i].axePwr = thePlayer.axePwr;
                            activeBattlers[i].bowPwr = thePlayer.bowPwr;
                            activeBattlers[i].staffPwr = thePlayer.staffPwr;
                            activeBattlers[i].headArmorPwr = thePlayer.headArmorPwr;
                            activeBattlers[i].bodyArmorPwr = thePlayer.bodyArmorPwr;
                            activeBattlers[i].shieldPwr = thePlayer.shieldPwr;
                            activeBattlers[i].lastWeaponUsed = thePlayer.lastWeaponUsed;

                            //Le actualizo los moves aprendidos según nivel (los cojo del CharStat del GameManager):
                            newPlayer.movesLearnt.Clear();

                            foreach (string moveName in thePlayer.movesLearnt)
                            {
                                newPlayer.movesLearnt.Add(moveName);
                            }

                            break;//Ya he terminado con esta PlayerPosition[i]
                        }
                    }
                }

            }

            Debug.Log("Reposicionar players");
            //Reposicionar los players si son menos de 4:
            if (activeBattlers.Count < 2)
            {
                activeBattlers[0].transform.position = playerPositions[1].position;
            }
            else if (activeBattlers.Count < 3)
            {
                activeBattlers[0].transform.position = playerPositions[1].position;
                activeBattlers[1].transform.position = playerPositions[2].position;
            }
            else if (activeBattlers.Count < 4)
            {
                activeBattlers[0].transform.position = playerPositions[1].position;
                activeBattlers[1].transform.position = playerPositions[2].position;
                activeBattlers[2].transform.position = playerPositions[3].position;
            }

            List<string> rewardItemsList = new List<string>();
            int expReward = 0;
            int goldReward = 0;
            int eventEnemy = 0;//Esto me servirá para saber si estoy en una lucha de story event

            Debug.Log("Invocar enemigos");

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);

                            //Pongo al enemigo en visto:
                            newEnemy.sighted = true;

                            ////Establecer las debilidades y defensas de cada enemigo (OLD):
                            //GetWeaknessList(enemyPrefabs[j]);

                            //SetWeaknessSpriteList(enemyPrefabs[j]);

                            ////Referencio el holder de los iconos del enemigo
                            //GameObject weaknessHolder = newEnemy.transform.GetChild(2).gameObject;

                            //for (int k = 1; k < tempEnemyWeaknessList.Count + 1; k++)
                            //{
                            //    weaknessHolder.transform.GetChild(k).gameObject.SetActive(true);
                            //    weaknessHolder.transform.GetChild(k).GetComponent<SpriteRenderer>().sprite = weaknessSpritesList[k - 1];
                            //}


                            //Establecer debilidades de cada enemigo (NEW):
                            SetWeaknessSpritesList(enemyPrefabs[j], newEnemy);

                            GameObject enemyShield = newEnemy.transform.GetChild(2).GetChild(0).gameObject;
                            enemyShield.SetActive(true);
                            enemyShield.transform.GetChild(1).GetComponent<TMPro.TextMeshPro>().text = enemyPrefabs[j].shieldMaxAmount.ToString();



                            //Decidir a random qué item dropea el enemigo y hacer la lista de rewards:
                            if (Random.Range(0, 100) < 5)
                            {
                                rewardItemsList.Add(enemyPrefabs[j].improbableReward);
                            }
                            else if (Random.Range(0, 100) < 25)
                            {
                                rewardItemsList.Add(enemyPrefabs[j].seldomReward);
                            }
                            else
                            {
                                rewardItemsList.Add(enemyPrefabs[j].commonReward);
                            }

                            //Reward de experiencia y oros de cada enemigo:
                            expReward += enemyPrefabs[j].expReward;
                            goldReward += enemyPrefabs[j].goldReward;

                            //Marcar si algún enemigo es un boss o requiere story event:
                            if (enemyPrefabs[j].isEnemyEvent)//Esto se marca en el enemigo, en el battleChar de su prefab
                            {
                                eventEnemy += 1;
                            }
                        }
                    }
                }
            }

            rewardEXP = expReward;
            rewardGold = goldReward;
            rewardItems = new List<string>();
            rewardItems = rewardItemsList;

            //Decido si estoy en una batalla especial si hay algun enemigo especial:
            if (eventEnemy >= 1)
            {
                isEventBattle = true;
            }
            else
            {
                isEventBattle = false;
            }

            Debug.Log("Turn waiting true first time");
            turnWaiting = true;
            turnVerified = false;
            //currentTurn = Random.Range(0, activeBattlers.Count);//Hay que ordenar segun la vel de cada uno


            //Tengo que hacer el orden de la primera ronda aquí:

            //Vacío las listas de la ronda actual y siguiente:
            currentRoundList.Clear();
            nextRoundList.Clear();

            for (int i = 0; i < activeBattlers.Count; i++)
            {
                //Juego con un 20% arriba y abajo de la velocidad de cada battler a random para ordenar las rondas
                //Pero en la primera de todas sólo un 5% para que haya algo de consistencia:
                activeBattlers[i].roundSpeed = (int)(activeBattlers[i].speed * Random.Range(0.98f, 1.02f));
                activeBattlers[i].nextRoundSpeed = (int)(activeBattlers[i].speed * Random.Range(0.8f, 1.2f));

                //Relleno la lista de la ronda actual (y la siguiente) desordenada:
                currentRoundList.Add(activeBattlers[i]);
                nextRoundList.Add(activeBattlers[i]);
            }
            //Ordeno la lista de la ronda actual:
            currentRoundList.Sort(SortByCurrentRoundSpeed);
            //Ordeno la lista de la siguiente ronda:
            nextRoundList.Sort(SortByNextRoundSpeed);
            //Ordeno la lista de activeBattlers según el orden de la ronda actual:
            activeBattlers.Sort(SortByCurrentRoundSpeed);

            //Ya tengo los valores de velocidad de la ronda actual y la siguiente, ahora sólo tengo que
            //ir modificando el orden de ambas listas según los efectos secundarios de la batalla:
            //En la ronda actual, al ser la primera y aun no haber hecho nadie nada, no hay cambios
            //(a menos que introduzca ataques por sorpresa, pero de momento no).

            //Actualizo la UI:
            for (int i = 0; i < currentRoundList.Count; i++)
            {
                currentRoundHolder.transform.GetChild(i).gameObject.SetActive(true);
                currentRoundHolder.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = currentRoundList[i].theSprite.sprite;
            }
            for (int i = 0; i < nextRoundList.Count; i++)
            {
                nextRoundHolder.transform.GetChild(i).gameObject.SetActive(true);
                nextRoundHolder.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = nextRoundList[i].theSprite.sprite;
            }

            //Ojo que aquí se desordena la lista de activeBattlers de players primero y luego los enemies:
            //activeBattlers.Sort(SortBySpeed);


            //Renombrar a 1, 2, etc si hay varios enemigos de la misma especie:
            //Vacío la lista de comprobación
            checkRepeatedBattlers.Clear();

            //Hago una lista auxiliar con los battlers:
            for (int i = 0; i < activeBattlers.Count; i++)
            {
                checkRepeatedBattlers.Add(activeBattlers[i]);
                activeBattlers[i].tempName = activeBattlers[i].charName;
            }
            //Ahora para cada battler de la lista que acabo de hacer:
            for (int i = 0; i < checkRepeatedBattlers.Count; i++)
            {
                //Lo comparo con la lista de battlers original:
                for (int j = 0; j < activeBattlers.Count; j++)
                {
                    if (i != j)//Sólo comparo si no ocupan la misma posición en las listas
                    {
                        if (checkRepeatedBattlers[i].charName == activeBattlers[j].charName)
                        {
                            activeBattlers[j].tempName = activeBattlers[j].charName + "'";
                            checkRepeatedBattlers[i].charName = checkRepeatedBattlers[j].charName + "'";
                        }
                    }
                }
            }


            currentTurn = 0;
            currentRound = 1;
            turnJustStarted = true;

            UpdateBattle();
            UpdateUIStats();

            UpdateRoundOrderUI();

            //Asigno la posicion en batalla de cada battler:
            for(int i = 0; i < activeBattlers.Count; i++)
            {
                activeBattlers[i].battlePos = activeBattlers[i].transform.position;
            }
        }

        //Ahora que se haga la luz:
        UIFade.instance.transitionCurtainRight.SetBool("BlackScreen", false);
        UIFade.instance.TransitionCurtainRightLighten();
        StartCoroutine(ReadyForBattleCo());
    }

    public IEnumerator ReadyForBattleCo()
    {
        yield return new WaitForSecondsRealtime(1);
        readyForBattle = true;
    }

    //private static int SortBySpeed(BattleChar battler1, BattleChar battler2)
    //{
    //    return battler2.speed.CompareTo(battler1.speed);
    //}
    private static int SortByCurrentRoundSpeed(BattleChar battler1, BattleChar battler2)
    {
        return battler2.roundSpeed.CompareTo(battler1.tempRoundSpeed);
    }
    private static int SortByNextRoundSpeed(BattleChar battler1, BattleChar battler2)
    {
        return battler2.nextRoundSpeed.CompareTo(battler1.tempNextRoundSpeed);
    }

    public void SetOrderSpeedsForNextRound()//Esta funcion solo se debe llamar una vez al cambiar de ronda
    {
        Debug.Log("Nuevo orden de ronda.");

        //Asigno a la ronda actual el orden de la siguiente y creo una nueva siguiente
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            activeBattlers[i].roundSpeed = activeBattlers[i].tempNextRoundSpeed;//Uso la temporal para mantener las modificaciones por stun y speedModifiers.
            activeBattlers[i].nextRoundSpeed = (int)(activeBattlers[i].speed * Random.Range(0.8f, 1.2f));
        }
    }

    public void UpdateRoundOrderUI()//Esta funcion debería llamarse cada vez que suceda algo que pueda alterar los turnos
    {
        //Ahora modifico el orden base de cada ronda según los efectos de batalla:
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            //Utilizo vars temporales para no acumular modificaciones
            activeBattlers[i].tempRoundSpeed = activeBattlers[i].roundSpeed;
            activeBattlers[i].tempNextRoundSpeed = activeBattlers[i].nextRoundSpeed;

            //En currentRound:
            //Elimino a los muertos
            //Elimino a los stuneados por rotura de defensas
            //Elimino a los que ya han actuado esta ronda
            //Eliminaciones las hago efectivas a la hora de actualizar la UI

            //En nextRound:
            //Elimino a los muertos
            //Elimino a los recién stuneados por rotura de defensas
            //Adelanto a los previamente stuneados por rotura
            //Adelanto a los que tienen turnsModSpd positivo
            //Pongo a la cola a los que tienen turnsModSpd negativo
            if (activeBattlers[i].stunCount == 1)
            {
                activeBattlers[i].tempNextRoundSpeed += 15000;
            }
            if (activeBattlers[i].turnsModSpd > 0)
            {
                activeBattlers[i].tempNextRoundSpeed += 2000;
            }
            if (activeBattlers[i].turnsModSpd < 0)
            {
                activeBattlers[i].tempNextRoundSpeed -= 2000;
            }
        }

        //Vacío las listas de la ronda actual y siguiente:
        currentRoundList.Clear();
        nextRoundList.Clear();

        //Relleno la lista de la ronda actual (y la siguiente) desordenada:
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            currentRoundList.Add(activeBattlers[i]);
            nextRoundList.Add(activeBattlers[i]);
        }

        //Ordeno la lista de la ronda actual:
        currentRoundList.Sort(SortByCurrentRoundSpeed);
        //Ordeno la lista de la siguiente ronda:
        nextRoundList.Sort(SortByNextRoundSpeed);
        //Actualizo el orden de activeBattlers:
        activeBattlers.Sort(SortByCurrentRoundSpeed);



        //Si tengo que quitar a alguien de la lista Ui, es aquí:
        //Actualizo la UI currentRound:
        for (int i = 0; i < currentRoundHolder.transform.childCount; i++)
        {
            if (i < currentRoundList.Count)
            {
                currentRoundHolder.transform.GetChild(i).gameObject.SetActive(true);
                currentRoundHolder.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = currentRoundList[i].portrait;
                //Tengo que desactivar a los muertos/ya actuados ahora para que no tenga problemas al revivirlos:
                if (currentRoundList[i].turnDone && currentRoundList[i] != activeBattlers[currentTurn])
                {//La segunda condición es para que no me quite al battler del currentTurn
                    currentRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (currentRoundList[i].hasDied)
                {
                    currentRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (currentRoundList[i].stunCount > 0)
                {
                    currentRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
                //if (currentRoundList[i].turnsBroken > 0)
                //{
                //    currentRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                //}
            }
            else
            {
                currentRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        //Actualizo la UI nextRound:
        for (int i = 0; i < nextRoundHolder.transform.childCount; i++)
        {
            if (i < currentRoundList.Count)
            {
                nextRoundHolder.transform.GetChild(i).gameObject.SetActive(true);
                nextRoundHolder.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = nextRoundList[i].portrait;
                //Tengo que desactivar a los muertos ahora para que no tenga problemas al revivirlos:
                if (nextRoundList[i].hasDied)
                {
                    nextRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (nextRoundList[i].stunCount > 1)
                {
                    nextRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                nextRoundHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void GetEquippedWeapons()
    {
        equippeddWeaponsList = new List<string>();

        if (activeBattlers[currentTurn].swordPwr != 0)
        {
            equippeddWeaponsList.Add("Sword");
            activeBattlers[currentTurn].anim.SetBool("usingSword", true);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Sword")
        {
            activeBattlers[currentTurn].lastWeaponUsed = "";
        }

        if (activeBattlers[currentTurn].axePwr != 0)
        {
            equippeddWeaponsList.Add("Axe");
            activeBattlers[currentTurn].anim.SetBool("usingAxe", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Axe")
        {
            activeBattlers[currentTurn].lastWeaponUsed = "";
        }

        if (activeBattlers[currentTurn].bowPwr != 0)
        {
            equippeddWeaponsList.Add("Bow");
            activeBattlers[currentTurn].anim.SetBool("usingBow", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Bow")
        {
            activeBattlers[currentTurn].lastWeaponUsed = "";
        }

        if (activeBattlers[currentTurn].staffPwr != 0)
        {
            equippeddWeaponsList.Add("Staff");
            activeBattlers[currentTurn].anim.SetBool("usingStaff", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Staff")
        {
            activeBattlers[currentTurn].lastWeaponUsed = "";
        }

        if (activeBattlers[currentTurn].lastWeaponUsed != "")
        {
            usingWeapon = activeBattlers[currentTurn].lastWeaponUsed;
        }
        else //Si NO he detectado ningun arma
        {
            if (equippeddWeaponsList.Count > 0)//Pero tengo algún arma
            {
                weaponPositionInList = 0;
                usingWeapon = equippeddWeaponsList[weaponPositionInList];//Asigno la primera por orden y avan
                activeBattlers[currentTurn].lastWeaponUsed = usingWeapon;
            }
            else//En principio aquí no llego nunca
            {
                usingWeapon = "";
                activeBattlers[currentTurn].anim.SetBool("usingSword", false);
                activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
                activeBattlers[currentTurn].anim.SetBool("usingBow", false);
                activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
            }
        }

        //actualizo mi valor de weaponPositionInList:
        for (int i = 0; i < equippeddWeaponsList.Count; i++)
        {
            if(usingWeapon == equippeddWeaponsList[i])
            {
                weaponPositionInList = i;
            }
        }

        turnJustStarted = false;
    }

    public void ChangeWeapon()
    {
        if (equippeddWeaponsList.Count > 1)
        {
            Debug.Log("Lista de armas mayor a 1");
            weaponPositionInList++;
            if (weaponPositionInList >= equippeddWeaponsList.Count)
            {
                weaponPositionInList = 0;
                Debug.Log("Limite de lista de armas alcanzado");
            }
            usingWeapon = equippeddWeaponsList[weaponPositionInList];
            activeBattlers[currentTurn].lastWeaponUsed = usingWeapon;
            Debug.Log("usingWeapon debería ser " + equippeddWeaponsList[weaponPositionInList]);
            SetWeaponButtonSprite();
        }
    }

    public void SetWeaponButtonSprite()
    {
        if (activeBattlers[currentTurn].lastWeaponUsed == "Sword")
        {
            changeWeaponButton.transform.GetChild(1).GetComponent<Image>().sprite = usingWeaponIcons[0];
            activeBattlers[currentTurn].anim.SetBool("usingSword", true);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Axe")
        {
            changeWeaponButton.transform.GetChild(1).GetComponent<Image>().sprite = usingWeaponIcons[1];
            activeBattlers[currentTurn].anim.SetBool("usingAxe", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Bow")
        {
            changeWeaponButton.transform.GetChild(1).GetComponent<Image>().sprite = usingWeaponIcons[2];
            activeBattlers[currentTurn].anim.SetBool("usingBow", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
        else if (activeBattlers[currentTurn].lastWeaponUsed == "Staff")
        {
            changeWeaponButton.transform.GetChild(1).GetComponent<Image>().sprite = usingWeaponIcons[3];
            activeBattlers[currentTurn].anim.SetBool("usingStaff", true);
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
        }
        else
        {
            changeWeaponButton.transform.GetChild(1).GetComponent<Image>().sprite = iconPunch;
            activeBattlers[currentTurn].anim.SetBool("usingSword", false);
            activeBattlers[currentTurn].anim.SetBool("usingAxe", false);
            activeBattlers[currentTurn].anim.SetBool("usingBow", false);
            activeBattlers[currentTurn].anim.SetBool("usingStaff", false);
        }
    }


    //Esta función elige el movimiento que se usa al darle a Atacar según el arma equipada en ese momento:
    public BattleMove SetMoveFromWeapon()
    {
        if (activeBattlers[currentTurn].isPlayer)
        {
            if (usingWeapon == "Sword")
            {
                for (int i = 0; i < movesList.Length; i++)
                {
                    if (movesList[i].moveName == "Slash")
                    {
                        return movesList[i];
                    }
                }
            }
            else if (usingWeapon == "Axe")
            {
                for (int i = 0; i < movesList.Length; i++)
                {
                    if (movesList[i].moveName == "Chop")
                    {
                        return movesList[i];
                    }
                }
            }
            else if (usingWeapon == "Bow")
            {
                for (int i = 0; i < movesList.Length; i++)
                {
                    if (movesList[i].moveName == "Shoot")
                    {
                        return movesList[i];
                    }
                }
            }
            else if (usingWeapon == "Staff")
            {
                for (int i = 0; i < movesList.Length; i++)
                {
                    if (movesList[i].moveName == "Hit")
                    {
                        return movesList[i];
                    }
                }

            }
            else if (usingWeapon == "")
            {
                for (int i = 0; i < movesList.Length; i++)
                {
                    if (movesList[i].moveName == "Punch")
                    {
                        return movesList[i];
                    }
                }

            }

        }

        Debug.Log("No tienes un arma reconocida equipada");
        return null;
    }


    //Esta función elige el nombre del movimiento que se usa al darle a Atacar según el arma equipada en ese momento:
    public string SetAttackFromWeapon(string inputName)
    {
        if (activeBattlers[currentTurn].isPlayer)
        {
            if (usingWeapon == "Sword")
            {
                return "Slash";
            }
            else if (usingWeapon == "Axe")
            {
                return "Chop";
            }
            else if (usingWeapon == "Bow")
            {
                return "Shoot";
            }
            else if (usingWeapon == "Staff")
            {
                return "Hit";
            }
            else if (usingWeapon == "")
            {
                return "Punch";
            }

        }

        return inputName;
    }


    public void CheckCharStateEndTurn()
    {
        //Referencio variable local para ahorrar texto:
        GameObject statAnimHolder = activeBattlers[currentTurn].transform.GetChild(1).gameObject;

        //Distingo entre players o enemigos porque los iconos de los players están metidos en la UI,
        //mientras que los de los enemigos estan en los propios enemigos
        if (activeBattlers[currentTurn].isPlayer)
        {
            //Comprobar cada cambio de estado
            if (activeBattlers[currentTurn].turnsPoison > 0)
            {
                //Aplicar animación de envenenamiento
                statAnimHolder.transform.GetChild(0).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsPoison--;

                //Desactivar animación de envenenamiento
                if (activeBattlers[currentTurn].turnsPoison == 0)
                {
                    statAnimHolder.transform.GetChild(0).gameObject.SetActive(false);
                }

                //Calcular y aplicar el daño por envenenamiento (20% de normal, 1% en bosses):
                activeBattlers[currentTurn].currentHP -= Mathf.RoundToInt(activeBattlers[currentTurn].maxHP * 0.2f * Random.Range(0.9f, 1.1f));
                if (activeBattlers[currentTurn].currentHP < 0)
                {
                    activeBattlers[currentTurn].currentHP = 0;
                }
            }
            if (activeBattlers[currentTurn].turnsSleep > 0)
            {
                //Animación de sueño:
                statAnimHolder.transform.GetChild(1).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsSleep--;

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsSleep == 0)
                {
                    statAnimHolder.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsCursed > 0)
            {
                //Animación de maldición -1
                statAnimHolder.transform.GetChild(2).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsCursed--;

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsCursed == 0)
                {
                    statAnimHolder.transform.GetChild(2).gameObject.SetActive(false);
                }

                if (activeBattlers[currentTurn].turnsCursed <= 0)
                {
                    //Activar animación de maldición consumida (ToDo)
                    //Matar al personaje:
                    activeBattlers[currentTurn].currentHP = 0;
                }
            }
            if (activeBattlers[currentTurn].turnsBlind > 0)
            {
                //Animación de ceguera
                statAnimHolder.transform.GetChild(3).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsBlind--;

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsBlind == 0)
                {
                    statAnimHolder.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsMute > 0)
            {
                //Animación de mudo
                statAnimHolder.transform.GetChild(4).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsMute--;

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsMute == 0)
                {
                    statAnimHolder.transform.GetChild(4).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsModStr != 0)
            {
                if (activeBattlers[currentTurn].turnsModStr > 0)
                {
                    activeBattlers[currentTurn].turnsModStr--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModStr++;
                }
            }
            if (activeBattlers[currentTurn].turnsModDef != 0)
            {
                if (activeBattlers[currentTurn].turnsModDef > 0)
                {
                    activeBattlers[currentTurn].turnsModDef--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModDef++;
                }

            }
            if (activeBattlers[currentTurn].turnsModMgcStr != 0)
            {
                if (activeBattlers[currentTurn].turnsModMgcStr > 0)
                {
                    activeBattlers[currentTurn].turnsModMgcStr--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModMgcStr++;
                }

            }
            if (activeBattlers[currentTurn].turnsModMgcDef != 0)
            {
                if (activeBattlers[currentTurn].turnsModMgcDef > 0)
                {
                    activeBattlers[currentTurn].turnsModMgcDef--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModMgcDef++;
                }

            }
            if (activeBattlers[currentTurn].turnsModSpd != 0)
            {
                if (activeBattlers[currentTurn].turnsModSpd > 0)
                {
                    activeBattlers[currentTurn].turnsModSpd--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModSpd++;
                }

            }
            if (activeBattlers[currentTurn].turnsModEva != 0)
            {
                if (activeBattlers[currentTurn].turnsModEva > 0)
                {
                    activeBattlers[currentTurn].turnsModEva--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsModEva++;
                }

            }
            if (activeBattlers[currentTurn].turnsInmune != 0)
            {
                if (activeBattlers[currentTurn].turnsInmune > 0)
                {
                    activeBattlers[currentTurn].turnsInmune--;
                }
                else
                {
                    activeBattlers[currentTurn].turnsInmune++;
                }

            }
        }
        else//Si es un enemigo incluyo tambien la gestion de los iconos de estado:
        {
            GameObject statIconHolder = activeBattlers[currentTurn].transform.GetChild(3).gameObject;

            //Comprobar cada cambio de estado
            if (activeBattlers[currentTurn].turnsPoison > 0)
            {
                //Aplicar animación de envenenamiento
                statAnimHolder.transform.GetChild(0).gameObject.SetActive(true);
                //Activar icono de envenenamiento
                statIconHolder.transform.GetChild(0).gameObject.SetActive(true);

                //Descuento este turno de cambio de estado restante
                activeBattlers[currentTurn].turnsPoison--;

                //Asigno el nº de turnos restantes al icono:
                statIconHolder.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsPoison.ToString();

                //Desactivar animación de envenenamiento
                if (activeBattlers[currentTurn].turnsPoison == 0)
                {
                    statAnimHolder.transform.GetChild(0).gameObject.SetActive(false);
                    statIconHolder.transform.GetChild(0).gameObject.SetActive(false);
                }

                //Calcular y aplicar el daño por envenenamiento (20% de normal, 1% en bosses):
                activeBattlers[currentTurn].currentHP -= Mathf.RoundToInt(activeBattlers[currentTurn].maxHP * 0.2f * Random.Range(0.9f, 1.1f));
                if (activeBattlers[currentTurn].currentHP < 0)
                {
                    activeBattlers[currentTurn].currentHP = 0;
                }
            }
            if (activeBattlers[currentTurn].turnsSleep > 0)
            {
                //Animación de sueño:
                statAnimHolder.transform.GetChild(1).gameObject.SetActive(true);
                statIconHolder.transform.GetChild(1).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsSleep--;

                //Asigno el nº de turnos restantes al icono:
                statIconHolder.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsSleep.ToString();

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsSleep == 0)
                {
                    statAnimHolder.transform.GetChild(1).gameObject.SetActive(false);
                    statIconHolder.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsCursed > 0)
            {
                //Animación de maldición -1
                statAnimHolder.transform.GetChild(2).gameObject.SetActive(true);
                statIconHolder.transform.GetChild(2).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsCursed--;

                //Asigno el nº de turnos restantes al icono:
                statIconHolder.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsCursed.ToString();

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsCursed == 0)
                {
                    statAnimHolder.transform.GetChild(2).gameObject.SetActive(false);
                    statIconHolder.transform.GetChild(2).gameObject.SetActive(false);
                }

                if (activeBattlers[currentTurn].turnsCursed <= 0)
                {
                    //Activar animación de maldición consumida (ToDo)
                    //Matar al personaje:
                    activeBattlers[currentTurn].currentHP = 0;
                }
            }
            if (activeBattlers[currentTurn].turnsBlind > 0)
            {
                //Animación de ceguera
                statAnimHolder.transform.GetChild(3).gameObject.SetActive(true);
                statIconHolder.transform.GetChild(3).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsBlind--;

                //Asigno el nº de turnos restantes al icono:
                statIconHolder.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsBlind.ToString();

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsBlind == 0)
                {
                    statAnimHolder.transform.GetChild(3).gameObject.SetActive(false);
                    statIconHolder.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsMute > 0)
            {
                //Animación de mudo
                statAnimHolder.transform.GetChild(4).gameObject.SetActive(true);
                statIconHolder.transform.GetChild(4).gameObject.SetActive(true);

                activeBattlers[currentTurn].turnsMute--;

                //Asigno el nº de turnos restantes al icono:
                statIconHolder.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsMute.ToString();

                //Desactivar animación
                if (activeBattlers[currentTurn].turnsMute == 0)
                {
                    statAnimHolder.transform.GetChild(4).gameObject.SetActive(false);
                    statIconHolder.transform.GetChild(4).gameObject.SetActive(false);
                }
            }
            if (activeBattlers[currentTurn].turnsModStr != 0)
            {
                if (activeBattlers[currentTurn].turnsModStr > 0)
                {
                    statIconHolder.transform.GetChild(5).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModStr--;

                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(5).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModStr.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(5).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(6).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModStr++;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModStr).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(6).gameObject.SetActive(false);
                    }

                }
            }
            if (activeBattlers[currentTurn].turnsModDef != 0)
            {
                if (activeBattlers[currentTurn].turnsModDef > 0)
                {
                    statIconHolder.transform.GetChild(7).gameObject.SetActive(true);

                    activeBattlers[currentTurn].turnsModDef--;

                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(7).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModDef.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(7).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(8).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModDef++;

                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(8).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModDef).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(8).gameObject.SetActive(false);
                    }
                }
            }
            if (activeBattlers[currentTurn].turnsModMgcStr != 0)
            {
                if (activeBattlers[currentTurn].turnsModMgcStr > 0)
                {
                    statIconHolder.transform.GetChild(9).gameObject.SetActive(true);

                    activeBattlers[currentTurn].turnsModMgcStr--;

                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(9).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModMgcStr.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(9).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(10).gameObject.SetActive(true);

                    activeBattlers[currentTurn].turnsModMgcStr++;

                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(10).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModMgcStr).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(10).gameObject.SetActive(false);
                    }
                }

            }
            if (activeBattlers[currentTurn].turnsModMgcDef != 0)
            {
                if (activeBattlers[currentTurn].turnsModMgcDef > 0)
                {
                    statIconHolder.transform.GetChild(11).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModMgcDef--;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(11).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModMgcDef.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(11).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(12).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModMgcDef++;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(12).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModMgcDef).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(12).gameObject.SetActive(false);
                    }
                }

            }
            if (activeBattlers[currentTurn].turnsModSpd != 0)
            {
                if (activeBattlers[currentTurn].turnsModSpd > 0)
                {
                    statIconHolder.transform.GetChild(13).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModSpd--;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(13).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModSpd.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(13).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(14).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModSpd++;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(14).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModSpd).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(14).gameObject.SetActive(false);
                    }
                }

            }
            if (activeBattlers[currentTurn].turnsModEva != 0)
            {
                if (activeBattlers[currentTurn].turnsModEva > 0)
                {
                    statIconHolder.transform.GetChild(15).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModEva--;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(15).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsModEva.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(15).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(16).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsModEva++;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(16).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsModEva).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(16).gameObject.SetActive(false);
                    }
                }

            }
            if (activeBattlers[currentTurn].turnsInmune != 0)
            {
                if (activeBattlers[currentTurn].turnsInmune > 0)
                {
                    statIconHolder.transform.GetChild(17).gameObject.SetActive(true);
                    activeBattlers[currentTurn].turnsInmune--;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(17).GetChild(0).GetComponent<Text>().text = activeBattlers[currentTurn].turnsInmune.ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(17).gameObject.SetActive(false);
                    }
                }
                else
                {
                    statIconHolder.transform.GetChild(17).gameObject.SetActive(false);
                    activeBattlers[currentTurn].turnsInmune++;
                    //Asigno el nº de turnos restantes al icono:
                    statIconHolder.transform.GetChild(17).GetChild(0).GetComponent<Text>().text = (-1f * activeBattlers[currentTurn].turnsInmune).ToString();
                    //Desactivar icono
                    if (activeBattlers[currentTurn].turnsModStr == 0)
                    {
                        statIconHolder.transform.GetChild(17).gameObject.SetActive(false);
                    }
                }

            }
        }
    }


    public void NextTurn()
    {
        Debug.Log("Termina turno de " + activeBattlers[currentTurn]);

        //Devuelvo al player a su posicion estandard (en su caso):
        if (activeBattlers[currentTurn].isPlayer)
        {
            if(Vector2.Distance(activeBattlers[currentTurn].transform.position, activePos.position) < 0.001f)
            {
                Debug.Log(activeBattlers[currentTurn].charName + ", volvemos?");
                if (!playerMoving)
                {
                    Debug.Log("Debería empezar corrutina de vuelta para " + activeBattlers[currentTurn].charName);
                    StartCoroutine(ReturnFromActivePositionCo(activeBattlers[currentTurn]));
                }
            }
        }

        //Comprobar, ejecutar y actualizar los cambios de estado (excepto los iconos de estado, esos 
        //se gestionan en UpdateUIbattle):
        CheckCharStateEndTurn();

        currentTurn++;
        turnJustStarted = true;

        if (currentTurn >= activeBattlers.Count)//Cambio de turno grupal
        {
            NextRound();
            return;//Para terminar ya con NextTurn();
        }

        turnWaiting = true;
        turnVerified = false;//Debe estar en false a cada cambio de turno

        itemButton.SetActive(true);
        fleeButton.SetActive(true);

        UpdateBattle();
        UpdateUIStats();
        //Actualizo la UI de los turnos de ronda:
        UpdateRoundOrderUI();
    }

    public void NextRound()
    {
        currentRound++;
        fleeFailed = false;//Se acaba la penalización por no conseguir huir
        currentTurn = 0;
        turnJustStarted = true;

        for (int i = 0; i < activeBattlers.Count; i++)//Si alguien estaba stuneado recupera sus defensas y su conciencia
        {
            activeBattlers[i].turnDone = false;

            if (activeBattlers[i].stunCount > 0)
            {
                activeBattlers[i].stunCount--;
            }
            if (activeBattlers[i].stunCount == 0 && activeBattlers[i].shieldAmount <= 0 && !activeBattlers[i].isPlayer)
            {
                activeBattlers[i].goesFirstNextTurn = true;
                activeBattlers[i].shieldAmount = activeBattlers[i].shieldMaxAmount;
                activeBattlers[i].transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = shieldSprite;
            }

            if (!activeBattlers[i].isPlayer)
            {
                Debug.Log("Actualizo UI Stats del enemy " + activeBattlers[i].charName);
                activeBattlers[i].transform.GetChild(2).gameObject.SetActive(true);

                //Actualizar las defensas
                activeBattlers[i].transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshPro>().text = activeBattlers[i].shieldAmount.ToString();
            }
            else
            {
                if (activeBattlers[i].justUsedFocus)
                {
                    activeBattlers[i].justUsedFocus = false;
                }
                else
                {
                    activeBattlers[i].focusLevel++;
                }

                if (activeBattlers[i].focusLevel > activeBattlers[i].focusMaxLevel)
                {
                    activeBattlers[i].focusLevel = activeBattlers[i].focusMaxLevel;
                }
            }
        }

        //SortBattlersNextRound();
        SetOrderSpeedsForNextRound();

        turnWaiting = true;
        turnVerified = false;//Debe estar en false a cada cambio de turno

        itemButton.SetActive(true);
        fleeButton.SetActive(true);

        UpdateBattle();
        UpdateUIStats();
        //Actualizo la UI de los turnos de ronda:
        UpdateRoundOrderUI();
    }


    public void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].currentHP < 0)
            {
                activeBattlers[i].currentHP = 0;
            }

            if (activeBattlers[i].currentHP == 0)
            {
                //Anular cambios de estado y ponerlo en muerte:
                activeBattlers[i].hasDied = true;
                activeBattlers[i].turnsPoison = 0;
                activeBattlers[i].turnsSleep = 0;
                activeBattlers[i].turnsCursed = 0;
                activeBattlers[i].turnsBlind = 0;
                activeBattlers[i].turnsMute = 0;
                activeBattlers[i].stunCount = 0;

                //Handle dead battler
                if (activeBattlers[i].isPlayer)
                {
                    //activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
                    Debug.Log(activeBattlers[i].charName + " ha muerto.");
                    activeBattlers[i].anim.SetBool("dead", true);
                }
                else
                {
                    if (!activeBattlers[i].deadAndDestroyed)
                    {
                        Debug.Log(activeBattlers[i].charName + " ha muerto.");
                        activeBattlers[i].EnemyFade();
                    }
                }
            }
            else//Si el battler no está muerto:
            {
                if (activeBattlers[i].isPlayer)//Si es un player
                {
                    allPlayersDead = false;

                    activeBattlers[i].anim.SetBool("dead", false);

                    if (activeBattlers[i].currentHP < activeBattlers[i].maxHP * 0.25f)
                    {
                        activeBattlers[i].anim.SetBool("lowEnergy", true);
                    }
                    else
                    {
                        activeBattlers[i].anim.SetBool("lowEnergy", false);
                    }
                }
                else//Si es un enemigo
                {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead)
        {
            if (allEnemiesDead)
            {
                //end battle in victory
                StartCoroutine(EndBattleCo());
            }
            else
            {
                //Tengo en cuenta si tengo que perder por temas de modo historia:
                if(specialBattle && forceLoseBattle)
                {
                    StartCoroutine(EndLostBattleCo());
                }
                else
                {
                    //end battle in failure
                    StartCoroutine(GameOverCo());
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        //Comprobar que no haya ningún mensaje de batalla activo para que no se solapen
        while (battleMessage.gameObject.activeInHierarchy)
        {
            Debug.Log("Holding on to read the message");
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Empieza turno de " + activeBattlers[currentTurn]);
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        if (activeBattlers[currentTurn].stunCount > 0)
        {
            Debug.Log(activeBattlers[currentTurn].charName + " está noqueado y no puede hacer nada.");
            battleMessage.theText.text = activeBattlers[currentTurn].charName + " está noqueado y no puede hacer nada.";
            battleMessage.ActivateTemp();
            yield return new WaitForSecondsRealtime(0.3f);

            NextTurn();
        }
        else if (activeBattlers[currentTurn].turnsSleep > 0)
        {
            Debug.Log(activeBattlers[currentTurn].charName + " is sleeping...");
            battleMessage.theText.text = "¡ " + activeBattlers[currentTurn].charName + " está dormido!";
            battleMessage.ActivateTemp();
            yield return new WaitForSecondsRealtime(0.3f);

            NextTurn();
        }
        else
        {
            //EnemyAttack();
            yield return EnemyAttackCo();
            NextTurn();
        }
        yield return null;
    }

    //public void OldEnemyAttack()
    //{
    //    //Creo una lista con los players vivos
    //    List<int> players = new List<int>();
    //    for(int i = 0; i < activeBattlers.Count; i++)
    //    {
    //        if(activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
    //        {
    //            players.Add(i);
    //        }
    //    }
    //    //Elijo a uno al azar como objetivo (añadir aqui la posibilidad de atraer ataques, atacar a todos o afectar a otros enemigos)
    //    int selectedTarget = players[Random.Range(0, players.Count)];
    //    Debug.Log(activeBattlers[currentTurn].charName + " va a atacar a " + activeBattlers[selectedTarget].charName);

    //    //Aquí introduzco el chequeo para las batallas de eventos especiales (bosses y cosas de la story):
    //    if (activeBattlers[currentTurn].isEnemyEvent)
    //    {
    //        //Compruebo si se cumple la condición especial (si el enemigo tiene menos de x hp):
    //        if(activeBattlers[currentTurn].currentHP <= activeBattlers[currentTurn].hp1condition)
    //        {
    //            //Hacer el ataque de la condicion
    //            //Replace("n-", "")
    //        }
    //    }

    //    //Elijo un ataque random
    //    int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length - 1);
    //    Debug.Log(activeBattlers[currentTurn].charName + " va a usar " + activeBattlers[currentTurn].movesAvailable[selectAttack]);
    //    int movePower = 0;
    //    for(int i = 0; i < movesList.Length; i++)
    //    {
    //        if(movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack])
    //        {
    //            //Mandar mensaje de que el enemigo ataca
    //            battleMessage.theText.text = activeBattlers[currentTurn].charName + " used " + movesList[i].moveName + " over " + activeBattlers[selectedTarget].charName;
    //            Debug.Log(activeBattlers[currentTurn].charName + " used " + movesList[i].moveName + " over " + activeBattlers[selectedTarget].charName);
    //            //TODO: En lugar de "enemigo uso nombreDeAtaque sobre objetivo", que cada ataque tenga una descripción propia (y que los enemigos tengan
    //            //ataques personalizados.
    //            //battleMessage.theText.text = activeBattlers[currentTurn].charName + movesList[i].description + activeBattlers[selectedTarget].charName;
    //            battleMessage.Activate();

    //            Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
    //            movePower = movesList[i].movePower;
    //        }
    //    }

    //    Instantiate(highlightAttackerEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

    //    DealDamage(selectedTarget, movePower, shouldWeakenShield);
    //}

    public IEnumerator EnemyAttackCo()
    {
        //Creo una lista con los players vivos
        List<BattleChar> players = new List<BattleChar>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
            {
                players.Add(activeBattlers[i]);
            }
        }

        //Ahora elijo el ataque que hará el enemigo:
        string selectedMove;//Var para almacenar el ataque elegido
        int selectAttack;//Var necesaria para elegir ataques random (lo normal en random enemies)
        BattleMove enemyMove;//Var para almacenar el BatteMove escogido
        BattleChar selectedTarget;//Var para almacenar a single target
        int movePower = 0;//Var para modificar el daño del ataque


        //Compruebo si hay un nextMove preparado:
        if (activeBattlers[currentTurn].nextMove != "")
        {
            selectedMove = activeBattlers[currentTurn].nextMove;
            Debug.Log("Preparando nextMove " + selectedMove + " de " + activeBattlers[currentTurn]);
            activeBattlers[currentTurn].nextMove = "";
        }
        else
        {
            //Elijo un ataque random
            selectAttack = Random.Range(0, activeBattlers[currentTurn].movesLearnt.Count - 1);
            Debug.Log(activeBattlers[currentTurn].charName + " va a usar " + activeBattlers[currentTurn].movesLearnt[selectAttack]);
            selectedMove = activeBattlers[currentTurn].movesLearnt[selectAttack];
        }

        //Compruebo que no haya ningun mensaje activo:
        while (battleMessage.gameObject.activeInHierarchy)
        {
            Debug.Log("Esperando a desaparición de mensaje");
            yield return new WaitForEndOfFrame();
        }

        //Paso del string del move al BattleMove para poder trabajar bien:
        //Si es el ataque "Sin Efecto" no aparece ni el mensaje de ataque:
        if (selectedMove != "Sin Efecto")
        {
            for (int i = 0; i < movesList.Length; i++)
            {
                if (movesList[i].moveName == selectedMove)
                {
                    enemyMove = movesList[i];

                    //Elijo al target o targets si es multitarget:
                    if (!enemyMove.multiTarget)//Si NO es multitarget
                    {
                        //Elijo a un player al azar como objetivo
                        selectedTarget = players[Random.Range(0, players.Count)];
                        Debug.Log(activeBattlers[currentTurn].charName + " va a atacar a " + selectedTarget.charName);

                        //Mandar mensaje de que el enemigo ataca
                        //battleMessage.theText.text = activeBattlers[currentTurn].charName + " used " + enemyMove.moveName + " over " + selectedTarget.charName;
                        Debug.Log(activeBattlers[currentTurn].charName + " used " + enemyMove.moveName + " over " + selectedTarget.charName);
                        //TODO: En lugar de "enemigo uso nombreDeAtaque sobre objetivo", que cada ataque tenga una
                        //descripción propia (y que los enemigos tengan ataques personalizados)
                        battleMessage.theText.text = enemyMove.battleDescription[0].Replace("u-", activeBattlers[currentTurn].charName).Replace("t-", selectedTarget.charName);
                        battleMessage.ActivateTemp();

                        //Y espero a que el player lea el mensaje:
                        while (battleMessage.gameObject.activeInHierarchy)
                        {
                            yield return new WaitForEndOfFrame();
                        }

                        Debug.Log("Enemy message read");

                        Instantiate(highlightAttackerEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
                        yield return new WaitForSecondsRealtime(0.1f);

                        //Enemy attack VFX:
                        AttackEffect enemyAttackVFX = Instantiate(enemyAttacksVfx, activeBattlers[currentTurn].transform.position, enemyAttacksVfx.transform.rotation);

                        //Animación del efecto del movimiento:
                        //Instancio el moveEfect del enemy:
                        AttackEffect theEnemyAttackInstance = Instantiate(movesList[i].theEffect, selectedTarget.transform.position, selectedTarget.transform.rotation);

                        //Indico de qué color debe ser el shader Emission del Attackffect:
                        //OPCION 1: coger el color específico de cada BattleMove:
                        //theEnemyAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = enemyMove.emissionColor;

                        //OPCIÓN 2: coger el color de las distintas posibles opciones del Battlemanager:
                        theEnemyAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = SelectEmissionColor(movesList[i], 1);//El 1 indica que el autor es un enemigo (0 es player)

                        movePower = movesList[i].movePower;

                        StartCoroutine(DealDamageCo(selectedTarget, movePower, shouldWeakenShield));

                    }
                    else//Si el Move es multiTarget:
                    {
                        Debug.Log(activeBattlers[currentTurn].charName + " used " + enemyMove.moveName + " over everyone.");
                        //TODO: En lugar de "enemigo uso nombreDeAtaque sobre objetivo", que cada ataque tenga una
                        //descripción propia (y que los enemigos tengan ataques personalizados)
                        battleMessage.theText.text = enemyMove.battleDescription[0].Replace("u-", activeBattlers[currentTurn].charName);
                        battleMessage.ActivateTemp();

                        //Y espero a que el player lea el mensaje:
                        while (battleMessage.gameObject.activeInHierarchy)
                        {
                            yield return new WaitForEndOfFrame();
                        }

                        //recorro y aplico el movimiento a cada player:
                        for (int j = 0; j < players.Count; j++)
                        {
                            //Chequeo si aun no estoy en el ultimo ataque para no alterar el orden de los players a mitad en DealDamage
                            if (j < players.Count)
                            {
                                doingMultiTarget = true;
                            }
                            else
                            {
                                doingMultiTarget = false;
                            }

                            Instantiate(highlightAttackerEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
                            yield return new WaitForSecondsRealtime(0.1f);//Pequeña espera entre highlight y efecto

                            //Enemy attack VFX:
                            Instantiate(enemyAttacksVfx, activeBattlers[currentTurn].transform.position, enemyAttacksVfx.transform.rotation);

                            //Animación del efecto del movimiento:
                            //Instancio el moveEfect del enemy:
                            AttackEffect theEnemyAttackInstance = Instantiate(enemyMove.theEffect, players[j].transform.position, players[j].transform.rotation);
                            //Indico de qué color debe ser el shader Emission del Attackffect:
                            //OPCION 1: coger el color específico de cada BattleMove:
                            //theEnemyAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = enemyMove.emissionColor;

                            //OPCIÓN 2: coger el color de las distintas posibles opciones del Battlemanager:
                            theEnemyAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = SelectEmissionColor(movesList[i], 1);//El 1 indica que el autor es un enemigo (0 es player)


                            movePower = movesList[i].movePower;

                            StartCoroutine(DealDamageCo(players[j], movePower, shouldWeakenShield));
                        }
                    }
                }
            }
        }
        yield return null;
    }

    //public void OldDealDamage(int target, float movePower, bool affectsShield)
    //{
    //    float atkPwr = activeBattlers[currentTurn].strength;

    //    if (activeBattlers[currentTurn].isPlayer)
    //    {
    //        if (usingWeapon == "Sword")
    //        {
    //            atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].swordPwr;
    //        }
    //        if (usingWeapon == "Axe")
    //        {
    //            atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].axePwr;
    //        }
    //        if (usingWeapon == "Bow")
    //        {
    //            atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].bowPwr;
    //        }
    //        if (usingWeapon == "Staff")
    //        {
    //            atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].staffPwr;
    //        }
    //    }

    //    //float atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].swordPwr;//SOLO SE CUENTA EL POWER DE LA ESPADA
    //    float defPwr = activeBattlers[target].defence + activeBattlers[target].headArmorPwr + activeBattlers[target].bodyArmorPwr + activeBattlers[target].shieldPwr;

    //    float damageCalc = (atkPwr / defPwr) * movePower * Random.Range(.9f, 1.1f);
    //    int damageToGive = Mathf.RoundToInt(damageCalc);

    //    Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToGive + ") damage to " + activeBattlers[target].charName);

    //    activeBattlers[target].currentHP -= damageToGive;

    //    //Compruebo si se ataca a un player para animar su retroceso/respuesta
    //    if (activeBattlers[target].isPlayer)
    //    {
    //        //Faltaría contemplar la esquiva/ bloqueo!!
    //        activeBattlers[target].anim.SetTrigger("attacked");
    //    }

    //    //Si un enemigo recibe daño compruebo si es débil al tipo de ataque y si se le han roto las defensas y lo stuneo el turno actual y el siguiente:
    //    if (!activeBattlers[target].isPlayer && affectsShield)
    //    {
    //        activeBattlers[target].shieldAmount--;
    //        if (activeBattlers[target].shieldAmount < 0)
    //        {
    //            activeBattlers[target].shieldAmount = 0;
    //        }

    //        if (activeBattlers[target].shieldAmount <= 0 && activeBattlers[target].stunCount == 0)
    //        {
    //            activeBattlers[target].stunCount = 2;
    //        }
    //    }
    //    Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamageHP(damageToGive);

    //    UpdateUIStats();
    //    //Actualizo la UI de los turnos de ronda:
    //    UpdateRoundOrderUI();
    //}

    //Aplico el daño de ataques de enemigos
    public IEnumerator DealDamageCo(BattleChar target, float movePower, bool affectsShield)
    {
        float atkPwr = activeBattlers[currentTurn].strength;

        if (activeBattlers[currentTurn].isPlayer)
        {
            if (usingWeapon == "Sword")
            {
                atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].swordPwr;
            }
            if (usingWeapon == "Axe")
            {
                atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].axePwr;
            }
            if (usingWeapon == "Bow")
            {
                atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].bowPwr;
            }
            if (usingWeapon == "Staff")
            {
                atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].staffPwr;
            }
        }

        float defPwr = target.defence + target.headArmorPwr + target.bodyArmorPwr + target.shieldPwr;

        float damageCalc = (atkPwr / defPwr) * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);

        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToGive + ") damage to " + target.charName);

        target.currentHP -= damageToGive;

        //Compruebo si se ataca a un player para animar su retroceso/respuesta
        if (target.isPlayer)
        {
            //Faltaría contemplar la esquiva/ bloqueo!!
            target.anim.SetTrigger("attacked");
        }

        //Si un enemigo recibe daño compruebo si es débil al tipo de ataque y si se le han roto las defensas y lo stuneo el turno actual y el siguiente:
        if (!target.isPlayer && affectsShield)
        {
            Debug.Log("Esto no debería salir nunca!");
            target.shieldAmount--;
            if (target.shieldAmount < 0)
            {
                target.shieldAmount = 0;
            }

            if (target.shieldAmount <= 0 && target.stunCount == 0)
            {
                target.stunCount = 2;
            }
        }

        Instantiate(theDamageNumber, target.transform.position, target.transform.rotation).SetDamageHP(damageToGive);
        yield return new WaitForSecondsRealtime(0.2f);
        Instantiate(playerHitVfx, target.transform.position, target.transform.rotation);
        //Small shake:
        CameraShake(0.5f, 5f);

        UpdateUIStats();
        //Actualizo la UI de los turnos de ronda (creo que esto no debería durante un multitarget):
        if (!doingMultiTarget)
        {
            UpdateRoundOrderUI();
        }
    }

    public void UpdateUIStats()
    {
        //Asigno los datos de cada player en su playerStatHolder
        for (int i = 0; i < battlePlayers.Count; i++)
        {
            BattleChar playerData = battlePlayers[i];

            playerStatsHolder[i].gameObject.SetActive(true);
            playerName[i].text = playerData.tempName;
            playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
            playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
            playerSliderHP[i].maxValue = playerData.maxHP;
            playerSliderHP[i].value = playerData.currentHP;
            playerSliderMP[i].maxValue = playerData.maxMP;
            playerSliderMP[i].value = playerData.currentMP;

            //Actualizar el orden de turnos de la ronda actual y la siguiente:
            //(la determinación del orden se hace sólo al cambiar de ronda ya que hay random envuelto)


            //Actualizar el sistema de energía:

            for (int k = 0; k < energyHolder[i].transform.childCount; k++)//El numero de Spots tiene que coincidir con el battler.focusMaxLevel
            {
                if (battlePlayers[i].focusLevel > 0 && k < battlePlayers[i].focusLevel)
                {
                    energyHolder[i].transform.GetChild(k).GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    energyHolder[i].transform.GetChild(k).GetChild(0).gameObject.SetActive(false);
                }
            }

            //Actualizar iconos de cambio de estado:
            //Creo una lista temporal donde meto los cambios de estado activos:
            List<GameObject> activeStatsList = new List<GameObject>();

            //Ahora compruebo los cambios de estado
            if (playerData.turnsPoison > 0)
            {
                statsHolder[i].transform.GetChild(0).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerData.turnsPoison.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(0).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            if (playerData.turnsSleep > 0)
            {
                statsHolder[i].transform.GetChild(1).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerData.turnsSleep.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(1).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            if (playerData.turnsCursed > 0)
            {
                statsHolder[i].transform.GetChild(2).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = playerData.turnsCursed.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(2).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(2).gameObject.SetActive(false);
            }
            if (playerData.turnsBlind > 0)
            {
                statsHolder[i].transform.GetChild(3).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = playerData.turnsBlind.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(3).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(3).gameObject.SetActive(false);
            }
            if (playerData.turnsMute > 0)
            {
                statsHolder[i].transform.GetChild(4).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = playerData.turnsMute.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(4).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(4).gameObject.SetActive(false);
            }
            if (playerData.turnsModStr != 0)
            {
                if (playerData.turnsModStr > 0)
                {
                    statsHolder[i].transform.GetChild(5).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(6).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(5).GetChild(0).GetComponent<Text>().text = playerData.turnsModStr.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(5).gameObject);
                }
                else if (playerData.turnsModStr < 0)
                {
                    statsHolder[i].transform.GetChild(6).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(5).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(6).GetChild(0).GetComponent<Text>().text = playerData.turnsModStr.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(6).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(5).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(6).gameObject.SetActive(false);
            }
            if (playerData.turnsModDef != 0)
            {
                if (playerData.turnsModDef > 0)
                {
                    statsHolder[i].transform.GetChild(7).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(8).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(7).GetChild(0).GetComponent<Text>().text = playerData.turnsModDef.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(7).gameObject);
                }
                else if (playerData.turnsModDef < 0)
                {
                    statsHolder[i].transform.GetChild(8).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(7).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(8).GetChild(0).GetComponent<Text>().text = playerData.turnsModDef.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(8).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(7).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(8).gameObject.SetActive(false);
            }
            if (playerData.turnsModMgcStr != 0)
            {
                if (playerData.turnsModMgcStr > 0)
                {
                    statsHolder[i].transform.GetChild(9).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(10).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(9).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcStr.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(9).gameObject);
                }
                else if (playerData.turnsModMgcStr < 0)
                {
                    statsHolder[i].transform.GetChild(10).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(9).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(10).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcStr.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(10).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(9).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(10).gameObject.SetActive(false);
            }
            if (playerData.turnsModMgcDef != 0)
            {
                if (playerData.turnsModMgcDef > 0)
                {
                    statsHolder[i].transform.GetChild(11).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(12).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(11).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcDef.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(11).gameObject);
                }
                else if (playerData.turnsModMgcDef < 0)
                {
                    statsHolder[i].transform.GetChild(12).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(11).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(12).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcDef.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(12).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(11).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(12).gameObject.SetActive(false);
            }
            if (playerData.turnsModSpd != 0)
            {
                if (playerData.turnsModSpd > 0)
                {
                    statsHolder[i].transform.GetChild(13).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(14).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(13).GetChild(0).GetComponent<Text>().text = playerData.turnsModSpd.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(13).gameObject);
                }
                else if (playerData.turnsModSpd < 0)
                {
                    statsHolder[i].transform.GetChild(14).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(13).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(14).GetChild(0).GetComponent<Text>().text = playerData.turnsModSpd.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(14).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(13).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(14).gameObject.SetActive(false);
            }
            if (playerData.turnsModEva != 0)
            {
                if (playerData.turnsModEva > 0)
                {
                    statsHolder[i].transform.GetChild(15).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(16).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(15).GetChild(0).GetComponent<Text>().text = playerData.turnsModEva.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(15).gameObject);
                }
                else if (playerData.turnsModEva < 0)
                {
                    statsHolder[i].transform.GetChild(16).gameObject.SetActive(true);
                    statsHolder[i].transform.GetChild(15).gameObject.SetActive(false);
                    statsHolder[i].transform.GetChild(16).GetChild(0).GetComponent<Text>().text = playerData.turnsModEva.ToString();
                    activeStatsList.Add(statsHolder[i].transform.GetChild(16).gameObject);
                }
            }
            else
            {
                statsHolder[i].transform.GetChild(15).gameObject.SetActive(false);
                statsHolder[i].transform.GetChild(16).gameObject.SetActive(false);
            }
            if (playerData.turnsInmune > 0)
            {
                statsHolder[i].transform.GetChild(17).gameObject.SetActive(true);
                statsHolder[i].transform.GetChild(17).GetChild(0).GetComponent<Text>().text = playerData.turnsInmune.ToString();
                activeStatsList.Add(statsHolder[i].transform.GetChild(17).gameObject);
            }
            else
            {
                statsHolder[i].transform.GetChild(17).gameObject.SetActive(false);
            }

            //Ahora reubico los cambios de estado activos y voy llenando mi lista de posiciones:
            if (activeStatsList.Count > 0)
            {
                for (int m = 0; m < activeStatsList.Count; m++)
                {
                    activeStatsList[m].transform.position = statPosHolder[i].transform.GetChild(m).transform.position;
                }
            }
        }

        //Actualizo el escudo de los enemigos:


        Debug.Log("Fin UpdateUIStats");
    }

    //public void OldUpdateUIStats()
    //{
    //    //Lista temporal para ir descartando a los players que ya he establecido en los UIStats
    //    List<BattleChar> playersSet = new List<BattleChar>();

    //    for (int i = 0; i < playerStatsHolder.Length; i++)
    //    {
    //        if (activeBattlers.Count > i)
    //        {
    //            for (int j = 0; j < activeBattlers.Count; j++)
    //            {
    //                if (activeBattlers[j].isPlayer && !playersSet.Contains(activeBattlers[j]))
    //                {
    //                    playersSet.Add(activeBattlers[j]);

    //                    BattleChar playerData = activeBattlers[j];

    //                    playerStatsHolder[i].gameObject.SetActive(true);
    //                    playerName[i].text = playerData.tempName;
    //                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
    //                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
    //                    playerSliderHP[i].maxValue = playerData.maxHP;
    //                    playerSliderHP[i].value = playerData.currentHP;
    //                    playerSliderMP[i].maxValue = playerData.maxMP;
    //                    playerSliderMP[i].value = playerData.currentMP;

    //                    //Actualizar el orden de turnos de la ronda actual y la siguiente:
    //                    //(la determinación del orden se hace sólo al cambiar de ronda ya que hay random envuelto)


    //                    //Actualizar el sistema de energía:

    //                    for (int k = 0; k < energyHolder[i].transform.childCount; k++)//El numero de Spots tiene que coincidir con el battler.focusMaxLevel
    //                    {
    //                        if (activeBattlers[j].focusLevel > 0 && k < activeBattlers[j].focusLevel)
    //                        {
    //                            energyHolder[i].transform.GetChild(k).GetChild(0).gameObject.SetActive(true);
    //                        }
    //                        else
    //                        {
    //                            energyHolder[i].transform.GetChild(k).GetChild(0).gameObject.SetActive(false);
    //                        }
    //                    }

    //                    //Actualizar iconos de cambio de estado:
    //                    //Creo una lista temporal donde meto los cambios de estado activos:
    //                    List<GameObject> activeStatsList = new List<GameObject>();

    //                    //Ahora compruebo los cambios de estado
    //                    if (playerData.turnsPoison > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(0).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerData.turnsPoison.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(0).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(0).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsSleep > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(1).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = playerData.turnsSleep.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(1).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(1).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsCursed > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(2).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = playerData.turnsCursed.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(2).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(2).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsBlind > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(3).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(3).GetChild(0).GetComponent<Text>().text = playerData.turnsBlind.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(3).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(3).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsMute > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(4).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = playerData.turnsMute.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(4).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(4).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModStr != 0)
    //                    {
    //                        if (playerData.turnsModStr > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(5).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(6).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(5).GetChild(0).GetComponent<Text>().text = playerData.turnsModStr.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(5).gameObject);
    //                        }
    //                        else if (playerData.turnsModStr < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(6).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(5).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(6).GetChild(0).GetComponent<Text>().text = playerData.turnsModStr.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(6).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(5).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(6).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModDef != 0)
    //                    {
    //                        if (playerData.turnsModDef > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(7).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(8).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(7).GetChild(0).GetComponent<Text>().text = playerData.turnsModDef.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(7).gameObject);
    //                        }
    //                        else if (playerData.turnsModDef < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(8).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(7).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(8).GetChild(0).GetComponent<Text>().text = playerData.turnsModDef.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(8).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(7).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(8).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModMgcStr != 0)
    //                    {
    //                        if (playerData.turnsModMgcStr > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(9).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(10).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(9).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcStr.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(9).gameObject);
    //                        }
    //                        else if (playerData.turnsModMgcStr < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(10).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(9).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(10).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcStr.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(10).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(9).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(10).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModMgcDef != 0)
    //                    {
    //                        if (playerData.turnsModMgcDef > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(11).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(12).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(11).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcDef.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(11).gameObject);
    //                        }
    //                        else if (playerData.turnsModMgcDef < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(12).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(11).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(12).GetChild(0).GetComponent<Text>().text = playerData.turnsModMgcDef.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(12).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(11).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(12).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModSpd != 0)
    //                    {
    //                        if (playerData.turnsModSpd > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(13).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(14).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(13).GetChild(0).GetComponent<Text>().text = playerData.turnsModSpd.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(13).gameObject);
    //                        }
    //                        else if (playerData.turnsModSpd < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(14).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(13).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(14).GetChild(0).GetComponent<Text>().text = playerData.turnsModSpd.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(14).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(13).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(14).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsModEva != 0)
    //                    {
    //                        if (playerData.turnsModEva > 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(15).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(16).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(15).GetChild(0).GetComponent<Text>().text = playerData.turnsModEva.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(15).gameObject);
    //                        }
    //                        else if (playerData.turnsModEva < 0)
    //                        {
    //                            statsHolder[i].transform.GetChild(16).gameObject.SetActive(true);
    //                            statsHolder[i].transform.GetChild(15).gameObject.SetActive(false);
    //                            statsHolder[i].transform.GetChild(16).GetChild(0).GetComponent<Text>().text = playerData.turnsModEva.ToString();
    //                            activeStatsList.Add(statsHolder[i].transform.GetChild(16).gameObject);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(15).gameObject.SetActive(false);
    //                        statsHolder[i].transform.GetChild(16).gameObject.SetActive(false);
    //                    }
    //                    if (playerData.turnsInmune > 0)
    //                    {
    //                        statsHolder[i].transform.GetChild(17).gameObject.SetActive(true);
    //                        statsHolder[i].transform.GetChild(17).GetChild(0).GetComponent<Text>().text = playerData.turnsInmune.ToString();
    //                        activeStatsList.Add(statsHolder[i].transform.GetChild(17).gameObject);
    //                    }
    //                    else
    //                    {
    //                        statsHolder[i].transform.GetChild(17).gameObject.SetActive(false);
    //                    }

    //                    //Ahora reubico los cambios de estado activos y voy llenando mi lista de posiciones:
    //                    if (activeStatsList.Count > 0)
    //                    {
    //                        for (int m = 0; m < activeStatsList.Count; m++)
    //                        {
    //                            activeStatsList[m].transform.position = statPosHolder[i].transform.GetChild(m).transform.position;
    //                        }
    //                    }

    //                    break;

    //                }
    //                else if (!activeBattlers[j].isPlayer)
    //                {
    //                    playerStatsHolder[i].gameObject.SetActive(false);
    //                }
    //            }

    //        }
    //        else
    //        {
    //            playerStatsHolder[i].gameObject.SetActive(false);
    //            //playerName[i].gameObject.SetActive(false);
    //        }
    //    }

    //    //Actualizo el escudo de los enemigos:


    //    Debug.Log("Fin UpdateUIStats");
    //}



    public void EnergyUpButton()//Ojo, comprobar que no se pueda pulsar este boton tras seleccionar al enemigo
    {
        if (activeBattlers[currentTurn].isPlayer)
        {
            if (activeBattlers[currentTurn].focusLevel > 0)
            {
                if(activeBattlers[currentTurn].focusCharged == 3)
                {
                    //Si ya estoy en el nivel 3 no subo más:
                    Debug.Log("Focus level maxed out!");
                    PlayErrorSFX();
                }
                else
                {
                    PlaySelectSFX();
                    activeBattlers[currentTurn].focusLevel--;
                    activeBattlers[currentTurn].focusCharged++;

                    itemButton.SetActive(false);
                    fleeButton.SetActive(false);

                    //Vfx de focus:
                    if (activeBattlers[currentTurn].focusCharged == 1)
                    {
                        AudioManager.instance.PlaySFX(53);
                        Instantiate(focus1Vfx, activeBattlers[currentTurn].transform);
                    }
                    else if (activeBattlers[currentTurn].focusCharged == 2)
                    {
                        AudioManager.instance.PlaySFX(53);
                        Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                        Instantiate(focus2Vfx, activeBattlers[currentTurn].transform);
                    }
                    else if (activeBattlers[currentTurn].focusCharged == 3)
                    {
                        AudioManager.instance.PlaySFX(53);
                        Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                        Instantiate(focus3Vfx, activeBattlers[currentTurn].transform);
                        ////Como he alcanzado el nivel maximo de focus, desactivo el focusUpButton:
                        //buttons[5].interactable = false;
                        //buttons[5].transform.GetChild(0).GetComponent<Image>().color = buttonsColorFaded;
                    }

                    UpdateUIStats();
                }
            }
        }
    }

    public void EnergyDownButton()
    {
        if (activeBattlers[currentTurn].isPlayer)
        {
            if (activeBattlers[currentTurn].focusCharged > 0)
            {
                PlaySelectSFX();

                activeBattlers[currentTurn].focusCharged--;
                activeBattlers[currentTurn].focusLevel++;

                if (activeBattlers[currentTurn].focusCharged == 0)
                {
                    itemButton.SetActive(true);
                    fleeButton.SetActive(true);
                    //Destruyo el focus1Vfx que debe quedar:
                    AudioManager.instance.PlaySFX(54);
                    Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                }

                //Actualizo el focusVfx:
                if (activeBattlers[currentTurn].focusCharged == 1)
                {
                    AudioManager.instance.PlaySFX(54);
                    Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                    Instantiate(focus1Vfx, activeBattlers[currentTurn].transform);
                }
                else if (activeBattlers[currentTurn].focusCharged == 2)
                {
                    AudioManager.instance.PlaySFX(54);
                    Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                    Instantiate(focus2Vfx, activeBattlers[currentTurn].transform);
                    ////Activo el boton up (por si he bajado del nivel 3):
                    //buttons[5].interactable = true;
                    //buttons[5].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;
                }

                UpdateUIStats();
            }
            else
            {
                PlayErrorSFX();
            }
        }
    }

    public void PlayerAttack(int selectedTarget)
    {
        //Desactivar los iconos de seleccion de target
        if (activeMove.multiTarget)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                activeBattlers[Targets[i]].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            activeBattlers[selectedTarget].transform.GetChild(0).gameObject.SetActive(false);
        }

        //Detecto mi tipo de ataque
        GetTypeOfAttack(activeMove);

        //Desactivo los botones de la UI:
        uiButtonsHolder.SetActive(false);

        //Desactivo el Targets Menu
        targetMenu.SetActive(false);

        //La siguiente Corutina gestiona todo tipo de ataques hechos por los Players:
        StartCoroutine(ApplyMoveCo(selectedTarget));
    }


    public IEnumerator ApplyMoveCo(int selectedTarget)//Movimiento de los players
    {
        //Instancio animacion de resaltar atacante
        Instantiate(highlightAttackerEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
        yield return new WaitForSecondsRealtime(0.2f);

        //Animación de ataque:
        activeBattlers[currentTurn].anim.SetTrigger(activeMove.animKeyword);

        //VFX de ataque:
        Instantiate(playerAttacksVfx, activeBattlers[currentTurn].transform.position, playerAttacksVfx.transform.rotation);

        Debug.Log("activeBattler(focus): " + activeBattlers[currentTurn] + "(" + activeBattlers[currentTurn].focusCharged + ")");
        yield return new WaitForSecondsRealtime(0.2f);//Esta espera podría depender de la animKeyword.

        if (activeMove.multiTarget)//Si es multiTarget (ataca a todos a la vez)
        {
            for (int i = 0; i < Targets.Count; i++)//Recorro la lista de enemigos vivos (Targets)
            {
                selectedTarget = Targets[i];//Hago que selectedTarget sea cada uno de los enemigos vivos

                //Lanzo mensaje de batalla del movimiento:
                battleMessage.theText.text = activeMove.battleDescription[0].Replace("u-", activeBattlers[currentTurn].charName);
                battleMessage.ActivateTemp();
                Debug.Log("mensaje a narrar: " + activeMove.battleDescription[0].Replace("u-", activeBattlers[currentTurn].charName));

                //Animación de efecto del ataque:
                //Instancio el moveEffect:
                AttackEffect theAttackInstance = Instantiate(activeMove.theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                
                //Indico de qué color debe ser el shader Emission del Attackffect:
                //OPCION 1: coger el color específico de cada BattleMove:
                //theAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = activeMove.emissionColor;

                //OPCIÓN 2: coger el color de las distintas posibles opciones del Battlemanager:
                theAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = SelectEmissionColor(activeMove, 0);//El 1 indica que el autor es un enemigo (0 es player)


                yield return new WaitForSecondsRealtime(0.15f);//Esta espera podría depender del activeMove.

                Debug.Log("apply multitarget");
                DealEffect(selectedTarget);
            }
        }
        else if (!activeMove.multiTarget)//Si no es multiTarget
        {
            //Lanzo mensaje de batalla del movimiento:
            battleMessage.theText.text = activeMove.battleDescription[0].Replace("u-", activeBattlers[currentTurn].charName).Replace("t-", activeBattlers[selectedTarget].charName);
            battleMessage.ActivateTemp();
            Debug.Log("apply monotarget a " + activeBattlers[selectedTarget].charName);
            yield return new WaitForSecondsRealtime(0.1f);//Pequeña espera para leer el mensaje

            //Animación de efecto del ataque:
            //instancio el moveEffect:
            AttackEffect theAttackInstance = Instantiate(activeMove.theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
            //Indico de qué color debe ser el shader Emission del Attackffect:
            //OPCION 1: coger el color específico de cada BattleMove:
            //theAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = activeMove.emissionColor;

            //OPCIÓN 2: coger el color de las distintas posibles opciones del Battlemanager:
            theAttackInstance.gameObject.GetComponent<SpriteRenderer>().material.color = SelectEmissionColor(activeMove, 0);//El 1 indica que el autor es un enemigo (0 es player)


            yield return new WaitForSecondsRealtime(0.1f);//Esta espera podría depender del activeMove.
            DealEffect(selectedTarget);

        }

        //Aquí se comprobaría si estoy en focus y es repetible:
        if (activeBattlers[currentTurn].focusCharged > 0)//Si estoy usando focus
        {
            activeBattlers[currentTurn].justUsedFocus = true;
            if (activeMove.focusIterates)//Si el movimiento usado debe iterar por focus
            {
                Debug.Log("iteration");
                activeBattlers[currentTurn].focusCharged--;//Resto 1 al focus

                //Desactivo focusVfx:
                if (activeBattlers[currentTurn].focusCharged == 0)
                {
                    Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                }

                StartCoroutine(ApplyMoveCo(selectedTarget));//Itero el ataque
            }
            else
            {
                //Descuento el focus cargado
                activeBattlers[currentTurn].focusCharged -= activeBattlers[currentTurn].focusCharged;

                //Desactivo focusVfx:
                if (activeBattlers[currentTurn].focusCharged == 0)
                {
                    Destroy(activeBattlers[currentTurn].transform.GetChild(2).gameObject);
                }

                ////Activo el boton up (por si había llegado al nivel 3):
                //buttons[5].interactable = true;
                //buttons[5].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;

                Debug.Log("El battler " + activeBattlers[currentTurn].name + " acaba aquí su turno con un focusCharged de " + activeBattlers[currentTurn].focusCharged);
                NextTurn();//Es muy importante que no se cambie el turno hasta que cada battler haya terminado de ejecutar el suyo.
            }
        }
        else//Si no he cargado focus
        {
            Debug.Log("El battler " + activeBattlers[currentTurn].name + " acaba aquí su turno con un focusCharged de " + activeBattlers[currentTurn].focusCharged);
            NextTurn();//Es muy importante que no se cambie el turno hasta que cada battler haya terminado de ejecutar el suyo.
        }
    }

    public void DealEffect(int target)//Applico efecto de movimiento de players
    {
        //Compruebo si desbloqueo debilidad oculta:
        CheckUnknownWeakness(typeOfAttack, activeBattlers[target]);

        //Compruebo debilidad
        CheckShieldBreak(typeOfAttack, activeBattlers[target]);
        //Almaceno la variable localmente por si acaso
        bool affectsShield = shouldWeakenShield;
        Debug.Log("affectsShield es " + affectsShield);

        //Inicializo variables de modificación por focus:
        float tempMovePower = activeMove.movePower;
        int tempMoveDuration = activeMove.turnsDuration;

        //Pillo el nivel del focus en una var local:
        int focusLevel = activeBattlers[currentTurn].focusCharged;

        //Compruebo si estoy usando focus y aplico su efecto:
        if (activeBattlers[currentTurn].isPlayer && focusLevel > 0)
        {
            if (activeMove.focusMultiplies)
            {
                tempMovePower = activeMove.movePower * activeBattlers[currentTurn].focusCharged * activeMove.multiplier;
                Debug.Log("tempMovePower(" + tempMovePower + " = movePower(" + activeMove.movePower + " * focusCharged(" + activeBattlers[currentTurn].focusCharged + " * move.multiplier(" + activeMove.multiplier);
            }
            else if (activeMove.focusExtends)
            {
                tempMoveDuration = activeMove.turnsDuration + focusLevel * 2;
                Debug.Log("tempMoveDuration(" + tempMoveDuration + " = moveTurnsDuration(" + activeMove.turnsDuration + " * focusCharged(" + activeBattlers[currentTurn].focusCharged);
            }
            else
            {
                Debug.Log("Iterando ataque");
            }
        }


        //Gestión de ataques que quitan o recuperan Hp/Mp:
        if (activeMove.hpMod || activeMove.mpMod || activeMove.statMod)
        {
            //Inicializo aquí estas vars por tema de scope
            float atkPwr = 0;
            float defPwr = 0;

            if (activeMove.physical)//Si es un ataque físico
            {
                //Calculo el daño a ejercer (stat base + su mod por si está bajo los efectos de un nerf/buff):
                atkPwr = activeBattlers[currentTurn].strength * activeBattlers[currentTurn].modStr;
                defPwr = activeBattlers[target].defence * activeBattlers[target].modDef;

                //Si el atacante es un player hay que sumarle los efectos del equipamiento:
                if (activeBattlers[currentTurn].isPlayer)
                {
                    if (usingWeapon == "Sword")
                    {
                        atkPwr += activeBattlers[currentTurn].swordPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.Strength(" + activeBattlers[currentTurn].strength + ") + sword strength(" + activeBattlers[currentTurn].swordPwr);
                    }
                    if (usingWeapon == "Axe")
                    {
                        atkPwr += activeBattlers[currentTurn].axePwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.Strength(" + activeBattlers[currentTurn].strength + ") + axe strength(" + activeBattlers[currentTurn].axePwr);
                    }
                    if (usingWeapon == "Bow")
                    {
                        atkPwr += activeBattlers[currentTurn].bowPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.Strength(" + activeBattlers[currentTurn].strength + ") + bow strength(" + activeBattlers[currentTurn].bowPwr);
                    }
                    if (usingWeapon == "Staff")
                    {
                        atkPwr += activeBattlers[currentTurn].staffPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.Strength(" + activeBattlers[currentTurn].strength + ") + staff strength(" + activeBattlers[currentTurn].staffPwr);
                    }
                }

                //Si el target es un player hay que sumarle los efectos del equipamiento:
                if (activeBattlers[target].isPlayer)
                {
                    defPwr += activeBattlers[target].headArmorPwr + activeBattlers[target].bodyArmorPwr + activeBattlers[target].shieldPwr;
                }
            }
            else if (activeMove.magical)//Si es un ataque mágico
            {
                //Calculo el daño a ejercer (stat base + su mod por si está bajo los efectos de un nerf/buff):
                atkPwr = activeBattlers[currentTurn].magicStr * activeBattlers[currentTurn].modMgcStr;
                defPwr = activeBattlers[target].magicDef * activeBattlers[target].modMgcDef;

                //Si el atacante es un player hay que sumarle los efectos del equipamiento:
                if (activeBattlers[currentTurn].isPlayer)
                {
                    if (usingWeapon == "Sword")
                    {
                        atkPwr += activeBattlers[currentTurn].swordMgcPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.magicStr(" + activeBattlers[currentTurn].magicStr + ") + sword magicStr(" + activeBattlers[currentTurn].swordMgcPwr);
                    }
                    if (usingWeapon == "Axe")
                    {
                        atkPwr += activeBattlers[currentTurn].axeMgcPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.magicStr(" + activeBattlers[currentTurn].magicStr + ") + axe magicStr(" + activeBattlers[currentTurn].axeMgcPwr);
                    }
                    if (usingWeapon == "Bow")
                    {
                        atkPwr += activeBattlers[currentTurn].bowMgcPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.magicStr(" + activeBattlers[currentTurn].magicStr + ") + bow magicStr(" + activeBattlers[currentTurn].bowMgcPwr);
                    }
                    if (usingWeapon == "Staff")
                    {
                        atkPwr += activeBattlers[currentTurn].staffMgcPwr;
                        Debug.Log("atkPwr(" + atkPwr + ") = user.magicStr(" + activeBattlers[currentTurn].magicStr + ") + staff magicStr(" + activeBattlers[currentTurn].staffMgcPwr);
                    }
                }

                //Si el target es un player hay que sumarle los efectos del equipamiento:
                if (activeBattlers[target].isPlayer)
                {
                    defPwr += activeBattlers[target].headArmorMgcPwr + activeBattlers[target].bodyArmorMgcPwr + activeBattlers[target].shieldMgcPwr;
                }
            }

            //Si el ataque ademas de hacer daño o curar tiene efecto sobre algún stat (como revivir)
            if (activeMove.statMod)
            {
                //Aplico la lógica de DealEffect según el stat correspondiente:
                if (activeMove.strengthMod)//Si afecta a la strength
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModStr += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModStr -= tempMoveDuration;
                    }
                }

                if (activeMove.defenceMod)//Si afecta a la def
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModDef += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModDef -= tempMoveDuration;
                    }
                }

                if (activeMove.speedMod)//Si afecta a la speed
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModSpd += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModSpd -= tempMoveDuration;
                    }
                }

                if (activeMove.magicStrMod)//Si afecta a la magic strength
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModMgcStr += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModMgcStr -= tempMoveDuration;
                    }
                }

                if (activeMove.magicDefMod)//Si afecta a la magic def
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModMgcDef += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModMgcDef -= tempMoveDuration;
                    }
                }

                if (activeMove.evasionMod)//Si afecta a la evasion
                {
                    if (activeMove.moveModStatMultiplier > 1)
                    {
                        activeBattlers[target].turnsModEva += tempMoveDuration;
                    }
                    else
                    {
                        activeBattlers[target].turnsModEva -= tempMoveDuration;
                    }
                }

            }


            //Calculo el daño a ejercer:
            //Pongo los stats a 1 no vaya a ser que haya algun 0 y se rallen las divisiones
            if (defPwr == 0)
            {
                defPwr = 1f;
            }
            if (atkPwr == 0)
            {
                atkPwr = 1f;
            }

            //float damageCalc = (atkPwr / defPwr) * tempMovePower * Random.Range(.9f, 1.1f);
            //Debug.Log("(" + atkPwr + " / " + defPwr + ") * " + tempMovePower + " * " + Random.Range(.9f, 1.1f));



            //Hasta aquí el cálculo del ataque, ahora lo aplico y represento:
            if (activeMove.movePower > 0) //Si es un ataque que causa daño
            {
                float damageCalc = (atkPwr - defPwr * 0.5f) * tempMovePower/100 * Random.Range(.95f, 1.05f);
                Debug.Log("atkPwr(" + atkPwr + ") - defPwr(" + defPwr + ") * 0.5" + ") * tempMovePower/100(" + tempMovePower/100 + ") * random5%");

                //Si el target es un enemigo stuneado, multiplico el daño x1.9
                if(activeBattlers[target].shieldAmount <= 0)
                {
                    damageCalc *= 1.9f;
                }

                //Redondeo el daño a un entero:
                int damageToGive = Mathf.RoundToInt(damageCalc);

                Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageToGive + " damage to " + activeBattlers[target].charName);

                if (activeMove.hpMod)//Si afecta a los hp
                {
                    activeBattlers[target].currentHP -= damageToGive;
                    Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamageHP(damageToGive);

                    //Vfx del hit:
                    if (activeMove.physical)//Hit de ataques físicos
                    {
                        Instantiate(physicalHitVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                    }
                    else//Hit de ataques mágicos
                    {
                        Instantiate(specialHitVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                    }
                    //Small shake:
                    CameraShake(0.5f, 5f);
                }
                if (activeMove.mpMod)//Si afecta a los mp
                {
                    activeBattlers[target].currentMP -= damageToGive;
                    Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamageMP(damageToGive);

                    //Vfx del hit:
                    if (activeMove.physical)//Hit de ataques físicos
                    {
                        Instantiate(physicalHitVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                    }
                    else//Hit de ataques mágicos
                    {
                        Instantiate(specialHitVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                    }
                    //Small shake:
                    CameraShake(0.5f, 5f);
                }

                //Compruebo si se ataca a un player para animar su retroceso/respuesta
                if (activeBattlers[target].isPlayer)
                {
                    //Faltaría contemplar la esquiva/ bloqueo!! (ToDo)
                    activeBattlers[target].anim.SetTrigger("attacked");

                    //HitVfx a player:
                    Instantiate(playerHitVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                    //Small shake:
                    CameraShake(0.5f, 5f);
                }

                //Si un enemigo recibe daño compruebo si es débil al tipo de ataque y si se le han roto las defensas y lo stuneo el turno actual y el siguiente:
                if (!activeBattlers[target].isPlayer && affectsShield)
                {
                    activeBattlers[target].shieldAmount--;
                    if (activeBattlers[target].shieldAmount <= 0)
                    {
                        activeBattlers[target].shieldAmount = 0;
                    }
                    Debug.Log("Escudo de " + activeBattlers[target].charName + " baja a " + activeBattlers[target].shieldAmount);

                    //Escudo roto aquí:
                    if (activeBattlers[target].shieldAmount <= 0 && activeBattlers[target].stunCount == 0)
                    {
                        activeBattlers[target].stunCount = 2;
                        Debug.Log("Escudo de " + activeBattlers[target].charName + " roto!");
                        //Vfx de shieldbreak:
                        Instantiate(shieldBreakVfx, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation);
                        activeBattlers[target].transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = shieldBrokenSprite;
                        //Big shake:
                        CameraShake(1f, 10);
                    }

                    //Actualizo UI del escudo del enemigo:
                    Debug.Log("Actualizo escudo del enemy " + activeBattlers[target].charName);
                    activeBattlers[target].transform.GetChild(2).gameObject.SetActive(true);

                    //Actualizar las defensas
                    activeBattlers[target].transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshPro>().text = activeBattlers[target].shieldAmount.ToString();
                }

            }
            else if (activeMove.movePower < 0) //Si el ataque cura
            {
                //Calculo el daño a ejercer (stat base + su mod por si está bajo los efectos de un nerf/buff):
                atkPwr = activeBattlers[currentTurn].magicDef * activeBattlers[currentTurn].modMgcDef;

                //Si el atacante es un player hay que sumarle los efectos del equipamiento:
                if (activeBattlers[currentTurn].isPlayer)
                {
                    if (usingWeapon == "Sword")
                    {
                        atkPwr += activeBattlers[currentTurn].swordMgcPwr;
                        Debug.Log("recoveryPwr(" + atkPwr + ") = user.magicDef(" + activeBattlers[currentTurn].magicDef + ") + sword magicPwr(" + activeBattlers[currentTurn].swordMgcPwr);
                    }
                    if (usingWeapon == "Axe")
                    {
                        atkPwr += activeBattlers[currentTurn].axeMgcPwr;
                        Debug.Log("recoveryPwr(" + atkPwr + ") = user.magicDef(" + activeBattlers[currentTurn].magicDef + ") + axe magicPwr(" + activeBattlers[currentTurn].axeMgcPwr);
                    }
                    if (usingWeapon == "Bow")
                    {
                        atkPwr += activeBattlers[currentTurn].bowMgcPwr;
                        Debug.Log("recoveryPwr(" + atkPwr + ") = user.magicDef(" + activeBattlers[currentTurn].magicDef + ") + bow magicPwr(" + activeBattlers[currentTurn].bowMgcPwr);
                    }
                    if (usingWeapon == "Staff")
                    {
                        atkPwr += activeBattlers[currentTurn].staffMgcPwr;
                        Debug.Log("recoveryPwr(" + atkPwr + ") = user.magicDef(" + activeBattlers[currentTurn].magicDef + ") + staff magicPwr(" + activeBattlers[currentTurn].staffMgcPwr);
                    }
                }

                float damageCalc = atkPwr * tempMovePower/100 * Random.Range(.95f, 1.05f);
                Debug.Log("recovery = user.magicDef(" + atkPwr + ") * moveHealingPwr(" + tempMovePower/100 + ") * random5%" + Random.Range(.95f, 1.05f));


                //Redondeo el daño a un entero:
                int damageToGive = Mathf.RoundToInt(damageCalc);

                Debug.Log(activeBattlers[currentTurn].charName + " is recovering " + damageToGive + " points to " + activeBattlers[target].charName);

                if (activeMove.hpMod)
                {
                    activeBattlers[target].currentHP -= damageToGive;

                    Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetRecoveryHP(damageToGive);

                    //No puedo haberme curado por encima de mis statsMaximos:
                    //(Opcional: comprobar atributo de unlimitedMaxHP para poder curarme por encima de mi vida)
                    if (activeBattlers[target].currentHP > activeBattlers[target].maxHP)
                    {
                        activeBattlers[target].currentHP = activeBattlers[target].maxHP;
                    }
                }
                if (activeMove.mpMod)
                {
                    activeBattlers[target].currentMP -= damageToGive;

                    Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetRecoveryMP(damageToGive);

                    //No puedo haberme curado por encima de mis statsMaximos:
                    //(Opcional: comprobar atributo de unlimitedMaxMP para poder curarme por encima de mi vida)
                    if (activeBattlers[target].currentMP > activeBattlers[target].maxMP)
                    {
                        activeBattlers[target].currentMP = activeBattlers[target].maxMP;
                    }
                }
            }
            else//si el activeMove.movePower == 0, será un ataque de cambio de estado
            //y como no aplica daño directo lo dejo en blanco en principio
            {

            }

        }
        //Este else está obsoleto:
        //else //Si el ataque es un modificador de estados
        //{
        //    //Aplico la lógica de DealEffect según el stat correspondiente:
        //    if (activeMove.strengthMod)//Si afecta a la strength
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }

        //    if (activeMove.defenceMod)//Si afecta a la def
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }

        //    if (activeMove.speedMod)//Si afecta a la speed
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }

        //    if (activeMove.magicStrMod)//Si afecta a la magic strength
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }

        //    if (activeMove.magicDefMod)//Si afecta a la magic def
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }

        //    if (activeMove.evasionMod)//Si afecta a la evasion
        //    {
        //        StatModAttackManagement(activeBattlers[target].modStr, activeBattlers[target].turnsModStr, tempMoveDuration);
        //    }
        //}

        UpdateUIStats();
        //Actualizo la UI de los turnos de ronda a menos que esté en una iteración por focus:
        if (activeBattlers[currentTurn].isPlayer && focusLevel > 0)
        {
            Debug.Log(activeBattlers[currentTurn].charName + "está iterando y no se actualiza orden de turnos");
        }
        else
        {
            UpdateRoundOrderUI();
        }
    }

    //OBSOLETO: Lógica de DealEffect para stat modifiers según el stat:
    //private void StatModAttackManagement(float modStat, int turnsModStat, int tempMoveDuration)
    //{
    //    //El tiempo y efecto del modificador del stat depende del modificador opuesto:
    //    if (modStat < 0 && activeMove.moveModStatMultiplier > 0)//Si actualmente hay un nerf en strength y yo aplico un buff
    //    {
    //        if (turnsModStat >= tempMoveDuration)//Si hay más turnos pendientes del nerf que los que yo buffeo
    //        {
    //            turnsModStat -= tempMoveDuration;//Le resto los que yo buffeo y mantengo el nerf
    //        }
    //        else//Si yo buffeo más de los que hay pendientes de nerf
    //        {
    //            turnsModStat = tempMoveDuration - turnsModStat;//Descuento los pendientes
    //            modStat = activeMove.moveModStatMultiplier;//Y aplico el buff
    //        }
    //    }
    //    else if (modStat > 0 && activeMove.moveModStatMultiplier < 0)//Si actualmente hay un buff en strength y yo aplico un nerf
    //    {
    //        if (turnsModStat >= tempMoveDuration)//Si hay más turnos pendientes del buff que los que yo nerfeo
    //        {
    //            turnsModStat -= tempMoveDuration;//Le resto los que yo nerfeo y mantengo el buff
    //        }
    //        else//Si yo nerfeo más de los que hay pendientes de buff
    //        {
    //            turnsModStat = tempMoveDuration - turnsModStat;//Descuento los pendientes
    //            modStat = activeMove.moveModStatMultiplier;//Y aplico el nerf
    //        }
    //    }
    //    else//Si no hay nerf o buff de strength vigentes opuestos a los que yo aplico
    //    {
    //        modStat = activeMove.moveModStatMultiplier;//Aplico el nerf/buff
    //        turnsModStat += tempMoveDuration;//Sumo los turnos que dura mi nerf/buff
    //    }
    //}


    public string GetTypeOfAttack(BattleMove move)
    {
        typeOfAttack = "";

        if (move.isSword)
        {
            typeOfAttack = "Sword";
            return typeOfAttack;
        }
        else if (move.isAxe)
        {
            typeOfAttack = "Axe";
            return typeOfAttack;
        }
        else if (move.isBow)
        {
            typeOfAttack = "Bow";
            return typeOfAttack;
        }
        else if (move.isStaff)
        {
            typeOfAttack = "Staff";
            return typeOfAttack;
        }
        else if (move.isFire)
        {
            typeOfAttack = "Fire";
            return typeOfAttack;
        }
        else if (move.isIce)
        {
            typeOfAttack = "Ice";
            return typeOfAttack;
        }
        else if (move.isThunder)
        {
            typeOfAttack = "Thunder";
            return typeOfAttack;
        }
        else if (move.isWind)
        {
            typeOfAttack = "Wind";
            return typeOfAttack;
        }
        else if (move.isWater)
        {
            typeOfAttack = "isWater";
            return typeOfAttack;
        }
        else if (move.isEarth)
        {
            typeOfAttack = "Earth";
            return typeOfAttack;
        }
        else if (move.isDark)
        {
            typeOfAttack = "Dark";
            return typeOfAttack;
        }
        else if (move.isLight)
        {
            typeOfAttack = "Light";
            return typeOfAttack;
        }

        return null;
    }

    public void GetWeaknessList(BattleChar enemy)
    {
        //Hago una lista con las debilidades del enemigo
        tempEnemyWeaknessList = new List<string>();

        if (enemy.weakToSword)
        {
            tempEnemyWeaknessList.Add("Sword");
        }
        if (enemy.weakToAxe)
        {
            tempEnemyWeaknessList.Add("Axe");
        }
        if (enemy.weakToBow)
        {
            tempEnemyWeaknessList.Add("Bow");
        }
        if (enemy.weakToStaff)
        {
            tempEnemyWeaknessList.Add("Staff");
        }
        if (enemy.weakToFire)
        {
            tempEnemyWeaknessList.Add("Fire");
        }
        if (enemy.weakToIce)
        {
            tempEnemyWeaknessList.Add("Ice");
        }
        if (enemy.weakToThunder)
        {
            tempEnemyWeaknessList.Add("Thunder");
        }
        if (enemy.weakToWind)
        {
            tempEnemyWeaknessList.Add("Wind");
        }
        if (enemy.weakToWater)
        {
            tempEnemyWeaknessList.Add("Water");
        }
        if (enemy.weakToEarth)
        {
            tempEnemyWeaknessList.Add("Earth");
        }
        if (enemy.weakToDark)
        {
            tempEnemyWeaknessList.Add("Dark");
        }
        if (enemy.weakToLight)
        {
            tempEnemyWeaknessList.Add("Light");
        }

        enemy.weaknessStringList = new List<string>();
        enemy.weaknessStringList = tempEnemyWeaknessList;
    }

    //private void SetWeaknessSpriteList(BattleChar enemy)
    //{
    //    weaknessSpritesList = new List<Sprite>();

    //    //Creo que debería usar enemy.weaknessList en lugar de tempEnemyWeaknessList, pero en principio da igual
    //    for (int i = 0; i < tempEnemyWeaknessList.Count; i++)
    //    {
    //        if (tempEnemyWeaknessList[i] == "Sword")
    //        {
    //            weaknessSpritesList.Add(iconSword);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Axe")
    //        {
    //            weaknessSpritesList.Add(iconAxe);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Bow")
    //        {
    //            weaknessSpritesList.Add(iconBow);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Staff")
    //        {
    //            weaknessSpritesList.Add(iconStaff);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Fire")
    //        {
    //            weaknessSpritesList.Add(iconFire);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Ice")
    //        {
    //            weaknessSpritesList.Add(iconIce);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Thunder")
    //        {
    //            weaknessSpritesList.Add(iconThunder);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Wind")
    //        {
    //            weaknessSpritesList.Add(iconWind);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Water")
    //        {
    //            weaknessSpritesList.Add(iconWater);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Earth")
    //        {
    //            weaknessSpritesList.Add(iconEarth);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Dark")
    //        {
    //            weaknessSpritesList.Add(iconDark);
    //        }
    //        else if (tempEnemyWeaknessList[i] == "Light")
    //        {
    //            weaknessSpritesList.Add(iconLight);
    //        }
    //    }
    //    enemy.weaknessSpriteList = weaknessSpritesList;
    //}

    public void SetWeaknessSpritesList(BattleChar enemyPrefab, BattleChar enemyInstance)
    {
        //Hago que el enemigo cree su matriz de debilidades (tanto el prefab como la instancia del combate)
        enemyPrefab.FillWeaknessArray();
        enemyInstance.FillWeaknessArray();

        //Referencio el holder de los iconos del enemigo
        GameObject weaknessHolder = enemyInstance.transform.GetChild(2).gameObject;

        //Var temporal para contar el nº total de debilidades para ir posicionandolas en orden:
        int weaknessCount = 0;

        //Recorro cada icono
        for (int k = 0; k < enemyInstance.enemyWeaknessesArray.Length; k++)
        {
            //Por cada debilidad true,
            if (enemyInstance.enemyWeaknessesArray[k])
            {
                //Activo el icono correspondiente en la pos k+1 (Porque el primero de la lista es el icono del escudo)
                weaknessHolder.transform.GetChild(k + 1).gameObject.SetActive(true);

                //Y posiciono el icono para que aparezcan en orden:
                weaknessHolder.transform.GetChild(k + 1).position = enemyInstance.weaknessesTransformArray[weaknessCount].position;

                //Sumo al contador de debilidades:
                weaknessCount++;
            }
        }
        //Y ahora le asigno el sprite:
        //Recorro la lista de habilidades según su posición:
        for(int i = 0; i < enemyInstance.weaknessIntList.Count; i++)
        {
            //Si esta posicion es conocida
            if (enemyInstance.weaknessUnknownIntList[i] == 1)
            {
                //Le asigno al icono correspondiente su sprite
                weaknessHolder.transform.GetChild(enemyInstance.weaknessIntList[i] + 1).GetComponent<SpriteRenderer>().sprite = weaknessSprites[enemyInstance.weaknessIntList[i]];
            }
            else
            {
                //Y si no es conocida le asigno el sprite unknown
                weaknessHolder.transform.GetChild(enemyInstance.weaknessIntList[i] + 1).GetComponent<SpriteRenderer>().sprite = unknownIcon;
            }
        }
    }

    public void CheckUnknownWeakness(string typeAttack, BattleChar target)
    {
        //Recorro la lista de strings de debilidades utiles del enemigo
        for (int i = 0; i < target.weaknessStringList.Count; i++)
        {
            //Si el tipo del ataque coincide con alguna debilidad
            if (target.weaknessStringList[i] == typeAttack)
            {
                //Chequeo si dicha habilidad está oculta y la desbloqueo, en su caso:
                if(target.weaknessUnknownIntList[i] == 0)
                {
                    target.weaknessUnknownIntList[i] = 1;

                    //Y actualizo el icono visualmente:
                    //Referencio el holder de los iconos del enemigo
                    GameObject weaknessHolder = target.transform.GetChild(2).gameObject;

                    //Activo el icono correspondiente y le asigno su sprite:
                    weaknessHolder.transform.GetChild(target.weaknessIntList[i] + 1).GetComponent<SpriteRenderer>().sprite = target.weaknessSpriteList[i];                }
            }
        }
    }


    public void CheckShieldBreak(string typeInput, BattleChar enemy)
    {

        shouldWeakenShield = false;

        //Compruebo si el ataque que le hago hace match con alguna de sus debilidades
        for (int i = 0; i < enemy.weaknessStringList.Count; i++)
        {
            if (typeInput == enemy.weaknessStringList[i])
            {
                shouldWeakenShield = true;
                Debug.Log("Debil a " + typeInput);
                break;//Salgo del bucle for.
            }
        }
        Debug.Log("No hay debilidad a " + typeInput);
    }

    public void OpenAttackTargetMenu()
    {
        targetMenu.SetActive(true);

        Targets = new List<int>();//Lista de (en este caso) enemigos en escena (vivos o muertos)
        List<int> tempTargets = new List<int>();//Lista que usaré para crear mi lista de Targets vivos

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (!activeBattlers[i].isPlayer)
            {
                tempTargets.Add(i);
                Debug.Log("Añado a " + activeBattlers[i] + " a tempTargets");
            }
        }

        //Referencio el BattleMove según mi arma equipada:
        activeMove = SetMoveFromWeapon();

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (tempTargets.Count > i && activeBattlers[tempTargets[i]].currentHP > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].theMove = activeMove;
                targetButtons[i].moveName = activeMove.moveName;
                targetButtons[i].activeBattlerTarget = tempTargets[i];
                targetButtons[i].targetName.text = activeBattlers[tempTargets[i]].charName;

                Targets.Add(tempTargets[i]);
                Debug.Log("Añado a " + activeBattlers[tempTargets[i]] + " a Targets");
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }

        currentSelectedTarget = Targets[0];

        //Selecciono al/los target/s:
        //SelectTarget(activeMove, currentSelectedTarget);
        SelectTarget(currentSelectedTarget);
    }



    public void OpenTargetMenu(BattleMove theMove)
    {
        //Almaceno el ataque elegido:
        activeMove = theMove;

        //Almaceno el coste magico que ya he descontado por si cancelo el ataque al final:
        spellCostDebt = activeMove.moveCost;

        //Indico si el ataque que voy a hacer es multiTarget
        currentMoveIsMultiTarget = activeMove.multiTarget;

        targetMenu.SetActive(true);

        Targets = new List<int>();//Targets es la lista de enemigos vivos o activos
        List<int> tempTargets = new List<int>();//tempTargets es una lista con todos los enemigos (vivos o muertos)
        //List<int> activeTargets = new List<int>();//Lista de targets VIVOS para seleccionar un objetivo por defecto

        //Diferencio si el movimiento se dirige a enemigos o a players:
        if (activeMove.affectsPlayers)//Si va dirigido a los players
        {
            for (int i = 0; i < activeBattlers.Count; i++)
            {
                if (activeBattlers[i].isPlayer)
                {
                    tempTargets.Add(i);
                }
            }
        }
        else//Si el ataque se dirige a enemigos
        {
            for (int i = 0; i < activeBattlers.Count; i++)
            {
                if (!activeBattlers[i].isPlayer)
                {
                    tempTargets.Add(i);
                }
            }
        }

        //Creo la lista de Targets a partir de los targets vivos
        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (tempTargets.Count > i && activeBattlers[tempTargets[i]].currentHP > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = activeMove.moveName;
                targetButtons[i].activeBattlerTarget = tempTargets[i];
                targetButtons[i].targetName.text = activeBattlers[tempTargets[i]].charName;

                Targets.Add(tempTargets[i]);
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }

        //La lista que me interesa es activeTargets porque es la que sólo cuenta a los targets que están vivos, así que actualizo Targets
        //Targets = new List<int>();
        //Targets = activeTargets;

        currentSelectedTarget = Targets[0];

        //Que por defecto se seleccione un objetivo
        //SelectTarget(moveName, Targets[activeTargets[0]]);
        SelectTarget(currentSelectedTarget);
    }


    public void OldOpenTargetMenu(string moveName, bool multiTarget, int spellCost, bool affectsPlayers)
    {
        //Almaceno el ataque elegido:
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == moveName)
            {
                activeMove = movesList[i];
            }
        }

        //Almaceno el coste magico que ya he descontado por si cancelo el ataque al final:
        spellCostDebt = spellCost;

        //Indico si el ataque que voy a hacer es multiTarget
        currentMoveIsMultiTarget = multiTarget;

        targetMenu.SetActive(true);

        Targets = new List<int>();//Targets es la lista de enemigos vivos o activos
        List<int> tempTargets = new List<int>();//tempTargets es una lista con todos los enemigos (vivos o muertos)
        //List<int> activeTargets = new List<int>();//Lista de targets VIVOS para seleccionar un objetivo por defecto

        //Diferencio si el movimiento se dirige a enemigos o a players:
        if (affectsPlayers)//Si va dirigido a los players
        {
            for (int i = 0; i < activeBattlers.Count; i++)
            {
                if (activeBattlers[i].isPlayer)
                {
                    tempTargets.Add(i);
                }
            }
        }
        else//Si el ataque se dirige a enemigos
        {
            for (int i = 0; i < activeBattlers.Count; i++)
            {
                if (!activeBattlers[i].isPlayer)
                {
                    tempTargets.Add(i);
                }
            }

            if (moveName == "Slash" || moveName == "Chop" || moveName == "Shoot" || moveName == "Hit" || moveName == "Punch")
            {
                moveName = SetAttackFromWeapon(moveName);
            }
        }

        //Creo la lista de Targets a partir de los targets vivos
        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (tempTargets.Count > i && activeBattlers[tempTargets[i]].currentHP > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = tempTargets[i];
                targetButtons[i].targetName.text = activeBattlers[tempTargets[i]].charName;

                Targets.Add(tempTargets[i]);
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }

        //La lista que me interesa es activeTargets porque es la que sólo cuenta a los targets que están vivos, así que actualizo Targets
        //Targets = new List<int>();
        //Targets = activeTargets;

        currentSelectedTarget = Targets[0];

        //Que por defecto se seleccione un objetivo
        //SelectTarget(moveName, Targets[activeTargets[0]]);
        SelectTarget(currentSelectedTarget);
    }



    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (activeBattlers[currentTurn].movesLearnt.Count > i)
            {
                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = activeBattlers[currentTurn].movesLearnt[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                for (int j = 0; j < movesList.Length; j++)
                {
                    if (movesList[j].moveName == magicButtons[i].spellName)
                    {
                        magicButtons[i].theMove = movesList[j];
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                        magicButtons[i].multiTarget = movesList[j].multiTarget;
                        magicButtons[i].affectsPlayers = movesList[j].affectsPlayers;
                    }
                }

            }
            else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }


    public void Flee()
    {
        if (cannotFlee)
        {
            battleNotice.theText.text = "¡No puedes huir de este combate!";
            battleNotice.ActivateTemp();
        }
        else
        {
            int fleeSuccess = Random.Range(0, 100);
            if (fleeSuccess < chanceToFlee)
            {
                //end the battle
                fleeing = true;
                StartCoroutine(EndBattleCo());
            }
            else
            {
                //La penalización por intentar huir es que todo el equipo pierde su turno:
                //for (int i = 0; i < activeBattlers.Count; i++)
                //{
                //    if (activeBattlers[i].isPlayer)
                //    {
                //        activeBattlers[i].turnDone = true;
                //    }
                //}
                fleeFailed = true;
                battleNotice.theText.text = "¡No has podido escapar!";
                battleNotice.ActivateTemp();
                NextTurn();
            }
        }
    }


    public IEnumerator EndBattleCo()
    {
        ActivateMenuButtons();
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        //Vacío las listas de la ronda actual y siguiente:
        currentRoundList.Clear();
        nextRoundList.Clear();


        yield return new WaitForSeconds(1.3f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        //Actualizo datos de enemigos (debilidades ocultas):
        UpdateEnemyBattleData();

        //Actualizo stats de los players
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                        GameManager.instance.playerStats[j].lastWeaponUsed = activeBattlers[i].lastWeaponUsed;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;

        if (fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
            UIFade.instance.FadeFromBlack();
            //Paro la música actual:
            AudioManager.instance.StopBGM(AudioManager.instance.GetBgmPlaying());

            //Resumo la música pausada al inicio del combate:
            AudioManager.instance.bgm[pausedMusic].UnPause();
        }
        else
        {
            //Paro la música actual:
            AudioManager.instance.StopBGM(AudioManager.instance.GetBgmPlaying());
            AudioManager.instance.PlayBGM(5);
            BattleReward.instance.OpenRewardScreen(rewardEXP, rewardGold, rewardItems, isEventBattle);
        }
    }

    //Traslado la información relevante (debilidades descubiertas, estado de visto/tamed del enemigo instanciado al prefab)
    public void UpdateEnemyBattleData()
    {
        //Recorro listado de battlers
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            //Si es un enemigo
            if (!activeBattlers[i].isPlayer)
            {
                //busco su prefab correspondiente:
                for(int j = 0; j < enemyPrefabs.Length; j++)
                {
                    //Si sus nombres coinciden:
                    if (activeBattlers[i].charName == enemyPrefabs[j].charName)
                    {
                        //Actualizo su estado de sighted/tamed:
                        enemyPrefabs[j].sighted = activeBattlers[i].sighted;
                        enemyPrefabs[j].tamed = activeBattlers[i].tamed;

                        //Actualizo sus habilidades ocultas:
                        enemyPrefabs[j].weaknessUnknownIntList = activeBattlers[i].weaknessUnknownIntList;
                    }
                }
            }
        }
    }

    public IEnumerator GameOverCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        //Vacío las listas de la ronda actual y siguiente:
        currentRoundList.Clear();
        nextRoundList.Clear();

        yield return new WaitForSeconds(1.5f);

        UIFade.instance.FadeToBlackTime(2);

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            Destroy(activeBattlers[i].gameObject);
        }

        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;

        GameManager.instance.battleActive = false;
        fleeing = false;
        SceneManager.LoadScene(gameOverScene);
    }


    //Terminar una batalla sin ser GameOver (por exigencias del guion):
    public IEnumerator EndLostBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        //Vacío las listas de la ronda actual y siguiente:
        currentRoundList.Clear();
        nextRoundList.Clear();


        //yield return new WaitForSeconds(.5f);

        //UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        UIFade.instance.FadeToBlackTime(2);

        yield return new WaitForSeconds(1.5f);

        //Actualizo info de enemigos
        UpdateEnemyBattleData();

        //Actualizo info de players
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                        GameManager.instance.playerStats[j].lastWeaponUsed = activeBattlers[i].lastWeaponUsed;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;

        GameManager.instance.battleActive = false;
        fleeing = false;
        UIFade.instance.FadeFromBlack();

        //Paro la música actual:
        AudioManager.instance.StopBGM(AudioManager.instance.GetBgmPlaying());
        //Resumo la música pausada al inicio del combate:
        AudioManager.instance.bgm[pausedMusic].UnPause();
    }


    //testing items menu:
    public void ShowItemsMenu()
    {
        itemsMenu.SetActive(true);

        if (battleActiveItem == null)
        {
            useItemButton.transform.localScale = new Vector3(0, 0, 0);
        }

        GameManager.instance.SortItems();

        for (int i = 0; i < battleItemButtons.Length; i++)
        {
            battleItemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeldBattle[i] != "")
            {
                battleItemButtons[i].buttonImage.gameObject.SetActive(true);
                battleItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldBattle[i]).itemSprite;
                battleItemButtons[i].amountText.text = GameManager.instance.numberOfItemsBattle[i].ToString();
            }
            else
            {
                battleItemButtons[i].buttonImage.gameObject.SetActive(false);
                battleItemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectItem(Item newItem)
    {
        battleActiveItem = newItem;

        useItemButton.transform.localScale = new Vector3(1, 1, 1);
        useButtonText.text = "Usar";

        itemName.text = battleActiveItem.itemName;
        itemDescription.text = battleActiveItem.description;
    }

    public void BackItemsMenu()
    {

        if (itemBattleChoiceMenu.activeInHierarchy)
        {
            itemBattleChoiceMenu.SetActive(false);
        }
        else if (itemsMenu.activeInHierarchy)
        {
            itemsMenu.SetActive(false);
        }
    }

    public void OpenItemBattleChoice()
    {
        itemBattleChoiceMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++)
        {
            if(i < GameManager.instance.activePlayerStats.Count)
            {
                itemCharChoiceNames[i].text = GameManager.instance.activePlayerStats[i].charName;
                itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.activePlayerStats[i].gameObject.activeInHierarchy);
            }
            else
            {
                itemCharChoiceNames[i].text = "";
            }
        }
    }

    public void CloseItemBattleChoice()
    {
        deselectItem();

        itemBattleChoiceMenu.SetActive(false);
    }

    public void UseItem(int selectChar)
    {
        //Animación de usar item:
        activeBattlers[currentTurn].anim.SetTrigger("useItem");

        battleActiveItem.Use(selectChar);
        CloseItemBattleChoice();
        BackItemsMenu();
        UpdateUIStats();
        //Instancio algún vfx de uso de item:
        Instantiate(highlightAttackerEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        //Actualizo la UI de los turnos de ronda:
        UpdateRoundOrderUI();
        NextTurn();
    }

    public void deselectItem()
    {
        battleActiveItem = null;
        itemName.text = "Selecciona un objeto";
        itemDescription.text = "";

        useItemButton.transform.localScale = new Vector3(0, 0, 0);
    }

    public void SelectTarget(int selectedTarget)
    {
        //targetAttackButton.transform.GetChild(1).GetComponent<Text>().text = activeMove.moveName;
        targetAttackButton.transform.GetChild(1).GetComponent<Text>().text = "Atacar";

        activeBattlers[currentSelectedTarget].transform.GetChild(0).gameObject.SetActive(false);
        currentSelectedTarget = selectedTarget;

        //Multitarget
        if (activeMove.multiTarget)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                activeBattlers[Targets[i]].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            activeBattlers[selectedTarget].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void AttackTargetMenu()
    {
        spellCostDebt = 0;
        PlayerAttack(currentSelectedTarget);
        //PlayerAttack(activeMove, currentSelectedTarget);
    }

    public void BackTargetMenu()
    {
        //Recupero el coste de MP que había descontado al seleccionar la magia:
        activeBattlers[currentTurn].currentMP += spellCostDebt;
        spellCostDebt = 0;

        targetAttackButton.transform.GetChild(1).GetComponent<Text>().text = "";

        //Elimino los iconos de selección de Targets
        if (currentMoveIsMultiTarget)
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                activeBattlers[Targets[i]].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            activeBattlers[currentSelectedTarget].transform.GetChild(0).gameObject.SetActive(false);
        }

        targetMenu.SetActive(false);
        currentMoveName = "";
        currentMoveIsMultiTarget = false;//Por si acaso
    }

    public void BackMagicMenu()
    {
        magicMenu.SetActive(false);
    }

    public void DeactivateMenuButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            buttons[i].transform.GetChild(0).GetComponent<Image>().color = buttonsColorFaded;
        }
    }

    public void ActivateMenuButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            buttons[i].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;
        }
    }


    //Cambiar paleta de colores de UI:
    public void SetCustomUI(int i)
    {
        foreach(Image img in lightButtonsUI)
        {
            img.color = GameMenu.instance.UIColorLightBtns[i];
        }
        foreach (Image img in darkButtonsUI)
        {
            img.color = GameMenu.instance.UIColorDarkBtns[i];
        }
        foreach (Image img in lightBgUI)
        {
            img.color = GameMenu.instance.UIColorLightBg[i];
        }
        foreach (Image img in darkBgUI)
        {
            img.color = GameMenu.instance.UIColorLightBg[i];
        }
        Debug.Log("BattleManager UI changed to palette " + i);
    }

    public void PlayAcceptSFX()
    {
        AudioManager.instance.PlaySFX(44);
    }
    public void PlayDeclineSFX()
    {
        AudioManager.instance.PlaySFX(45);
    }
    public void PlaySelectSFX()
    {
        AudioManager.instance.PlaySFX(46);
    }
    public void PlayErrorSFX()
    {
        AudioManager.instance.PlaySFX(47);
    }
    public void PlayOpenMenuSFX()
    {
        AudioManager.instance.PlaySFX(48);
    }

    //Para cambiar momentaneamente la tonalidad del sprite de algun battleChar:
    public void HighlightChar(BattleChar battler, Color newColor)
    {
        SpriteRenderer sprite = battler.GetComponent<SpriteRenderer>();

        sprite.color = newColor;
    }


    public int GetPlayers()
    {
        int playersCount = 0;

        for(int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer)
            {
                playersCount++;
            }
        }
        return playersCount;
    }

    public int GetPlayersAlive()
    {
        int playersCount = 0;

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0)
            {
                playersCount++;
            }
        }
        return playersCount;
    }


    //Función que determina el color del shader emission según tipo y autor del ataque:
    public Color SelectEmissionColor(BattleMove move, int i)
    {
        //Si el ataque envenena:
        if (move.poison)
        {
            return emissionColorPoison;
        }

        switch (i)
        {
            case 0://El autor es un player char
                return emissionColorDefaultPlayers;
            case 1://El autor es un enemigo
                return emissionColorDefaultEnemy;
            default:
                return emissionColorDefaultPlayers;
        }
    }




    //A partir de aquí pongo las descripciones de batallas especiales (special event battles) del modo historia
    public IEnumerator Tutorial1Co()
    {
        //Pongo la vel de Daria bien abajo para que no actue antes que el P1:
        for(int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].charName == "Daria")
            {
                activeBattlers[i].speed = 1;
            }
        }

        while (battleMessage.gameObject.activeInHierarchy)
        {
            yield return new WaitForEndOfFrame();
        }

        //Básicamente muestro un mensaje en pantalla y resalto el comando a realizar:
        //Chequeo si la ronda actual triggerea tutorial
        for (int i = 0; i < eventTriggerRounds.Length; i++)
        {
            //Si coincide tanto la round:
            if (currentRound == eventTriggerRounds[i])
            {
                Debug.Log("Ronda " + eventTriggerRounds[i]);
                //...y el personaje actual que requiere las condiciones del tuto:
                if (activeBattlers[currentTurn].charName == eventCharTriggerRounds[i])
                {
                    Debug.Log("tutoRonda para " + eventCharTriggerRounds[i]);
                    //Muestro mensaje (fijo) de tutorial
                    //Antes de cambiar todo mensaje de batalla tengo que comprobar que no haya ningún otro activo:
                    while (battleMessage.gameObject.activeInHierarchy)
                    {
                        Debug.Log("Message already displayed");
                        yield return new WaitForEndOfFrame();
                    }

                    battleMessage.theText.text = eventRoundMessages[i];
                    battleMessage.ActivateFix();

                    yield return new WaitForEndOfFrame();

                    //Y espero a que el player lea el mensaje:
                    while (battleMessage.gameObject.activeInHierarchy)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    //Resalto el botón que hay que apretar
                    for (int j = 0; j < buttons.Length; j++)
                    {
                        if (eventObjectNames[i] == buttons[j].gameObject.name)
                        {
                            buttons[j].interactable = true;
                            buttons[j].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;
                        }
                        else
                        {
                            buttons[j].interactable = false;
                            buttons[j].transform.GetChild(0).GetComponent<Image>().color = buttonsColorFaded;
                        }
                    }
                }
                //Para que se active el boton attack despues de cambiar de arma:
                if (currentRound == 2 && activeBattlers[currentTurn].isPlayer)
                {
                    while (usingWeapon != "Bow")
                    {
                        Debug.Log("Waiting for bow");
                        yield return new WaitForEndOfFrame();
                    }

                    buttons[1].interactable = true;
                    buttons[1].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;

                    buttons[0].interactable = false;
                    buttons[0].transform.GetChild(0).GetComponent<Image>().color = buttonsColorFaded;
                    yield return null;
                }
                //Para que se activen los botones tras cargar focus:
                else if (currentRound == 4 && activeBattlers[currentTurn].isPlayer)
                {
                    while (activeBattlers[currentTurn].focusCharged == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    buttons[0].interactable = true;
                    buttons[0].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;

                    buttons[1].interactable = true;
                    buttons[1].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;

                    buttons[2].interactable = true;
                    buttons[2].transform.GetChild(0).GetComponent<Image>().color = buttonsColor;
                }
                else if (currentRound > 4 && activeBattlers[currentTurn].isPlayer)
                {
                    ActivateMenuButtons();
                }
            }
        }

        if (!activeBattlers[currentTurn].isPlayer)
        {
            Debug.Log("Old Tree should attack");
            //StartCoroutine(EnemyMoveCo());
            NextTurn();
        }
        yield return null;
    }

    public IEnumerator TerribleFate1Co()
    {
        //Chequeo si la ronda actual triggerea evento
        for (int i = 0; i < eventTriggerRounds.Length; i++)
        {
            //Si coincide tanto la round:
            if (currentRound == eventTriggerRounds[i])
            {
                Debug.Log("TerribleFate1Co evento de ronda " + currentRound);
                //...y el personaje actual de la ronda:
                if (activeBattlers[currentTurn].charName == eventCharTriggerRounds[i])
                {
                    //Muestro mensaje fijo (también podría ser mensaje temporal)
                    battleMessage.theText.text = eventRoundMessages[i];
                    battleMessage.ActivateFix();
                }
                yield return new WaitForEndOfFrame();

                //Y espero a que el player lea el mensaje:
                while (battleMessage.gameObject.activeInHierarchy)
                {
                    Debug.Log("BattleMessage is active...");
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        //Chequeo si se cumple alguna condicion por HP de alguien:
        for(int i = 0; i < eventHpConditions.Length; i++)
        {
            //Compruebo si el battler del turno actual coincide con el char de la condicion:
            if (activeBattlers[currentTurn].charName == eventCharHpConditions[i])
            {
                //Ahora compruebo si el hp del char en cuestión está por debajo del umbral:
                if (activeBattlers[currentTurn].currentHP <= eventHpConditions[i])
                {
                    Debug.Log("hp condition 1");

                    while (battleMessage.gameObject.activeInHierarchy)
                    {
                        Debug.Log("BattleMessage is active (hp condition)...");
                        yield return new WaitForEndOfFrame();
                    }

                    //En caso afirmativo, muestro el mensaje pertinente de la condición (y lo que sea):
                    //Muestro mensaje fijo (también podría ser mensaje temporal)
                    battleMessage.theText.text = eventHpMessages[i];
                    battleMessage.ActivateFix();

                    yield return new WaitForEndOfFrame();

                    //Y espero a que el player lea el mensaje:
                    while (battleMessage.gameObject.activeInHierarchy)
                    {
                        Debug.Log("Esperando que se elimine la lectura del mensaje");
                        yield return new WaitForEndOfFrame();
                    }
                    yield return new WaitForEndOfFrame();

                    //Si tengo que aplicar alguna accion más allá del mensaje, es aquí:
                    //Dispongo el ataque final de Drac y el fin del combate:
                    activeBattlers[currentTurn].nextMove = eventObjectNames[i];
                    //Le doy prioridad al siguiente ataque y hago que sea fijo en lugar de temp:
                    battleMessage.priorityMessage = true;
                    //Elimino stun del char (enemigo) para que haga el ataque a continuación siempre:
                    activeBattlers[currentTurn].stunCount = 0;
                    Debug.Log("nextMove now!");
                }
            }
        }

        if (!activeBattlers[currentTurn].isPlayer)
        {
            Debug.Log("Drac should attack");
            StartCoroutine(EnemyMoveCo());
        }
        yield return null;
    }

    public IEnumerator MoveToActivePositionCo(BattleChar charMoving)
    {
        playerMoving = true;

        while (activePosPlayer != null)
        {
            Debug.Log("Esperando a que el activePlayer (" + activePosPlayer.charName + ") vuelva a su posición neutral...");
            yield return null;
        }
        //Una vez no hay nadie moviendose, asigno el nuevo activePosPlayer
        activePosPlayer = charMoving;

        yield return new WaitForSecondsRealtime(0.7f);

        yield return new WaitForEndOfFrame();

        //Activo la anim del player de andar:
        charMoving.anim.SetBool("moving", true);

        yield return new WaitForEndOfFrame();

        //Muevo el player hasta el ActivePos:
        while (Vector2.Distance(charMoving.transform.position, activePos.position) > 0.001f)
        {
            charMoving.transform.position = Vector2.MoveTowards(charMoving.transform.position, activePos.position, 10 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(charMoving.charName + " ha llegado a ActivePos");
        //Una vez llega a la posición, detengo su animación:
        charMoving.anim.SetBool("moving", false);
        playerMoving = false;

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator ReturnFromActivePositionCo(BattleChar charMoving)
    {
        yield return new WaitForSecondsRealtime(0.7f);

        playerMoving = true;

        yield return new WaitForEndOfFrame();
        Debug.Log(charMoving.charName + " vuelve a su posicion");

        //Activo la anim del player de andar:
        charMoving.anim.SetBool("moving", true);

        yield return new WaitForEndOfFrame();

        //Muevo el player hasta el ActivePos:
        while (Vector2.Distance(charMoving.transform.position, charMoving.battlePos) > 0.001f)
        {
            charMoving.transform.position = Vector2.MoveTowards(charMoving.transform.position, charMoving.battlePos, 10 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(charMoving.charName + " ha llegado a su unactivePos");
        //Una vez llega a la posición, detengo su animación:
        charMoving.anim.SetBool("moving", false);
        playerMoving = false;
        activePosPlayer = null;
        Debug.Log("activePosPlayer debería ser null: " + activePosPlayer);

        yield return new WaitForEndOfFrame();
    }


    //Función que siempre devuelve true, es para comprobar que este script ya ha cargado y está funcional desde el resto de scripts
    public bool CheckIfLoaded()
    {
        return true;
    }



    //Camera shake
    public IEnumerator CameraShake(float time, float magnitude)
    {
        //Referencio el target original (el player)
        Transform originalTarget = PlayerController.instance.cam.target;
        Transform tempTarget = PlayerController.instance.cam.transform.GetChild(3);
        tempTarget.position = originalTarget.position;

        //Vector3 originalPos = transform.position;
        float elapsed = 0f;

        PlayerController.instance.cam.target = tempTarget;

        while (elapsed < magnitude)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            tempTarget.position += new Vector3((float)x, (float)y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        PlayerController.instance.cam.target = originalTarget;
    }

}
