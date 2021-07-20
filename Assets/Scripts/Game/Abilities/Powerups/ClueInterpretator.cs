using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ClueInterpretator : Powerup
{

    private List<GameObject> particles;
    private GameObject particle;

    //public ClueInterpretator() : base("Clue Interpretator", "Gain insight to where your nearby artefacts works", 5, AbilityUseTypes.RECHARGE, 20.0f, 10.0f )
    //{
    //    particles = new List<GameObject>();
    //}

    //public override void Use()
    //{
    //    if (particles == null)
    //        particles = new List<GameObject>();
    //    foreach (ArtefactBehaviour artefact in GameObject.FindObjectsOfType<ArtefactBehaviour>())
    //    {
    //        switch (artefact.GetRarity())
    //        {
    //            case ArtefactRarity.Common:
    //                particles.Add(GameObject.Instantiate(Resources.Load("Effects/CommonParticle", typeof(GameObject))) as GameObject);
    //                break;
    //            case ArtefactRarity.Exotic:
    //                particles.Add(GameObject.Instantiate(Resources.Load("Effects/ExoticParticle", typeof(GameObject))) as GameObject);
    //                break;
    //            case ArtefactRarity.Rare:
    //                particles.Add(GameObject.Instantiate(Resources.Load("Effects/RareParticle", typeof(GameObject))) as GameObject);
    //                break;
    //        }
    //        particles[particles.Count - 1].transform.position = artefact.transform.position;
    //    }
    //    inUse = true;
    //}

    protected override void EndEffect()
    {
        //Do end effect
        List<GameObject> temp = particles;
        foreach (GameObject gameObject in temp)
        {
            GameObject.Destroy(gameObject);
        }
        GameObject.Destroy(particle);
        //-------------
        base.EndEffect();
    }

    private ArtefactBehaviour FindRarestArtefact()
    {
        float maxDistance = 20.0f;
        ArtefactBehaviour rarestArtefact = null;
        ArtefactRarity highestRaritySoFar = ArtefactRarity.Common;
        foreach (ArtefactBehaviour artefact in GameObject.FindObjectsOfType<ArtefactBehaviour>())
        {
            if (Vector3.Distance(GetPlayerToEmpower().transform.position, artefact.transform.position) <= maxDistance)
            {
                if (artefact.GetRarity() >= highestRaritySoFar)
                {
                    highestRaritySoFar = artefact.GetComponent<ArtefactBehaviour>().GetRarity();
                    rarestArtefact = artefact;
                }
            }
        }
        if(rarestArtefact != null)
        {
            return rarestArtefact;
        }
        else
        {
            return null;
        }
    }

    //public override Ability Clone()
    //{
    //    return new ClueInterpretator();
    //}
}
