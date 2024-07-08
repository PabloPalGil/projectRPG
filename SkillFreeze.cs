using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class SkillFreeze : MonoBehaviour
{
    public float lightOutRadius;

    //Color del shader Emission (glow)
    public Color lightColor;

    //Los tiles del agua congelada:
    public TileBase frozenWater;
    public TileBase frozenUp;
    public TileBase frozenDown;
    public TileBase frozenRight;
    public TileBase frozenLeft;
    public TileBase frozenUpRight;
    public TileBase frozenUpLeft;
    public TileBase frozenDownRight;
    public TileBase frozenDownLeft;
    public TileBase frozenUpUpR;
    public TileBase frozenUpUpL;
    public TileBase frozenDownDownR;
    public TileBase frozenDownDownL;

    //Referencia del tileMap (donde dibujaré el agua congelada)
    public Tilemap freezingWaterMap;

    //referencia al mapa del agua donde quitar el collider de las tiles congeladas para poder caminar por encima
    public Tilemap removeColliderWaterMap;

    //Posición del tile sobre el que está el player
    private Vector3Int freezingMainPoint;

    //Lista de posiciones en cruz bajo el player
    private List<Vector3Int> mainWaterPositionsList;

    //Posiciones de los tiles a congelar en los extremos (no caminables)
    private Vector3Int frozenPosUp;
    private Vector3Int frozenPosDown;
    private Vector3Int frozenPosLeft;
    private Vector3Int frozenPosRight;
    private Vector3Int frozenPosUpRight;
    private Vector3Int frozenPosUpLeft;
    private Vector3Int frozenPosDownRight;
    private Vector3Int frozenPosDownLeft;
    private Vector3Int frozenPosUpUpR;
    private Vector3Int frozenPosUpUpL;
    private Vector3Int frozenPosDownDownR;
    private Vector3Int frozenPosDownDownL;
    private Vector3Int frozenPosRightRightU;
    private Vector3Int frozenPosRightRightD;
    private Vector3Int frozenPosLeftLeftU;
    private Vector3Int frozenPosLeftLeftD;


    private void Start()
    {
        ReferenceTilemaps();
    }


    public void ReferenceTilemaps()
    {
        if(GameObject.FindGameObjectWithTag("FreezingWater") != null)
        {
            freezingWaterMap = GameObject.FindGameObjectWithTag("FreezingWater").GetComponent<Tilemap>();
        }
        if (GameObject.FindGameObjectWithTag("Water") != null)
        {
            removeColliderWaterMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
        }
    }

    public void SkillFreezeSFX()//Este método se llama desde la propia animación (skillsAnimator -> Trigger ("Freeze"))
    {
        StartCoroutine(FreezeSkillCo());
    }

    public IEnumerator FreezeSkillCo()
    {
        AudioManager.instance.PlaySFX(65);

        //si hay poca luz
        if (FindObjectOfType<Weather>().dayCycle != DayTime.Day)
        {
            //Activo la luz de la skill (primer child transform):
            Light2D skillLight = transform.GetChild(0).GetComponent<Light2D>();
            skillLight.pointLightOuterRadius = lightOutRadius;

            yield return new WaitForSecondsRealtime(1f);

            skillLight.pointLightOuterRadius = 0;
        }

        //Ubico la posición del tile sobre el que está el player:
        freezingMainPoint = freezingWaterMap.WorldToCell(transform.position);

        //Creo la lista de posiciones de tiles en cruz bajo el player:
        mainWaterPositionsList = new List<Vector3Int>
        {
            freezingMainPoint,
            new Vector3Int(freezingMainPoint.x + 1, freezingMainPoint.y, freezingMainPoint.z),
            new Vector3Int(freezingMainPoint.x - 1, freezingMainPoint.y, freezingMainPoint.z),
            new Vector3Int(freezingMainPoint.x, freezingMainPoint.y + 1, freezingMainPoint.z),
            new Vector3Int(freezingMainPoint.x, freezingMainPoint.y - 1, freezingMainPoint.z)
        };

        //Indico la posicion de los tiles auxiliares a congelar:
        frozenPosRight = new Vector3Int(freezingMainPoint.x + 2, freezingMainPoint.y, freezingMainPoint.z);
        frozenPosLeft = new Vector3Int(freezingMainPoint.x - 2, freezingMainPoint.y, freezingMainPoint.z);
        frozenPosUp = new Vector3Int(freezingMainPoint.x, freezingMainPoint.y + 2, freezingMainPoint.z);
        frozenPosDown = new Vector3Int(freezingMainPoint.x, freezingMainPoint.y - 2, freezingMainPoint.z);
        frozenPosUpRight = new Vector3Int(freezingMainPoint.x + 1, freezingMainPoint.y + 1, freezingMainPoint.z);
        frozenPosUpLeft = new Vector3Int(freezingMainPoint.x - 1, freezingMainPoint.y + 1, freezingMainPoint.z);
        frozenPosDownRight = new Vector3Int(freezingMainPoint.x + 1, freezingMainPoint.y - 1, freezingMainPoint.z);
        frozenPosDownLeft = new Vector3Int(freezingMainPoint.x - 1, freezingMainPoint.y - 1, freezingMainPoint.z);
        frozenPosUpUpR = new Vector3Int(freezingMainPoint.x + 1, freezingMainPoint.y + 2, freezingMainPoint.z);
        frozenPosUpUpL = new Vector3Int(freezingMainPoint.x - 1, freezingMainPoint.y + 2, freezingMainPoint.z);
        frozenPosDownDownR = new Vector3Int(freezingMainPoint.x + 1, freezingMainPoint.y - 2, freezingMainPoint.z);
        frozenPosDownDownL = new Vector3Int(freezingMainPoint.x - 1, freezingMainPoint.y - 2, freezingMainPoint.z);
        frozenPosRightRightU = new Vector3Int(freezingMainPoint.x + 2, freezingMainPoint.y + 1, freezingMainPoint.z);
        frozenPosRightRightD = new Vector3Int(freezingMainPoint.x + 2, freezingMainPoint.y - 1, freezingMainPoint.z);
        frozenPosLeftLeftU = new Vector3Int(freezingMainPoint.x - 2, freezingMainPoint.y + 1, freezingMainPoint.z);
        frozenPosLeftLeftD = new Vector3Int(freezingMainPoint.x - 2, freezingMainPoint.y - 1, freezingMainPoint.z);


        //Para cada tile principal:
        for (int i = 0; i < mainWaterPositionsList.Count; i++)
        {
            if (removeColliderWaterMap.GetTile<TileBase>(mainWaterPositionsList[i]) != null){
                //Dibujo el agua congelada sobre la que se puede andar:
                freezingWaterMap.SetTile(mainWaterPositionsList[i], frozenWater);

                //Elimino el tile (y sus colisiones) del mapa de agua inferior al agua congelada para poder caminar
                removeColliderWaterMap.SetTile(mainWaterPositionsList[i], null);

                //Sfx de agua cristalizada:
                AudioManager.instance.PlaySFX(66);
            }
        }

        //Ahora dibujo los tiles de agua congelada de los extremos (no caminables), y para ello...
        //...compruebo que el tile a dibujar es de agua (existe en el mapa de agua original) y que no se ha congelado por completo ya
        if(removeColliderWaterMap.GetTile<TileBase>(frozenPosRight) != null && freezingWaterMap.GetTile<TileBase>(frozenPosRight) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosRight, frozenRight);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosLeft) != null && freezingWaterMap.GetTile<TileBase>(frozenPosLeft) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosLeft, frozenLeft);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosUp) != null && freezingWaterMap.GetTile<TileBase>(frozenPosUp) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosUp, frozenUp);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosDown) != null && freezingWaterMap.GetTile<TileBase>(frozenPosDown) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosDown, frozenDown);
        }
        //Tiles esquineras interiores:
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosUpRight) != null && freezingWaterMap.GetTile<TileBase>(frozenPosUpRight) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosUpRight, frozenUpRight);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosUpLeft) != null && freezingWaterMap.GetTile<TileBase>(frozenPosUpLeft) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosUpLeft, frozenUpLeft);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosDownRight) != null && freezingWaterMap.GetTile<TileBase>(frozenPosDownRight) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosDownRight, frozenDownRight);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosDownLeft) != null && freezingWaterMap.GetTile<TileBase>(frozenPosDownLeft) != frozenWater)
        {
            freezingWaterMap.SetTile(frozenPosDownLeft, frozenDownLeft);
        }
        //Y finalmente las tiles esquineras exteriores, que sólo se ponen si no hay ningun otro tile ya:
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosRightRightU) != null && freezingWaterMap.GetTile<TileBase>(frozenPosRightRightU) == null)
        {
            freezingWaterMap.SetTile(frozenPosRightRightU, frozenUpUpR);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosRightRightD) != null && freezingWaterMap.GetTile<TileBase>(frozenPosRightRightD) == null)
        {
            freezingWaterMap.SetTile(frozenPosRightRightD, frozenDownDownR);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosLeftLeftU) != null && freezingWaterMap.GetTile<TileBase>(frozenPosLeftLeftU) == null)
        {
            freezingWaterMap.SetTile(frozenPosLeftLeftU, frozenUpUpL);
        }
        if (removeColliderWaterMap.GetTile<TileBase>(frozenPosLeftLeftD) != null && freezingWaterMap.GetTile<TileBase>(frozenPosLeftLeftD) == null)
        {
            freezingWaterMap.SetTile(frozenPosLeftLeftD, frozenDownDownL);
        }
    }
}
