using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour {

    public Transform target;

    public Tilemap theMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;

    //Shake:
    private Vector2 shakeOffset;

    //Centrar camara
    private float diferencia;//El ancho (o alto) de más que le saca la cámara al tileMap target.

	// Use this for initialization
	void Start () {
        target = FindObjectOfType<PlayerController>().transform;
        Debug.Log("Player pos: " + target.transform.position);
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        Debug.Log("Original map pos: " + theMap.transform.position);
        theMap.CompressBounds();
        Debug.Log("Compressed map pos: " + theMap.transform.position);
        Debug.Log("theMap.localBounds.min: " + theMap.localBounds.min);
        Debug.Log("theMap.localBounds.max: " + theMap.localBounds.max);
        bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);
        Debug.Log("bottomLeftLimit: " + bottomLeftLimit);
        Debug.Log("topRightLimit: " + topRightLimit);
        //Debug.Log(halfWidth * 2f  + " > " + (theMap.localBounds.max.x - theMap.localBounds.min.x));

        //Pero si el map es más pequeño que la cámara, no quiero que se mueva, quiero que se quede centrado:
        //keep the camera at the center if the tileMap target is menos ancho que la cámara (para que no dé saltos):
        if (halfWidth * 2f >= (theMap.localBounds.max.x - theMap.localBounds.min.x)) //Si mi cámara es más ancha que el mapa, la quiero centrada en el eje X
        {
            diferencia = halfWidth * 2f - (theMap.localBounds.max.x - theMap.localBounds.min.x);
            //Debug.Log("Cam es mas ancha que mapa: " + halfWidth * 2f + " > " + theMap.localBounds.max.x + " - " +  theMap.localBounds.min.x);
            Debug.Log("diferencia X: " + diferencia);
            bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth - diferencia * 0.5f, halfHeight, 0f);
            topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth + diferencia * 0.5f, -halfHeight, 0f);

        }
        if (halfHeight * 2f >= (theMap.localBounds.max.y - theMap.localBounds.min.y))//Si es más alta que el mapa, la quiero centrada en Y
        {
            diferencia = halfWidth * 2f - (theMap.localBounds.max.y - theMap.localBounds.min.y);
            Debug.Log("diferencia Y: " + diferencia);
            bottomLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight - diferencia * 0.5f, 0f);
            topRightLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight + diferencia * 0.5f, 0f);
        }

        PlayerController.instance.SetBounds(theMap.localBounds.min, theMap.localBounds.max);
        Debug.Log("PlayerBounds: " + theMap.localBounds.min + ", " + theMap.localBounds.max);
    }
	
	// LateUpdate is called once per frame after Update
	void LateUpdate () {

        //Movimiento normal:
        transform.position = new Vector3(target.position.x + shakeOffset.x, target.position.y + shakeOffset.y, transform.position.z);


        //keep the camera inside the bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);


	}

    public IEnumerator ShakeCo (float magnitude)
    {
        Debug.Log("Start shaking");
        float m = magnitude;
        while(m > 0)
        {
            shakeOffset = new Vector2(m, m) * Random.insideUnitCircle;
            m -= 1 * Time.deltaTime;
            yield return null;
        }

        shakeOffset = new Vector2 (0, 0);
        Debug.Log("End of shake");
    }
}
