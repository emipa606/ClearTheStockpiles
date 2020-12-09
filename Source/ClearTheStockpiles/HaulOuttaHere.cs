using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ClearTheStockpiles
{
	// Token: 0x02000003 RID: 3
	public static class HaulOuttaHere
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002144 File Offset: 0x00000344
		public static bool CanHaulOuttaHere(Pawn p, Thing t, out IntVec3 storeCell)
		{
			storeCell = IntVec3.Invalid;
            bool result;
			if (!(t.def.EverHaulable && !t.IsBurning() && p.CanReserveAndReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), 1, -1, null, false)))
			{
				result = false;
			}
			else
			{
				var flag3 = TryFindBetterStoreCellInRange(t, p, p.Map, CTS_Loader.settings.radiusToSearch, StoragePriority.Unstored, p.Faction, out storeCell, true);
				var flag4 = !flag3;
				result = !flag4 || TryFindSpotToPlaceHaulableCloseTo(t, p, t.PositionHeld, out storeCell);
			}
			return result;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021DC File Offset: 0x000003DC
		public static Job HaulOuttaHereJobFor(Pawn p, Thing t)
		{
            Job result;
            if (!CanHaulOuttaHere(p, t, out IntVec3 c))
			{
				JobFailReason.Is("Can't clear: No place to clear to.", null);
				result = null;
			}
			else
			{
				HaulMode haulMode = HaulMode.ToCellNonStorage;
                if (c.GetSlotGroup(p.Map) != null)
				{
					haulMode = HaulMode.ToCellStorage;
				}
				result = new Job(JobDefOf.HaulToCell, t, c)
				{
					count = 99999,
					haulOpportunisticDuplicates = false,
					haulMode = haulMode,
					ignoreDesignations = true
				};
			}
			return result;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002260 File Offset: 0x00000460
		private static bool TryFindSpotToPlaceHaulableCloseTo(Thing haulable, Pawn worker, IntVec3 center, out IntVec3 spot)
		{
			var debugMessages = new List<string>();
			Region region = center.GetRegion(worker.Map, RegionType.Set_Passable);
            bool result;
			bool currentStockpile(IntVec3 slot)
			{
				return worker.Map.haulDestinationManager.SlotGroupAt(haulable.Position).CellsList.Contains(slot);
			}
			if (region == null)
			{
				spot = center;
				result = false;
			}
			else
			{
				var traverseParms = TraverseParms.For(worker, Danger.Deadly, TraverseMode.ByPawn, false);
				IntVec3 foundCell = IntVec3.Invalid;
                RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.Allows(traverseParms, false), delegate(Region r)
				{
                    candidates.Clear();
                    candidates.AddRange(r.Cells);
                    candidates.RemoveAll(new Predicate<IntVec3>(currentStockpile));
                    candidates.Sort((IntVec3 a, IntVec3 b) => a.DistanceToSquared(center).CompareTo(b.DistanceToSquared(center)));
					for (var i = 0; i < candidates.Count; i++)
					{
                        IntVec3 intVec = candidates[i];
                        if (HaulablePlaceValidator(haulable, worker, intVec, out var item))
                        {
                            foundCell = intVec;
                            var debug = CTS_Loader.settings.debug;
                            if (debug)
                            {
                                debugMessages.Add(item);
                            }
                            if (debugMessages.Count != 0)
                            {
                                foreach (var text in debugMessages)
                                {
                                    Log.Message(text, false);
                                }
                            }
                            return true;
                        }
                        var debug2 = CTS_Loader.settings.debug;
						if (debug2)
						{
							debugMessages.Add(item);
						}
					}
                    if (debugMessages.Count != 0)
					{
						foreach (var text2 in debugMessages)
						{
                            Log.Message(text2, false);
						}
					}
					return false;
				}, cellsToSearch, RegionType.Set_Passable);
				var isValid = foundCell.IsValid;
				if (isValid)
				{
					spot = foundCell;
					result = true;
				}
				else
				{
					spot = center;
					result = false;
				}
			}
			return result;

		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000233C File Offset: 0x0000053C
		private static bool HaulablePlaceValidator(Thing haulable, Pawn worker, IntVec3 c, out string debugText)
		{
            bool result;
			if (!worker.CanReserveAndReach(c, PathEndMode.OnCell, worker.NormalMaxDanger(), 1, -1, null, false))
			{
				debugText = "Could not reserve or reach";
				result = false;
			}
			else
			{
                if (GenPlace.HaulPlaceBlockerIn(haulable, c, worker.Map, true) != null)
				{
					debugText = "Place was blocked";
					result = false;
				}
				else
				{
					SlotGroup slotGroup = c.GetSlotGroup(worker.Map);
                    if (slotGroup != null)
					{
                        if (!slotGroup.Settings.AllowedToAccept(haulable))
						{
							debugText = "Stockpile does not accept";
							return false;
						}
					}
                    if (!c.Standable(worker.Map))
					{
						debugText = "Cell not standable";
						result = false;
					}
					else
					{
                        if (c == haulable.Position && haulable.Spawned)
						{
							debugText = "Current position of thing to be hauled";
							result = false;
						}
						else
						{
                            if (c.ContainsStaticFire(worker.Map))
							{
								debugText = "Cell has fire";
								result = false;
							}
							else
							{
                                if (haulable != null && haulable.def.BlockPlanting)
								{
									Zone zone = worker.Map.zoneManager.ZoneAt(c);
                                    if (zone is Zone_Growing)
									{
										debugText = "Growing zone here";
										return false;
									}
								}
                                if (haulable.def.passability > Traversability.Standable)
								{
									for (var i = 0; i < 8; i++)
									{
										IntVec3 c2 = c + GenAdj.AdjacentCells[i];
                                        if (c2.InBounds(worker.Map))
										{
                                            if (worker.Map.designationManager.DesignationAt(c2, DesignationDefOf.Mine) != null)
											{
												debugText = "Mining designated nearby";
												return false;
											}
										}
									}
								}
								var flag13 = false;
								IntVec3[] cardinalDirectionsAndInside = GenAdj.CardinalDirectionsAndInside;
								for (var j = 0; j < cardinalDirectionsAndInside.CountAllowNull(); j++)
								{
									IntVec3 c3 = c + cardinalDirectionsAndInside[j];
                                    if (c3.InBounds(worker.Map))
									{
										Building edifice = c3.GetEdifice(worker.Map);
                                        if (edifice != null)
										{
                                            if (edifice is Building_Door)
											{
												break;
											}
                                            if (edifice is Building_WorkTable)
											{
												slotGroup = c3.GetSlotGroup(worker.Map);
                                                if (slotGroup != null)
												{
                                                    if (slotGroup.Settings.AllowedToAccept(haulable))
													{
														flag13 = true;
													}
												}
											}
										}
										else
										{
											flag13 = true;
										}
									}
								}
                                if (!flag13)
								{
									debugText = "No valid position could be found.";
									result = false;
								}
								else
								{
									Building edifice2 = c.GetEdifice(worker.Map);
                                    if (edifice2 != null)
									{
                                        if (edifice2 is Building_Trap)
										{
											debugText = "It's a trap.";
											return false;
										}
                                        if (edifice2 is Building_WorkTable)
										{
											debugText = "It's not a trap, but we still can't put something here.";
											return false;
										}
									}
									debugText = "OK";
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000266C File Offset: 0x0000086C
		public static bool TryFindBetterStoreCellInRange(Thing t, Pawn carrier, Map map, int range, StoragePriority currentPriority, Faction faction, out IntVec3 foundCell, bool needAccurateResult = true)
		{
			List<SlotGroup> allGroupsListInPriorityOrder = map.haulDestinationManager.AllGroupsListInPriorityOrder;
            bool result;
			if (allGroupsListInPriorityOrder.Count == 0)
			{
				foundCell = IntVec3.Invalid;
				result = false;
			}
			else
			{
				IntVec3 a = (t.MapHeld == null) ? carrier.PositionHeld : t.PositionHeld;
				StoragePriority storagePriority = currentPriority;
				var num = Mathf.Pow(range, 2f);
				var intVec = default(IntVec3);
				var flag2 = false;
				var count = allGroupsListInPriorityOrder.Count;
				for (var i = 0; i < count; i++)
				{
					SlotGroup slotGroup = allGroupsListInPriorityOrder[i];
					StoragePriority priority = slotGroup.Settings.Priority;
                    if (priority < storagePriority || priority <= currentPriority)
					{
						break;
					}
                    if (slotGroup.Settings.AllowedToAccept(t))
					{
						List<IntVec3> cellsList = slotGroup.CellsList;
						var count2 = cellsList.Count;
						int num2;
						if (needAccurateResult)
						{
							num2 = Mathf.FloorToInt(count2 * Rand.Range(0.005f, 0.018f));
						}
						else
						{
							num2 = 0;
						}
						for (var j = 0; j < count2; j++)
						{
							IntVec3 intVec2 = cellsList[j];
							var num3 = (float)(a - intVec2).LengthHorizontalSquared;
                            if (num3 <= num)
							{
                                if (StoreUtility.IsGoodStoreCell(intVec2, map, t, carrier, faction))
								{
									flag2 = true;
									intVec = intVec2;
									num = num3;
									storagePriority = priority;
                                    if (j >= num2)
									{
										break;
									}
								}
							}
						}
					}
				}
                if (!flag2)
				{
					foundCell = IntVec3.Invalid;
					result = false;
				}
				else
				{
					foundCell = intVec;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		private static readonly List<IntVec3> candidates = new List<IntVec3>();

		// Token: 0x04000002 RID: 2
		private const int cellsToSearch = 100;
	}
}
