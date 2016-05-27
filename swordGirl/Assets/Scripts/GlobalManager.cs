using UnityEngine;
using System.Collections;

public static class GlobalManager {

    public static Vector3 playerStartPos = new Vector3(-131.6f, 0, -172.7f);
    public static bool gamepad = false;

    public static void SetPlayerRestartPosition(Vector3 pos)
    {
        playerStartPos = pos;
    }

}
