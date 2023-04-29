using UnityEngine;

public class UppingAValue : MonoBehaviour
{
    private void Update() {
        DebugVariables.FPS = (int)(1.0f / Time.smoothDeltaTime);

        if (Input.GetKeyDown(KeyCode.K))
            DebugVariables.playerPos += Vector3.one;
    }
}
