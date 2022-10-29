using Verse;

namespace ra2.Yuri;

public class Command_YuriAction : Command_Action
{
    public Pawn caster;

    public override void GizmoUpdateOnMouseover()
    {
        if (caster != null)
        {
            GenDraw.DrawRadiusRing(caster.Position, 7f);
        }
    }
}