using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ClearTheStockpiles
{
	// Token: 0x02000002 RID: 2
	public class WorkGiver_ClearStockpile : WorkGiver_Haul
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn Pawn)
		{
			List<Thing> list = Pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling();
			var list2 = new List<Thing>();
			foreach (Thing thing in list)
			{
                if (thing.IsInAnyStorage() && !thing.IsInValidStorage())
				{
					list2.Add(thing);
				}
			}
			return list2;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020E0 File Offset: 0x000002E0
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling().Count == 0;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000210C File Offset: 0x0000030C
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced)
		{
            Job result;
			if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced))
			{
				result = null;
			}
			else
			{
				result = HaulOuttaHere.HaulOuttaHereJobFor(pawn, t);
			}
			return result;
		}
	}
}
