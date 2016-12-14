#if PC2D_PLAYMAKER_SUPPORT

using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine;

[ActionCategory(ActionCategory.Camera)]
[Tooltip("Shakes the camera position along its horizontal and vertical axes with the values setup in the editor")]
public class ProCamera2DShakeAction : FsmStateAction 
{
    [RequiredField]
    [Tooltip("The camera with the ProCamera2D component, most probably the MainCamera")]
    public FsmGameObject MainCamera;

    public override void OnEnter() 
    {
        var shake = MainCamera.Value.GetComponent<ProCamera2DShake>();

        if (shake == null)
            Debug.LogError("The ProCamera2D component needs to have the Shake plugin enabled.");

        if (ProCamera2D.Instance != null && shake != null)
            shake.Shake();

        Finish();
    }
}

#endif