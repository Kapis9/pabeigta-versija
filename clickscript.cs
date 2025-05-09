using UnityEngine;

public class HitZone : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode key; // var uzlikt ar kâdâm kontrolçm strâdâ hitbox. manâ gadijumâ J,G,H
    public float inputBufferTime = 0.1f; // ja pa agru uzpieþ tad arî ir ok
    public float debounceTime = 0.03f; // maziòð delay starp pogu spieðanas lai nevar visulaiku spamot vinas

    [Header("Hit Detection")]// eksperimentâla funkcija kas parâda cik precîzi trâpîts pa noti.(râda tikai konsolç)
    public float perfectRange = 0.05f;
    public float goodRange = 0.1f;
    public float hitRadius = 0.3f;

    private float lastKeyPressTime = -1f;
    private bool keyHeld = false;// parbauda vai taustins tiek turçts lai nesanâk tâ ka tikai vari turçt visus taustiòus un visu trâpît 
    private float[] inputBuffer = new float[3];// salglabâti laiki kad taustiòi ir nospiesti

    void Update()
    {
        HandleInput();
        Debug.DrawRay(transform.position, Vector2.down * hitRadius, Color.red);// vizualizç trâpijumu(laikam nestrâdâ nez kpe)
    }

    void HandleInput()
    {
        // nospiesta poga
        if (Input.GetKey(key) && !keyHeld)
        {
            if (Time.time - lastKeyPressTime > debounceTime)
            {
                keyHeld = true;
                lastKeyPressTime = Time.time;
                BufferInput();
                CheckNoteHit();
            }
        }
        // poga atlaista
        else if (!Input.GetKey(key))
        {
            keyHeld = false;
        }

        CheckBufferedHits();
    }

    void BufferInput()
    {
        // saglabâ nospiesto pogu laikus ar mazu +- lai nevajadzetu parak precizi trâpît
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            if (inputBuffer[i] <= Time.time)
            {
                inputBuffer[i] = Time.time + inputBufferTime;
                break;
            }
        }
    }

    void CheckBufferedHits()//ja nots ir trâpita iekavçjuma laikâ(tas +- par ko gaja runa augða redzamâ funkcijâ)
    {
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            if (Time.time <= inputBuffer[i])
            {
                CheckNoteHit();
                inputBuffer[i] = 0f;
                break;
            }
        }
    }

    void CheckNoteHit()// parbauda vai nots ir virs hitbox un vai tas hitbox ir aktivizçts(tad kad nospieþ taustiòus)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        bool hitRegistered = false;

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.CompareTag("Note"))
            {
                float distance = Mathf.Abs(hit.transform.position.y - transform.position.y);
                hitRegistered = true;

                if (distance <= perfectRange)
                {
                    Debug.Log("Perfect!");
                    Destroy(hit.gameObject);
                    return; 
                }
                else if (distance <= goodRange)
                {
                    Debug.Log("Good!");
                    Destroy(hit.gameObject);
                    return;
                }
            }
        }

        if (!hitRegistered)// ja netrapa pa noti 
        {
            Debug.Log("Miss!");
        }
    }
}