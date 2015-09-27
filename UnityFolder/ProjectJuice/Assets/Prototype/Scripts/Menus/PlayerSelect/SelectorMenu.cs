using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

public class SelectorMenu : Menu {
    private PlayerData m_Player;
    private PlayerSelect m_ParentMenu;

    private int m_ControllerID;
    private int m_SponsorSelection = 0;
    private int m_MaxSponsorSelection = 0;
    private int m_AbilitySelection = 0;
    private int m_MaxAbilitySelection = 0;
    private SelectionState m_CurrentSelectionState = SelectionState.Preselection;
    enum SelectionState { Preselection, Sponsor, Ability, Confirmed}
    private DelayManager m_DelayManager;
    [SerializeField] private int m_PanelId = -1;
    public int PanelId { get { return m_PanelId; } }
    [SerializeField] private float m_NavigationDelay = 0.3f;
    [SerializeField] private Text m_SelectionName;
    [SerializeField] private Text m_Ledger;
    [SerializeField] private Image m_ConfirmedOverlay;
    [SerializeField] Image tempImages;
    [SerializeField] Image tempAbilityImage;

    protected override void Start()
    {
        base.Start();
        m_MaxSponsorSelection = Database.instance.ListofSponsors.Count;
        m_MaxAbilitySelection = Database.instance.ListofAbilities.Count;
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
        m_ConfirmedOverlay.enabled = false;
    }

    // Update is called once per frame
    protected override void Update () {
        if (m_DelayManager.m_CanShoot)
        {
            if (m_CurrentSelectionState == SelectionState.Sponsor)
            {
                if (m_Controls.X[m_ControllerID] > 0)
                    m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, true);
                else if (m_Controls.X[m_ControllerID] < 0)
                    m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, false);
            }

            if (m_CurrentSelectionState == SelectionState.Ability)
            {
                if (m_Controls.X[m_ControllerID] > 0)
                    m_AbilitySelection = MoveSelection(m_AbilitySelection, m_MaxAbilitySelection, true);
                else if (m_Controls.X[m_ControllerID] < 0)
                    m_AbilitySelection = MoveSelection(m_AbilitySelection, m_MaxAbilitySelection, false);
            }

            if (m_CurrentSelectionState != SelectionState.Confirmed && m_CurrentSelectionState != SelectionState.Preselection)
            {
                if (m_Controls.Confirm[m_ControllerID])
                    ConfirmSelection();
            }

            if (m_Controls.Cancel[m_ControllerID])
                BackTrack();
        }

        if (m_CurrentSelectionState == SelectionState.Confirmed) CheckReady();
    }

    public void SetPlayer(PlayerData player, PlayerSelect parent, int controllerID)
    {
        m_Player = player;
        m_ParentMenu = parent;
        m_CurrentSelectionState = SelectionState.Sponsor;
        m_ControllerID = controllerID;
        DisplaySponsor(m_SponsorSelection);
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

        if (m_CurrentSelectionState == SelectionState.Sponsor){
            DisplaySponsor(selection);
        }

        if(m_CurrentSelectionState == SelectionState.Ability)
        {
            m_SelectionName.text = Database.instance.ListofAbilities[selection].AbilityName;
        }

        m_DelayManager.AddDelay(m_NavigationDelay);

        return selection;
    }

    private void DisplaySponsor(int selection)
    {
        tempImages.color = Database.instance.ListofSponsors[selection].SponsorColor;

        m_SelectionName.text = Database.instance.ListofSponsors[selection].SponsorName;
        m_SelectionName.color = Database.instance.NormalTextColor;

        if (Database.instance.ListofSponsors[selection].isTaken)
        {
            tempImages.color = Database.instance.TakenColor;
            m_SelectionName.color = Database.instance.TakenColor;
        }
    }

    private void ConfirmSelection()
    {

        if (m_CurrentSelectionState == SelectionState.Ability)
        {
            m_Player.PlayerAbility = Database.instance.ListofAbilities[m_AbilitySelection].AbilityEnum;
            m_CurrentSelectionState = SetStateAndTexts(5, m_AbilitySelection, 3, true);
            m_ParentMenu.ReadyUp(true);
            m_ConfirmedOverlay.enabled = true;
            CheckReady();
        }
        else if (m_CurrentSelectionState == SelectionState.Sponsor)
        {
            if (!Database.instance.ListofSponsors[m_SponsorSelection].isTaken)
            {
                m_Player.PlayerSponsor = Database.instance.ListofSponsors[m_SponsorSelection];
                Database.instance.ListofSponsors[m_SponsorSelection].TakeSponsor();
                m_CurrentSelectionState = SetStateAndTexts(2, m_SponsorSelection, 3, true);
            }
        }
    }

    private void BackTrack()
    {
        switch (m_CurrentSelectionState)
        {
            case SelectionState.Preselection:

                break;
            case SelectionState.Sponsor:
                m_Player.PlayerSponsor = null;
                m_SponsorSelection = 0;
                m_CurrentSelectionState = SetStateAndTexts(0, 0, 3, false);
                m_ParentMenu.DeactivateScreen(m_PanelId, m_Player);
                break;
            case SelectionState.Ability:
                m_Player.PlayerAbility = Abilities.None;
                m_AbilitySelection = 0;
                Database.instance.ListofSponsors[m_SponsorSelection].ReleaseSponsor();
                m_CurrentSelectionState = SetStateAndTexts(1, m_SponsorSelection, 3, false);
                break;
            case SelectionState.Confirmed:
                m_ParentMenu.ReadyUp(false);
                m_CurrentSelectionState = SetStateAndTexts(2, m_AbilitySelection, 3, false);
                m_ConfirmedOverlay.enabled = false;
                break;
        }
    }

    private void CheckReady()
    {
        if (m_ParentMenu.ReadyCount > 1 && m_ParentMenu.ReadyCount == m_ParentMenu.PlayerCount)
        {
            m_Ledger.text = Database.instance.GameTexts[7];
        }
        else
            m_Ledger.text = Database.instance.GameTexts[6];
    }

    private SelectionState SetStateAndTexts(int header, int selection, int ledger, bool isForward)
    {
        m_ParentMenu.HeaderText[m_PanelId].text = Database.instance.GameTexts[header];

        SelectionState temp = m_CurrentSelectionState;

        if (isForward) temp++;
        else temp--;

        switch (temp)
        {
            case SelectionState.Sponsor:
                m_SelectionName.text = Database.instance.ListofSponsors[selection].SponsorName;
                break;
            case SelectionState.Ability:
                m_SelectionName.text = Database.instance.ListofAbilities[m_AbilitySelection].AbilityName;
                break;
        }

        m_Ledger.text = Database.instance.GameTexts[ledger];

        return temp;
    }
}
