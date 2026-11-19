using BriarSinger.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Framework
{
    internal class Buildings
    {       
        public const string JuniperBuilder = "Id";

        //building strings
        public const string Id = "Teoshen.BriarSinger_Juniper";
        public const string OpenAction = "Teoshen.BriarSinger_OpenJuniper";
        public const int StageCount = 4; //TODO make this a thing

        public static string StageTexture(int stage) => $"BriarSinger/JuniperStage{stage}";
        private static string SkinId(int stage) => $"Stage{stage}";

        public static void EditBuildingData(IDictionary<string, BuildingData> data)             

        {
            var skins = new List<BuildingSkin>();
            for (int stage = 2; stage <= StageCount; stage++)
            {
                skins.Add(new BuildingSkin
                {
                    Id = SkinId(stage),
                    Name = "Juniper", //TODO i18n
                    Description = "A tree that represents your connection to the forest.", //TODO i18n
                    Texture = StageTexture(stage),
                    Condition = "FALSE"
                });
            }

            var actionTiles = new List<BuildingActionTile>(); //screw it just make the whole tree interactable
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    actionTiles.Add(new BuildingActionTile
                    {
                        Id = $"Open_{x}_{y}",
                        Tile = new Point(x, y),
                        Action = OpenAction
                    });

        data[Id] = new BuildingData
                {
                    Name = "Juniper-Moss tree",
                    Description = "A tree that represents your connection to the forest. Friendly spirits may be attracted to it.",
                    Texture = "BriarSinger/juniperstage1",
                    SourceRect = new Rectangle(0, 0, 48, 96),
                    Size = new Point(3, 3),
                    Builder = "Id",
                    BuildCost = 0,
                    BuildDays = 0,
                    BuildCondition = "!BUILDINGS_CONSTRUCTED Here {Id}",
                    DefaultAction = "OpenAction",
                  //  Skins = skins, //TODO add this back in when the skins are fixed and there
                    MagicalConstruction = true,
                 };
        }

        public static void TryLoadTexture(AssetRequestedEventArgs e)
        {
            for (int stage = 1; stage <= StageCount; stage++)
            {
                if (e.NameWithoutLocale.IsEquivalentTo(StageTexture(stage)))
                {
                    e.LoadFromModFile<Texture2D>($"assets/juniperstage{stage}.png",
                        AssetLoadPriority.Medium);
                    return;
                }
            }
        }

        public static int GetStage(Building building)
        {
            for (int stage = 2; stage <= StageCount; stage++)
                if (building.skinId.Value == SkinId(stage)) return stage;
            return 1;
        }
        
        public static void SetStage(Building building, int stage)
        {
            stage = Math.Clamp(stage, 1, StageCount);
            building.skinId.Value = stage == 1 ? null : SkinId(stage);
            building.resetTexture();
        }
    }
}
