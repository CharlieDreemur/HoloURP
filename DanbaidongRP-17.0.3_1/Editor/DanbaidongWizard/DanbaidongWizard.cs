using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Universal
{

    [InitializeOnLoad]
    public class DanbaidongWizard : EditorWindowWithHelpButton
    {
        enum Configuration
        {
            DanbaidongRP_RayTracing,
            //DanbaidongRP
        }

        Configuration m_Configuration;
        VisualElement m_BaseUpdatable;

        DanbaidongRPPresets m_pipelinePresets = new DanbaidongRPPresets();

        [MenuItem("Window/Rendering/DanbaidongRP Wizard", priority = 10000)]
        static internal void OpenWindow()
        {
            var window = GetWindow<DanbaidongWizard>(Style.title.text);
            window.minSize = new Vector2(500, 450);
            DanbaidongUserSettings.wizardPopupAlreadyShownOnce = true;
        }

        void OnGUI()
        {
            if (m_BaseUpdatable == null)
                return;

            foreach (VisualElementUpdatable updatable in m_BaseUpdatable.Children().Where(c => c is VisualElementUpdatable))
                updatable.CheckUpdate();
        }

        static DanbaidongWizard()
        {
            LoadReflectionMethods();
            WizardBehaviour();
        }

        private void CreateGUI()
        {
            titleContent = Style.title;

            DanbaidongWizardEditorUtils.AddStyleSheets(rootVisualElement, DanbaidongWizardEditorUtils.FormatingPath);
            DanbaidongWizardEditorUtils.AddStyleSheets(rootVisualElement, DanbaidongWizardEditorUtils.WizardSheetPath);

            m_pipelinePresets.LoadPipelinePresets();

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            rootVisualElement.Add(scrollView);
            var container = scrollView.contentContainer;

            container.Add(CreateTitle(Style.confiTitle));
            container.Add(CreateTabbedBox(new[]
                {
                    (Style.danbaidongRP_DXR_ConfigLabel, Style.danbaidongRP_DXR_Tooltip),
                    //(Style.danbaidongRPConfigLabel, Style.danbaidongRPTooltip),
                }, out m_BaseUpdatable));


            ScopeBox globalScope = new ScopeBox(Style.global);
            ScopeBox raytracingScope = new ScopeBox(Style.raytracing);

            m_BaseUpdatable.Add(globalScope);
            m_BaseUpdatable.Add(raytracingScope);

            AddDanbaidongRPConfigInfo(globalScope, QualityScope.Global);
            AddDanbaidongRPConfigInfo(raytracingScope, QualityScope.Raytracing);

            container.Add(CreateWizardBehaviour());

            CheckPersistantNeedReboot();
        }

        void AddDanbaidongRPConfigInfo(VisualElement container, QualityScope quality)
            => GroupEntriesForDisplay(container, InclusiveMode.DanbaidongRP, quality);

        void GroupEntriesForDisplay(VisualElement container, InclusiveMode filter, QualityScope scope)
        {
            foreach (var entry in entries.Where(e => e.scope == scope && filter.Contains(e.inclusiveScope)))
            {
                string error = entry.configStyle.error;

                // If it is necessary, append tht name of the current asset.
                var danbaidongRPAsset = UniversalRenderPipeline.asset;
                if (entry.displayAssetName && danbaidongRPAsset != null)
                {
                    error += " (" + danbaidongRPAsset.name + ").";
                }

                container.Add(new ConfigInfoLine(
                    entry.configStyle.label,
                    error,
                    entry.configStyle.messageType,
                    entry.configStyle.button,
                    () => entry.check(),
                    () => entry.fix?.Invoke(false),
                    entry.indent,
                    entry.configStyle.messageType == MessageType.Error || entry.forceDisplayCheck,
                    entry.skipErrorIcon));
            }
        }


        static class Style
        {
            public static readonly GUIContent title = EditorGUIUtility.TrTextContent("DanbaidongRP Wizard");
            public static readonly string confiTitle = L10n.Tr("Pipeline Configurations");

            public const string danbaidongRP_DXR_ConfigLabel = "DanbaidongRP RayTracing";
            public static readonly string danbaidongRP_DXR_Tooltip = L10n.Tr("This tab contains configuration check for DX RayTracing Danbaidong Render Pipeline.");
            public const string danbaidongRPConfigLabel = "DanbaidongRP";
            public static readonly string danbaidongRPTooltip = L10n.Tr("This tab contains configuration check for Danbaidong Render Pipeline.");

            public static readonly string global = L10n.Tr("Global");
            public static readonly string raytracing = L10n.Tr("Raytracing");
            public static readonly string resolve = L10n.Tr("Fix");
            public static readonly string resolveAll = L10n.Tr("Fix All");
            public static readonly string showOnStartUp = L10n.Tr("Show wizard after start");

            public struct ConfigStyle
            {
                public readonly string label;
                public readonly string error;
                public readonly string button;
                public readonly MessageType messageType;
                public ConfigStyle(string label, string error, string button = null, MessageType messageType = MessageType.Error)
                {
                    if (button == null)
                        button = resolve;
                    this.label = label;
                    this.error = error;
                    this.button = button;
                    this.messageType = messageType;
                }
            }

            public static readonly ConfigStyle pipelineAssetGraphicsAssigned = new ConfigStyle(
                label: L10n.Tr("Assigned - Graphics"),
                error: L10n.Tr("There is no Pipeline asset assigned to the Graphic Settings!"));
            public static readonly ConfigStyle pipelineGlobalSettingsAssigned = new ConfigStyle(
                label: L10n.Tr("Assigned - Pipeline Global Settings"),
                error: L10n.Tr("There is no PipelineGlobalSettings assigned to Edit > Project Settings > Graphics > URP Global Settings!"));
            public static readonly ConfigStyle danbaidongRPColorSpace = new ConfigStyle(
                label: L10n.Tr("Linear color space"),
                error: L10n.Tr("Only linear color space supported!"));

            public static readonly ConfigStyle directionalLightPresets = new ConfigStyle(
                label: L10n.Tr("Presets - Directional Light"),
                error: L10n.Tr("There is no preset of directional light!"));
            public static readonly ConfigStyle pointLightPresets = new ConfigStyle(
                label: L10n.Tr("Presets - Point Light"),
                error: L10n.Tr("There is no preset of Point light!"));
            public static readonly ConfigStyle spotLightPresets = new ConfigStyle(
                label: L10n.Tr("Presets - Spot Light"),
                error: L10n.Tr("There is no preset of Spot light!"));

            public static readonly ConfigStyle meshRendererPresets = new ConfigStyle(
                label: L10n.Tr("Presets - MeshRenderer"),
                error: L10n.Tr("There is no preset of MeshRenderer!"));

            public const string mainlightshadows = "MainLightShadows";
            public static readonly ConfigStyle renderinglayerMainLightShadow = new ConfigStyle(
                label: L10n.Tr("RenderingLayers - MainLightShadows"),
                error: L10n.Tr("Rendering Layer1 is used by mainLightShadow and perObjectShadow, should set to \"MainLightShadows\"!"));

            public static readonly ConfigStyle dxrD3D12 = new ConfigStyle(
                label: L10n.Tr("Direct3D 12"),
                error: L10n.Tr("Direct3D 12 needs to be the active device! (Editor restart is required). If an API different than D3D12 is forced via command line argument, clicking Fix won't change it, so please consider removing it if wanting to run DXR."));
            public static readonly ConfigStyle dxrRaytracing = new ConfigStyle(
                label: L10n.Tr("Enable Raytracing"),
                error: L10n.Tr("Raytracing needs to be true, check Render Pipeline Asset > Rendering!"),
                messageType: MessageType.Warning);
        }



        #region SCRIPT_RELOADING

        static int frameToWait;

        static void WizardBehaviourDelayed()
        {
            if (frameToWait > 0)
            {
                --frameToWait;
                return;
            }

            // No need to update this method, unsubscribe from the application update
            EditorApplication.update -= WizardBehaviourDelayed;

            // If the wizard does not need to be shown at start up, do nothing.
            if (!DanbaidongUserSettings.wizardIsStartPopup)
                return;

            //Application.isPlaying cannot be called in constructor. Do it here
            if (Application.isPlaying)
                return;

            EditorApplication.quitting += () => DanbaidongUserSettings.wizardPopupAlreadyShownOnce = false;

            ShowWizardFirstTime();
        }

        static void ShowWizardFirstTime()
        {
            // Unsubscribe from possible events
            // If the event has not been registered the unsubscribe will do nothing
            RenderPipelineManager.activeRenderPipelineTypeChanged -= ShowWizardFirstTime;

            if (UniversalRenderPipeline.asset == null)
            {
                // Delay the show of the wizard for the first time that the user is using DanbaidongRP
                RenderPipelineManager.activeRenderPipelineTypeChanged += ShowWizardFirstTime;
                return;
            }

            // If we reach this point can be because
            // - That the user started Unity with DanbaidongRP in use
            // - That the SRP has changed to DanbaidongRP for the first time in the session
            if (!DanbaidongUserSettings.wizardPopupAlreadyShownOnce)
                OpenWindow();
        }

        [Callbacks.DidReloadScripts]
        static void WizardBehaviour()
        {
            // We should call ProjectSettings.wizardIsStartPopup to check here.
            // But if the Wizard is opened while a domain reload occurs, we end up calling
            // LoadSerializedFileAndForget at a time Unity associate with Constructor. This is not allowed.
            // As we should wait some frame for everything to be correctly loaded anyway, we do that in WizardBehaviourDelayed.

            //We need to wait at least one frame or the popup will not show up
            frameToWait = 10;
            EditorApplication.update += WizardBehaviourDelayed;
        }

        #endregion /* SCRIPT_RELOADING */



        #region UI_ELEMENT
        class ToolbarRadio : UIElements.Toolbar, INotifyValueChanged<int>
        {
            [Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
            public new class UxmlFactory : UxmlFactory<ToolbarRadio, UxmlTraits> { }
            [Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
            public new class UxmlTraits : Button.UxmlTraits { }

            List<ToolbarToggle> radios = new List<ToolbarToggle>();

            public new static readonly string ussClassName = "unity-toolbar-radio";

            public int radioLength => radios.Count;

            int m_Value;
            public int value
            {
                get => m_Value;
                set
                {
                    if (value == m_Value)
                        return;

                    if (panel != null)
                    {
                        using (ChangeEvent<int> evt = ChangeEvent<int>.GetPooled(m_Value, value))
                        {
                            evt.target = this;
                            SetValueWithoutNotify(value);
                            SendEvent(evt);
                        }
                    }
                    else
                    {
                        SetValueWithoutNotify(value);
                    }
                }
            }

            public ToolbarRadio()
            {
                AddToClassList(ussClassName);
            }

            void AddRadio(string label = null, string tooltip = null)
            {
                var toggle = new ToolbarToggle()
                {
                    text = label,
                    tooltip = tooltip
                };
                toggle.RegisterValueChangedCallback(InnerValueChanged(radioLength));
                toggle.SetValueWithoutNotify(radioLength == 0);
                if (radioLength == 0)
                    toggle.AddToClassList("SelectedRadio");
                radios.Add(toggle);
                Add(toggle);
                toggle.AddToClassList("Radio");
            }

            public void AddRadios((string label, string tooltip)[] tabs)
            {
                if (tabs.Length == 0)
                    return;

                if (radioLength > 0)
                {
                    radios[radioLength - 1].RemoveFromClassList("LastRadio");
                }
                foreach (var (label, tooltip) in tabs)
                    AddRadio(label, tooltip);

                radios[radioLength - 1].AddToClassList("LastRadio");
            }

            EventCallback<ChangeEvent<bool>> InnerValueChanged(int radioIndex)
            {
                return (ChangeEvent<bool> evt) =>
                {
                    if (radioIndex == m_Value)
                    {
                        if (!evt.newValue)
                        {
                            //cannot deselect in a radio
                            radios[m_Value].RemoveFromClassList("SelectedRadio");
                            radios[radioIndex].AddToClassList("SelectedRadio");
                            radios[radioIndex].SetValueWithoutNotify(true);
                        }
                        else
                            value = -1;
                    }
                    else
                        value = radioIndex;
                };
            }

            public void SetValueWithoutNotify(int newValue)
            {
                if (m_Value != newValue)
                {
                    if (newValue < 0 || newValue >= radioLength)
                        throw new System.IndexOutOfRangeException();

                    if (m_Value != newValue)
                    {
                        radios[m_Value].RemoveFromClassList("SelectedRadio");
                        radios[newValue].AddToClassList("SelectedRadio");
                        radios[newValue].SetValueWithoutNotify(true);
                        m_Value = newValue;
                    }
                }
            }
        }

        abstract class VisualElementUpdatable : VisualElement
        {
            protected Func<bool> m_Tester;
            bool m_HaveFixer;
            public bool currentStatus { get; private set; }

            protected VisualElementUpdatable(Func<bool> tester, bool haveFixer)
            {
                m_Tester = tester;
                m_HaveFixer = haveFixer;
            }

            public virtual void CheckUpdate()
            {
                bool wellConfigured = m_Tester();
                if (wellConfigured ^ currentStatus)
                {
                    UpdateDisplay(wellConfigured, m_HaveFixer);
                    currentStatus = wellConfigured;
                }
            }

            protected void Init() => UpdateDisplay(currentStatus, m_HaveFixer);

            protected abstract void UpdateDisplay(bool statusOK, bool haveFixer);
        }

        class HiddableUpdatableContainer : VisualElementUpdatable
        {
            public HiddableUpdatableContainer(Func<bool> tester, bool haveFixer = false) : base(tester, haveFixer) { }

            public override void CheckUpdate()
            {
                base.CheckUpdate();
                if (currentStatus)
                {
                    foreach (VisualElementUpdatable updatable in Children().Where(e => e is VisualElementUpdatable))
                        updatable.CheckUpdate();
                }
            }

            new public void Init() => base.Init();

            protected override void UpdateDisplay(bool visible, bool haveFixer)
                => style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        class ConfigInfoLine : VisualElementUpdatable
        {
            static class Style
            {
                public static readonly Texture ok = CoreEditorUtils.LoadIcon("icons", "GreenCheckmark", ".png");
                public static readonly Texture error = CoreEditorUtils.LoadIcon("icons", "console.erroricon", ".png");
                public static readonly Texture warning = CoreEditorUtils.LoadIcon("icons", "console.warnicon", ".png");

                public const int k_IndentStepSize = 15;
            }

            readonly bool m_VisibleStatus;
            readonly bool m_SkipErrorIcon;

            public ConfigInfoLine(string label, string error, MessageType messageType, string resolverButtonLabel, Func<bool> tester, Action resolver, int indent = 0, bool visibleStatus = true, bool skipErrorIcon = false)
                : base(tester, resolver != null)
            {
                m_VisibleStatus = visibleStatus;
                m_SkipErrorIcon = skipErrorIcon;
                var testLabel = new Label(label)
                {
                    name = "TestLabel"
                };
                var fixer = new Button(resolver)
                {
                    text = resolverButtonLabel,
                    name = "Resolver"
                };
                var testRow = new VisualElement() { name = "TestRow" };
                testRow.Add(testLabel);
                if (m_VisibleStatus)
                {
                    var statusOK = new Image()
                    {
                        image = Style.ok,
                        name = "StatusOK"
                    };
                    var statusKO = new Image()
                    {
                        image = Style.error,
                        name = "StatusError"
                    };
                    testRow.Add(statusOK);
                    testRow.Add(statusKO);
                }
                testRow.Add(fixer);

                Add(testRow);
                HelpBox.Kind kind;
                switch (messageType)
                {
                    default:
                    case MessageType.None: kind = HelpBox.Kind.None; break;
                    case MessageType.Error: kind = HelpBox.Kind.Error; break;
                    case MessageType.Warning: kind = HelpBox.Kind.Warning; break;
                    case MessageType.Info: kind = HelpBox.Kind.Info; break;
                }
                Add(new HelpBox(kind, error));

                testLabel.style.paddingLeft = style.paddingLeft.value.value + indent * Style.k_IndentStepSize;

                Init();
            }

            protected override void UpdateDisplay(bool statusOK, bool haveFixer)
            {
                if (!((hierarchy.parent as HiddableUpdatableContainer)?.currentStatus ?? true))
                {
                    if (m_VisibleStatus)
                    {
                        this.Q(name: "StatusOK").style.display = DisplayStyle.None;
                        this.Q(name: "StatusError").style.display = DisplayStyle.None;
                    }
                    this.Q(name: "Resolver").style.display = DisplayStyle.None;
                    this.Q(className: "HelpBox").style.display = DisplayStyle.None;
                }
                else
                {
                    if (m_VisibleStatus)
                    {
                        this.Q(name: "StatusOK").style.display = statusOK ? DisplayStyle.Flex : DisplayStyle.None;
                        this.Q(name: "StatusError").style.display = !statusOK ? (m_SkipErrorIcon ? DisplayStyle.None : DisplayStyle.Flex) : DisplayStyle.None;
                    }
                    this.Q(name: "Resolver").style.display = statusOK || !haveFixer ? DisplayStyle.None : DisplayStyle.Flex;
                    this.Q(className: "HelpBox").style.display = statusOK ? DisplayStyle.None : DisplayStyle.Flex;
                }
            }
        }

        class HelpBox : VisualElement
        {
            public enum Kind
            {
                None,
                Info,
                Warning,
                Error
            }

            readonly Label label;
            readonly Image icon;

            public string text
            {
                get => label.text;
                set => label.text = value;
            }

            Kind m_Kind = Kind.None;
            public Kind kind
            {
                get => m_Kind;
                set
                {
                    if (m_Kind != value)
                    {
                        m_Kind = value;

                        string iconName;
                        switch (kind)
                        {
                            default:
                            case Kind.None:
                                icon.style.display = DisplayStyle.None;
                                return;
                            case Kind.Info:
                                iconName = "console.infoicon";
                                break;
                            case Kind.Warning:
                                iconName = "console.warnicon";
                                break;
                            case Kind.Error:
                                iconName = "console.erroricon";
                                break;
                        }
                        icon.image = EditorGUIUtility.IconContent(iconName).image;
                        icon.style.display = DisplayStyle.Flex;
                    }
                }
            }

            public HelpBox(Kind kind, string message)
            {
                this.label = new Label(message);
                icon = new Image();

                AddToClassList("HelpBox");
                Add(icon);
                Add(this.label);

                this.kind = kind;
            }
        }

        class FixAllButton : VisualElementUpdatable
        {
            public FixAllButton(string label, Func<bool> tester, Action resolver)
                : base(tester, resolver != null)
            {
                Add(new Button(resolver)
                {
                    text = label,
                    name = "FixAll"
                });

                Init();
            }

            protected override void UpdateDisplay(bool statusOK, bool haveFixer)
                => this.Q(name: "FixAll").style.display = statusOK ? DisplayStyle.None : DisplayStyle.Flex;
        }

        class ScopeBox : VisualElementUpdatable
        {
            readonly Label label;
            bool initTitleBackground;

            public ScopeBox(string title) : base(null, false)
            {
                label = new Label(title);
                label.name = "Title";
                AddToClassList("ScopeBox");
                Add(label);
            }

            public override void CheckUpdate()
            {
                foreach (VisualElementUpdatable updatable in Children().Where(e => e is VisualElementUpdatable))
                    updatable.CheckUpdate();
            }

            protected override void UpdateDisplay(bool statusOK, bool haveFixer)
            { }
        }

        #endregion UI_ELEMENT


        #region Create VisualElement
        VisualElement CreateWizardBehaviour()
        {
            var toggle = new Toggle(Style.showOnStartUp)
            {
                value = DanbaidongUserSettings.wizardIsStartPopup,
                name = "WizardCheckbox"
            };
            toggle.AddToClassList("LeftToogle");
            toggle.RegisterValueChangedCallback(evt
                => DanbaidongUserSettings.wizardIsStartPopup = evt.newValue);
            return toggle;
        }

        Label CreateTitle(string title)
        {
            var label = new Label(title);
            label.AddToClassList("h1");
            return label;
        }

        VisualElement CreateTabbedBox((string label, string tooltip)[] tabs, out VisualElement innerBox)
        {
            var toolbar = new ToolbarRadio();
            toolbar.AddRadios(tabs);
            //make sure when we open the same project on different platforms the saved active tab is not out of range
            int tabIndex = toolbar.radioLength > DanbaidongUserSettings.wizardActiveTab ? DanbaidongUserSettings.wizardActiveTab : 0;
            toolbar.SetValueWithoutNotify(tabIndex);
            m_Configuration = (Configuration)tabIndex;
            toolbar.RegisterValueChangedCallback(evt =>
            {
                int index = evt.newValue;
                m_Configuration = (Configuration)index;
                DanbaidongUserSettings.wizardActiveTab = index;
            });

            var outerBox = new VisualElement() { name = "OuterBox" };
            innerBox = new VisualElement { name = "InnerBox" };

            outerBox.Add(toolbar);
            outerBox.Add(innerBox);

            return outerBox;
        }

        VisualElement CreateLargeButton(string title, Action action)
        {
            Button button = new Button(action) { text = title };
            button.AddToClassList("LargeButton");
            return button;
        }

        #endregion Create VisualElement







        class QueuedLauncher
        {
            Queue<Action> m_Queue = new Queue<Action>();
            bool m_Running = false;
            bool m_StopRequested = false;
            bool m_OnPause = false;

            public void Stop() => m_StopRequested = true;

            // Function to pause/unpause the action execution
            public void Pause() => m_OnPause = true;
            public void Unpause() => m_OnPause = false;

            public int remainingFixes => m_Queue.Count;

            void Start()
            {
                m_Running = true;
                EditorApplication.update += Run;
            }

            void End()
            {
                EditorApplication.update -= Run;
                m_Running = false;
            }

            void Run()
            {
                if (m_StopRequested)
                {
                    m_Queue.Clear();
                    m_StopRequested = false;
                }
                if (m_Queue.Count > 0)
                {
                    if (!m_OnPause)
                    {
                        m_Queue.Dequeue()?.Invoke();
                    }
                }
                else
                    End();
            }

            public void Add(Action function)
            {
                m_Queue.Enqueue(function);
                if (!m_Running)
                    Start();
            }

            public void Add(params Action[] functions)
            {
                foreach (Action function in functions)
                    Add(function);
            }
        }
        QueuedLauncher m_Fixer = new QueuedLauncher();


        static class ObjectSelector
        {
            static Action<UnityEngine.Object, Type, Action<UnityEngine.Object>> ShowObjectSelector;
            static Func<UnityEngine.Object> GetCurrentObject;
            static Func<int> GetSelectorID;
            static Action<int> SetSelectorID;

            const string ObjectSelectorUpdatedCommand = "ObjectSelectorUpdated";

            static int id;

            static int selectorID { get => GetSelectorID(); set => SetSelectorID(value); }

            public static bool opened
                => Resources.FindObjectsOfTypeAll(typeof(PlayerSettings).Assembly.GetType("UnityEditor.ObjectSelector")).Length > 0;

            // Action to be called with the window is closed
            static Action s_OnClose;

            static ObjectSelector()
            {
                Type playerSettingsType = typeof(PlayerSettings);
                Type objectSelectorType = playerSettingsType.Assembly.GetType("UnityEditor.ObjectSelector");
                var instanceObjectSelectorInfo = objectSelectorType.GetProperty("get", BindingFlags.Static | BindingFlags.Public);
#if UNITY_2022_2_OR_NEWER
                var showInfo = objectSelectorType.GetMethod("Show", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(UnityEngine.Object), typeof(Type), typeof(UnityEngine.Object), typeof(bool), typeof(List<int>), typeof(Action<UnityEngine.Object>), typeof(Action<UnityEngine.Object>), typeof(bool) }, null);
#elif UNITY_2020_1_OR_NEWER
                var showInfo = objectSelectorType.GetMethod("Show", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(UnityEngine.Object), typeof(Type), typeof(UnityEngine.Object), typeof(bool), typeof(List<int>), typeof(Action<UnityEngine.Object>), typeof(Action<UnityEngine.Object>) }, null);
#else
                var showInfo = objectSelectorType.GetMethod("Show", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(UnityEngine.Object), typeof(Type), typeof(SerializedProperty), typeof(bool), typeof(List<int>), typeof(Action<UnityEngine.Object>), typeof(Action<UnityEngine.Object>) }, null);
#endif
                var objectSelectorVariable = Expression.Variable(objectSelectorType, "objectSelector");
                var objectParameter = Expression.Parameter(typeof(UnityEngine.Object), "unityObject");
                var typeParameter = Expression.Parameter(typeof(Type), "type");
                var onClosedParameter = Expression.Parameter(typeof(Action<UnityEngine.Object>), "onClosed");
                var onChangedObjectParameter = Expression.Parameter(typeof(Action<UnityEngine.Object>), "onChangedObject");
                var showObjectSelectorBlock = Expression.Block(
                    new[] { objectSelectorVariable },
                    Expression.Assign(objectSelectorVariable, Expression.Call(null, instanceObjectSelectorInfo.GetGetMethod())),
#if UNITY_2022_2_OR_NEWER
                    Expression.Call(objectSelectorVariable, showInfo, objectParameter, typeParameter, Expression.Constant(null, typeof(UnityEngine.Object)), Expression.Constant(false), Expression.Constant(null, typeof(List<int>)), Expression.Constant(null, typeof(Action<UnityEngine.Object>)), onChangedObjectParameter, Expression.Constant(true))
#elif UNITY_2020_1_OR_NEWER
                    Expression.Call(objectSelectorVariable, showInfo, objectParameter, typeParameter, Expression.Constant(null, typeof(UnityEngine.Object)), Expression.Constant(false), Expression.Constant(null, typeof(List<int>)), Expression.Constant(null, typeof(Action<UnityEngine.Object>)), onChangedObjectParameter)
#else
                    Expression.Call(objectSelectorVariable, showInfo, objectParameter, typeParameter, Expression.Constant(null, typeof(SerializedProperty)), Expression.Constant(false), Expression.Constant(null, typeof(List<int>)), Expression.Constant(null, typeof(Action<UnityEngine.Object>)), onChangedObjectParameter)
#endif
                );
                var showObjectSelectorLambda = Expression.Lambda<Action<UnityEngine.Object, Type, Action<UnityEngine.Object>>>(showObjectSelectorBlock, objectParameter, typeParameter, onChangedObjectParameter);
                ShowObjectSelector = showObjectSelectorLambda.Compile();

                var instanceCall = Expression.Call(null, instanceObjectSelectorInfo.GetGetMethod());
                var objectSelectorIDField = Expression.Field(instanceCall, "objectSelectorID");
                var getSelectorIDLambda = Expression.Lambda<Func<int>>(objectSelectorIDField);
                GetSelectorID = getSelectorIDLambda.Compile();

                var inSelectorIDParam = Expression.Parameter(typeof(int), "value");
                var setSelectorIDLambda = Expression.Lambda<Action<int>>(Expression.Assign(objectSelectorIDField, inSelectorIDParam), inSelectorIDParam);
                SetSelectorID = setSelectorIDLambda.Compile();

                var getCurrentObjectInfo = objectSelectorType.GetMethod("GetCurrentObject");
                var getCurrentObjectLambda = Expression.Lambda<Func<UnityEngine.Object>>(Expression.Call(null, getCurrentObjectInfo));
                GetCurrentObject = getCurrentObjectLambda.Compile();
            }

            public static void Show(UnityEngine.Object obj, Type type, Action<UnityEngine.Object> onChangedObject, Action onClose)
            {
                id = GUIUtility.GetControlID("s_ObjectFieldHash".GetHashCode(), FocusType.Keyboard);
                GUIUtility.keyboardControl = id;
                ShowObjectSelector(obj, type, onChangedObject);
                selectorID = id;
                ObjectSelector.s_OnClose = onClose;
                EditorApplication.update += CheckClose;
            }

            static void CheckClose()
            {
                if (!opened)
                {
                    ObjectSelector.s_OnClose?.Invoke();
                    EditorApplication.update -= CheckClose;
                }
            }

            public static void CheckAssignationEvent<T>(Action<T> assignator)
                where T : UnityEngine.Object
            {
                Event evt = Event.current;
                if (evt.type != EventType.ExecuteCommand)
                    return;
                string commandName = evt.commandName;
                if (commandName != ObjectSelectorUpdatedCommand || selectorID != id)
                    return;
                T current = GetCurrentObject() as T;
                if (current == null)
                    return;
                assignator(current);
                GUI.changed = true;
                evt.Use();
            }
        }


        // Configurations

        #region Entry

        struct Entry
        {
            public readonly QualityScope scope;
            public readonly InclusiveMode inclusiveScope;
            public readonly Style.ConfigStyle configStyle;
            public readonly Func<bool> check;
            public readonly Action<bool> fix;
            public readonly int indent;
            public readonly bool forceDisplayCheck;
            public readonly bool skipErrorIcon;
            public readonly bool displayAssetName;

            public Entry(QualityScope scope, InclusiveMode mode, Style.ConfigStyle configStyle, Func<bool> check,
                Action<bool> fix, int indent = 0, bool forceDisplayCheck = false, bool skipErrorIcon = false, bool displayAssetName = false)
            {
                this.scope = scope;
                this.inclusiveScope = mode;
                this.configStyle = configStyle;
                this.check = check;
                this.fix = fix;
                this.forceDisplayCheck = forceDisplayCheck;
                this.indent = mode == InclusiveMode.XRManagement ? 1 : indent;
                this.skipErrorIcon = skipErrorIcon;
                this.displayAssetName = displayAssetName;
            }
        }


        [InitializeOnLoadMethod]
        static void InitializeEntryList()
        {
            //Check for playmode has been added to ensure the editor window wont take focus when entering playmode
            if (EditorWindow.HasOpenInstances<DanbaidongWizard>() && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += DelayedRebuildEntryList;

                // Case 1407981: Calling GetWindow in InitializeOnLoadMethod doesn't work and creates a new window instead of getting the existing one.
                void DelayedRebuildEntryList()
                {
                    EditorApplication.update -= DelayedRebuildEntryList;
                    DanbaidongWizard window = EditorWindow.GetWindow<DanbaidongWizard>(Style.title.text);
                    window.ReBuildEntryList();
                }
            }
        }

        Entry[] BuildEntryList()
        {
            var entryList = new List<Entry>();

            // Add the general
            entryList.AddRange(new[]
            {
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.pipelineAssetGraphicsAssigned,
                    IsPipelineAssetGraphicsUsedCorrect, FixPipelineAssetGraphicsUsed),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.pipelineGlobalSettingsAssigned,
                    IsPipelineGlobalSettingsUsedCorrect, FixPipelineGlobalSettingsUsed),
            });

            entryList.AddRange(new Entry[]
            {
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.danbaidongRPColorSpace, IsColorSpaceCorrect, FixColorSpace),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.directionalLightPresets, IsPresetsDirectionalLightCorrect, FixPresetsDirectionalLight),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.pointLightPresets, IsPresetsPointLightCorrect, FixPresetsPointLight),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.spotLightPresets, IsPresetsSpotLightCorrect, FixPresetsSpotLight),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.meshRendererPresets, IsPresetsMeshRendererCorrect, FixPresetsMeshRendererLight),
                new Entry(QualityScope.Global, InclusiveMode.DanbaidongRP, Style.renderinglayerMainLightShadow, IsRenderingLayersMainLightShadowCorrect, FixRenderingLayersMainLightShadow),
            });

            entryList.AddRange(new Entry[]
            {
                new Entry(QualityScope.Raytracing, InclusiveMode.DanbaidongRP, Style.dxrD3D12, IsDXRDirect3D12Correct, FixDXRDirect3D12),
                new Entry(QualityScope.Raytracing, InclusiveMode.DanbaidongRP, Style.dxrRaytracing, IsPipelineAssetRaytracingCorrect, null, forceDisplayCheck: true, skipErrorIcon: true, displayAssetName: true),
            });

            // Add the Optional checks
            //entryList.AddRange(new[]
            //{
            //    new Entry(QualityScope.CurrentQuality, InclusiveMode.DXROptional, Style.dxrScreenSpaceShadow, IsDXRScreenSpaceShadowCorrect, null, forceDisplayCheck: true, skipErrorIcon: true, displayAssetName: true),
            //    new Entry(QualityScope.Global, InclusiveMode.DXROptional, Style.dxrScreenSpaceShadowFS, IsDXRScreenSpaceShadowFSCorrect, null, forceDisplayCheck: true, skipErrorIcon: true, displayAssetName: false),
            //});

            return entryList.ToArray();
        }

        internal void ReBuildEntryList()
        {
            m_Entries = BuildEntryList();
        }

        Entry[] m_Entries;
        //To add elements in the Wizard configuration checker,
        //add your new checks in this array at the right position.
        //Both "Fix All" button and UI drawing will use it.
        //Indentation is computed in Entry if you use certain subscope.
        Entry[] entries
        {
            get
            {
                // due to functor, cannot static link directly in an array and need lazy init
                if (m_Entries == null)
                    m_Entries = BuildEntryList();
                return m_Entries;
            }
        }

        // Utility that grab all check within the scope or in sub scope included and check if everything is correct
        bool IsAllEntryCorrectInScope(InclusiveMode scope)
        {
            foreach (var e in entries)
            {
                if (!scope.Contains(e.inclusiveScope) || e.check == null)
                    continue;
                if (!e.check())
                    return false;
            }

            return true;
        }

        // Utility that grab all check and fix within the scope or in sub scope included and performe fix if check return incorrect
        void FixAllEntryInScope(InclusiveMode scope)
        {
            foreach (var e in entries)
            {
                if (!scope.Contains(e.inclusiveScope) || e.check == null || e.fix == null)
                    continue;

                m_Fixer.Add(() =>
                {
                    if (!e.check())
                        e.fix(true);
                });
            }
        }

        #endregion Entry

        #region Reflection methods
        static Func<BuildTarget> CalculateSelectedBuildTarget;
        static Func<BuildTarget, GraphicsDeviceType[]> GetSupportedGraphicsAPIs;
        static Func<BuildTarget, bool> WillEditorUseFirstGraphicsAPI;
        static Action RequestCloseAndRelaunchWithCurrentArguments;
        static Func<int, string, bool> SetRenderingLayerName;

        static void LoadReflectionMethods()
        {
            Type playerSettingsType = typeof(PlayerSettings);
            Type playerSettingsEditorType = playerSettingsType.Assembly.GetType("UnityEditor.PlayerSettingsEditor");
            Type editorUserBuildSettingsUtilsType = playerSettingsType.Assembly.GetType("UnityEditor.EditorUserBuildSettingsUtils");

            Type tagManagerType = playerSettingsType.Assembly.GetType("UnityEditor.TagManager");

            var buildTargetParameter = Expression.Parameter(typeof(BuildTarget), "platform");
            var int32Parameter = Expression.Parameter(typeof(int), "index");
            var stringNameParameter = Expression.Parameter(typeof(string), "name");

            var calculateSelectedBuildTargetInfo = editorUserBuildSettingsUtilsType.GetMethod("CalculateSelectedBuildTarget", BindingFlags.Static | BindingFlags.Public);
            var getSupportedGraphicsAPIsInfo = playerSettingsType.GetMethod("GetSupportedGraphicsAPIs", BindingFlags.Static | BindingFlags.NonPublic);
            var willEditorUseFirstGraphicsAPIInfo = playerSettingsEditorType.GetMethod("WillEditorUseFirstGraphicsAPI", BindingFlags.Static | BindingFlags.NonPublic);
            var requestCloseAndRelaunchWithCurrentArgumentsInfo = typeof(EditorApplication).GetMethod("RequestCloseAndRelaunchWithCurrentArguments", BindingFlags.Static | BindingFlags.NonPublic);
            var setRenderingLayerNameInfo = tagManagerType.GetMethod("Internal_TrySetRenderingLayerName", BindingFlags.Static | BindingFlags.NonPublic);

            var calculateSelectedBuildTargetLambda = Expression.Lambda<Func<BuildTarget>>(Expression.Call(null, calculateSelectedBuildTargetInfo));
            var getSupportedGraphicsAPIsLambda = Expression.Lambda<Func<BuildTarget, GraphicsDeviceType[]>>(Expression.Call(null, getSupportedGraphicsAPIsInfo, buildTargetParameter), buildTargetParameter);
            var willEditorUseFirstGraphicsAPILambda = Expression.Lambda<Func<BuildTarget, bool>>(Expression.Call(null, willEditorUseFirstGraphicsAPIInfo, buildTargetParameter), buildTargetParameter);
            var requestCloseAndRelaunchWithCurrentArgumentsLambda = Expression.Lambda<Action>(Expression.Call(null, requestCloseAndRelaunchWithCurrentArgumentsInfo));
            var setRenderingLayerNameLambda = Expression.Lambda<Func<int, string, bool>>(
                Expression.Call(null, setRenderingLayerNameInfo, int32Parameter, stringNameParameter),
                int32Parameter,
                stringNameParameter
            );

            CalculateSelectedBuildTarget = calculateSelectedBuildTargetLambda.Compile();
            GetSupportedGraphicsAPIs = getSupportedGraphicsAPIsLambda.Compile();
            WillEditorUseFirstGraphicsAPI = willEditorUseFirstGraphicsAPILambda.Compile();
            RequestCloseAndRelaunchWithCurrentArguments = requestCloseAndRelaunchWithCurrentArgumentsLambda.Compile();
            SetRenderingLayerName = setRenderingLayerNameLambda.Compile();
        }
        #endregion Reflection methods

        #region Correct and Fix Functions

        /// <summary>
        /// Pipeline asset
        /// </summary>
        /// <returns></returns>
        bool IsPipelineAssetGraphicsUsedCorrect() => GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset;

        void FixPipelineAssetGraphicsUsed(bool fromAsync)
        {
            Debug.Log("FixGraphicsUsed");
        }

        /// <summary>
        /// Global setting
        /// </summary>
        /// <returns></returns>
        bool IsPipelineGlobalSettingsUsedCorrect()
            => UniversalRenderPipelineGlobalSettings.instance != null;

        void FixPipelineGlobalSettingsUsed(bool fromAsync)
            => UniversalRenderPipelineGlobalSettings.Ensure();

        /// <summary>
        /// Linear color space
        /// </summary>
        /// <returns></returns>
        bool IsColorSpaceCorrect()
            => PlayerSettings.colorSpace == ColorSpace.Linear;

        void FixColorSpace(bool fromAsyncUnused)
            => PlayerSettings.colorSpace = ColorSpace.Linear;

        /// <summary>
        /// Directional Light preset check
        /// </summary>
        /// <returns></returns>
        bool IsPresetsDirectionalLightCorrect()
            => DanbaidongRPPresetUtils.CheckPresetIsDefault(ref m_pipelinePresets.directionalLightPreset);

        void FixPresetsDirectionalLight(bool fromAsyncUnused)
            => DanbaidongRPPresetUtils.InsertAsFirstDefault(ref m_pipelinePresets.directionalLightPreset);

        /// <summary>
        /// Point Light preset check
        /// </summary>
        /// <returns></returns>
        bool IsPresetsPointLightCorrect()
            => DanbaidongRPPresetUtils.CheckPresetIsDefault(ref m_pipelinePresets.pointLightPreset);

        void FixPresetsPointLight(bool fromAsyncUnused)
            => DanbaidongRPPresetUtils.InsertAsFirstDefault(ref m_pipelinePresets.pointLightPreset);

        /// <summary>
        /// Spoy Light preset check
        /// </summary>
        /// <returns></returns>
        bool IsPresetsSpotLightCorrect()
            => DanbaidongRPPresetUtils.CheckPresetIsDefault(ref m_pipelinePresets.spotLightPreset);

        void FixPresetsSpotLight(bool fromAsyncUnused)
            => DanbaidongRPPresetUtils.InsertAsFirstDefault(ref m_pipelinePresets.spotLightPreset);

        /// <summary>
        /// MeshRenderer preset check
        /// </summary>
        /// <returns></returns>
        bool IsPresetsMeshRendererCorrect()
            => DanbaidongRPPresetUtils.CheckPresetIsDefault(ref m_pipelinePresets.meshRendererPreset);

        void FixPresetsMeshRendererLight(bool fromAsyncUnused)
            => DanbaidongRPPresetUtils.InsertAsFirstDefault(ref m_pipelinePresets.meshRendererPreset);

        /// <summary>
        /// DXR DX12
        /// </summary>
        /// <returns></returns>
        bool IsDXRDirect3D12Correct()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 && !DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12;
        }

        void FixDXRDirect3D12(bool fromAsyncUnused)
        {
            if (GetSupportedGraphicsAPIs(CalculateSelectedBuildTarget()).Contains(GraphicsDeviceType.Direct3D12))
            {
                var buidTarget = CalculateSelectedBuildTarget();
                if (PlayerSettings.GetGraphicsAPIs(buidTarget).Contains(GraphicsDeviceType.Direct3D12))
                {
                    PlayerSettings.SetGraphicsAPIs(
                        buidTarget,
                        new[] { GraphicsDeviceType.Direct3D12 }
                            .Concat(
                            PlayerSettings.GetGraphicsAPIs(buidTarget)
                                .Where(x => x != GraphicsDeviceType.Direct3D12))
                            .ToArray());
                }
                else
                {
                    PlayerSettings.SetGraphicsAPIs(
                        buidTarget,
                        new[] { GraphicsDeviceType.Direct3D12 }
                            .Concat(PlayerSettings.GetGraphicsAPIs(buidTarget))
                            .ToArray());
                }
                DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12 = true;
                m_Fixer.Add(() => ChangedFirstGraphicAPI(buidTarget)); //register reboot at end of operations
            }
        }

        void ChangedFirstGraphicAPI(BuildTarget target)
        {
            //It seams that the 64 version is not check for restart for a strange reason
            if (target == BuildTarget.StandaloneWindows64)
                target = BuildTarget.StandaloneWindows;

            // If we're changing the first API for relevant editor, this will cause editor to switch: ask for scene save & confirmation
            if (WillEditorUseFirstGraphicsAPI(target))
            {
                if (EditorUtility.DisplayDialog("Changing editor graphics device",
                    "You've changed the active graphics API. This requires a restart of the Editor. After restarting, finish fixing DXR configuration by launching the wizard again.",
                    "Restart Editor", "Not now"))
                {
                    DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12 = false;
                    RequestCloseAndRelaunchWithCurrentArguments();
                }
                else
                    EditorApplication.quitting += () => DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12 = false;
            }
        }

        void CheckPersistantNeedReboot()
        {
            if (DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12)
                EditorApplication.quitting += () => DanbaidongUserSettings.wizardNeedRestartAfterChangingToDX12 = false;
        }

        /// <summary>
        /// Raytracing setting.
        /// </summary>
        /// <returns></returns>
        bool IsPipelineAssetRaytracingCorrect()
        {
            var pipelineAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            return pipelineAsset.supportsRayTracing;
        }

        /// <summary>
        /// Renderinglayers
        /// </summary>
        /// <returns></returns>
        bool IsRenderingLayersMainLightShadowCorrect()
        {
            var names = RenderingLayerMask.RenderingLayerToName(1);
            return string.Equals(names, Style.mainlightshadows);
        }

        void FixRenderingLayersMainLightShadow(bool fromAsyncUnused)
        {
            SetRenderingLayerName(1, Style.mainlightshadows);
        }

        #endregion Correct and Fix Functions
    }

}
