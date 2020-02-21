﻿using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Celeste.Mod.SpringCollab2020.Triggers {
    [CustomEntity("SpringCollab2020/RemoveLightSourcesTrigger")]
    [Tracked]
    class RemoveLightSourcesTrigger : Trigger {
        public RemoveLightSourcesTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            level = SceneAs<Level>();
        }

        public static void Load() {
            Everest.Events.Level.OnLoadLevel += LevelLoadHandler;
            Everest.Events.Level.OnExit += OnExitHandler;
        }

        public static void Unload() {
            Everest.Events.Level.OnLoadLevel -= LevelLoadHandler;
            Everest.Events.Level.OnExit -= OnExitHandler;
        }

        private static void LevelLoadHandler(Level loadedLevel, Player.IntroTypes playerIntro, bool isFromLoader) {
            if (loadedLevel.Session.GetFlag("lightsDisabled"))
                DisableLightRender();
        }

        private static void OnExitHandler(Level exitLevel, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow) {
            EnableLightRender();
        }

        private static void BloomRendererHook(On.Celeste.BloomRenderer.orig_Apply orig, BloomRenderer self, VirtualRenderTarget target, Scene scene) { }

        private static void LightHook(On.Celeste.VertexLight.orig_Update orig, VertexLight self) {
            if (self.SceneAs<Level>().Session.GetFlag("lightsDisabled"))
                self.Alpha = 0f;
            orig(self);
        }

        private static void EnableLightRender() {
            On.Celeste.VertexLight.Update -= LightHook;
            On.Celeste.BloomRenderer.Apply -= BloomRendererHook;
        }

        private static void DisableLightRender() {
            On.Celeste.VertexLight.Update += LightHook;
            On.Celeste.BloomRenderer.Apply += BloomRendererHook;
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);

            level = SceneAs<Level>();

            if (level.Session.GetFlag("lightsDisabled") == false) {
                level.Session.SetFlag("lightsDisabled", true);
                DisableLightRender();
            }
        }

        private Level level;
    }
}
