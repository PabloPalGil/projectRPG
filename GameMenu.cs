using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class GameMenu : MonoBehaviour {

    public GameObject theMenu;
    public GameObject[] windows;

    private CharStats[] playerStats;
    public List<CharStats> activePlayersList;

    public Text[] nameText, hpText, mpText, lvlText;
    public Slider[] hpSlider, mpSlider;

    public Image[] charImage;
    public GameObject[] charStatHolder;

    //Esta var me indica cuál de los 4 chars tengo actualmente abierto en Status, Heal o Equip window
    public int currentCharSelectedInMenu;

    //Esta var me dirá el target sobre el que quiero aplicar lo que sea que tenga seleccionado:
    public int currentTargetCharSelectedInMenu;//va de 0 a 3 y el 4 significa a todos!

    public GameObject[] statusButtons;

    //Status window
    [Header("Stats Window")]
    public Slider statusExpSlider;
    public Text statusName, statusHP, statusMP, statusStr, statusDef, statusMagicStr, statusMagicDef, statusSpd, statusEvasion, statusExp;
    public Text statusSwordEqpd, statusAxeEqpd, statusBowEqpd, statusStaffEqpd, statusHeadArmorEqpd, statusBodyArmorEqpd, statusShieldEqpd;
    public Text statusSwordPwr, statusAxePwr, statusBowPwr, statusStaffPwr, statusHeadArmorPwr, statusBodyArmorPwr, statusShieldPwr;
    public Image statusImage;
    public Button statsNextCharBtn;
    public Button statsPrevCharBtn;
    public Animator animStatusChar;


    //Equip window
    [Header("Equip Window")]
    public Button equipNextCharBtn;
    public Button equipPrevCharBtn;
    public Text equipName;
    public Text equipSwordEqpd, equipAxeEqpd, equipBowEqpd, equipStaffEqpd, equipHeadArmorEqpd, equipBodyArmorEqpd, equipShieldEqpd;
    public Text equipSwordPwr, equipAxePwr, equipBowPwr, equipStaffPwr, equipHeadArmorPwr, equipBodyArmorPwr, equipShieldPwr;
    public Image equipImage;
    public Animator animEquipChar;


    //Items window
    [Header("Items Window")]
    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public GameObject useItemButton;
    public GameObject discardItemButton;
    public Text itemName, itemDescription, useButtonText;
    public GameObject feedbackPanel;
    public Text feedbackMessage;
    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;
    public GameObject itemTargetCharsPanel;
    public GameObject[] itemCharStats;
    public GameObject[] itemTypeButtons;
    public bool itemsBattle, itemsEquip, itemsSpecial;



    //Heal window
    [Header("Heal Window")]
    public MenuMagicSelect[] magicButtons;
    public Text healMenuText;
    public Image healButtonsMask;
    public GameObject healCancelButton;
    public GameObject healUseButton;
    public Image healCharImage;
    public Text healCharName;
    public Button healNextChar;
    public Button healPrevChar;
    public BattleMove healMenuMove;
    public GameObject healWindowButton;



    //Specific Equip Items Window
    [Header("Specific Equip Items Window")]
    public bool itemsToEquipWindow;
    public string currentEquipItem;
    public GameObject cancelEquipSpecificItemButton;
    public GameObject equipCharStatsPanel;
    public Text charToEquip, charToEquipStr, charToEquipDef, charToEquipMagicStr, charToEquipMagicDef, charToEquipSpd, charToEquipEva;
    public Text newCharToEquipStr, newCharToEquipDef, newCharToEquipMagicStr, newCharToEquipMagicDef, newCharToEquipSpd, newCharToEquipEvasion;
    public List<Item> filteredItems;//Esta es la lista temporal de items a equipar


    //Bestiario Window
    [Header("Bestiario Window")]
    public GameObject[] enemyEntries;
    public Image enemyImage;
    public TextMeshProUGUI enemyDataText1;
    public TextMeshProUGUI enemyDataText2;
    public GameObject[] enemyWeaknessIcons;
    public Image enemyTypeImage;
    public Sprite unknownIconSprite;
    public Sprite[] typeIconSprites;
    public GameObject bestiaryWindowButton;

    public static GameMenu instance;
    public Text goldText;

    public string mainMenuName;

    public GameObject touchUIButtonsHolder;

    public Color selectedColor, deselectedColor;


    //CustomUI:
    //Guardar varios tonos para toda la UI (GameMenu y BattleMenu basicamente) y que el player pueda elegir el que mas le guste
    public Color[] UIColorLightBtns;//Botones claritos
    public Color[] UIColorDarkBtns;//Botones oscuros
    public Color[] UIColorLightBg;//Fondos paneles claros
    public Color[] UIColorDarkBg;//Fondos paneles oscuros
    public int UIpaletteID;
    //opciones de colores a elegir
    public Image[] lightButtonsUI;
    public Image[] darkButtonsUI;
    public Image[] lightBgUI;
    public Image[] darkBgUI;


    //Joystick
    public GameObject touchJoystick;
    public GameObject touchDPad;

    //Var que determina la velocidad de blinkeo de HighlightBlinkingCo
    public float highlightSpeed = 1f;

    //Var que indica que ventana está actualmente activa:
    private int currentActiveWindow;


    //Skills menu
    public GameObject skillsMenu;
    public bool choosingSkill;


    void Start () {
        instance = this;
	}
	

    //Función que siempre devuelve true, es para comprobar que este script ya ha cargado y está funcional desde el resto de scripts
    public bool CheckIfLoaded()
    {
        return true;
    }

    public void ToogleMainMenu()
    {
        itemsToEquipWindow = false;//Var que indica que debo cargar la ventana Items en versión SpecificItemsEquip
        GameManager.instance.GetPlayersActive();//Actualizo la lista activePlayerStats que me coinciden en posicion con los chars del menu.

        if (theMenu.activeInHierarchy)
        {
            //theMenu.SetActive(false);
            //GameManager.instance.gameMenuOpen = false;

            CloseMenu();
        }
        else
        {
            theMenu.SetActive(true);
            //Valores por defecto
            itemsBattle = true;
            currentCharSelectedInMenu = 0;
            currentTargetCharSelectedInMenu = 0;

            //Debo desactivar el botón Bestiario (si aun no lo he conseguido)?
            if(QuestManager.instance.CheckIfComplete("h_Leax Gift"))
            {
                bestiaryWindowButton.SetActive(true);
            }
            else
            {
                bestiaryWindowButton.SetActive(false);
            }

            PlayOpenMenuSFX();

            UpdateMainStats();
            GameManager.instance.gameMenuOpen = true;
        }

        
    }

    public void UpdateMainStats()
    {
        playerStats = GameManager.instance.playerStats;
        
        //Lista con los players activos / en la party (charStats del GameManager)
        activePlayersList = new List<CharStats> ();

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatHolder[i].SetActive(true);

                activePlayersList.Add(playerStats[i]);

                nameText[i].text = playerStats[i].charName;
                hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                lvlText[i].text = "Lvl: " + playerStats[i].playerLevel;
                hpSlider[i].maxValue = playerStats[i].maxHP;
                hpSlider[i].value = playerStats[i].currentHP;
                mpSlider[i].maxValue = playerStats[i].maxMP;
                mpSlider[i].value = playerStats[i].currentMP;
                charImage[i].sprite = playerStats[i].charImage;
            }
            else
            {
                charStatHolder[i].SetActive(false);
            }
        }

        goldText.text = $"{GameManager.instance.currentGold} g";
    }

    public void ToggleWindow(int windowNumber)
    {
        UpdateMainStats();
        UnhighlightCharStatsButtons();

        //Diferencio si salgo de la ventana de itemSpecificItem:
        if (itemsToEquipWindow)
        {
            itemsToEquipWindow = false;
            DeselectItem();
            //Activo y desactivo elementos de la UI para abrir la ventana Items:
            discardItemButton.SetActive(true);
            cancelEquipSpecificItemButton.SetActive(false);
            equipCharStatsPanel.SetActive(false);
            itemTypeButtons[0].gameObject.transform.localScale = new Vector3(1, 1, 1);
            itemTypeButtons[1].gameObject.transform.localScale = new Vector3(1, 1, 1);
            itemTypeButtons[2].gameObject.transform.localScale = new Vector3(1, 1, 1);
            windows[0].SetActive(false);
            windows[1].SetActive(true);
            //Actualizo los valores de equipo:
            OpenEquip();
        }
        else
        {
            itemsToEquipWindow = false;

            for (int i = 0; i < windows.Length; i++)
            {
                if (i == windowNumber)
                {
                    windows[i].SetActive(!windows[i].activeInHierarchy);
                    if (windows[i].name == "Items Window")
                    {
                        if (!itemsBattle && !itemsEquip && !itemsSpecial)
                        {
                            itemsBattle = true;
                            itemTypeButtons[0].GetComponent<Image>().color = selectedColor;
                            itemTypeButtons[1].GetComponent<Image>().color = deselectedColor;
                            itemTypeButtons[2].GetComponent<Image>().color = deselectedColor;
                        }
                        DeselectItem();
                        //Activo y desactivo elementos de la UI para abrir la ventana Items:
                        discardItemButton.SetActive(true);
                        cancelEquipSpecificItemButton.SetActive(false);
                        equipCharStatsPanel.SetActive(false);
                        itemTypeButtons[0].gameObject.transform.localScale = new Vector3(1, 1, 1);
                        itemTypeButtons[1].gameObject.transform.localScale = new Vector3(1, 1, 1);
                        itemTypeButtons[2].gameObject.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (windows[i].name == "Bestiary Window")//Si pulso la ventana bestiario
                    {
                        if (windows[i].activeInHierarchy)//Y la estoy abriendo (no cerrando)
                        {
                            OpenBestiaryWindow();
                        }
                    }
                    else if (windows[i].name == "Heal Menu")
                    {
                        if (windows[i].activeInHierarchy)//Si voy a abrir la ventana de sanar:
                        {
                            OpenHealWindow();
                        }
                    }
                }
                else
                {
                    UnhighlightCharStatsButtons();
                    windows[i].SetActive(false);
                }
            }
        }

        itemCharChoiceMenu.SetActive(false);
    }

    public void ToggleItemsWindow(string itemType)
    {
        UpdateMainStats();

        if(itemType == "Battle")
        {
            itemsBattle = true;
            itemsEquip = false;
            itemsSpecial = false;

            itemTypeButtons[0].GetComponent<Image>().color = selectedColor;
            itemTypeButtons[1].GetComponent<Image>().color = deselectedColor;
            itemTypeButtons[2].GetComponent<Image>().color = deselectedColor;

            ShowItems();
        }
        else if(itemType == "Equip")
        {
            itemsBattle = false;
            itemsEquip = true;
            itemsSpecial = false;

            itemTypeButtons[0].GetComponent<Image>().color = deselectedColor;
            itemTypeButtons[1].GetComponent<Image>().color = selectedColor;
            itemTypeButtons[2].GetComponent<Image>().color = deselectedColor;

            ShowItems();
        }
        else if(itemType == "Special")
        {
            itemsBattle = false;
            itemsEquip = false;
            itemsSpecial = true;

            itemTypeButtons[0].GetComponent<Image>().color = deselectedColor;
            itemTypeButtons[1].GetComponent<Image>().color = deselectedColor;
            itemTypeButtons[2].GetComponent<Image>().color = selectedColor;

            ShowItems();
        }

        itemCharChoiceMenu.SetActive(false);
    }

    //Elegir equipItem menu desde Equipar en EquipWindow (filtrando por tipo de equipo):
    public void ShowSpecificEquipItems(string typeEquipItem)
    {
        UpdateMainStats();

        //Almaceno el tipo de equipItem para recargar la pagina al equipar un item:
        currentEquipItem = typeEquipItem;

        //Esta bool indica que abro la ventana items en modo equipar a un pj en concreto
        itemsToEquipWindow = true;

        //Activo la ventana itemsToEquip
        windows[0].SetActive(true);
        
        //Activo y desactivo elementos de la UI para adaptar la ventana Items a EquipSpecificItems:
        discardItemButton.SetActive(false);
        cancelEquipSpecificItemButton.SetActive(true);
        equipCharStatsPanel.SetActive(true);
        itemTypeButtons[0].gameObject.transform.localScale = new Vector3(0, 0, 0);
        itemTypeButtons[1].gameObject.transform.localScale = new Vector3(0, 0, 0);
        itemTypeButtons[2].gameObject.transform.localScale = new Vector3(0, 0, 0);
        //Actualizo los stats del char actuales y según el arma seleccionada (a equipar):
        UpdateEquipCharStats();

        //Deselecciono cualquier item
        DeselectItem();

        //Desactivo el panel de elección de pj a equipar un item
        itemCharChoiceMenu.SetActive(false);

        //Filtro sólo items de tipo equipo:
        itemsBattle = false;
        itemsEquip = true;
        itemsSpecial = false;

        //Ordeno los items:
        GameManager.instance.SortItems();

        itemsEquip = false;

        DeselectItem();

        //Default values color:
        charToEquipStr.color = new Color(1, 1, 1);
        charToEquipMagicStr.color = new Color(1, 1, 1);
        charToEquipDef.color = new Color(1, 1, 1);
        charToEquipMagicDef.color = new Color(1, 1, 1);
        charToEquipSpd.color = new Color(1, 1, 1);
        charToEquipEva.color = new Color(1, 1, 1);

        //Creo una lista para almacenar la lista de items filtrados:
        filteredItems = new List<Item>();

        //Filtro por el tipo de equipItem buscado:
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeldEquip[i] != "")
            {
                //Referencio el item encontrado:
                Item thisItem = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldEquip[i]);

                //Ahora filtro según el item buscado:
                if (typeEquipItem == "isSword")
                {
                    if (GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldEquip[i]).isSword)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isAxe")
                {
                    //Filtro los items espada:
                    if (thisItem.isAxe)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isBow")
                {
                    //Filtro los items espada:
                    if (thisItem.isBow)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isStaff")
                {
                    //Filtro los items espada:
                    if (thisItem.isStaff)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isHeadArmor")
                {
                    //Filtro los items espada:
                    if (thisItem.isHeadArmor)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isBodyArmor")
                {
                    //Filtro los items espada:
                    if (thisItem.isBodyArmor)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
                else if (typeEquipItem == "isShield")
                {
                    //Filtro los items espada:
                    if (thisItem.isShield)
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(true);
                        itemButtons[i].buttonImage.sprite = thisItem.itemSprite;
                        itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                        filteredItems.Add(thisItem);
                    }
                    else
                    {
                        itemButtons[i].buttonImage.gameObject.SetActive(false);
                        itemButtons[i].amountText.text = "";
                    }
                }
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }

        List<int> filteredItemsPos = new List<int>();
        //Finalmente ordeno los items filtrados para que no queden huecos:
        //Primero creo una lista de ints y pongo las posiciones de los items filtrados
        for(int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i].buttonImage.gameObject.activeInHierarchy)
            {
                filteredItemsPos.Add(i);
            }
        }
        //Y ahora pongo, en orden, en los botonesItems la información de los items filtrados (cuya posicion me la dice la lista de antes)
        for(int j = 0; j < filteredItemsPos.Count; j++)
        {
            itemButtons[j].buttonImage.gameObject.SetActive(true);
            itemButtons[j].buttonImage.sprite = itemButtons[filteredItemsPos[j]].buttonImage.sprite;
            itemButtons[j].amountText.text = itemButtons[filteredItemsPos[j]].amountText.text;
            //Y borro los botones items originales para no duplicarlos (solo si su posicion original no era la posicion ordenada)
            if(filteredItemsPos[j] != j)
            {
                itemButtons[filteredItemsPos[j]].buttonImage.gameObject.SetActive(false);
                itemButtons[filteredItemsPos[j]].amountText.text = "";
            }
        }

        //Ahora tengo los items ordenados visualmente (propiedades image y text de los buttonItem,
        //pero la información que me da cada itemButton al pulsarlo viene por su funcion Press, y ahí usaré mi lista filteredList
    }

    //Actualiza los stats de la pantalla de equipar Item específico a Char
    public void UpdateEquipCharStats()
    {
        //Referencio el player para no hacer tantas llamadas al GameManager:
        CharStats myChar = GameManager.instance.activePlayerStats[currentCharSelectedInMenu];

        //Creo unas vars para almacenar los stats base + lo que aportan los equipos:
        int charTotalStr;
        int charTotalMagicStr;
        
        if(currentEquipItem == "isSword")
        {
            charTotalStr = myChar.strength + myChar.swordPwr;
            charTotalMagicStr = myChar.magicStr + myChar.swordMgcPwr;
            charToEquipStr.text = charTotalStr.ToString();
            charToEquipMagicStr.text = charTotalMagicStr.ToString();
        }
        else if(currentEquipItem == "isAxe")
        {
            charTotalStr = myChar.strength + myChar.axePwr;
            charTotalMagicStr = myChar.magicStr + myChar.axeMgcPwr;
            charToEquipStr.text = charTotalStr.ToString();
            charToEquipMagicStr.text = charTotalMagicStr.ToString();
        }
        else if (currentEquipItem == "isBow")
        {
            charTotalStr = myChar.strength + myChar.bowPwr;
            charTotalMagicStr = myChar.magicStr + myChar.bowMgcPwr;
            charToEquipStr.text = charTotalStr.ToString();
            charToEquipMagicStr.text = charTotalMagicStr.ToString();
        }
        else if (currentEquipItem == "isStaff")
        {
            charTotalStr = myChar.strength + myChar.staffPwr;
            charTotalMagicStr = myChar.magicStr + myChar.staffMgcPwr;
            charToEquipStr.text = charTotalStr.ToString();
            charToEquipMagicStr.text = charTotalMagicStr.ToString();
        }
        else//En principio nunca se dará este else, lo tengo que poner para poder compilar
        {
            charTotalStr = myChar.strength;
            charTotalMagicStr = myChar.magicStr;
            charToEquipStr.text = charTotalStr.ToString();
            charToEquipMagicStr.text = charTotalMagicStr.ToString();
        }

        int charTotalDef = myChar.defence + myChar.headArmorPwr + myChar.bodyArmorPwr + myChar.shieldPwr;
        int charTotalMagicDef = myChar.magicDef + myChar.headArmorMgcPwr + myChar.bodyArmorMgcPwr + myChar.shieldMgcPwr;
        int charTotalSpeed = myChar.speed + myChar.swordSpdPwr + myChar.axeSpdPwr + myChar.bowSpdPwr + myChar.staffSpdPwr +
            +myChar.headArmorSpdPwr + myChar.bodyArmorSpdPwr + myChar.shieldSpdPwr;
        int charTotalEvasion = myChar.evasion + myChar.swordEvsPwr + myChar.axeEvsPwr + myChar.bowEvsPwr + myChar.staffEvsPwr +
            +myChar.headArmorEvsPwr + myChar.bodyArmorEvsPwr + myChar.shieldEvsPwr;

        charToEquip.text = myChar.charName;
        charToEquipDef.text = charTotalDef.ToString();
        charToEquipMagicDef.text = charTotalMagicDef.ToString();
        charToEquipSpd.text = charTotalSpeed.ToString();
        charToEquipEva.text = charTotalEvasion.ToString();

        //Previsualizar el efecto del itemEquip seleccionado en los stats si se equipa:
        if(activeItem != null)//Me aseguro de que he seleccionado un item
        {
            //Uso esta var para agilizar:
            int value;

            //Distingo para cada tipo de equipo
            if(activeItem.isHeadArmor)//Head Armor
            {
                //Defensa:
                value = charTotalDef - myChar.headArmorPwr + activeItem.physicPower;
                charToEquipDef.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalDef < value)
                {
                    charToEquipDef.color = new Color(0, 1, 0);//Verde
                }
                else if(charTotalDef > value)
                {
                    charToEquipDef.color = new Color(1, 0, 0);//Rojo
                }
                else
                {
                    charToEquipDef.color = new Color(1, 1, 1);//Blanco
                }
                //Defensa Magica:
                value = charTotalMagicDef - myChar.headArmorMgcPwr + activeItem.magicPower;
                charToEquipMagicDef.text = value.ToString();
                if (charTotalMagicDef < value)
                {
                    charToEquipMagicDef.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicDef > value)
                {
                    charToEquipMagicDef.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicDef.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.headArmorSpdPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.headArmorEvsPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if(activeItem.isBodyArmor)//BODY ARMOR
            {
                //Defensa:
                value = charTotalDef - myChar.bodyArmorPwr + activeItem.physicPower;
                charToEquipDef.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalDef < value)
                {
                    charToEquipDef.color = new Color(0, 1, 0);
                }
                else if (charTotalDef > value)
                {
                    charToEquipDef.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipDef.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicDef - myChar.bodyArmorPwr + activeItem.magicPower;
                charToEquipMagicDef.text = value.ToString();
                if (charTotalMagicDef < value)
                {
                    charToEquipMagicDef.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicDef > value)
                {
                    charToEquipMagicDef.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicDef.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.bodyArmorPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.bodyArmorPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if (activeItem.isShield)//SHIELD
            {
                //Defensa:
                value = charTotalDef - myChar.shieldPwr + activeItem.physicPower;
                charToEquipDef.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalDef < value)
                {
                    charToEquipDef.color = new Color(0, 1, 0);
                }
                else if (charTotalDef > value)
                {
                    charToEquipDef.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipDef.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicDef - myChar.shieldPwr + activeItem.magicPower;
                charToEquipMagicDef.text = value.ToString();
                if (charTotalMagicDef < value)
                {
                    charToEquipMagicDef.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicDef > value)
                {
                    charToEquipMagicDef.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicDef.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.shieldPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.shieldPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if (activeItem.isSword)//SWORD
            {
                //Ataque:
                value = charTotalStr - myChar.swordPwr + activeItem.physicPower;
                charToEquipStr.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalStr < value)
                {
                    charToEquipStr.color = new Color(0, 1, 0);
                }
                else if (charTotalStr > value)
                {
                    charToEquipStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipStr.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicStr - myChar.swordMgcPwr + activeItem.magicPower;
                charToEquipMagicStr.text = value.ToString();
                if (charTotalMagicStr < value)
                {
                    charToEquipMagicStr.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicStr > value)
                {
                    charToEquipMagicStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicStr.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.swordSpdPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.swordEvsPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if (activeItem.isAxe)//AXE
            {
                //Ataque:
                value = charTotalStr - myChar.axePwr + activeItem.physicPower;
                charToEquipStr.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalStr < value)
                {
                    charToEquipStr.color = new Color(0, 1, 0);
                }
                else if (charTotalStr > value)
                {
                    charToEquipStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipStr.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicStr - myChar.axeMgcPwr + activeItem.magicPower;
                charToEquipMagicStr.text = value.ToString();
                if (charTotalMagicStr < value)
                {
                    charToEquipMagicStr.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicStr > value)
                {
                    charToEquipMagicStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicStr.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.axeSpdPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.axeEvsPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if (activeItem.isBow)//BOW
            {
                //Ataque:
                value = charTotalStr - myChar.bowPwr + activeItem.physicPower;
                charToEquipStr.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalStr < value)
                {
                    charToEquipStr.color = new Color(0, 1, 0);
                }
                else if (charTotalStr > value)
                {
                    charToEquipStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipStr.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicStr - myChar.bowMgcPwr + activeItem.magicPower;
                charToEquipMagicStr.text = value.ToString();
                if (charTotalMagicStr < value)
                {
                    charToEquipMagicStr.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicStr > value)
                {
                    charToEquipMagicStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicStr.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.bowSpdPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.bowEvsPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
            else if (activeItem.isStaff)//STAFF
            {
                //Ataque:
                value = charTotalStr - myChar.staffPwr + activeItem.physicPower;
                charToEquipStr.text = value.ToString();
                //Que cambie el color según mejore o empeore
                if (charTotalStr < value)
                {
                    charToEquipStr.color = new Color(0, 1, 0);
                }
                else if (charTotalStr > value)
                {
                    charToEquipStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipStr.color = new Color(1, 1, 1);
                }
                //Defensa Magica:
                value = charTotalMagicStr - myChar.staffMgcPwr + activeItem.magicPower;
                charToEquipMagicStr.text = value.ToString();
                if (charTotalMagicStr < value)
                {
                    charToEquipMagicStr.color = new Color(0, 1, 0);
                }
                else if (charTotalMagicStr > value)
                {
                    charToEquipMagicStr.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipMagicStr.color = new Color(1, 1, 1);
                }
                //Speed:
                value = charTotalSpeed - myChar.staffSpdPwr + activeItem.speedPower;
                charToEquipSpd.text = value.ToString();
                if (charTotalSpeed < value)
                {
                    charToEquipSpd.color = new Color(0, 1, 0);
                }
                else if (charTotalSpeed > value)
                {
                    charToEquipSpd.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipSpd.color = new Color(1, 1, 1);
                }
                //Evasion:
                value = charTotalEvasion - myChar.staffEvsPwr + activeItem.evasionPower;
                charToEquipEva.text = value.ToString();
                if (charTotalEvasion < value)
                {
                    charToEquipEva.color = new Color(0, 1, 0);
                }
                else if (charTotalEvasion > value)
                {
                    charToEquipEva.color = new Color(1, 0, 0);
                }
                else
                {
                    charToEquipEva.color = new Color(1, 1, 1);
                }
            }
        }
    }


    public void CloseMenu()
    {
        for(int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }

        currentCharSelectedInMenu = 0;
        currentTargetCharSelectedInMenu = 0;

        theMenu.SetActive(false);

        GameManager.instance.gameMenuOpen = false;

        PlayDeclineSFX();

        itemCharChoiceMenu.SetActive(false);
    }

    public void OpenStatus()
    {
        UpdateMainStats();

        //update the information that is shown
        StatusChar(currentCharSelectedInMenu);

        //DEPRECATED Actualizo los botones de cambio de char de status window (old)
        //for(int i = 0; i < statusButtons.Length; i++)
        //{
        //    statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
        //    statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName;
        //}
    }

    public void StatusChar(int selected)//Cambiado para que al no tener arma equipada, se improvise.
    {
        //Activo o desactivo botones de cambio de char:
        if (GameManager.instance.GetPlayersActive() > 1)//Si hay más de 1 char en el grupo
        {
            ScaleToOne(statsNextCharBtn.gameObject);
            ScaleToOne(statsPrevCharBtn.gameObject);
        }
        else//Si sólo hay un char
        {
            ScaleToZero(statsNextCharBtn.gameObject);
            ScaleToZero(statsPrevCharBtn.gameObject);
        }

        currentCharSelectedInMenu = selected;

        statusName.text = playerStats[selected].charName;
        statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
        statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusStr.text = playerStats[selected].strength.ToString();
        statusDef.text = playerStats[selected].defence.ToString();
        statusMagicStr.text = playerStats[selected].magicStr.ToString();
        statusMagicDef.text = playerStats[selected].magicDef.ToString();
        statusSpd.text = playerStats[selected].speed.ToString();
        statusEvasion.text = playerStats[selected].evasion.ToString();

        //Reset the equipment data when changing between players:
        statusSwordEqpd.text = "";
        statusAxeEqpd.text = "";
        statusBowEqpd.text = "";
        statusStaffEqpd.text = "";
        statusHeadArmorEqpd.text = "";
        statusBodyArmorEqpd.text = "";
        statusShieldEqpd.text = "";

        //DEPRECATED Antes mostraba el stat del equipo en stats windows (pero como pueden afectar a más de un stat, lo quito, esto se ve en el equip window mejor)
        //statusSwordPwr.text = "";
        //statusAxePwr.text = "";
        //statusBowPwr.text = "";
        //statusStaffPwr.text = "";
        //statusHeadArmorPwr.text = "";
        //statusBodyArmorPwr.text = "";
        //statusShieldPwr.text = "";


        if (playerStats[selected].equippedSword != "")
        {
            statusSwordEqpd.text = playerStats[selected].equippedSword;
        }
        else if(playerStats[selected].canUseSword)
        {
            statusSwordEqpd.text = "Espada improvisada";
            playerStats[selected].equippedSword = "Espada improvisada";
            playerStats[selected].swordPwr = 1;
        }

        if (playerStats[selected].equippedAxe != "")
        {
            statusAxeEqpd.text = playerStats[selected].equippedAxe;
        }
        else if (playerStats[selected].canUseAxe)
        {
            statusAxeEqpd.text = "Hacha improvisada";
            playerStats[selected].equippedAxe = "Hacha improvisada";
            playerStats[selected].axePwr = 1;

        }

        if (playerStats[selected].equippedBow != "")
        {
            statusBowEqpd.text = playerStats[selected].equippedBow;
        }
        else if (playerStats[selected].canUseBow)
        {
            statusBowEqpd.text = "Arco improvisado";
            playerStats[selected].equippedBow = "Arco improvisado";
            playerStats[selected].bowPwr = 1;

        }

        if (playerStats[selected].equippedStaff != "")
        {
            statusStaffEqpd.text = playerStats[selected].equippedStaff;
        }
        else if (playerStats[selected].canUseStaff)
        {
            statusStaffEqpd.text = "Báculo improvisado";
            playerStats[selected].equippedStaff = "Báculo improvisado";
            playerStats[selected].staffPwr = 1;
        }

        //Deprecated:
        //statusSwordPwr.text = playerStats[selected].swordPwr.ToString();
        //statusAxePwr.text = playerStats[selected].axePwr.ToString();
        //statusBowPwr.text = playerStats[selected].bowPwr.ToString();
        //statusStaffPwr.text = playerStats[selected].staffPwr.ToString();

        if (playerStats[selected].equippedHeadArmor != "")
        {
            statusHeadArmorEqpd.text = playerStats[selected].equippedHeadArmor;
        }
        else
        {
            statusHeadArmorEqpd.text = "";
        }

        if (playerStats[selected].equippedBodyArmor != "")
        {
            statusBodyArmorEqpd.text = playerStats[selected].equippedBodyArmor;
        }
        else
        {
            statusBodyArmorEqpd.text = "";
        }

        if (playerStats[selected].equippedShield != "")
        {
            statusShieldEqpd.text = playerStats[selected].equippedShield;
        }
        else
        {
            statusShieldEqpd.text = "";
        }

        //Deprecated
        //statusHeadArmorPwr.text = playerStats[selected].headArmorPwr.ToString();
        //statusBodyArmorPwr.text = playerStats[selected].bodyArmorPwr.ToString();
        //statusShieldPwr.text = playerStats[selected].shieldPwr.ToString();

        statusExp.text = "" + playerStats[selected].currentEXP + "/" + playerStats[selected].expToNextLevel[playerStats[selected].playerLevel];
        statusExpSlider.maxValue = playerStats[selected].expToNextLevel[playerStats[selected].playerLevel];
        statusExpSlider.value = playerStats[selected].currentEXP;
        //statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentEXP).ToString();

        animStatusChar.SetTrigger(GameManager.instance.activePlayerStats[currentCharSelectedInMenu].animMenuTriggerName);

        //if(currentCharSelectedInMenu == 0)
        //{
        //    animStatusChar.SetTrigger("P1");
        //}
        //else if (currentCharSelectedInMenu == 1)
        //{
        //    animStatusChar.SetTrigger("P2");
        //}
        //else if (currentCharSelectedInMenu == 2)
        //{
        //    animStatusChar.SetTrigger("P3");
        //}
        //else
        //{
        //    animStatusChar.SetTrigger("P4");
        //}
    }


    //Función para determinar y animar el char activo actualmente en cualquier ventana:
    public void UpdateCharMenuImage(Animator animChar)
    {
        //Los chars activos en el menu son los mismos (y en el mismo orden en principio) que los playerStats activos del GameManager:
        //Creo una lista con los chars activos:
        List<CharStats> playersList = new List<CharStats>();

        for(int i = 0; i < GameManager.instance.playerStats.Length; i++)
        {
            if (GameManager.instance.playerStats[i].isActiveAndEnabled)
            {
                playersList.Add(GameManager.instance.playerStats[i]);
            }
        }

        //Ahora simplemente activo el playerStat según el char activo en el menú:
        animChar.SetTrigger(playersList[currentCharSelectedInMenu].animMenuTriggerName);
    }

    //Función para contar los chars activos en la party
    public int CountActiveChars()
    {
        int chars = 0;
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                chars++;
            }
        }

        return chars;
    }

    public void OpenEquip()
    {
        UpdateMainStats();

        //update the information that is shown
        EquipChar(currentCharSelectedInMenu);

        //Gestion de botones de flechita para cambio personajes:
        //Activo o desactivo botones de cambio de char:
        if (GameManager.instance.GetPlayersActive() > 1)//Si hay más de 1 char en el grupo
        {
            ScaleToOne(equipNextCharBtn.gameObject);
            ScaleToOne(equipPrevCharBtn.gameObject);
        }
        else//Si sólo hay un char
        {
            ScaleToZero(equipNextCharBtn.gameObject);
            ScaleToZero(equipPrevCharBtn.gameObject);
        }

    }

    //Lógica de los botones flechita para cambiar chars en Equip window:
    public void ChangeCharForward()
    {
        int newChar = currentCharSelectedInMenu;
        newChar++;
        //Compruebo que no me salga de mis chars activos
        if (newChar >= GameManager.instance.activePlayerStats.Count)
        {
            newChar = 0;
        }
        Debug.Log("newChar: " + newChar);

        currentCharSelectedInMenu = newChar;

        //Lógica aquí para diferenciar entre windows al pulsar estos botones:
        if (windows[0].activeInHierarchy)//Ventana items, aquí en principio nada
        {
            //Aquí en principio nada
        }
        else if (windows[1].activeInHierarchy)//Ventana Equipo
        {
            EquipChar(currentCharSelectedInMenu);
        }
        else if (windows[2].activeInHierarchy)//Stats window
        {
            OpenStatus();
        }
        else if (windows[3].activeInHierarchy)//Heal window
        {
            OpenHealWindow();
        }
    }

    public void ChangeCharBackwards()
    {
        int newChar = currentCharSelectedInMenu;
        newChar--;
        //Compruebo que no me salga de mis chars activos
        if (newChar >= GameManager.instance.activePlayerStats.Count)
        {
            newChar = 0;
        }
        currentCharSelectedInMenu = newChar;

        //Lógica aquí para diferenciar entre windows al pulsar estos botones:
        if (windows[0].activeInHierarchy)//Ventana items, aquí en principio nada
        {
            //Aquí en principio nada
        }
        else if (windows[1].activeInHierarchy)//Ventana Equipo
        {
            EquipChar(currentCharSelectedInMenu);
        }
        else if (windows[2].activeInHierarchy)//Stats window
        {
            OpenStatus();
        }
        else if (windows[3].activeInHierarchy)//Heal window
        {
            OpenHealWindow();
        }
    }


    public void EquipChar(int selected)
    {
        equipName.text = playerStats[selected].charName;

        currentCharSelectedInMenu = selected;

        //Equip window:
        //Reset the equipment data when changing between players:
        equipSwordEqpd.text = "";
        equipAxeEqpd.text = "";
        equipBowEqpd.text = "";
        equipStaffEqpd.text = "";

        equipSwordPwr.text = "";
        equipAxePwr.text = "";
        equipBowPwr.text = "";
        equipStaffPwr.text = "";

        equipHeadArmorEqpd.text = "";
        equipBodyArmorEqpd.text = "";
        equipShieldEqpd.text = "";

        equipHeadArmorPwr.text = "";
        equipBodyArmorPwr.text = "";
        equipShieldPwr.text = "";


        if (playerStats[selected].equippedSword != "")
        {
            equipSwordEqpd.text = playerStats[selected].equippedSword;
        }
        else if (playerStats[selected].canUseSword)
        {
            equipSwordEqpd.text = "Espada improvisada";
            playerStats[selected].equippedSword = "Espada improvisada";
            playerStats[selected].swordPwr = 1;
        }

        if (playerStats[selected].equippedAxe != "")
        {
            equipAxeEqpd.text = playerStats[selected].equippedAxe;
        }
        else if (playerStats[selected].canUseAxe)
        {
            equipAxeEqpd.text = "Hacha improvisada";
            playerStats[selected].equippedAxe = "Hacha improvisada";
            playerStats[selected].axePwr = 1;

        }

        if (playerStats[selected].equippedBow != "")
        {
            equipBowEqpd.text = playerStats[selected].equippedBow;
        }
        else if (playerStats[selected].canUseBow)
        {
            equipBowEqpd.text = "Arco improvisado";
            playerStats[selected].equippedBow = "Arco improvisado";
            playerStats[selected].bowPwr = 1;

        }

        if (playerStats[selected].equippedStaff != "")
        {
            equipStaffEqpd.text = playerStats[selected].equippedStaff;
        }
        else if (playerStats[selected].canUseStaff)
        {
            equipStaffEqpd.text = "Báculo improvisado";
            playerStats[selected].equippedStaff = "Báculo improvisado";
            playerStats[selected].staffPwr = 1;
        }


        //equipSwordPwr.text = playerStats[selected].swordPwr.ToString();
        //equipAxePwr.text = playerStats[selected].axePwr.ToString();
        //equipBowPwr.text = playerStats[selected].bowPwr.ToString();
        //equipStaffPwr.text = playerStats[selected].staffPwr.ToString();

        if (playerStats[selected].equippedHeadArmor != "")
        {
            equipHeadArmorEqpd.text = playerStats[selected].equippedHeadArmor;
        }
        else
        {
            equipHeadArmorEqpd.text = "";
        }

        if (playerStats[selected].equippedBodyArmor != "")
        {
            equipBodyArmorEqpd.text = playerStats[selected].equippedBodyArmor;
        }
        else
        {
            equipBodyArmorEqpd.text = "";
        }

        if (playerStats[selected].equippedShield != "")
        {
            equipShieldEqpd.text = playerStats[selected].equippedShield;
        }
        else
        {
            equipShieldEqpd.text = "";
        }


        //equipHeadArmorPwr.text = playerStats[selected].headArmorPwr.ToString();
        //equipBodyArmorPwr.text = playerStats[selected].bodyArmorPwr.ToString();
        //equipShieldPwr.text = playerStats[selected].shieldPwr.ToString();

        //Actualizar la imagen del char
        animEquipChar.SetTrigger(GameManager.instance.activePlayerStats[currentCharSelectedInMenu].animMenuTriggerName);
        Debug.Log("Trigger anim es: " + GameManager.instance.activePlayerStats[currentCharSelectedInMenu].animMenuTriggerName);

        //if (selected == 0)
        //{
        //    animEquipChar.SetTrigger("P1");
        //}
        //else if (selected == 1)
        //{
        //    animEquipChar.SetTrigger("P2");
        //}
        //else if (selected == 2)
        //{
        //    animEquipChar.SetTrigger("P3");
        //}
        //else
        //{
        //    animEquipChar.SetTrigger("P4");
        //}


        //Heal window:
        //OpenHealWindow();
    }

    public int GetActiveWindow()
    {
        for (int i = 0; i < windows.Length; i++)
        {
            if (windows[i].activeInHierarchy)
            {
                currentActiveWindow = i;
                return i;
            }
        }
        Debug.Log("ninguna ventana activa; return 9");
        currentActiveWindow = 9;
        return 9;
    }


    public void ShowItems()
    {
        GameManager.instance.SortItems();

        //Activo y desactivo elementos de la UI para abrir la ventana Items:
        discardItemButton.SetActive(true);
        cancelEquipSpecificItemButton.SetActive(false);
        equipCharStatsPanel.SetActive(false);

        DeselectItem();

        if (itemsSpecial)
        {
            discardItemButton.SetActive(false);//Los items special no los puedes descartar 

            for (int i = 0; i < itemButtons.Length; i++)
            {
                itemButtons[i].buttonValue = i;

                if (GameManager.instance.itemsHeldSpecial[i] != "")
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(true);
                    itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldSpecial[i]).itemSprite;
                    itemButtons[i].amountText.text = "";//En principio los items special son incontables
                }
                else
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(false);
                    itemButtons[i].amountText.text = "";
                }
            }
        }
        else if (itemsEquip)
        {
            for (int i = 0; i < itemButtons.Length; i++)
            {
                itemButtons[i].buttonValue = i;

                if (GameManager.instance.itemsHeldEquip[i] != "")
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(true);
                    itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldEquip[i]).itemSprite;
                    itemButtons[i].amountText.text = GameManager.instance.numberOfItemsEquip[i].ToString();
                }
                else
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(false);
                    itemButtons[i].amountText.text = "";
                }
            }
        }
        else if(itemsBattle)
        {
            
            for (int i = 0; i < itemButtons.Length; i++)
            {
                itemButtons[i].buttonValue = i;

                if (GameManager.instance.itemsHeldBattle[i] != "")
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(true);
                    itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeldBattle[i]).itemSprite;
                    itemButtons[i].amountText.text = GameManager.instance.numberOfItemsBattle[i].ToString();
                }
                else
                {
                    itemButtons[i].buttonImage.gameObject.SetActive(false);
                    itemButtons[i].amountText.text = "";
                }
            }
        }
    }


    public void SelectItem(Item newItem)
    {
        activeItem = newItem;

        useItemButton.SetActive(true);
        //discardItemButton.SetActive(true);

        if (activeItem.isItem)
        {
            useButtonText.text = "Usar";
        }

        if(activeItem.isSword || activeItem.isAxe || activeItem.isBow || activeItem.isStaff || activeItem.isHeadArmor || activeItem.isBodyArmor || activeItem.isShield)
        {
            useButtonText.text = "Equipar";
        }

        if (activeItem.isItemSpecial)
        {
            useItemButton.SetActive(false);
            discardItemButton.SetActive(false);
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }

    public void SelectFilteredItemToEquip(int itemPosition)
    {
        activeItem = filteredItems[itemPosition];

        useItemButton.SetActive(true);
        useButtonText.text = "Equipar";
        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;

        //Actualizo los stats del char actuales y según el arma seleccionada (a equipar):
        UpdateEquipCharStats();
    }

    public void DiscardItem()
    {
        if(activeItem != null)
        {
            GameManager.instance.RemoveItem(activeItem.itemName);
            DeselectItem();
            //Habría que reordenar
        }
    }

    public void OpenItemCharChoice()
    {
        //Si estoy en la ventana de equipar item específico, usar un item lo equipa directamente, sin seleccionar el player:
        if (itemsToEquipWindow)
        {
            Debug.Log("Current Char: " + currentCharSelectedInMenu);
            activeItem.Use(currentCharSelectedInMenu);
        }
        else//Si no, compruebo de qué tipo de item se trata
        {
            if (activeItem.isForPlayers)//Si es un item que se usa en algún player
            {
                itemCharChoiceMenu.SetActive(true);

                //Activo panel de eleccion de target char:
                itemTargetCharsPanel.SetActive(true);

                //Referencio a mis itemCharStats con los charStatsHolders:
                for(int i = 0; i < itemCharStats.Length; i++)
                {
                    //Si el CharStatHolder está activo
                    if (charStatHolder[i].activeInHierarchy)
                    {
                        //Relleno todo copiandolo
                        itemCharStats[i].SetActive(true);

                        itemCharStats[i].transform.GetChild(1).GetComponent<Text>().text = nameText[i].text;
                        itemCharStats[i].transform.GetChild(3).GetComponent<Text>().text = hpText[i].text;
                        itemCharStats[i].transform.GetChild(4).GetComponent<Text>().text = mpText[i].text;
                        itemCharStats[i].transform.GetChild(2).GetComponent<Text>().text = lvlText[i].text;
                        itemCharStats[i].transform.GetChild(3).GetChild(0).GetComponent<Slider>().maxValue = hpSlider[i].maxValue;
                        itemCharStats[i].transform.GetChild(3).GetChild(0).GetComponent<Slider>().value = hpSlider[i].value;
                        itemCharStats[i].transform.GetChild(4).GetChild(0).GetComponent<Slider>().maxValue = mpSlider[i].maxValue;
                        itemCharStats[i].transform.GetChild(4).GetChild(0).GetComponent<Slider>().value = mpSlider[i].value;
                        itemCharStats[i].transform.GetChild(0).GetComponent<Image>().sprite = charImage[i].sprite;

                        //Me aseguro de que esté el charStat esté activo y visible:
                        //Habilito el botón
                        itemCharStats[i].GetComponent<Button>().interactable = true;
                        //Quito la mascara del charStat
                        itemCharStats[i].transform.GetChild(5).GetComponent<Image>().enabled = false;
                    }
                    else
                    {
                        //Si no, desactivo el itemCharStat:
                        itemCharStats[i].SetActive(false);
                    }
                }

                //Tengo que desactivar y oscurecer a los players no compatibles con el item seleccionado:
                for (int i = 0; i < itemCharStats.Length; i++)
                {
                    //Si este itemCharStat está desactivado (no hay char), paso al siguiente charStat:
                    if (!itemCharStats[i].activeInHierarchy)
                    {
                        continue;
                    }

                    int itemType = DetectTypeOfItem();

                    Button thisButton = itemCharStats[i].GetComponent<Button>();

                    switch (itemType)
                    {
                        case 1://Sword
                            if (!GameManager.instance.activePlayerStats[i].canUseSword)
                            {
                                //Deshabilito el botón
                                thisButton.interactable = false;
                                //Oscurezco el charStat
                                itemCharStats[i].transform.GetChild(5).GetComponent<Image>().enabled = true;
                            }
                            break;
                        case 2://Axe
                            if (!GameManager.instance.activePlayerStats[i].canUseAxe)
                            {
                                //Deshabilito el botón
                                thisButton.interactable = false;
                                //Oscurezco el charStat
                                itemCharStats[i].transform.GetChild(5).GetComponent<Image>().enabled = true;
                            }
                            break;
                        case 3://Bow
                            if (!GameManager.instance.activePlayerStats[i].canUseBow)
                            {
                                //Deshabilito el botón
                                thisButton.interactable = false;
                                //Oscurezco el charStat
                                itemCharStats[i].transform.GetChild(5).GetComponent<Image>().enabled = true;                            }
                            break;
                        case 4://Staff
                            if (!GameManager.instance.activePlayerStats[i].canUseStaff)
                            {
                                //Deshabilito el botón
                                thisButton.interactable = false;
                                //Oscurezco el charStat
                                itemCharStats[i].transform.GetChild(5).GetComponent<Image>().enabled = true;                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            else//Si el item NO se usa en los players
            {
                if (activeItem.specialEffect)
                {
                    //Aquí tendré que programar individualmente la funcion de cada Special Effect Item:
                    if (activeItem.itemName == "Cebo")
                    {
                        if (GameObject.FindWithTag("BattleZone") == null)
                        {
                            feedbackMessage.text = "¡No puedes usar un cebo aquí!";
                            StartCoroutine(FeedbackNotificationCo());
                        }
                        else
                        {
                            StartCoroutine(CeboCo());
                        }
                    }
                }
            }
        }
    }

    //public void OldOpenItemCharChoice()
    //{
    //    //Si estoy en la ventana de equipar item específico, usar un item lo equipa directamente, sin seleccionar el player:
    //    if (itemsToEquipWindow)
    //    {
    //        Debug.Log("Current Char: " + currentCharSelectedInMenu);
    //        activeItem.Use(currentCharSelectedInMenu);
    //    }
    //    else//Si no, compruebo de qué tipo de item se trata
    //    {
    //        if (activeItem.isForPlayers)//Si es un item que se usa en algún player
    //        {
    //            itemCharChoiceMenu.SetActive(true);

    //            //Activo panel de eleccion de target char:
    //            targetCharsPanel.SetActive(true);

    //            for (int i = 0; i < itemCharChoiceNames.Length; i++)
    //            {
    //                itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
    //                itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);

    //                int itemType = DetectTypeOfItem();

    //                Button thisButton = itemCharChoiceNames[i].transform.parent.gameObject.GetComponent<Button>();

    //                switch (itemType)
    //                {
    //                    case 1://Sword
    //                        if (!GameManager.instance.playerStats[i].canUseSword)
    //                        {
    //                            thisButton.interactable = false;
    //                        }
    //                        break;
    //                    case 2://Axe
    //                        if (!GameManager.instance.playerStats[i].canUseAxe)
    //                        {
    //                            thisButton.interactable = false;
    //                        }
    //                        break;
    //                    case 3://Bow
    //                        if (!GameManager.instance.playerStats[i].canUseBow)
    //                        {
    //                            thisButton.interactable = false;
    //                        }
    //                        break;
    //                    case 4://Staff
    //                        if (!GameManager.instance.playerStats[i].canUseStaff)
    //                        {
    //                            thisButton.interactable = false;
    //                        }
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                //Desactivar los botones de los personajes cuando no pueden equipar cierto item:
    //                //1-Detectar el tipo de item (isSword, isBow, isAxe o isStaff)
    //                //2-Comprobar si el char puede equipar el tipo de arma (GameManager.instance.playerStats[i].usesAxe)
    //                //3- Si puede perfecto; si no puede, bloquear el boton
    //            }
    //        }
    //        else//Si el item NO se usa en los players
    //        {
    //            if (activeItem.specialEffect)
    //            {
    //                //Aquí tendré que programar individualmente la funcion de cada Special Effect Item:
    //                if(activeItem.itemName == "Cebo")
    //                {
    //                    if(GameObject.FindWithTag("BattleZone") == null)
    //                    {
    //                        feedbackMessage.text = "¡No puedes usar un cebo aquí!";
    //                        StartCoroutine(FeedbackNotificationCo());
    //                    }
    //                    else
    //                    {
    //                        StartCoroutine(CeboCo());
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void CloseItemCharChoice()
    {
        DeselectItem();

        for (int i = 0; i < itemCharChoiceNames.Length; i++)
        {
            itemCharChoiceNames[i].transform.parent.gameObject.GetComponent<Button>().interactable = true;
        }

        //Desactivo los paneles de selección y confirmación de target:
        itemCharChoiceMenu.SetActive(false);
        itemTargetCharsPanel.SetActive(false);
    }

    public void DeselectItem()
    {
        itemName.text = "Selecciona un objeto";
        itemDescription.text = "";
        useItemButton.SetActive(false);
        discardItemButton.SetActive(false);

        activeItem = null;

        if (itemsToEquipWindow)
        {
            UpdateEquipCharStats();
            charToEquipStr.color = new Color(1, 1, 1);
            charToEquipMagicStr.color = new Color(1, 1, 1);
            charToEquipDef.color = new Color(1, 1, 1);
            charToEquipMagicDef.color = new Color(1, 1, 1);
            charToEquipSpd.color = new Color(1, 1, 1);
            charToEquipEva.color = new Color(1, 1, 1);
        }
    }

    public void UseItem(int selectChar)
    {
        activeItem.Use(selectChar);
        CloseItemCharChoice();
    }

    public void SaveGame()
    {
        GameManager.instance.SaveData();
        //QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlaySFX(4);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(mainMenuName);

        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(FindObjectOfType<CameraController>());
        Destroy(gameObject);
    }

    public int DetectTypeOfItem()
    {
        if (activeItem.isSword)
        {
            return (1);
        }
        else if (activeItem.isAxe)
        {
            return (2);
        }
        else if (activeItem.isBow)
        {
            return (3);
        }
        else if (activeItem.isStaff)
        {
            return (4);
        }
        else
        {
            return (0);
        }
    }


    public IEnumerator FeedbackNotificationCo()
    {
        feedbackPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        feedbackPanel.SetActive(false);
    }

    //Corrutinas de los items de SpecialEffect:
    public IEnumerator CeboCo()
    {
        feedbackMessage.text = "Esparces el cebo y esperas en silencio...";
        StartCoroutine(FeedbackNotificationCo());
        yield return new WaitForSecondsRealtime(1f);

        ToogleMainMenu();

        yield return new WaitForEndOfFrame();

        GameObject.FindWithTag("BattleZone").GetComponent<BattleStarter>().TriggerBaitedBattle();
        yield return null;
    }

    //Cambiar paleta de colores de UI:
    public void SetCustomUI(int i)
    {
        foreach (Image img in lightButtonsUI)
        {
            img.color = UIColorLightBtns[i];
        }
        foreach (Image img in darkButtonsUI)
        {
            img.color = UIColorDarkBtns[i];
        }
        foreach (Image img in lightBgUI)
        {
            img.color = UIColorLightBg[i];
        }
        foreach (Image img in darkBgUI)
        {
            img.color = UIColorLightBg[i];
        }
        Debug.Log("GameMenu UI changed to palette " + i);
    }

    public void ChangePaletteUI()
    {
        UIpaletteID++;
        if(UIpaletteID >= UIColorLightBtns.Length)
        {
            UIpaletteID = 0;
        }
        SetCustomUI(UIpaletteID);
        BattleManager.instance.SetCustomUI(UIpaletteID);
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


    //Activar DPad tactil en lugar de joystick:
    public void TouchDPad()
    {
        touchDPad.transform.localScale = Vector3.one;
        touchJoystick.transform.localScale = Vector3.zero;
    }

    //Activar Joystick tactil en lugar de D-Pad:
    public void TouchJoystick()
    {
        touchDPad.transform.localScale = Vector3.zero;
        touchJoystick.transform.localScale = Vector3.one;
    }

    public void UnhighlightCharStatsButtons()//Desactivo los botones de los char stats
    {
        for(int i = 0; i < charStatHolder.Length; i++)
        {
            //Quito el highligh del boton del charStats:
            charStatHolder[i].GetComponent<Image>().enabled = false;

            //charStatHolder[i].GetComponent<Button>().interactable = false;
        }
    }

    public void EnableCharStatsButtons()//Activo los botones de los char stats
    {
        for (int i = 0; i < charStatHolder.Length; i++)
        {
            charStatHolder[i].GetComponent<Button>().interactable = true;
        }
    }

    public void ScaleToZero(GameObject g)//Escala objetos a 0 para que desaparezcan (especialmente botones)
    {
        g.transform.localScale = Vector3.zero;
    }

    public void ScaleToOne(GameObject g)//Escala objetos a 1
    {
        g.transform.localScale = Vector3.one;
    }

    //Método generico para desplazar el alfa de un sprite de 0 a 1 sinfin:
    public IEnumerator HighlightBlinkingCo(Image image)
    {
        yield return new WaitForEndOfFrame();

        //Activo la imagen que resalta el objeto:
        image.enabled = true;

        //Ahora ciclo el alpha de esta imagen de 0 a 1 indefinidamente:
        float alpha = 1;
        bool vanishing = true;

        while (image.enabled == true)
        {
            if (vanishing)//Si estoy en fase de desaparición de la imagen
            {
                alpha -= highlightSpeed * Time.deltaTime;
                //Cuando la imagen ha desaparecido pongo vanishing a false
                if(alpha <= 0)
                {
                    vanishing = false;
                }

                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }
            else//Si estoy en fase de aparición de la imagen:
            {
                alpha += highlightSpeed * Time.deltaTime;
                //Cuando la imagen ha aparecido pongo vanishing a true
                if (alpha >= 1)
                {
                    vanishing = true;
                }

                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }
        }

        Debug.Log($"Salgo del blinkeo de {image.gameObject.name}");

        image.enabled = false;
    }




    //---------BESTIARIO--------------

    //Funcion al abrir la ventana bestiario:
    public void OpenBestiaryWindow()
    {
        //Quito cualquier dato que haya por defecto:
        EmptyEnemyData();

        //Tengo que ir poniendo las entradas de enemigos de mi lista general de enemigos
        for (int j = 0; j < enemyEntries.Length; j++)
        {
            //Configuro que el botón llame a GetEnemyData y al enemigo en cuestión:
            enemyEntries[j].GetComponent<EnemyEntry>().enemy = BattleManager.instance.enemyPrefabs[j];

            //Activo el botón de obtener datos:
            enemyEntries[j].GetComponent<Button>().interactable = true;

            //Miro en orden de enemigos según están en el BattleManager y, si están amaestrados:
            if (BattleManager.instance.enemyPrefabs[j].tamed)
            {
                //Pongo su nombre en la entrada en cuestion:
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().text = BattleManager.instance.enemyPrefabs[j].charName;

                //Y lo pongo en negrita para hacer notar que es invocable (aplicar aquí otros cambios estéticos):
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            }
            //Si no lo he amaestrado pero sí me lo he encontrado alguna vez:
            else if (BattleManager.instance.enemyPrefabs[j].sighted)
            {
                //Pongo su nombre en la entrada en cuestion:
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().text = BattleManager.instance.enemyPrefabs[j].charName;

                //Y lo pongo en normal (no negrita):
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
            }
            else//Si ni lo he visto
            {
                //No pongo su nombre en la entrada en cuestion:
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().text = "...";

                //Y lo pongo en normal (no negrita):
                enemyEntries[j].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
            }
        }
    }


    //Función que se llama indirectamente al pulsar sobre una entrada de enemigo en el bestiario para que aparezcan sus datos a la derecha.
    public void GetEnemyData(BattleChar enemy)
    {
        if (enemy.tamed)
        {
            //Si el enemigo no está ni listado (nunca visto):
            enemyImage.sprite = enemy.theSprite.sprite;//Pongo la imagen
            //Escribo el texto de arriba a la derecha
            enemyDataText1.text = $"{enemy.charName}\nHP: {enemy.maxHP}\nNivel de escudo: {enemy.shieldMaxAmount}\n \nDebilidades:";
            //Activo los iconos de sus debilidades
            GetEnemyWeaknesses(enemy);
            //Pongo el icono de su efecto (si lo hay):
            enemyTypeImage = enemyWeaknessIcons[enemy.invocationType].GetComponent<Image>();
            //Escribo el texto de abajo:
            enemyDataText2.text = $"Coste de invocación: {enemy.invocationCost}\n \n          Efecto: {enemy.invocationEffect}\n\n\nDescripción: {enemy.invocationDescription}";
        }
        else if (enemy.sighted)
        {
            //Si el enemigo no está ni listado (nunca visto):
            enemyImage.sprite = enemy.theSprite.sprite;//Pongo la imagen
            //Escribo el texto de arriba a la derecha
            enemyDataText1.text = $"{enemy.charName}\nHP: ???\nNivel de escudo: ???\n \nDebilidades:";
            //Activo los iconos de sus debilidades
            GetEnemyWeaknesses(enemy);

            //Pongo el icono de su efecto (si lo hay):
            enemyTypeImage.GetComponent<Image>().sprite = unknownIconSprite;
            //Escribo el texto de abajo:
            enemyDataText2.text = "";
        }
        else
        {
            EmptyEnemyData();
        }
    }

    //Vacío los datos de las enemyEntry del bestiario:
    public void EmptyEnemyData()
    {
        //Si el enemigo no está ni listado (nunca visto):
        enemyImage.sprite = unknownIconSprite;
        //Escribo el texto de arriba a la derecha
        enemyDataText1.text = "";
        //Desactivo los iconos de sus debilidades
        for (int i = 0; i < enemyWeaknessIcons.Length; i++)
        {
            //Desactivo todos los iconos:
            enemyWeaknessIcons[i].SetActive(false);
        }

        //Pongo el icono de su efecto (si lo hay):
        enemyTypeImage.gameObject.SetActive(false);
        //Escribo el texto de abajo:
        enemyDataText2.text = "";
    }

    private void GetEnemyWeaknesses(BattleChar enemy)
    {
        //Inicializo una lista temporal con los gameObjects iconos de debilidades utiles:
        List<GameObject> iconsList = new List<GameObject>();

        //Actualizo la matriz de debilidades del BattleChar enemigo
        enemy.FillWeaknessArray();
        //Activo los gameObject iconos de debilidades de la UI y les asigno el sprite que toca a su imagen.UI:
        for (int i = 0; i < enemyWeaknessIcons.Length; i++)
        {
            //Compruebo las debilidades del enemigo:
            if (enemy.enemyWeaknessesArray[i])
            {
                //Activo el icono
                enemyWeaknessIcons[i].SetActive(true);

                //Añado dicho icono a la lista temporal:
                iconsList.Add(enemyWeaknessIcons[i]);
            }
            else
            {
                //Si el enemigo no tiene esta debilidad, desactivo el icono:
                enemyWeaknessIcons[i].SetActive(false);
            }
        }
        
        //Compruebo que las debilidades no estén ocultas:
        for(int i = 0; i < iconsList.Count; i++)
        {
            //Si la debilidad es conocida:
            if (enemy.weaknessUnknownIntList[i] == 1)
            {
                //Le pongo el sprite correspondiente
                iconsList[i].GetComponent<Image>().sprite = enemy.weaknessSpriteList[i];
            }
            else//Si es desconocida
            {
                //Le pongo el sprite unknown:
                iconsList[i].GetComponent<Image>().sprite = unknownIconSprite;
            }
        }
    }



    //--------HEAL MENU----------
    //Al abrir o actualizar la ventana de HealWindow
    public void OpenHealWindow()
    {
        UpdateMainStats();//Actualizo los stats de los chars
        //Desactivo cualquier posible highlighting de los charStats:
        HealMagicUnselect();

        //Actualizo nombre del currentChar:
        healCharName.text = GameManager.instance.activePlayerStats[currentCharSelectedInMenu].charName;
        //Actualizo imagen del currentChar:
        healCharImage.GetComponent<Animator>().SetTrigger(GameManager.instance.activePlayerStats[currentCharSelectedInMenu].animMenuTriggerName);
        //if (currentCharSelectedInMenu == 0)
        //{
        //    healCharImage.GetComponent<Animator>().SetTrigger("P1");
        //}
        //else if (currentCharSelectedInMenu == 1)
        //{
        //    healCharImage.GetComponent<Animator>().SetTrigger("P2");
        //}
        //else if (currentCharSelectedInMenu == 2)
        //{
        //    healCharImage.GetComponent<Animator>().SetTrigger("P3");
        //}
        //else
        //{
        //    healCharImage.GetComponent<Animator>().SetTrigger("P4");
        //}

        //Activo o desactivo botones de cambio de char:
        if (GameManager.instance.GetPlayersActive() > 1)//Si hay más de 1 char en el grupo
        {
            ScaleToOne(healNextChar.gameObject);
            ScaleToOne(healPrevChar.gameObject);
        }
        else//Si sólo hay un char
        {
            ScaleToZero(healNextChar.gameObject);
            ScaleToZero(healPrevChar.gameObject);
        }

        //Actualizo mensaje de texto:
        healMenuText.text = "";

        //Quito el highligh del boton del charStats:
        charStatHolder[currentTargetCharSelectedInMenu].GetComponent<Image>().enabled = false;

        UnhighlightCharStatsButtons();//Desactivo los botones de los charStats (por si acaso)
        GetHealButtons(currentCharSelectedInMenu);//Le meto el int del player que tenga seleccionado
        healButtonsMask.enabled = false;//Me aseguro de que la mascara de botones esta quitada
        ScaleToZero(healCancelButton);//Quito el boton de cancelar uso de magia
        ScaleToZero(healUseButton);//Quito el botón de confirmar uso de magia
    }
    //Esta función busca de entre los movimientos que sabe el char seleccionado y filtra las magias blancas (que son las unicas usables desde el menu), y las pone en los botones del heal menu:
    public void GetHealButtons(int player)
    {
        //Creo una lista temporal con los moves del player que tengo que asignar:
        List<BattleMove> charMovesTemplist = new List<BattleMove>();

        //Para abaratar, creo una lista con los moves usables desde el menu:
        List<BattleMove> menuMovesList = new List<BattleMove>();
        for (int i = 0; i < BattleManager.instance.movesList.Length; i++)
        {
            if (BattleManager.instance.movesList[i].usableFromMainMenu)
            {
                menuMovesList.Add(BattleManager.instance.movesList[i]);
                
                //Aprovecho para llenar la lista temporal de moves del char que son usables:
                for(int j = 0; j < GameManager.instance.activePlayerStats[currentCharSelectedInMenu].movesLearnt.Count; j++)
                {
                    //Si algún move del char coincide con alguno de los moves usables
                    if (GameManager.instance.activePlayerStats[currentCharSelectedInMenu].movesLearnt[j] == BattleManager.instance.movesList[i].moveName)
                    {
                        //Lo añado a la lista temporal del char:
                        charMovesTemplist.Add(BattleManager.instance.movesList[i]);
                    }
                }
            }
        }

        //Para cada boton disponible en el healMenu
        for (int i = 0; i < magicButtons.Length; i++)
        {
            //Mientras tenga moves por asignar
            if(i < charMovesTemplist.Count)
            {
                //Asigno el move al botón
                Debug.Log($"Asigno el movimiento {charMovesTemplist[i].moveName}");
                //lo puedo asignar al botón del HealMenu:
                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = charMovesTemplist[i].moveName;
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                magicButtons[i].theMove = charMovesTemplist[i];
                magicButtons[i].spellCost = charMovesTemplist[i].moveCost;
                magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                magicButtons[i].multiTarget = charMovesTemplist[i].multiTarget;

                //Si no tengo MP suficientes o este player esta muerto, la tengo que enmascarar:
                if (GameManager.instance.activePlayerStats[currentCharSelectedInMenu].currentMP < magicButtons[i].spellCost || GameManager.instance.activePlayerStats[currentCharSelectedInMenu].currentHP <= 0)
                {
                    magicButtons[i].nameText.color = Color.grey;
                    magicButtons[i].costText.color = Color.red;
                    magicButtons[i].GetComponent<Button>().interactable = false;
                }
                else
                {
                    magicButtons[i].nameText.color = Color.white;
                    magicButtons[i].costText.color = Color.white;
                    magicButtons[i].GetComponent<Button>().interactable = true;
                }
            }
            else//Cuando ya no me quedan moves por asignar:
            {
                //Desactivo el botón:
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    //Función llamada desde MenuMagicSelect al pulsar sobre una magia usable desde el menu (sanar)
    public void HealMagicSelect(BattleMove move)//Se llama desde MenuMagicSelect al seleccionar la magia del heal menu.
    {
        //Muestro los botones de cancelar/usar magia:
        ScaleToOne(healCancelButton);
        ScaleToOne(healUseButton);

        //Si ya tengo este move seleccionado, lo deselecciono:
        for (int i = 0; i < magicButtons.Length; i++)
        {
            //Si hay algún botón resaltado y coincide con el move pedido, deselecciono todo:
            if (magicButtons[i].transform.GetChild(0).GetComponent<Image>().enabled && magicButtons[i].theMove == move)
            {
                HealMagicUnselect();
                return;
            }
        }

        healMenuMove = move;//Almaceno el movimiento seleccionado

        //Resalto el move seleccionado:
        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (magicButtons[i].theMove == healMenuMove)
            {
                magicButtons[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                StartCoroutine(HighlightBlinkingCo(magicButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>()));
            }
            else//Y quito el highlight del resto:
            {
                magicButtons[i].transform.GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            }
        }

        if (move.multiTarget)//Si es multitarget resalto hasta a los 4 chars:
        {
            //Cambiar el texto:
            if(charStatHolder.Length > 1)
            {
                healMenuText.text = $"¿Aplicar {move.moveName} al grupo?";
            }
            else
            {
                healMenuText.text = $"¿Aplicar {move.moveName} a {GameManager.instance.activePlayerStats[currentCharSelectedInMenu].charName}?";
            }

            //ScaleToOne(healCancelButton);//Muestro el botón de cancelar

            for (int i = 0; i < charStatHolder.Length; i++)
            {
                //Resalto el borde de cada charStatHolder:
                StartCoroutine(HighlightBlinkingCo(charStatHolder[i].GetComponent<Image>()));
            }
        }
        else//Si no es multitarget
        {
            //Cambiar el texto:
            healMenuText.text = $"¿Aplicar {move.moveName} a {GameManager.instance.activePlayerStats[currentTargetCharSelectedInMenu].charName}?";
            EnableCharStatsButtons();//Activo los botones de los charStats para poder elegir el target
            //ScaleToOne(healCancelButton);//Muestro el botón de cancelar
            //Resalto al player actualmente seleccionado:
            StartCoroutine(HighlightBlinkingCo(charStatHolder[currentTargetCharSelectedInMenu].GetComponent<Image>()));
        }

        //Activo los charStatButtons:
        EnableCharStatsButtons();
    }

    //Deseleccionar heal magic:
    public void HealMagicUnselect()
    {
        //Actualizo el text:
        healMenuText.text = "";

        //Quito el highlight del boton de la magia:
        for (int i = 0; i < magicButtons.Length; i++)
        {
            magicButtons[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
        }

        //Desactivo los botones charStat:
        UnhighlightCharStatsButtons();

        ScaleToZero(healCancelButton);//Quito el boton de cancelar uso de magia
        ScaleToZero(healUseButton);//Quito el botón de confirmar uso de magia
    }

    //Función que aplica el efecto de la magia de heal menu:
    public void UseHealMove()
    {
        //Referencio al que hace la magia y al target:
        CharStats author = activePlayersList[currentCharSelectedInMenu];
        CharStats target = activePlayersList[currentTargetCharSelectedInMenu];

        //Es multiTarget?
        if (healMenuMove.multiTarget)
        {
            for (int i = 0; i < activePlayersList.Count; i++)
            {
                //Si es multiTarget, hago que el target sea cada vez un player activo:
                target = activePlayersList[i];

                ApplyHealMove(author, target);

                //Aquí iría el vfx/sfx del movimiento (...)
                Instantiate(healMenuMove.theEffect, charStatHolder[i].transform.position, charStatHolder[i].transform.rotation);
            }

            //Le quito el mp del coste del move:
            author.currentMP -= healMenuMove.moveCost;

        }
        else//No es multiTarget:
        {
            ApplyHealMove(author, target);

            //Le quito el mp del coste del move:
            author.currentMP -= healMenuMove.moveCost;

            //Aquí iría el vfx/sfx del movimiento (...)
            Instantiate(healMenuMove.theEffect, charStatHolder[currentTargetCharSelectedInMenu].transform.position, charStatHolder[currentTargetCharSelectedInMenu].transform.rotation);
        }

        //Actualizo la UI:
        OpenHealWindow();
    }


    public void ApplyHealMove(CharStats author, CharStats target)
    {
        //Aplicar el movimiento seleccionado sobre el target y actualizar los stats y la UI. Compruebo los distintos posibles efectos del move y los voy aplicando según corresponda:
        if (healMenuMove.statMod)//Cambio de estado
        {
            //poison, sleep, cursed, blind, mute, dead
            if (healMenuMove.dead)
            {
                //Un ataque que revive sólo aplica si el target está muerto:
                if (target.currentHP <= 0)
                {
                    target.currentHP = Mathf.RoundToInt(target.maxHP * 0.2f);

                    //Le quito el mp del coste del move:
                    author.currentMP -= healMenuMove.moveCost;

                    //Aquí iría el vfx/sfx del movimiento (...)
                    Instantiate(healMenuMove.theEffect, target.transform.position, target.transform.rotation);
                }
            }
        }
        //Curación de vida:
        if (healMenuMove.hpMod && target.currentHP > 0 && target.currentHP < target.maxHP)
        {
            //Calculo la vida a curar:
            int hp = Mathf.RoundToInt(author.magicDef * healMenuMove.movePower / 100 * Random.Range(.95f, 1.05f));
            target.currentHP -= hp;

            Debug.Log($"{author.charName} cura a {target.charName} con {healMenuMove} con {hp}");
            //Compruebo que no se supere el hp máximo:
            if (target.currentHP > target.maxHP)
            {
                target.currentHP = target.maxHP;
            }
        }
        //Curación de PE:
        if (healMenuMove.mpMod && target.currentHP > 0 && target.currentMP < target.maxMP)
        {
            //Calculo la vida a curar:
            int mp = Mathf.RoundToInt(author.magicDef * healMenuMove.movePower / 100 * Random.Range(.95f, 1.05f));
            target.currentMP -= mp;

            //Compruebo que no se supere el mp máximo:
            if (target.currentMP > target.maxMP)
            {
                target.currentMP = target.maxMP;
            }
        }
    }


    //-----------------------COMÚN A TODAS LAS VENTANAS-----------------
    //Botón de los charStats:
    public void CharStatButtonPress(int i)
    {
        //Según la ventana abierta en el menú, elijo una  u otra función:
        if (windows[3].activeInHierarchy)//Heal window
        {
            Debug.Log("In heal window");

            //Si he seleccionado al char que ya tengo seleccionado o el move es multiTarget:
            if (currentTargetCharSelectedInMenu == i || healMenuMove.multiTarget)
            {
                //Si he seleccionado al que ya está seleccionado o es multitarget, habría que aplicar la cura:
                UseHealMove();
            }
            else
            {
                //Si he seleccionado a otro, simplemente cambio de target:
                currentTargetCharSelectedInMenu = i;

                //Cambiar el texto:
                healMenuText.text = $"¿Aplicar {healMenuMove.moveName} a {GameManager.instance.activePlayerStats[currentTargetCharSelectedInMenu].charName}?";

                //Desresalto a los players:
                UnhighlightCharStatsButtons();

                //Resalto al player actualmente seleccionado:
                StartCoroutine(HighlightBlinkingCo(charStatHolder[currentTargetCharSelectedInMenu].GetComponent<Image>()));
            }

            //HealMagicSelect(healMenuMove);
        }
        else if (windows[2].activeInHierarchy)//Stats window
        {
            Debug.Log("In stats window");
        }
        else if (windows[1].activeInHierarchy)//Ventana Equipo
        {
            Debug.Log("In equip window");
            EquipChar(currentCharSelectedInMenu);
        }
        else if (windows[0].activeInHierarchy)//Ventana items
        {
            Debug.Log("In items window");
            UseItem(i);
        }
        else
        {
            Debug.Log("No window active");
        }
    }

    public void ShowSkillsMenu()
    {
        GameManager.instance.choosingSkill = true;
        skillsMenu.SetActive(true);

        //Mostrar sólo las skills aprendidas so far:
        for(int i = 0; i < PlayerController.instance.transform.GetChild(0).childCount; i++)
        {
            if(i < GameManager.instance.skillsLearnt)
            {
                skillsMenu.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                skillsMenu.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void HideSkillsMenu()
    {
        GameManager.instance.choosingSkill = false;
        skillsMenu.SetActive(false);
    }

}
