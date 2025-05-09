using UnityEngine;

public class HitZone : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode key; // var uzlikt ar k�d�m kontrol�m str�d� hitbox. man� gadijum� J,G,H
    public float inputBufferTime = 0.1f; // ja pa agru uzpie� tad ar� ir ok
    public float debounceTime = 0.03f; // mazi�� delay starp pogu spie�anas lai nevar visulaiku spamot vinas

    [Header("Hit Detection")]// eksperiment�la funkcija kas par�da cik prec�zi tr�p�ts pa noti.(r�da tikai konsol�)
    public float perfectRange = 0.05f;
    public float goodRange = 0.1f;
    public float hitRadius = 0.3f;

    private float lastKeyPressTime = -1f;
    private bool keyHeld = false;// parbauda vai taustins tiek tur�ts lai nesan�k t� ka tikai vari tur�t visus tausti�us un visu tr�p�t 
    private float[] inputBuffer = new float[3];// salglab�ti laiki kad tausti�i ir nospiesti

    void Update()
    {
        HandleInput();
        Debug.DrawRay(transform.position, Vector2.down * hitRadius, Color.red);// vizualiz� tr�pijumu(laikam nestr�d� nez kpe)
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
        // saglab� nospiesto pogu laikus ar mazu +- lai nevajadzetu parak precizi tr�p�t
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            if (inputBuffer[i] <= Time.time)
            {
                inputBuffer[i] = Time.time + inputBufferTime;
                break;
            }
        }
    }

    void CheckBufferedHits()//ja nots ir tr�pita iekav�juma laik�(tas +- par ko gaja runa aug�a redzam� funkcij�)
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

    void CheckNoteHit()// parbauda vai nots ir virs hitbox un vai tas hitbox ir aktiviz�ts(tad kad nospie� tausti�us)
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