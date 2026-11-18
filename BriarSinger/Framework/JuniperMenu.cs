using BriarSinger.Core;
using BriarSinger.Spells;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using static BriarSinger.Framework.HarpSword;

namespace BriarSinger.Framework;

internal class JuniperMenu : IClickableMenu
{
    private const int WindowWidth = 800;
    private const int WindowHeight = 600;
    private const int TabHeight = 64;
    private const int TabWidth = 220;

    private readonly List<JuniperTab> pages;
    private readonly List<ClickableComponent> tabButtons = new();
    private int currentTab;

    public JuniperMenu()
        : base((Game1.uiViewport.Width - WindowWidth) / 2,
            (Game1.uiViewport.Height - WindowHeight) / 2,
            WindowWidth, WindowHeight, true)
    {
        //Tab content draw area
        var content = new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + 32,
            WindowWidth - 64, WindowHeight - 64);

        pages = new List<JuniperTab> { new OfferingsTab(content), new UpgradesTab(content) };

        //One button per tab
        for (int i = 0; i < pages.Count; i++)
        {
            tabButtons.Add(new ClickableComponent(
                new Rectangle(xPositionOnScreen + 16 + i * (TabWidth + 8),
                yPositionOnScreen - TabHeight, TabWidth, TabHeight),
                pages[i].Title));
        }
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        pages[currentTab].PerformHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound); //close button

        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (!tabButtons[i].containsPoint(x, y)) continue;

            if (i != currentTab)
            {
                currentTab = i;
                Game1.playSound("smallSelect");
            }
            return;
        }
        pages[currentTab].ReceiveLeftClick(x, y);
    }

    public override bool overrideSnappyMenuCursorMovementBan()
    {
        return true;
    }

    public override void draw(SpriteBatch b)
    {
        //Window then tabs
        IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen,
            WindowWidth, WindowHeight, Color.White);

        for (int i = 0; i < tabButtons.Count; i++)
        {
            bool selected = i == currentTab;
            Rectangle r = tabButtons[i].bounds;

            //tab will extend into the top border
            int height = selected ? r.Height + 12 : r.Height;
            IClickableMenu.drawTextureBox(b, r.X, r.Y, r.Width, height, selected ? Color.White : Color.Gray);

            Vector2 size = Game1.smallFont.MeasureString(tabButtons[i].name);
            Utility.drawTextWithShadow(b, tabButtons[i].name, Game1.smallFont,
                new Vector2(r.X + (r.Width - size.X) / 2f, r.Y + (r.Height - size.Y) / 2f),
                Game1.textColor);
        }

        pages[currentTab].Draw(b);

        string hover = pages[currentTab].HoverText;
        if (!string.IsNullOrEmpty(hover))
            drawHoverText(b, hover, Game1.smallFont);

        base.draw(b); //close button
        drawMouse(b);
    }

    internal abstract class JuniperTab
    {
        internal string Title { get; }
        public string HoverText { get; protected set; } = "";
        protected Rectangle Area { get; }

        protected JuniperTab(string title, Rectangle area)
        {
            Title = title;
            Area = area;
        }

        public abstract void Draw(SpriteBatch b);
        public virtual void ReceiveLeftClick(int x, int y) { }
        public virtual void PerformHover(int x, int y) { HoverText = ""; }
    }

    internal class OfferingsTab : JuniperTab
    {
        public OfferingsTab(Rectangle area) : base("Offerings", area) { } //TODO i18n

        public override void Draw(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, "Put offerings here!", Game1.smallFont, //TODO add the offerings
                new Vector2(Area.X, Area.Y), Game1.textColor);
        }
    }

    internal class UpgradesTab : JuniperTab
    {
        private readonly Rectangle harpSlot, fuseSlot, infuseButton;
        private readonly List<(HarpUpgrade Upgrade, Rectangle Bounds)> upgradeIcons = new();
        private readonly InventoryMenu inventory;

        private MeleeWeapon harp;
        private Item fuse;

        public UpgradesTab(Rectangle area) : base("Weapon Upgrades", area) //TODO i18n
        {
            //Left: two slots and button
            harpSlot = new Rectangle(area.X + 16, area.Y + 40, 80, 80);
            fuseSlot = new Rectangle(area.X + 16, area.Y + 150, 80, 80);
            infuseButton = new Rectangle(area.X + 120, area.Y + 100, 140, 56);

            //Right: icons for upgrades
            int perRow = 6;
            for (int i = 0; i < HarpUpgrades.All.Count; i++)
            {
                upgradeIcons.Add((HarpUpgrades.All[i],
                    new Rectangle(area.X + 320 + (i % perRow) * 80, area.Y + 40 + (i / perRow) * 80, 64, 64)));
            }
            //bottom: inventory
            inventory = new InventoryMenu(area.X, area.Bottom - 3 * 64 - 8, true, null, item => item is MeleeWeapon);
        }

        //Upgrade that fuse slot gives
        private HarpUpgrade Match => HarpUpgrades.FindFor(fuse);

        private bool CanInfuse => harp != null && Match != null
            && !HarpUpgrades.Has(Game1.player, Match.Id)
            && Game1.player.Items.Contains(harp) && Game1.player.Items.Contains(fuse);

        public override void ReceiveLeftClick(int x, int y)
        {
            //click a weapon to send it to the slot
            if (inventory.getItemAt(x, y) is MeleeWeapon w)
            {
                if (w.ItemId == HarpSword.WeaponId) harp = w;
                else fuse = w;
                Game1.playSound("smallSelect");
                return;
            }
            //click a slot to clear it
            if (harpSlot.Contains(x, y)) { harp = null; return; }
            if (fuseSlot.Contains(x, y)) { fuse = null; return; }

            if (infuseButton.Contains(x, y))
                TryInfuse();
        }

        private void TryInfuse()
        {
            if (!CanInfuse) { Game1.playSound("cancel"); return; }

            HarpUpgrades.Apply(Game1.player, Match.Id);
            Game1.player.removeItemFromInventory(fuse); //destroy fused weapon
            fuse = null;
            Game1.playSound("powerup");
        }

        public override void PerformHover(int x, int y)
        {
            HoverText = "";
            foreach (var (upgrade, bounds) in upgradeIcons)
            {
                if (!bounds.Contains(x, y)) continue;

                bool owned = harp != null && HarpUpgrades.Has(Game1.player, upgrade.Id);
                string fuseName = ItemRegistry.GetDataOrErrorItem(upgrade.FuseItemId).DisplayName;

                HoverText = owned
                    ? $"{upgrade.Name} (applied)\n{upgrade.Description}"
                    : $"{upgrade.Name}\n{upgrade.Description}\nFuse with: {fuseName}"; //TODO i18n
            }
        }


        public override void Draw(SpriteBatch b)
        {
            DrawSlot(b, harpSlot, harp, "HarpSword");
            DrawSlot(b, fuseSlot, fuse, "Fuse Weapon");

            //upgrade icons will dim by default and light up when fuse weapon matches
            var match = Match;
            float pulse = 0.5f + 0.5f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0);

            foreach (var (upgrade, bounds) in upgradeIcons)
            {
                bool owned = harp != null && HarpUpgrades.Has(Game1.player, upgrade.Id);
                bool lit = match == upgrade && !owned;

                if (match != null && HarpUpgrades.Has(Game1.player, match.Id))
                    Utility.drawTextWithShadow(b, "Already applied", Game1.smallFont,
                        new Vector2(infuseButton.X, infuseButton.Bottom + 8), Color.Red); //TODO i18n

                if (lit)
                {
                    Rectangle glow = bounds;
                    glow.Inflate(6, 6);
                    b.Draw(Game1.staminaRect, glow, Color.Gold * (0.35f + 0.35f * pulse));
                }

                Texture2D tex = upgrade.Icon ?? Game1.mouseCursors; //placeholder TODO
                Rectangle src = upgrade.Icon != null
                    ? new Rectangle(0, 0, upgrade.Icon.Width, upgrade.Icon.Height)
                    : new Rectangle(0, 0, 16, 16);

                b.Draw(tex, bounds, src, (lit || owned) ? Color.White : Color.DimGray);
            }

            inventory.draw(b);
            DrawButton(b, infuseButton, "Infuse", CanInfuse); //TODO i18n
        }
        private static void DrawSlot(SpriteBatch b, Rectangle slot, Item item, string label)
        {
            Utility.drawTextWithShadow(b, label, Game1.smallFont,
                new Vector2(slot.X, slot.Y - 28), Game1.textColor);
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                slot.X, slot.Y, slot.Width, slot.Height, Color.White);
            item?.drawInMenu(b, new Vector2(slot.Y + 8), 1f);
        }
        private static void DrawButton(SpriteBatch b, Rectangle r, string text, bool enabled)
        {
            IClickableMenu.drawTextureBox(b, r.X, r.Y, r.Width, r.Height, enabled ? Color.White : Color.Gray);
            Vector2 size = Game1.smallFont.MeasureString(text);
            Utility.drawTextWithShadow(b, text, Game1.smallFont,
                new Vector2(r.X + (r.Width - size.X) / 2f, r.Y + (r.Height - size.Y) / 2f),
                enabled ? Game1.textColor : Game1.textColor * 0.5f);
        }
    }
}

