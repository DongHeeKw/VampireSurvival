using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
        {
            return;
        }

        Vector3 playerPos = GameManagerNew.instance.player.transform.position;
        Vector3 myPos = transform.position;
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDor = GameManagerNew.instance.player.inputVec;
        float dirX = playerDor.x < 0 ? -1 : 1;
        float dirY = playerDor.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 41);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 41);
                }
                break;
            case "Enemy":
                break;

        }
    }
}
