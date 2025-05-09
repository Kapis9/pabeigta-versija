using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NoteData
{
    public float time;// definçts laiks kad nots/bannans parâdâs
    public int lane;// defin;eta kollona kurâ nots/bannans parâdas
}

[System.Serializable]
public class NoteList
{
    public NoteData[] notes;//masivs kurâ ir notis kas tiek izveidotas spçles laikâ
}

public class NoteSpawner : MonoBehaviour //mûzika un noðu parâdîðanâs
{
    // Assign these in Inspector
    public GameObject notePrefab;
    public Transform[] spawnPoints; // ar ðo var pievienot spawnpoint punktus no kuriem parâdîsies notis
    public TextAsset jsonFile; // ar so var pievienot json failu kurâ atrodas specifiskie laiki un kollonas
    public float musicStartDelay = 2.0f; // var mainît kad sâkas mûzika 
    public float audioCalibration = 0.05f;// ja notis ir drusku neritmâ tad te var kalibrçt kad sâkas audio drusku precîzâk

    private AudioSource music;// ar ðo var pievienot audio failu(dziemsa)
    private List<NoteData> notesToSpawn = new List<NoteData>();
    private bool musicStarted = false;
    private float musicStartTime = 0f;

    void Start()
    {
        music = GetComponent<AudioSource>();

        // no json faila nolasa specifiskos laikus un kollonas kurâs parâdâs notis
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
            Debug.LogError("Assign exactly 5 spawnPoints!"); // ja nav tieði 5 punkti pievienoti (loti noderigs)
            return;
        }

        Invoke("StartMusic", musicStartDelay);
    }

    void StartMusic()// atskaòo dziesmu un saglabâ starta laiku
    {
        music.Play();
        musicStarted = true;
        musicStartTime = (float)AudioSettings.dspTime;
        Debug.Log($"sâkâs: {musicStartTime:F4}s"); // atzîmç kad sâkas muzika(noder tâlâkajâ funkcijâ)
    }

    void Update()//ja piemeram kkas ielago vai dators lens tad sakuma laiks ir noderigs lai precizi novietot notis pat ja liels delay
    {
        if (!musicStarted || notesToSpawn.Count == 0) return;

        float currentMusicTime = (float)(AudioSettings.dspTime - musicStartTime) - audioCalibration;

        //godigi teikðu es ðito neizdomâju
        for (int i = notesToSpawn.Count - 1; i >= 0; i--)
        {
            float timeDifference = currentMusicTime - notesToSpawn[i].time;

            if (timeDifference >= -0.01f && timeDifference <= 0.05f)//parâda kad tieði nots parâdâs lai klaibrçtu muzikas sakumu lai vis ritmâ un noòem to tad kad tâ ir arpus ekrâna
            {
                SpawnNote(notesToSpawn[i].lane);
                Debug.Log($"Note spawned at: {currentMusicTime:F4}s (Target: {notesToSpawn[i].time:F4}s) " +
                         $"Lane: {notesToSpawn[i].lane}");
                notesToSpawn.RemoveAt(i);
            }
            else if (timeDifference > 0.05f)// ja netrâpija pa noti
            {
                Debug.LogWarning($"Missed note at {notesToSpawn[i].time:F4}s");
                notesToSpawn.RemoveAt(i);
            }
        }
    }

    void SpawnNote(int laneIndex)// parbauda vai json failâ joslas indekss ir pareizs
    {
        if (laneIndex < 0 || laneIndex >= 5)
        {
            Debug.LogError($"Invalid lane index: {laneIndex}");
            return;
        }
        Instantiate(notePrefab, spawnPoints[laneIndex].position, Quaternion.identity); //izveido jaunu noti îstajâ joslâ
    }
}