using UnityEngine;

public class FallingNote : MonoBehaviour
{
    public float fallSpeed = 5f;// atrums ar kadu kriit nots
    public Vector3 targetPosition;
    public bool useLerp = true;//bezjedzigas paliekas 
    public float lerpSpeed = 2f;// bezjedzigas paliekas. bet vismaz var redzet kâda ideja bija sakumâ

    
    public float destroyYPosition = -10f; //y pozicija kur noris tiek iznicinatas(arpus ekrana)

    void Update()
    {
        if (useLerp)// lerp ir veids kâ gludi pârvietot objektu no vienas vietas uz otru.bet beigas izdomaju ka vinu neizmantosu 
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                lerpSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }

        // iznicina noti kad ta ir arpus ekrana
        if (transform.position.y < destroyYPosition)
        {
            Destroy(gameObject);
            Debug.Log("Missed note destroyed!");
        }
    }
}