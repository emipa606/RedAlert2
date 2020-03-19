using System;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ra2.Yuri
{
    public class Command_YuriAction:Command_Action
    {
        public Pawn caster;
        public override void GizmoUpdateOnMouseover()
        {
                 if(this.caster!=null)
                    GenDraw.DrawRadiusRing(this.caster.Position,7f);
                
            

        }

    }
}
