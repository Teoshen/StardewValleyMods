using BriarSinger;
using BriarSinger.Core;
using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;


namespace BriarSinger.Profession;


    internal class BaseProfession : SpaceCore.Skills.Skill.Profession
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public BaseProfession(SpaceCore.Skills.Skill skill, string theId)
            : base(skill, theId) { }

        public override string GetName() => this.Name;
        public override string GetDescription() => this.Description;
    }



