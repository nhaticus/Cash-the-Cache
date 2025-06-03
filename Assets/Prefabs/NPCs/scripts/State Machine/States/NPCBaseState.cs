using UnityEngine;

public class NPCBaseState
{
    protected GenericNPC npc;
    protected NPCStateManager manager;

    public NPCBaseState(GenericNPC _npc, NPCStateManager _manager)
    {
        npc = _npc;
        manager = _manager;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }

}
