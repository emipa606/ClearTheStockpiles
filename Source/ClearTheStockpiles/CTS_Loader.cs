using UnityEngine;
using Verse;

namespace ClearTheStockpiles
{
    // Token: 0x02000004 RID: 4
    internal class CTS_Loader : Mod
    {
        // Token: 0x04000003 RID: 3
        public static CTS_Settings settings;

        // Token: 0x0600000B RID: 11 RVA: 0x00002841 File Offset: 0x00000A41
        public CTS_Loader(ModContentPack content) : base(content)
        {
            settings = GetSettings<CTS_Settings>();
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002857 File Offset: 0x00000A57
        public override string SettingsCategory()
        {
            return Content.Name;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002864 File Offset: 0x00000A64
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
            listing_Standard.End();
        }

        // Token: 0x02000006 RID: 6
        public class CTS_Settings : ModSettings
        {
            // Token: 0x0400000B RID: 11
            public bool debug;

            // Token: 0x0400000A RID: 10
            public int radiusToSearch = 18;

            // Token: 0x06000013 RID: 19 RVA: 0x00002B39 File Offset: 0x00000D39
            public override void ExposeData()
            {
                Scribe_Values.Look(ref radiusToSearch, "val_RadiusToSearch", 18, true);
                Scribe_Values.Look(ref debug, "mode_debug", false, true);
            }
        }
    }
}