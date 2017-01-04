#if PC2D_PLAYMAKER_SUPPORT

using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

[ActionCategory(ActionCategory.Camera)]
[Tooltip("Remove a target from the camera")]
public class ProCamera2DRemoveCameraTargetAction : FsmStateAction 
{
    [RequiredField]
    [Tooltip("The Transform of the target")]
    public FsmGameObject target;

    [Tooltip("The time it takes for this target to reach a zero influence. Use for a more progressive transition.")]
    public FsmFloat duration = 0;

    public override void OnEnter() 
    {
        if (ProCamera2D.Instance != null && target.Value)
            ProCamera2D.Instance.RemoveCameraTarget(target.Value.transform, duration.Value);

        Finish();
    }
}

#endif