using System;
using System.Globalization;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AdofaiQolMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class AdofaiQolMod : BaseUnityPlugin
{
    public static AdofaiQolMod Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private ConfigEntry<bool> hidePerfect = null!;
    public bool HidePerfect => hidePerfect.Value;

    private ConfigEntry<string> accuracyFormat = null!;
    public string AccuracyFormat => accuracyFormat.Value;
    private ConfigEntry<string> xAccuracyFormat = null!;
    public string XAccuracyFormat => xAccuracyFormat.Value;
    private ConfigEntry<string> percentageFormat = null!;
    public string PercentageFormat => percentageFormat.Value;

    private ConfigEntry<TextAnchor> anchor = null!;
    public TextAnchor Anchor => anchor.Value;
    private ConfigEntry<int> verticalOffset = null!;
    public int VerticalOffset => verticalOffset.Value;
    private ConfigEntry<float> horizontalOffset = null!;
    public float HorizontalOffset => horizontalOffset.Value;
    private ConfigEntry<float> spacing = null!;
    public float Spacing => spacing.Value;
    private ConfigEntry<float> fontSize = null!;
    public float FontSize => fontSize.Value;

    private GameObject modCanvas = null!;
    private RectTransform? progressDisplay;
    private TextMeshProUGUI? accuracy;
    private TextMeshProUGUI? xAccuracy;
    private TextMeshProUGUI? percentage;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        hidePerfect = Config.Bind("General", "HidePerfect", true, "Hides the \"Perfect\" score");

        accuracyFormat = Config.Bind(
            "ProgressDisplay",
            "AccuracyFormat",
            "A: {0}",
            "How to format the accuracy display"
        );
        xAccuracyFormat = Config.Bind(
            "ProgressDisplay",
            "XAccuracyFormat",
            "X-A: {0}",
            "How to format the x-accuracy display"
        );
        percentageFormat = Config.Bind(
            "ProgressDisplay",
            "PercentageFormat",
            "P: {0}",
            "How to format the progress display"
        );
        accuracyFormat.SettingChanged += LayoutSettingChanged;
        xAccuracyFormat.SettingChanged += LayoutSettingChanged;
        percentageFormat.SettingChanged += LayoutSettingChanged;
        anchor = Config.Bind(
            "ProgressDisplay",
            "Anchor",
            TextAnchor.UpperLeft,
            "Where to anchor the progress display"
        );
        verticalOffset = Config.Bind(
            "ProgressDisplay",
            "VerticalOffset",
            0,
            "Y offset of the progress display"
        );
        horizontalOffset = Config.Bind(
            "ProgressDisplay",
            "HorizontalOffset",
            10f,
            "X offset of the progress display"
        );
        spacing = Config.Bind("ProgressDisplay", "Spacing", 10f, "The space between values");
        fontSize = Config.Bind("ProgressDisplay", "FontSize", 72f, "The font size");
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

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        return;

        void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
            Logger.LogDebug("Patching...");
            Harmony.PatchAll();
            Logger.LogDebug("Finished patching!");
        }
    }

    public void RebuildProgressDisplay()
    {
        Logger.LogDebug(
            $">> RebuildProgressDisplay() modCanvas:{modCanvas} progressDisplay:{progressDisplay} "
        );
        Destroy(progressDisplay?.gameObject);
        var container = new GameObject("ProgressDisplay", typeof(RectTransform));
        progressDisplay = (RectTransform)container.transform;
        progressDisplay.pivot = progressDisplay.anchorMin = Vector2.zero;
        progressDisplay.anchorMax = Vector2.one;
        progressDisplay.parent = modCanvas.transform;
        progressDisplay.anchoredPosition = Vector2.zero;
        progressDisplay.offsetMin = new Vector2(HorizontalOffset, 0f);
        progressDisplay.offsetMax = new Vector2(0f, -VerticalOffset);
        var layout = container.AddComponent<VerticalLayoutGroup>();
        layout.spacing = Spacing;
        layout.childForceExpandHeight = false;
        layout.childAlignment = Anchor;
        if (!string.IsNullOrWhiteSpace(AccuracyFormat))
        {
            var accuracyObject = new GameObject("Accuracy", typeof(RectTransform));
            ((RectTransform)accuracyObject.transform).parent = progressDisplay;
            accuracy = accuracyObject.AddComponent<TextMeshProUGUI>();
            accuracy.fontSize = FontSize;
        }
        else
            accuracy = null;

        if (!string.IsNullOrWhiteSpace(XAccuracyFormat))
        {
            var xAccuracyObject = new GameObject("XAccuracy", typeof(RectTransform));
            ((RectTransform)xAccuracyObject.transform).parent = progressDisplay;
            xAccuracy = xAccuracyObject.AddComponent<TextMeshProUGUI>();
            xAccuracy.fontSize = FontSize;
        }
        else
            xAccuracy = null;
        if (!string.IsNullOrWhiteSpace(PercentageFormat))
        {
            var percentageObject = new GameObject("Percentage", typeof(RectTransform));
            ((RectTransform)percentageObject.transform).parent = progressDisplay;
            percentage = percentageObject.AddComponent<TextMeshProUGUI>();
            percentage.fontSize = FontSize;
        }
        else
            percentage = null;
        UpdateProgressDisplay();
    }

    public void UpdateProgressDisplay(
        string accuracyValue,
        string xAccuracyValue,
        string percentageValue
    )
    {
        const string INVALID = "<i>Invalid format string</i>";
        Logger.LogDebug(
            $">> UpdateProgressDisplay(accuracyValue: {accuracyValue}, xAccuracyValue: {xAccuracyValue}, percentageValue: {percentageValue}) accuracy:{accuracy} xAccuracy:{xAccuracy} percentage:{percentage}"
        );
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

    private const string PLACEHOLDER = "...";

    public void UpdateProgressDisplay(bool show = true)
    {
        Logger.LogDebug(
            $">> UpdateProgressDisplay(show: {show}) progressDisplay:{progressDisplay} accuracy:{accuracy} xAccuracy:{xAccuracy} percentage:{percentage}"
        );
        progressDisplay?.gameObject.SetActive(show);

        if (!show)
            return;

        if (scrController.instance != null)
            UpdateProgressDisplay(scrController.instance);
        else
            UpdateProgressDisplay(PLACEHOLDER, PLACEHOLDER, PLACEHOLDER);
    }

    public void UpdateProgressDisplay(scrController __instance)
    {
        Logger.LogDebug($">> UpdateProgressDisplay(__instance: {__instance})");
        var p = PLACEHOLDER;
        try
        {
            if (!float.IsNaN(__instance.percentComplete))
                p = $"{__instance.percentComplete * 100f:0.##}%";
        }
        catch (NullReferenceException) { }
        UpdateProgressDisplay(
            float.IsNaN(__instance.mistakesManager.percentAcc)
                ? PLACEHOLDER
                : $"{__instance.mistakesManager.percentAcc * 100f:0.##}%",
            float.IsNaN(__instance.mistakesManager.percentXAcc)
                ? PLACEHOLDER
                : $"{__instance.mistakesManager.percentXAcc * 100f:0.##}%",
            p
        );
    }

    private void LayoutSettingChanged(object _, EventArgs e) => RebuildProgressDisplay();
}
