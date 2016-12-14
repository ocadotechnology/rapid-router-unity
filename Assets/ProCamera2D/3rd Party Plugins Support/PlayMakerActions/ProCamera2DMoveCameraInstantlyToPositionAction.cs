#if PC2D_PLAYMAKER_SUPPORT

using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;

[ActionCategory(ActionCategory.Camera)]
[Tooltip("Moves the camera instantly to the defined position")]
public class ProCamera2DMoveCameraInstantlyToPositionAction : FsmStateAction 
{
    [RequiredField]
    [Tooltip("The final position of the camera")]
    public FsmVector3 CameraPos;

    public override void OnEnter() 
    {
        if (ProCamera2D.Instance != null)
            ProCamera2D.Instance.MoveCameraInstantlyToPosition(CameraPos.Value);

        Finish();
    }
}

#endif