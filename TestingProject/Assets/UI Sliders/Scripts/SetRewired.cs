using UnityEngine;
using Rewired;

public class SetRewired : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        ReInput.players.GetPlayer(0).controllers.maps.LoadMap(0, 0, "UI", "Default", true);
    }
}
