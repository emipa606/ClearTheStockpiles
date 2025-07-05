using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ClearTheStockpiles;

public class WorkGiver_ClearStockpile : WorkGiver_Haul
{
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        var list = pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling();
        var list2 = new List<Thing>();
        foreach (var thing in list)
        {
            if (thing.IsInAnyStorage() && !thing.IsInValidStorage())
            {
                list2.Add(thing);
            }
        }

        return list2;
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling().Count == 0;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var result = !HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced)
            ? null
            : HaulOuttaHere.HaulOuttaHereJobFor(pawn, t);

        return result;
    }
}