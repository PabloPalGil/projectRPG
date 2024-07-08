using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string[] dialogLines;
    private bool writingChars;
    private float writingCharsDelay;
    private float normalWriting = 0.025f;

    public int currentLine;
    public bool justStarted;

    public static DialogManager instance;

    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    //Blips
    private AudioSource audioSrc;
    public int blip;

    //Disyuntivas:
    [HideInInspector]
    private string originalLine;//Aquí guardo la línea original para recuperar que empiece por "c-" para elecciones reiterativas
    public bool inChoice;
    public string[] linesA;
    public string[] linesB;
    public bool eventA;
    public bool eventB;
    public string coruNameA;
    public string coruNameB;
    public DialogActivator dialog;//El DialogActivator que ha llamado a escribir su texto (var necsaria para el LevelManager en su caso)


    // Use this for initialization
    void Start () {
        instance = this;

        writingCharsDelay = 1f;
	}
	

    public void ShowDialog(string[] newLines, bool isPerson)
    {
        dialogLines = newLines;

        currentLine = 0;

        CheckIfName();

        CheckIfChoice();

        dialogText.text = "";
        dialogBox.SetActive(true);

        //justStarted = true;

        nameBox.SetActive(isPerson);

        if (!isPerson)
        {
            blip = 9;
            Debug.Log("Blip es default");
        }

        GameManager.instance.dialogActive = true;

        if (!writingChars)
        {
            //Lanzo la corrutina que me va a permitir mostrar las palabras letra a letra:
            StartCoroutine(WriteCharsCo());
        }
        else
        {
            writingChars = false;
        }
    }

    //Este ShowDialog overloaded con un DialogActivator se usa sólo cuando en el dialogo hay que tomar elecciones
    public void ShowDialog(string[] newLines, bool isPerson, DialogActivator dialogAct)
    {
        dialogLines = newLines;

        currentLine = 0;

        CheckIfName();

        CheckIfChoice();

        dialogText.text = "";
        dialogBox.SetActive(true);

        //justStarted = true;

        nameBox.SetActive(isPerson);

        if (!isPerson)
        {
            blip = 9;
            Debug.Log("Blip es default");
        }

        GameManager.instance.dialogActive = true;

        if (!writingChars)
        {
            //Lanzo la corrutina que me va a permitir mostrar las palabras letra a letra:
            StartCoroutine(WriteCharsCo());
        }
        else
        {
            writingChars = false;
        }

        //Disyuntivas:
        dialog = dialogAct;
        linesA = dialogAct.linesA;
        linesB = dialogAct.linesB;
        eventA = dialogAct.eventA;
        eventB = dialogAct.eventB;
        coruNameA = dialogAct.coruNameA;
        coruNameB = dialogAct.coruNameB;
    }


    public IEnumerator WriteCharsCo()
    {
        writingChars = true;//Indico que se están imprimiendo los chars aun (no puedo avanzar la linea)
        writingCharsDelay = normalWriting;

        audioSrc = AudioManager.instance.transform.GetChild(1).GetChild(blip).GetComponent<AudioSource>();

        //Convierto la linea actual a chars:
        char[] currentLineChars = dialogLines[currentLine].ToCharArray();
        Debug.Log("Línea a escribir en WriteCharsCo: " + dialogLines[currentLine]);
        //String del trozo de linea que llevo escrito:
        string enteredLine = "";

        for(int i = 0; i < currentLineChars.Length; i++)
        {
            if (!writingChars)//Al pulsar boton, se completa el dialogo al instante:
            {
                dialogText.text = dialogLines[currentLine];
                break;
            }
            else//Si no, se escribe char a char:
            {
                enteredLine += currentLineChars[i];
                dialogText.text = enteredLine;
                if (currentLineChars[i].ToString() != " " && currentLineChars[i].ToString() != "." && currentLineChars[i].ToString() != ",")
                {
                    AudioManager.instance.PlaySFX(blip);
                }
                yield return new WaitForSecondsRealtime(writingCharsDelay);
            }
        }

        writingCharsDelay = normalWriting;
        writingChars = false;
    }

    public void AdvanceDialog()
    {
        if (dialogBox.activeInHierarchy)
        {
            if (!writingChars)
            {
                if (inChoice)//Implementar efecto de respuesta en InChoice
                {
                    inChoice = false;
                    dialogLines[currentLine] = originalLine;//Recupero la linea original por si vuelvo a hablar luego
                    ChooseA();
                }
                else
                {
                    currentLine++;

                    if (currentLine >= dialogLines.Length)
                    {
                        dialogBox.SetActive(false);

                        GameManager.instance.dialogActive = false;

                        if (shouldMarkQuest)
                        {
                            shouldMarkQuest = false;
                            if (markQuestComplete)
                            {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            }
                            else
                            {
                                QuestManager.instance.MarkQuestIncomplete(questToMark);
                            }
                        }

                        //Añadido para activar el script pickupItem desde el botón táctil, cuando proceda:
                        if (PlayerController.instance.canPickup)
                        {
                            PlayerController.instance.interactableObject.GetComponent<PickupItem>().GrabItem();
                        }

                        //Llamar a batalla
                    }
                    else
                    {
                        CheckIfName();

                        StartCoroutine(WriteCharsCo());
                    }
                }
            }
            else
            {
                writingChars = false;
            }
        }
    }

    public void CheckIfName()
    {
        Debug.Log("CurrentLine inicial: " + dialogLines[currentLine].ToString());

        //Reemplazo de nombres:
        if (dialogLines[currentLine].Contains("P1"))
        {
            dialogLines[currentLine] = dialogLines[currentLine].Replace("P1", GameManager.instance.P1);
        }
        if (dialogLines[currentLine].Contains("P2"))
        {
            dialogLines[currentLine] = dialogLines[currentLine].Replace("P2", GameManager.instance.P2);
        }
        if (dialogLines[currentLine].Contains("P3"))
        {
            dialogLines[currentLine] = dialogLines[currentLine].Replace("P3", GameManager.instance.P3);
        }
        if (dialogLines[currentLine].Contains("P4"))
        {
            dialogLines[currentLine] = dialogLines[currentLine].Replace("P4", GameManager.instance.P4);
        }
        if (dialogLines[currentLine].Contains("P5"))
        {
            dialogLines[currentLine] = dialogLines[currentLine].Replace("P5", GameManager.instance.P5);
        }

        Debug.Log("CurrentLine tras cambio nombre: " + dialogLines[currentLine].ToString());

        //Nombre del que habla:
        if (dialogLines[currentLine].StartsWith("n-"))
        {
            nameText.text = dialogLines[currentLine].Replace("n-", "");
            CheckBlip();
            Debug.Log("CurrentLine del que habla: " + dialogLines[currentLine].ToString());

            currentLine++;
        }


        //Reemplazo de names:
        //dialogLines[currentLine] = dialogLines[currentLine].Replace("Daria", "Friend1");
        //dialogLines[currentLine] = dialogLines[currentLine].Replace("Greath", "Player1");
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete)
    {
        questToMark = questName;
        markQuestComplete = markComplete;

        shouldMarkQuest = true;
    }

    public void CheckIfChoice()
    {
        if (dialogLines[currentLine].StartsWith("c-"))//Si es una linea de elección:
        {
            originalLine = dialogLines[currentLine];//Almaceno la frase antes de modificarla para recuperarla luego
            inChoice = true;
            dialogLines[currentLine] = dialogLines[currentLine].Replace("c-", "") + "\n" + dialogLines[currentLine + 1] + "\n" + dialogLines[currentLine + 2];
            Debug.Log("Línea mod: " + dialogLines[currentLine]);
        }
        else //Si no es una línea de eleccion, inChoice es falso
        {
            inChoice = false;
        }
    }

    public void CheckBlip()
    {
        if(dialogLines[currentLine] == "n-Greath")
        {
            blip = 10;
            Debug.Log("Blip es Greath");
        }
        else if(dialogLines[currentLine] == "n-Daria")
        {
            blip = 11;
            Debug.Log("Blip es Daria");
        }
        else
        {
            blip = 9;
        }
    }

    public void ChooseA()
    {
        //Termino el diálogo con la disyuntiva de golpe
        dialogBox.SetActive(false);

        //Compruebo si lanzo la Corutina que sea al LevelManager para definir cualquier efecto desde allí como un storyEvent más:
        if (eventA)
        {
            StartCoroutine(coruNameA);
        }
        else
        {
            ShowDialog(linesA, dialog.isPerson);
        }
    }

    public void ChooseB()
    {
        if (writingChars)
        {
            return;
        }
        //Termino el diálogo con la disyuntiva de golpe
        dialogBox.SetActive(false);
        inChoice = false;
        dialogLines[currentLine] = originalLine;//Recupero la linea original por si vuelvo a hablar luego

        if (eventB)
        {
            StartCoroutine(coruNameB);
        }
        else
        {
            ShowDialog(linesB, dialog.isPerson);
        }
    }

    public IEnumerator SaveGameCo()
    {
        GameManager.instance.SaveData();
        Debug.Log("Game Saved");
        ShowDialog(linesA, false);

        yield return null;
    }

    public IEnumerator NoAnswerCo()
    {
        GameManager.instance.dialogActive = false;
        yield return null;
    }

    public IEnumerator SleepAndRestCo()
    {
        ShowDialog(linesA, true);

        GameManager.instance.storyEvent = true;
        //Fade black
        UIFade.instance.FadeToBlackTime(2);

        yield return new WaitForSecondsRealtime(0.3f);

        AudioManager.instance.StopAllMusic();
        AudioManager.instance.PlaySFX(51);

        yield return new WaitForSecondsRealtime(1);

        yield return new WaitForSecondsRealtime(0.5f);
        //Ahora curo:
        GameManager.instance.HealParty();

        yield return new WaitForSecondsRealtime(5f);
        UIFade.instance.FadeFromBlackTime(2);
        yield return new WaitForSecondsRealtime(1);

        if (GameManager.instance.dialogActive && !dialogBox.activeInHierarchy)
        {
            GameManager.instance.dialogActive = false;
        }
        GameManager.instance.storyEvent = false;
    }
}
