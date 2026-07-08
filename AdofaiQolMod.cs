using System;
using System.Reflection;
using ADOFAI;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !DEBUG
using System.Runtime.CompilerServices;
#endif

namespace AdofaiQolMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class AdofaiQolMod : BaseUnityPlugin
{
    private const string PLACEHOLDER = "...";

    #region Config - General

    [Obsolete("Unused cause i'm too lazy to implement proper layout mode")]
    public enum LDMLevels
    {
        None,
        LowDetail,
        Layout,
    }

    private ConfigEntry<bool> hidePerfect = null!;
    private ConfigEntry<bool> ldmLevel = null!;
#if DEBUG
    private ConfigEntry<bool> overrideAllowDebug = null!;
#endif

    public bool HidePerfect
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => hidePerfect.Value;
    }

    public bool LDMLevel
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => ldmLevel.Value;
    }

#if DEBUG
    public bool OverrideAllowDebug => overrideAllowDebug.Value;
#endif

    #endregion

    #region Config - ProgressDisplay

    private ConfigEntry<string> accuracyFormat = null!;
    private ConfigEntry<string> xAccuracyFormat = null!;
    private ConfigEntry<string> percentageFormat = null!;

    private ConfigEntry<TextAnchor> anchor = null!;
    private ConfigEntry<int> verticalOffset = null!;
    private ConfigEntry<float> horizontalOffset = null!;
    private ConfigEntry<float> spacing = null!;
    private ConfigEntry<float> fontSize = null!;

    public string AccuracyFormat
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => accuracyFormat.Value;
    }

    public string XAccuracyFormat
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => xAccuracyFormat.Value;
    }

    public string PercentageFormat
    {
#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => percentageFormat.Value;
    }

    public TextAnchor Anchor => anchor.Value;
    public int VerticalOffset => verticalOffset.Value;
    public float HorizontalOffset => horizontalOffset.Value;
    public float Spacing => spacing.Value;
    public float FontSize => fontSize.Value;

    #endregion

    #region State

    private GameObject modCanvas = null!;

    #endregion

    #region State - ProgressDisplay

    private GameObject? progressDisplay;

    private TextMeshProUGUI? accuracy;
    private TextMeshProUGUI? xAccuracy;
    private TextMeshProUGUI? percentage;

    #endregion


    public static AdofaiQolMod Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake()
    {
        const string SECTION_GENERAL = "General";
        const string SECTION_PROGRESS_DISPLAY = "ProgressDisplay";

        Logger = base.Logger;
        Instance = this;

        hidePerfect = Config.Bind(
            SECTION_GENERAL,
            nameof(HidePerfect),
            true,
            "Hides the \"Perfect\" score"
        );
        ldmLevel = Config.Bind(
            SECTION_GENERAL,
            nameof(LDMLevel),
            false,
            "Disables some effects and filters for better performance and visibility"
        );
#if DEBUG
        overrideAllowDebug = Config.Bind(
            SECTION_GENERAL,
            nameof(OverrideAllowDebug),
            true,
            "Force-enables debug features"
        );
#endif

        accuracyFormat = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(AccuracyFormat),
            "A: {0}",
            "How to format the accuracy display"
        );
        xAccuracyFormat = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(XAccuracyFormat),
            "X-A: {0}",
            "How to format the x-accuracy display"
        );
        percentageFormat = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(PercentageFormat),
            "P: {0}",
            "How to format the progress display"
        );
        accuracyFormat.SettingChanged += LayoutSettingChanged;
        xAccuracyFormat.SettingChanged += LayoutSettingChanged;
        percentageFormat.SettingChanged += LayoutSettingChanged;
        anchor = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(Anchor),
            TextAnchor.UpperLeft,
            "Where to anchor the progress display"
        );
        verticalOffset = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(VerticalOffset),
            0,
            "Y offset of the progress display"
        );
        horizontalOffset = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(HorizontalOffset),
            10f,
            "X offset of the progress display"
        );
        spacing = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(Spacing),
            10f,
            "The space between values"
        );
        fontSize = Config.Bind(
            SECTION_PROGRESS_DISPLAY,
            nameof(FontSize),
            72f,
            "The font size of the progress display"
        );
        anchor.SettingChanged += LayoutSettingChanged;
        verticalOffset.SettingChanged += LayoutSettingChanged;
        horizontalOffset.SettingChanged += LayoutSettingChanged;
        spacing.SettingChanged += LayoutSettingChanged;
        fontSize.SettingChanged += LayoutSettingChanged;

        modCanvas = new GameObject($"{MyPluginInfo.PLUGIN_GUID}-Canvas");
        modCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        DontDestroyOnLoad(modCanvas);

        RebuildProgressDisplay();
        UpdateProgressDisplay(false);

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger.LogDebug("Patching...");
        Harmony.PatchAll();
        Logger.LogDebug("Finished patching!");

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
#if DEBUG
        Logger.LogWarning(
            "This is a debug build of the mod. Performance may be negatively impacted and logs may be spammed."
        );
