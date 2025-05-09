using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NoteData
{
    public float time;// defin�ts laiks kad nots/bannans par�d�s
    public int lane;// defin;eta kollona kur� nots/bannans par�das
}

[System.Serializable]
public class NoteList
{
    public NoteData[] notes;//masivs kur� ir notis kas tiek izveidotas sp�les laik�
}

public class NoteSpawner : MonoBehaviour //m�zika un no�u par�d��an�s
{
    // Assign these in Inspector
    public GameObject notePrefab;
    public Transform[] spawnPoints; // ar �o var pievienot spawnpoint punktus no kuriem par�d�sies notis
    public TextAsset jsonFile; // ar so var pievienot json failu kur� atrodas specifiskie laiki un kollonas
    public float musicStartDelay = 2.0f; // var main�t kad s�kas m�zika 
    public float audioCalibration = 0.05f;// ja notis ir drusku neritm� tad te var kalibr�t kad s�kas audio drusku prec�z�k

    private AudioSource music;// ar �o var pievienot audio failu(dziemsa)
    private List<NoteData> notesToSpawn = new List<NoteData>();
    private bool musicStarted = false;
    private float musicStartTime = 0f;

    void Start()
    {
        music = GetComponent<AudioSource>();

        // no json faila nolasa specifiskos laikus un kollonas kur�s par�d�s notis
        if (jsonFile != null)
        {
            NoteList loadedNotes = JsonUtility.FromJson<NoteList>(jsonFile.text);
            notesToSpawn = new List<NoteData>(loadedNotes.notes);
        }
        else
        {
            Debug.LogError("No JSON file assigned!");// ja nav faila
        }

        // Validate setup
        if (spawnPoints.Length != 5)
        {
            Debug.LogError("Assign exactly 5 spawnPoints!"); // ja nav tie�i 5 punkti pievienoti (loti noderigs)
            return;
        }

        Invoke("StartMusic", musicStartDelay);
    }

    void StartMusic()// atska�o dziesmu un saglab� starta laiku
    {
        music.Play();
        musicStarted = true;
        musicStartTime = (float)AudioSettings.dspTime;
        Debug.Log($"s�k�s: {musicStartTime:F4}s"); // atz�m� kad s�kas muzika(noder t�l�kaj� funkcij�)
    }

    void Update()//ja piemeram kkas ielago vai dators lens tad sakuma laiks ir noderigs lai precizi novietot notis pat ja liels delay
    {
        if (!musicStarted || notesToSpawn.Count == 0) return;

        float currentMusicTime = (float)(AudioSettings.dspTime - musicStartTime) - audioCalibration;

        //godigi teik�u es �ito neizdom�ju
        for (int i = notesToSpawn.Count - 1; i >= 0; i--)
        {
            float timeDifference = currentMusicTime - notesToSpawn[i].time;

            if (timeDifference >= -0.01f && timeDifference <= 0.05f)//par�da kad tie�i nots par�d�s lai klaibr�tu muzikas sakumu lai vis ritm� un no�em to tad kad t� ir arpus ekr�na
            {
                SpawnNote(notesToSpawn[i].lane);
                Debug.Log($"Note spawned at: {currentMusicTime:F4}s (Target: {notesToSpawn[i].time:F4}s) " +
                         $"Lane: {notesToSpawn[i].lane}");
                notesToSpawn.RemoveAt(i);
            }
            else if (timeDifference > 0.05f)// ja netr�pija pa noti
            {
                Debug.LogWarning($"Missed note at {notesToSpawn[i].time:F4}s");
                notesToSpawn.RemoveAt(i);
            }
        }
    }

    void SpawnNote(int laneIndex)// parbauda vai json fail� joslas indekss ir pareizs
    {
        if (laneIndex < 0 || laneIndex >= 5)
        {
            Debug.LogError($"Invalid lane index: {laneIndex}");
            return;
        }
        Instantiate(notePrefab, spawnPoints[laneIndex].position, Quaternion.identity); //izveido jaunu noti �staj� josl�
    }
}