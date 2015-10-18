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
    private int m_VerticalPosition = 0;
    private int m_MaxVerticalPosition = 2;
    private SelectionState m_CurrentSelectionState = SelectionState.Preselection;
    enum SelectionState { Preselection, Selection, Confirmation, Confirmed}
    private DelayManager m_DelayManager;
    [SerializeField] private int m_PanelId = -1;
    public int PanelId { get { return m_PanelId; } }

    //sponsor
    [SerializeField] private Text m_SponsorName;
    [SerializeField] private bool m_UseSponsorColor = true;
    [SerializeField] private Image m_SponsorColor;
    [SerializeField] private Image m_SponsorImage;

    //ability
    [SerializeField] private Text m_AbilityName;
    [SerializeField] private Image m_TempAbilityImage;

    [SerializeField] private GameObject[] m_selectors;

    [SerializeField] private Text m_Ledger;
    [SerializeField] private GameObject m_ConfirmedOverlay;
    [SerializeField] private Text m_ConfirmationOverlayText;

    [HideInInspector] public int m_headerPreSelection = 20;
    [HideInInspector] public int m_headerSelection = 19;
    [HideInInspector] public int m_headerConfirmation = 3;
    [HideInInspector] public int m_headerConfirmed = 5;
    [HideInInspector] public int m_ledgerSelection = 3;
    [HideInInspector] public int m_ledgerConfirmation = 18;
    [HideInInspector] public int m_ledgerConfirmedWaiting = 6;
    [HideInInspector] public int m_ledgerConfirmedCanStart = 7;
    [HideInInspector] public int m_ConfirmTextConfirmation = 8;
    [HideInInspector] public int m_ConfirmTextConfirmed = 5;
    [HideInInspector] public string m_VerticalSlide;
    [HideInInspector] public string m_PanelActivate;


    protected override void Start()
    {
        base.Start();
        m_MaxSponsorSelection = Database.instance.ListofSponsors.Count;
        m_MaxAbilitySelection = Database.instance.ListofAbilities.Count;
        m_DelayManager = GetComponent<DelayManager>();
        m_DelayManager.Reset();
        m_ConfirmedOverlay.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update () {
        if (m_DelayManager.CanShoot)
        {
            if(m_CurrentSelectionState == SelectionState.Selection || m_CurrentSelectionState == SelectionState.Confirmation)
            {
                if (m_Controls.Confirm[m_ControllerID])
                    ConfirmSelection();
            }

            if (m_Controls.Cancel[m_ControllerID])
                BackTrack();
        }

        if (m_CurrentSelectionState == SelectionState.Selection && m_DelayManager.CanShield)
        {

            if (m_VerticalPosition == 0 && m_Controls.X[m_ControllerID] > Database.instance.MenuNavigationDeadZone)
                m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, true);
            else if (m_VerticalPosition == 0 && m_Controls.X[m_ControllerID] < -Database.instance.MenuNavigationDeadZone)
                m_SponsorSelection = MoveSelection(m_SponsorSelection, m_MaxSponsorSelection, false);
            if (m_VerticalPosition == 1 && m_Controls.X[m_ControllerID] > Database.instance.MenuNavigationDeadZone)
                m_AbilitySelection = MoveSelection(m_AbilitySelection, m_MaxAbilitySelection, true);
            else if (m_VerticalPosition == 1 && m_Controls.X[m_ControllerID] < -Database.instance.MenuNavigationDeadZone)
                m_AbilitySelection = MoveSelection(m_AbilitySelection, m_MaxAbilitySelection, false);


            if (m_Controls.Y[m_ControllerID] > Database.instance.MenuNavigationDeadZone)
                m_VerticalPosition = MoveVerticalSelection(m_VerticalPosition, true);
            else if (m_Controls.Y[m_ControllerID] < -Database.instance.MenuNavigationDeadZone)
                m_VerticalPosition = MoveVerticalSelection(m_VerticalPosition, false);
        }

        if (m_CurrentSelectionState == SelectionState.Confirmed) CheckReady();
    }

    public void SetPlayer(PlayerData player, PlayerSelect parent, int controllerID)
    {
        m_MaxSponsorSelection = Database.instance.ListofSponsors.Count;
        m_MaxAbilitySelection = Database.instance.ListofAbilities.Count;
        m_DelayManager = GetComponent<DelayManager>();
        m_CurrentSelectionState = SelectionState.Preselection;
        m_Player = player;
        m_ParentMenu = parent;
        m_ControllerID = controllerID;
        DisplaySponsor(m_SponsorSelection);
        DisplayAbility(m_AbilitySelection);
        m_CurrentSelectionState =  SetStateAndTexts(true);
        SetSelector(m_VerticalPosition);
        m_DelayManager.Reset();
        SoundManager.PlaySFX(m_PanelActivate);
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

        if (m_VerticalPosition == 0){
            DisplaySponsor(selection);
        }

        if(m_VerticalPosition == 1)
        {
            DisplayAbility(selection);
        }

        m_DelayManager.AddShieldDelay(Database.instance.MenuInputDelay);

        SoundManager.PlaySFX(Database.instance.MenuSlideName);

        return selection;
    }

    private void SetSelector(int selection)
    {

        for (int i = 0; i < m_MaxVerticalPosition; i++)
        {
            if (i == selection) m_selectors[i].SetActive(true);
            else m_selectors[i].SetActive(false);
        }

    }

    private int MoveVerticalSelection(int selection, bool isUp)
    {
        if (isUp)
        {
            selection--;
            if (selection < 0) selection = m_MaxVerticalPosition - 1;
        }
        else
        {
            selection++;
            if (selection >= m_MaxVerticalPosition) selection = 0;
        }

        SetSelector(selection);

        m_DelayManager.AddShieldDelay(Database.instance.MenuInputDelay);

        SoundManager.PlaySFX(m_VerticalSlide);

        return selection;
    }

    private void DisplaySponsor(int selection)
    {
        m_SponsorName.text = Database.instance.ListofSponsors[selection].SponsorName;
        if(m_UseSponsorColor)
            m_SponsorColor.color = Database.instance.ListofSponsors[selection].SponsorColor;
        if (Database.instance.ListofSponsors[selection].SponsorImage != null)
            m_SponsorImage.sprite = Database.instance.ListofSponsors[selection].SponsorImage;

        if (Database.instance.ListofSponsors[selection].isTaken)
        {
            m_SponsorColor.color = Database.instance.TakenColor;
        }
    }

    private void DisplayAbility(int selection)
    {
        m_AbilityName.text = Database.instance.ListofAbilities[selection].AbilityName;
        m_TempAbilityImage.sprite = Database.instance.ListofAbilities[selection].mySprite;
    }

    private void ConfirmSelection()
    {
        switch (m_CurrentSelectionState)
        {
            case SelectionState.Selection:
                if (!Database.instance.ListofSponsors[m_SponsorSelection].isTaken)
                {
                    SoundManager.PlaySFX(Database.instance.MenuClickName);
                m_CurrentSelectionState = SetStateAndTexts(true);
                }
                else
                    SoundManager.PlaySFX(Database.instance.MenuErrorName);

                break;

            case SelectionState.Confirmation:
                    //sponsor
                    m_Player.PlayerSponsor = Database.instance.ListofSponsors[m_SponsorSelection];
                    Database.instance.ListofSponsors[m_SponsorSelection].TakeSponsor();

                    //ability
                    m_Player.PlayerAbility = Database.instance.ListofAbilities[m_AbilitySelection].AbilityEnum;
                    m_CurrentSelectionState = SetStateAndTexts(true);
                    m_ParentMenu.ReadyUp(true);
                    m_ConfirmedOverlay.SetActive(true);

                    SoundManager.PlaySFX(Database.instance.MenuClickName);

                    CheckReady();
                break;
        }
        m_DelayManager.AddDelay(Database.instance.MenuInputDelay);
    }

    private void BackTrack()
    {
        switch (m_CurrentSelectionState)
        {
            case SelectionState.Preselection:

                break;
            case SelectionState.Selection:
                m_Player.PlayerSponsor = null;
                m_SponsorSelection = 0;
                m_AbilitySelection = 0;
                m_Player.PlayerAbility = Abilities.None;
                m_CurrentSelectionState = SetStateAndTexts(false);
                m_ParentMenu.DeactivateScreen(m_PanelId, m_Player);
                break;
            case SelectionState.Confirmation:
                Database.instance.ListofSponsors[m_SponsorSelection].ReleaseSponsor();
                m_CurrentSelectionState = SetStateAndTexts(false);
                m_ConfirmedOverlay.SetActive(false);
                break;
            case SelectionState.Confirmed:
                m_ParentMenu.ReadyUp(false);
                m_CurrentSelectionState = SetStateAndTexts(false);
                break;
        }

        SoundManager.PlaySFX(Database.instance.MenuCancelName);
        m_DelayManager.AddDelay(Database.instance.MenuInputDelay);
    }

    private void CheckReady()
    {
        if (m_ParentMenu.ReadyCount > 1 && m_ParentMenu.ReadyCount == m_ParentMenu.PlayerCount)
        {
            m_Ledger.text = Database.instance.GameTexts[m_ledgerConfirmedCanStart];
        }
        else
            m_Ledger.text = Database.instance.GameTexts[m_ledgerConfirmedWaiting];
    }

    private SelectionState SetStateAndTexts(bool isForward) {

        //print("Old State " + m_CurrentSelectionState);
        SelectionState temp = m_CurrentSelectionState;

        if (isForward) temp++;
        else temp--;

        switch (temp)
        {
            case SelectionState.Preselection:
                m_ParentMenu.HeaderText[m_PanelId].text = Database.instance.GameTexts[m_headerPreSelection];
                break;
            case SelectionState.Selection:
                m_ParentMenu.HeaderText[m_PanelId].text = Database.instance.GameTexts[m_headerSelection];
                m_Ledger.text = Database.instance.GameTexts[m_ledgerSelection];
                break;
            case SelectionState.Confirmation:
                m_ParentMenu.HeaderText[m_PanelId].text = Database.instance.GameTexts[m_headerConfirmation];
                m_Ledger.text = Database.instance.GameTexts[m_ledgerConfirmation];
                m_ConfirmationOverlayText.text = Database.instance.GameTexts[m_ConfirmTextConfirmation];
                m_ConfirmedOverlay.SetActive(true);
                break;
            case SelectionState.Confirmed:
                m_ParentMenu.HeaderText[m_PanelId].text = Database.instance.GameTexts[m_headerConfirmed];
                m_ConfirmationOverlayText.text = Database.instance.GameTexts[m_ConfirmTextConfirmed];
                m_Ledger.text = Database.instance.GameTexts[m_ledgerConfirmedWaiting];
                break;
        }
        //print("New State " + temp);
        return temp;
    }
}
