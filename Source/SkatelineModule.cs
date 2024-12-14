using Celeste.Mod.Skateline.Triggers;
using System;

namespace Celeste.Mod.Skateline;

public class SkatelineModule : EverestModule {
    public static SkatelineModule Instance { get; private set; }

    public override Type SettingsType => typeof(SkatelineModuleSettings);
    public static SkatelineModuleSettings Settings => (SkatelineModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(SkatelineModuleSession);
    public static SkatelineModuleSession Session => (SkatelineModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(SkatelineModuleSaveData);
    public static SkatelineModuleSaveData SaveData => (SkatelineModuleSaveData) Instance._SaveData;

    public SkatelineModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(SkatelineModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(SkatelineModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        SkateboardTrigger.Load();
    }

    public override void Unload() {
        SkateboardTrigger.Unload();
    }

    public override void LoadContent(bool firstLoad)
    {
        base.LoadContent(firstLoad);

        SkateboardTrigger.InitializeTextures();
    }
}