using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Celeste.Mod.Skateline.Triggers
{
    [CustomEntity("Skateline/SkateboardTrigger")]
    [Tracked]
    class SkateboardTrigger : Trigger
    {
        #region static
        private static Vector2 PlayerSpriteOffset = new Vector2(0, -3);
        private static MTexture SkateboardSprite;
        private static ILHook OrigUpdateSpriteHook;
        public static void InitializeTextures()
        {
            SkateboardSprite = GFX.Game["objects/Skateline/skateboard"];
            Logger.Log("Skateline", "loaded");
        }

        public static void Load()
        {
            On.Celeste.Player.Render += Player_Render;
            On.Celeste.PlayerHair.Render += PlayerHair_Render;
            OrigUpdateSpriteHook = new ILHook(
                typeof(Player).GetMethod("orig_UpdateSprite", BindingFlags.NonPublic | BindingFlags.Instance),
                Player_origUpdateSprite);
        }

        public static void Unload()
        {
            On.Celeste.Player.Render -= Player_Render;
            On.Celeste.PlayerHair.Render -= PlayerHair_Render;
            OrigUpdateSpriteHook.Dispose();
        }

        private static void Player_origUpdateSprite(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After, instr => (instr.MatchLdstr("runSlow") ||
                     instr.MatchLdstr("runFast")) &&
                     instr.Next.Next.Next.MatchCallvirt<Monocle.Sprite>("Play")))
            {
                cursor.EmitDelegate<Func<String, String>>((orig) => {
                    return SkatelineModule.Session.SkateboardEnabled ? "idle" : orig;
                });
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdstr("runSlow_carry") &&
                     instr.Next.Next.Next.MatchCallvirt<Monocle.Sprite>("Play")))
            {
                cursor.EmitDelegate<Func<String, String>>((orig) => {
                    return SkatelineModule.Session.SkateboardEnabled ? "idle_carry" : orig;
                });
            }
        }

        private static void Player_Render(On.Celeste.Player.orig_Render orig, Player self)
        {
            if (SkatelineModule.Session.SkateboardEnabled)
            {
                self.Sprite.RenderPosition += PlayerSpriteOffset;
            }
            orig(self);
            if (SkatelineModule.Session.SkateboardEnabled)
            {
                self.Sprite.RenderPosition -= PlayerSpriteOffset;
                SkateboardSprite.Draw(
                    self.Sprite.RenderPosition + new Vector2(self.Facing == Facings.Left ? 8 : -8, -4),
                    Vector2.Zero, Color.White, new Vector2(self.Facing == Facings.Left ? -1 : 1, 1)
                );
            }
        }

        static void PlayerHair_Render(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self)
        {
            if (SkatelineModule.Session.SkateboardEnabled)
            {
                for (int i = 0; i < self.Nodes.Count; i++)
                {
                    self.Nodes[i] += PlayerSpriteOffset;
                }
            }
            orig(self);
            if (SkatelineModule.Session.SkateboardEnabled)
            {
                for (int i = 0; i < self.Nodes.Count; i++)
                {
                    self.Nodes[i] -= PlayerSpriteOffset;
                }
            }
        }
        #endregion

        #region instance
        enum TriggerMode
        {
            Enable,
            Disable,
            Toggle
        }
        private TriggerMode triggerMode;

        public SkateboardTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            triggerMode = data.Enum<TriggerMode>("mode", TriggerMode.Enable);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            SkatelineModule.Session.SkateboardEnabled = triggerMode switch
            {
                TriggerMode.Enable => true,
                TriggerMode.Disable => false,
                TriggerMode.Toggle => !SkatelineModule.Session.SkateboardEnabled,

                _ => SkatelineModule.Session.SkateboardEnabled
            };
        }
    }
    #endregion
}
