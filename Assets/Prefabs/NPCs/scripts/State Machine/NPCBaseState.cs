using UnityEngine;

public abstract class NPCBaseState
{
    public abstract void EnterState(NPCStateManager manager);

    public abstract void UpdateState(NPCStateManager manager);

}
