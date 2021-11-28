using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : ScriptableObject
{
    public virtual IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        yield return null;
    }
}
