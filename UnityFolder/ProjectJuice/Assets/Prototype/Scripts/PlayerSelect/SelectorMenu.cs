using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour {
    private PlayerData m_Player;
    private PlayerSelect m_ParentMenu;
    private MenuControls m_Controls;
    private int m_ControllerID;
    private int m_SponsorSelection = 0;
    private int m_MaxSponsorSelection = 0;
    private int m_AbilitySelection = 0;
    private int m_MaxAbilitySelection = 0;
    private SelectionState m_CurrentState = SelectionState.Preselection;
    enum SelectionState { Preselection, Sponsor, Ability, Confirmed}
    private DelayManager m_DelayManager;
    [SerializeField] private float m_NavigationDelay = 0.3f;
    [SerializeField] private Text m_SelectionName;
    [SerializeField] Image tempImages;
    [SerializeField] Color[] tempColors;

    void Start()
    {
        m_Controls = FindObjectOfType<MenuControls>();
        m_MaxSponsorSelection = System.Enum.GetValues(typeof(Sponsors)).Length;
        m_MaxAbilitySelection = System.Enum.GetValues(typeof(Abilities)).Length;
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
    }
	
	// Update is called once per frame
	void Update () {
        if(m_CurrentState == SelectionState.Sponsor && m_DelayManager.m_CanShoot)
        {
            if(m_Controls.X[m_ControllerID] > 0)
                m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, true);
            else if(m_Controls.X[m_ControllerID] < 0)
                m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, false);
        }
	}

    public void SetPlayer(PlayerData player, PlayerSelect parent, int controllerID)
    {
        m_Player = player;
        m_ParentMenu = parent;
        m_CurrentState = SelectionState.Sponsor;
        m_ControllerID = controllerID;
        tempImages.color = tempColors[m_SponsorSelection];
    }

    private int MoveSelection(int selection, int max, bool isRight)
    {
        if (isRight)
        {
            selection++;
            if (selection == max) selection = 0;
        }
        else
        {
            selection--;
            if (selection < 0) selection = max - 1;
        }

        tempImages.color = tempColors[selection];

        m_DelayManager.AddDelay(m_NavigationDelay);

        return selection;
    }

    private void ConfirmSelection()
    {

    }
}
