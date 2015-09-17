using UnityEngine;
using System.Collections;
using GamepadInput;

public class SelectorMenu : MonoBehaviour {
    private PlayerData m_Player;
    private PlayerSelect m_ParentMenu;
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPlayer(PlayerData player, PlayerSelect parent)
    {
        m_Player = player;
        m_ParentMenu = parent;
    }
}
