using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TormentKnowledgeMenuItem : KnowledgeMenuItem
{
    protected override void ShowBoughtIndicator()
    {
        BoughtIndicator.gameObject.SetActive(true);
        IndicatorPalette.Palette = Upgrade.ChoiceValue;
        BoughtIndicator.sprite = Controller.TormentPowersSprites[Upgrade.ChoiceValue - 1];
    }

    protected override void ChangeActiveStatus()
    {
        Controller.SetUpgradeChoiceValue(Upgrade, Mathf.Max(1, (Upgrade.ChoiceValue + 1) % (Upgrade.NumChoices + 1)));
        IndicatorPalette.Palette = Upgrade.ChoiceValue;
        BoughtIndicator.sprite = Controller.TormentPowersSprites[Upgrade.ChoiceValue - 1];
        Select();
    }

    protected override void Buy()
    {
        Controller.BuyUpgrade(Upgrade);
        BoughtIndicator.gameObject.SetActive(true);
        IndicatorPalette.Palette = Controller.SetUpgradeChoiceValue(Upgrade, Upgrade.ChoiceValue);
        BoughtIndicator.sprite = Controller.TormentPowersSprites[Upgrade.ChoiceValue - 1];
        Select();
    }

    public override void Select()
    {
        base.Select();
        Controller.Description.text =
            (Upgrade.ChoiceValue == 1 ? Upgrade.Description.ToColoredString(1) : Upgrade.Description) + "\n==== OR ====\n" +
            (Upgrade.ChoiceValue == 2 ? Upgrade.AltDescription.ToColoredString(2) : Upgrade.AltDescription);
    }
}