#if PC2D_PLAYMAKER_SUPPORT

using Com.LuisPedroFonseca.ProCamera2D;
using HutongGames.PlayMaker;
using TooltipAttribute = HutongGames.PlayMaker.TooltipAttribute;
using UnityEngine;

[ActionCategory(ActionCategory.Camera)]
[Tooltip("Apply the given influences to the camera during the corresponding durations")]
public class ProCamera2DApplyInfluencesTimedAction : FsmStateAction 
{
    [RequiredField]
    [Tooltip("An array of the vectors representing the influences to be applied")]
    public FsmVector2[] Influences;

    [RequiredField]
    [Tooltip("An array of the vectors representing the influences to be applied")]
    public FsmFloat[] Durations;

    public override void Reset()
    {
        Influences = new FsmVector2[0];
        Durations = new FsmFloat[0];
    }

    public override void OnEnter() 
    {
        if (ProCamera2D.Instance != null)
        {
            var entries = Influences.GetLength(0);

            var influences = new Vector2[entries];
            for (int i = 0; i < entries; i++)
            {
                influences[i] = (Influences.GetValue(i) as FsmVector2).Value;
            }

            var durations = new float[entries];
            for (int i = 0; i < entries; i++)
            {
                durations[i] = (Durations.GetValue(i) as FsmFloat).Value;
            }

            ProCamera2D.Instance.ApplyInfluencesTimed(influences, durations);
        }

        Finish();
    }
}

#endif