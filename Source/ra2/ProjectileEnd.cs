using Verse;

namespace ra2;

public class ProjectileEnd
{
    public Thing launcher;
    public Projectile shell;

    public ProjectileEnd(Projectile s, Thing l)
    {
        shell = s;

        launcher = l;
    }
}