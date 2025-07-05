using Mlie;
using UnityEngine;
using Verse;

namespace ClearTheStockpiles;

internal class CTS_Loader : Mod
{
    public static CTS_Settings Settings;
    private static string currentVersion;

    public CTS_Loader(ModContentPack content) : base(content)
    {
        Settings = GetSettings<CTS_Settings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return Content.Name;
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var text = Settings.RadiusToSearch.ToString();
        var listingStandard = new Listing_Standard
        {
            ColumnWidth = inRect.width / 3f
        };
        listingStandard.Begin(inRect);
        listingStandard.Label("CTS_LookRadiusLabel".Translate());
        listingStandard.TextFieldNumeric(ref Settings.RadiusToSearch, ref text, 1f, 25f);
        listingStandard.Gap();
        listingStandard.CheckboxLabeled("CTS_Debug".Translate(), ref Settings.Debug);
        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("CTS_ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    public class CTS_Settings : ModSettings
    {
        public bool Debug;

        public int RadiusToSearch = 18;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RadiusToSearch, "val_RadiusToSearch", 18, true);
            Scribe_Values.Look(ref Debug, "mode_debug", false, true);
        }
    }
}