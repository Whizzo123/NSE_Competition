using System.Collections;
using UnityEngine;

public class MortalSpell : Debuff
{

    public MortalSpell() : base("Mortal Spell", "Brings an enemy back down to the mortal plane all abilities are stripped for 10 secs", 3, AbilityUseTypes.RECHARGE, 20.0f, 10.0f)
    {

    }

    public override void Use()
    {
        if (target == null)
        {
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            target = closestPlayer;
        }
        else
        {
            inUse = true;
            var request = MortalSpellEvent.Create();
            //request.Target = target.entity;
            request.End = false;
            request.Send();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, true);
            base.Use();
        }
    }

    public override void EndEffect()
    {
        inUse = false;
        var request = MortalSpellEvent.Create();
       // request.Target = target.entity;
        request.End = true;
        request.Send();
        target = null;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, false);
        base.EndEffect();
    }

    public override Ability Clone()
    {
        return new MortalSpell();
    }
}
