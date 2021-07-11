using UnityEngine;
using Verse;

namespace ra2
{
    public class SovietNuclearFallen : ThingWithComps
    {
        public Vector3 ExactPosition = Vector3.zero;

        public IntVec3 targetCell;

        //  public SovietNuclearFallen(IntVec3 target) {
        //  this.targetCell = target;
        //this.ExactPosition = new Vector3(target.x,10,this.Map.Size.z);
        // }
        /*
        public override Vector3 DrawPos
        {
            get
            {
                return this.ExactPosition;
            }
        }

        
    */
        public override void Draw()
        {
            //base.Draw();
            base.DrawAt(DrawPos, true);
        }

        public override void Tick()
        {
            base.Tick();
            //  if (this.ExactPosition == Vector3.zero) { this.ExactPosition= new Vector3(targetCell.x, 10, this.Map.Size.z); }

            //  this.ExactPosition -= new Vector3(0,0,1);
            Position -= new IntVec3(0, 0, 1);
            if (Position.z > targetCell.z)
            {
                return;
            }

            var bomb = (SovietNuclearStrike) ThingMaker.MakeThing(ThingDef.Named("SovietNuclearStrike"));
            bomb.landPos = targetCell;
            GenSpawn.Spawn(bomb, targetCell, Map);
            Destroy();

            /*
            if (this.ExactPosition.z == targetCell.z) {
                SovietNuclearStrike bomb= (SovietNuclearStrike)ThingMaker.MakeThing(ThingDef.Named("SovietNuclearStrike"));
                GenSpawn.Spawn(bomb,this.targetCell,this.Map);
                this.Destroy(DestroyMode.Vanish);
            }
            */
        }
    }
}