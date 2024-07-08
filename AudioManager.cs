using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour {

    public AudioSource[] sfx;
    public AudioSource[] bgm;

    public static AudioManager instance;

    public int bgmPlayingNow;

    // Use this for initialization
    void Start () {
        instance = this;

        DontDestroyOnLoad(gameObject);
	}	

    public void PlaySFX(int soundToPlay)//Reproduce un sfx
    {
        if (soundToPlay < sfx.Length)
        {
            sfx[soundToPlay].Play();
        }
    }

    public void PlayBGM(int musicToPlay)//Reproduce un tema desde donde se quedó
    {
        if (!bgm[musicToPlay].isPlaying)
        {
            StopAllMusic();//Detiene sólo la musica que está sonando (si hay alguna pausada no la toca)

            if (musicToPlay < bgm.Length)
            {
                bgm[musicToPlay].Play();
            }
        }
    }

    public void PlayBGMSmooth(int musicToPlay)//Reproduce un tema aumentando su volumen progresivamente
    {
        if (!bgm[musicToPlay].isPlaying)
        {
            StopAllMusic();//Detiene sólo la musica que está sonando (si hay alguna pausada no la toca)

            if (musicToPlay < bgm.Length)
            {
                bgm[musicToPlay].Play();
                StartCoroutine(SmoothPlayCo(musicToPlay));
            }
        }
    }


    public IEnumerator SmoothPlayCo(int i)//Para aumentar el volumen de un tema progresivamente
    {
        float volume = bgm[i].volume;
        bgm[i].volume = 0;
        float timer = 1f;
        float timeToTake = 1f;
        while (timer >= 0)
        {
            bgm[i].volume = Mathf.SmoothStep(volume, 0, timer / timeToTake);
            timer -= Time.deltaTime;
            Debug.Log("volumen: " + bgm[i].volume);
            yield return null;
        }
    }

    public void PlayBGMWithIntro(int musicToPlay)//Reproduce un tema con intro (que se loopea luego la parte sin intro)
    {
        Debug.Log("About to Trigger bgm intro");

        if (!bgm[musicToPlay].isPlaying)
        {
            if (musicToPlay < bgm.Length)
            {
                StartCoroutine(PlayBGMWithIntroCo(musicToPlay));
            }
        }
    }

    public IEnumerator PlayBGMWithIntroCo(int musicToPlay)
    {
        Debug.Log("About to Start bgm intro");
        StopAllMusic();

        bgm[musicToPlay].Play();

        while (bgm[musicToPlay].isPlaying)
        {
            Debug.Log("Playing bgm intro");
            yield return null;
        }
        Debug.Log("Fin de Bgm-intro");
        if (!bgm[musicToPlay].isPlaying)
        {
            bgm[musicToPlay + 1].Play();
            Debug.Log("Playing bgm loop");
        }

        yield return null;
    }

    public void StopBGM(int i)//Detiene un tema bgm en concreto
    {
        bgm[i].Stop();
    }

    public void StopAllMusic()//Detiene sólo la música que está actualmente reproduciéndose
    {
        for(int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
            {
                bgm[i].Stop();
            }
        }
    }

    public void ResetMusic()//Detiene toda la música y pone su reprodcción (.time) a 0.
    {
        for(int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    public void StopSFX(int i)//Detiene un sfx concreto
    {
        sfx[i].Stop();
    }

    public void StopSounds()//Detiene todos los SFX
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            sfx[i].Stop();
        }
    }

    public void PlayRandomSFX(int soundToPlay)//Reproduce un sonido random de un grupo de posibles sfx
    {
        if (soundToPlay < sfx.Length && soundToPlay > 0)
        {
            //Variable temporal para elegir uno de los sfx a random (el Sxf audiosource tiene que tener el script SfxArray)
            int i = Random.Range(0, sfx[soundToPlay].GetComponent<SfxArray>().sfxArray.Length);
            sfx[soundToPlay].clip = sfx[soundToPlay].GetComponent<SfxArray>().sfxArray[i];
            sfx[soundToPlay].Play();
        }
    }

    public int GetBgmPlaying()//Devuelve el indice de la primera musica sonando
    {
        for(int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
            {
                bgmPlayingNow = i;
                return i;
            }
        }
        return 0;
    }

    //Función que siempre devuelve true, es para comprobar que este script ya ha cargado y está funcional desde el resto de scripts
    public bool CheckIfLoaded()
    {
        return true;
    }

}