#endif
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void RebuildProgressDisplay()
    {
#if DEBUG
        Logger.LogDebug(
            $">> {nameof(RebuildProgressDisplay)}() {nameof(modCanvas)}:{modCanvas} {nameof(progressDisplay)}:{progressDisplay} "
        );
#endif
        var show = progressDisplay != null && progressDisplay.activeSelf;

        Destroy(progressDisplay);
        progressDisplay = new GameObject(nameof(progressDisplay), typeof(RectTransform));
        var transform = (RectTransform)progressDisplay.transform;
        transform.pivot = transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.SetParent(modCanvas.transform, false);
        transform.anchoredPosition = Vector2.zero;
        transform.offsetMin = new Vector2(HorizontalOffset, 0f);
        transform.offsetMax = new Vector2(0f, -VerticalOffset);
        var layout = progressDisplay.AddComponent<VerticalLayoutGroup>();
        layout.spacing = Spacing;
        layout.childForceExpandHeight = false;
        layout.childAlignment = Anchor;
        if (!string.IsNullOrWhiteSpace(AccuracyFormat))
        {
            var accuracyObject = new GameObject(nameof(accuracy), typeof(RectTransform));
            ((RectTransform)accuracyObject.transform).SetParent(transform, false);
            accuracy = accuracyObject.AddComponent<TextMeshProUGUI>();
            accuracy.fontSize = FontSize;
        }
        else
        {
            accuracy = null;
        }

        if (!string.IsNullOrWhiteSpace(XAccuracyFormat))
        {
            var xAccuracyObject = new GameObject(nameof(xAccuracy), typeof(RectTransform));
            ((RectTransform)xAccuracyObject.transform).SetParent(transform, false);
            xAccuracy = xAccuracyObject.AddComponent<TextMeshProUGUI>();
            xAccuracy.fontSize = FontSize;
        }
        else
        {
            xAccuracy = null;
        }

        if (!string.IsNullOrWhiteSpace(PercentageFormat))
        {
            var percentageObject = new GameObject(nameof(percentage), typeof(RectTransform));
            ((RectTransform)percentageObject.transform).SetParent(transform, false);
            percentage = percentageObject.AddComponent<TextMeshProUGUI>();
            percentage.fontSize = FontSize;
        }
        else
        {
            percentage = null;
        }

        UpdateProgressDisplay(show);
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void UpdateProgressDisplay(
        string accuracyValue,
        string xAccuracyValue,
        string percentageValue
    )
    {
        const string INVALID = "<i>Invalid format string</i>";
#if DEBUG
        Logger.LogDebug(
            $">> {nameof(UpdateProgressDisplay)}({nameof(accuracyValue)}: {accuracyValue}, {nameof(xAccuracyValue)}: {xAccuracyValue}, {nameof(percentageValue)}: {percentageValue}) {nameof(accuracy)}:{accuracy} {nameof(xAccuracy)}:{xAccuracy} {nameof(percentage)}:{percentage}"
        );
#endif
        if (accuracy != null)
            try
            {
                accuracy.text = string.Format(AccuracyFormat, accuracyValue);
            }
            catch (FormatException)
            {
                accuracy.text = INVALID;
            }

        if (xAccuracy != null)
            try
            {
                xAccuracy.text = string.Format(XAccuracyFormat, xAccuracyValue);
            }
            catch (FormatException)
            {
                xAccuracy.text = INVALID;
            }

        if (percentage != null)
            try
            {
                percentage.text = string.Format(PercentageFormat, percentageValue);
            }
            catch (FormatException)
            {
                percentage.text = INVALID;
            }
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void UpdateProgressDisplay(bool show = true)
    {
#if DEBUG
        Logger.LogDebug(
            $">> {nameof(UpdateProgressDisplay)}({nameof(show)}: {show}) {nameof(progressDisplay)}:{progressDisplay}"
        );
#endif
        progressDisplay?.gameObject.SetActive(show);

        if (!show)
            return;

        if (scrController.instance)
            UpdateProgressDisplay(scrController.instance);
        else
            UpdateProgressDisplay(PLACEHOLDER, PLACEHOLDER, PLACEHOLDER);
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private void UpdateProgressDisplay(scrController __instance)
    {
#if DEBUG
        Logger.LogDebug($">> {nameof(UpdateProgressDisplay)}({nameof(__instance)}: {__instance})");
#endif

        if (!__instance.gameworld)
            UpdateProgressDisplay(false);
        else
            UpdateProgressDisplay(
                float.IsNaN(__instance.mistakesManager.percentAcc)
                    ? PLACEHOLDER
                    : $"{__instance.mistakesManager.percentAcc * 100f:0.##}%",
                float.IsNaN(__instance.mistakesManager.percentXAcc)
                    ? PLACEHOLDER
                    : $"{__instance.mistakesManager.percentXAcc * 100f:0.##}%",
                float.IsNaN(__instance.percentComplete)
                    ? PLACEHOLDER
                    : $"{__instance.percentComplete * 100f:0.##}%"
            );
    }

    private void LayoutSettingChanged(object _, EventArgs e)
    {
        RebuildProgressDisplay();
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public bool AllowEvent(LevelEventType eventType)
    {
        if (!LDMLevel)
            return true;
        switch (eventType)
        {
            // TODO: investigate RepeatEvents and SetFrameRate

            case LevelEventType.Bloom:
            case LevelEventType.Flash:
            case LevelEventType.HallOfMirrors:
            case LevelEventType.ScreenScroll:
            case LevelEventType.ScreenTile:
            case LevelEventType.SetFilter:
            case LevelEventType.SetFilterAdvanced:
            case LevelEventType.ShakeScreen:
                return false;

            default:
                return true;
        }
    }

    public static string GetVersionText()
    {
        return $"{MyPluginInfo.PLUGIN_NAME} ("
            +
#if DEBUG
            "DEBUG"
#else
            "RELEASE"
#endif
            + $" build at {typeof(BuildInformation)
                   .GetCustomAttribute<BuildInformation.BuildTimeAttribute>()
                   ?.ToString() ?? "[UNKNOWN]"})";
    }
}
