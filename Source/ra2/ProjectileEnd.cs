using Verse;

namespace ra2;

public class ProjectileEnd(Projectile s, Thing l)
{
    public readonly Thing launcher = l;
    public readonly Projectile shell = s;
}