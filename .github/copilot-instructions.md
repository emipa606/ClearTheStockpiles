# GitHub Copilot Instructions for RimWorld Mod: Clear The Stockpiles (Continued)

## Mod Overview and Purpose

**Clear The Stockpiles (Continued)** is an update to MarvinKosh's original mod, designed to enhance stockpile management in the RimWorld game. The mod introduces a new WorkGiver that prioritizes clearing stockpiles of blockages caused by excess products from work tables. This ensures smoother logistics and more efficient management of resources within the colony.

## Key Features and Systems

### WorkGiver: ClearStockpile
- **Behavior**: The WorkGiver checks if an item belongs in a stockpile. If not, it searches for a valid stockpile within a specified radius (1-25). If no valid stockpile is found or is full, the item is dumped in a safe location away from doorways, traps, and other obstacles.
- **Priority**: The clearing routine has a higher priority than hauling corpses but lower than burying and stripping corpses.
- **Configuration**: Users can adjust the search radius for stockpiles and enable a debug mode for troubleshooting.

### Settings
- **Search Radius**: Configurable between 1 and 25.
- **Debug Mode**: Toggle for troubleshooting.

## Coding Patterns and Conventions

### C# Code Structure
- **CTS_Loader.cs**: Contains the main loader for the mod, including settings management and UI components.
- **WorkGiver_ClearStockpile.cs**: Implements the logic for the ClearStockpile WorkGiver, including job generation and potential work items.
- **HaulOuttaHere.cs**: Provides utility methods for hauling items, such as finding better storage cells and validating haulable places.

### Harmony Patching
- The mod uses Harmony patching to integrate with the RimWorld game logic. Harmony patches are typically applied in the `DoSettingsWindowContents` method to modify or extend game functionality.

### XML Integration
- **WorkGivers_ClearStockpiles.xml**: Contains XML definitions for the WorkGiver, including its name, description, and other metadata.

## XML Integration

The mod includes an XML file named `WorkGivers_ClearStockpiles.xml` which defines the WorkGiver. This file is located in the `TranslationTemplate\WorkGiverDef` directory. Here is a brief overview of its structure:

xml
<WorkGiverDef>
    <defName>ClearStockpile</defName>
    <label>Clear Stockpile</label>
    <description>A WorkGiver that clears stockpiles of blockages.</description>
    <workType>Hauling</workType>
    <prioritized>true</prioritized>
    <priority>85</priority>
    <targetingPriority>85</targetingPriority>
    <workGiverClass>WorkGiver_ClearStockpile</workGiverClass>
    <workType>Clearing</workType>
    <workGiverClass>WorkGiver_ClearStockpile</workGiverClass>
</WorkGiverDef>


### Harmony Patching

Harmony patching is used to extend or modify the game's behavior. Here is an example of how a Harmony patch might be implemented in the mod:

csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace ClearTheStockpiles
{
    [HarmonyPatch(typeof(JobGiver_HaulToCell))]
    [HarmonyPatch("TryFindCellNear")]
    public static class Patch_JobGiver_HaulToCell_TryFindCellNear
    {
        public static void Postfix(ref IntVec3 __result, Pawn pawn, Thing t, bool needStorage)
        {
            if (t is Corpse)
            {
                __result = HaulOuttaHere.HaulOuttaHereJobFor(t, pawn);
            }
        }
    }
}


## Suggestions for Copilot

### General Suggestions
- **Follow Code Conventions**: Use consistent naming conventions and structure for C# files and classes.
- **Document Code**: Add comments to explain complex logic and ensure ease of maintenance.
- **Test Thoroughly**: After implementing changes, thoroughly test the mod to ensure it behaves as expected.
- **Error Reporting**: Use the provided Discord channel for error reporting and support.

### C# Code Suggestions
- **Encapsulate Logic**: Encapsulate complex logic within methods to improve readability and maintainability.
- **Use Harmony Patches judiciously**: Ensure that Harmony patches do not interfere with the game's core functionality.

### XML Suggestions
- **Validate XML**: Ensure XML definitions are correctly formatted and conform to the game's XML schema.
- **Update XML Definitions**: Keep XML definitions up-to-date with changes in the game's API.

By following these guidelines, you can contribute effectively to the Clear The Stockpiles (Continued) mod and ensure that it remains a valuable tool for RimWorld players.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

