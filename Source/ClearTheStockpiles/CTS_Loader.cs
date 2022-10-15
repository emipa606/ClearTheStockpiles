using Mlie;
using UnityEngine;
using Verse;

namespace ClearTheStockpiles;

internal class CTS_Loader : Mod
{
    public static CTS_Settings settings;
    private static string currentVersion;

    public CTS_Loader(ModContentPack content) : base(content)
    {
        settings = GetSettings<CTS_Settings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(
                ModLister.GetActiveModWithIdentifier("Mlie.ClearTheStockpiles"));
    }

    public override string SettingsCategory()
    {
        return Content.Name;
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var text = settings.radiusToSearch.ToString();
        var listing_Standard = new Listing_Standard
        {
            ColumnWidth = inRect.width / 3f
        };
        listing_Standard.Begin(inRect);
        listing_Standard.Label("CTS_LookRadiusLabel".Translate());
        listing_Standard.TextFieldNumeric(ref settings.radiusToSearch, ref text, 1f, 25f);
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("CTS_Debug".Translate(), ref settings.debug);
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("CTS_ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    public class CTS_Settings : ModSettings
    {
        public bool debug;

        public int radiusToSearch = 18;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref radiusToSearch, "val_RadiusToSearch", 18, true);
            Scribe_Values.Look(ref debug, "mode_debug", false, true);
        }
    }
}