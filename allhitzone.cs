using UnityEngine;

public class HitZoneController : MonoBehaviour
{
    public float moveSpeed = 10f; 
    public int currentCenterLane = 2; // spçle sâkas centrâ
    public Transform[] hitZones; // te var pievienot hitboxus kurus kontrolç spçlçtâjs

    void Update()
    {
        //pakustçties pa kreisi
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentCenterLane > 1)
        {
            currentCenterLane--;
            UpdateHitZonePositions();
        }
        // pakusteteis pa labi
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentCenterLane < 3)
        {
            currentCenterLane++;
            UpdateHitZonePositions();
        }
    }

    void UpdateHitZonePositions()
    {
        // pozîcijas kurâs var atrasties 3 hitboxi
        hitZones[0].position = GetLanePosition(currentCenterLane - 1); // pa kreisi
        hitZones[1].position = GetLanePosition(currentCenterLane);     // centrâ
        hitZones[2].position = GetLanePosition(currentCenterLane + 1); // pa labi
    }

    Vector3 GetLanePosition(int lane)
    {
        // formula kas parâda skriptam kurâs no 5 rindâm jâatrodas hitboxiem
        return new Vector3(-4f + 2f * lane, hitZones[0].position.y, 0f);
    }
}