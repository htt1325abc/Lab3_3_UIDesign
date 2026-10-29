#if UNITY_EDITOR
using System.IO;
using Lab33;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lab33Editor
{
    /// <summary>
    /// Rebuilds the complete Lab 3.3 project: 3D takeoff demo, TMP popup, trigger and next scene.
    /// </summary>
    public static class Lab33ProjectBuilder
    {
        private const string Root = "Assets/Lab33";
        private const string Scenes = Root + "/Scenes";
        private const string Materials = Root + "/Materials";
        private const string Fonts = Root + "/Fonts";
        private const string DemoScene = Scenes + "/Lab33_UI_Popup.unity";
        private const string CompleteScene = Scenes + "/Lab33_Complete.unity";

        private static readonly Color Night = new(0.025f, 0.065f, 0.12f, 1f);
        private static readonly Color Panel = new(0.055f, 0.14f, 0.22f, 0.98f);
        private static readonly Color PanelEdge = new(0.10f, 0.29f, 0.40f, 1f);
        private static readonly Color Cyan = new(0.34f, 0.84f, 0.91f, 1f);
        private static readonly Color Amber = new(1f, 0.60f, 0.20f, 1f);
        private static readonly Color Mist = new(0.82f, 0.91f, 0.95f, 1f);

        [MenuItem("Lab 3.3/Build Complete UI Lab")]
        public static void Build()
        {
            EnsureFolder(Root);
            EnsureFolder(Scenes);
            EnsureFolder(Materials);
            EnsureFolder(Fonts);
            EnsureTmpSettings();

            TMP_FontAsset font = CreateFontAsset();
            Material runway = CreateMaterial("Runway", new Color(0.055f, 0.07f, 0.09f), 0.05f, 0.22f);
            Material ground = CreateMaterial("Ground", new Color(0.025f, 0.065f, 0.07f), 0.02f, 0.18f);
            Material white = CreateMaterial("Aircraft White", new Color(0.82f, 0.90f, 0.94f), 0.48f, 0.72f);
            Material navy = CreateMaterial("Aircraft Navy", new Color(0.035f, 0.13f, 0.21f), 0.55f, 0.68f);
            Material amber = CreateMaterial("Runway Amber", Amber, 0.18f, 0.58f);
            Material cyan = CreateMaterial("Gate Cyan", Cyan, 0.15f, 0.70f, true);

            CreateDemoScene(font, runway, ground, white, navy, amber, cyan);
            CreateCompleteScene(font, ground, cyan, amber);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(DemoScene, true),
                new EditorBuildSettingsScene(CompleteScene, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(DemoScene);
            Debug.Log("[Lab 3.3] Build complete: TMP popup, Enter Wall trigger, Restart and Continue are ready.");
        }

        [MenuItem("Lab 3.3/Open Demo and Play")]
        public static void OpenDemoAndPlay()
        {
            EditorSceneManager.OpenScene(DemoScene, OpenSceneMode.Single);
            EditorApplication.ExecuteMenuItem("Window/General/Game");
            EditorApplication.delayCall += () =>
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    EditorApplication.isPlaying = true;
                }
            };
        }

        private static void CreateDemoScene(TMP_FontAsset font, Material runway, Material ground, Material white, Material navy, Material amber, Material cyan)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.11f, 0.20f, 0.30f);
            RenderSettings.ambientEquatorColor = new Color(0.035f, 0.09f, 0.12f);
            RenderSettings.ambientGroundColor = new Color(0.01f, 0.025f, 0.03f);

            CreateEnvironment(runway, ground, amber, cyan);
            AirplaneTakeoffController airplane = CreateAirplane(white, navy, amber);

            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Night;
            camera.fieldOfView = 52f;
            cameraObject.transform.position = airplane.transform.position + new Vector3(0f, 5.4f, -9.5f);
            cameraObject.AddComponent<AudioListener>();
            RunwayCamera follow = cameraObject.AddComponent<RunwayCamera>();
            follow.SetTarget(airplane.transform);

            Canvas gameMenu = CreatePopupCanvas(font, out TMP_Text title, out TMP_Text message, out Button continueButton, out Button restartButton);
            GameManager manager = cameraObject.AddComponent<GameManager>();
            manager.Configure(gameMenu, title, message, continueButton, restartButton);
            UnityEventTools.AddPersistentListener(continueButton.onClick, manager.NextScene);
            UnityEventTools.AddPersistentListener(restartButton.onClick, manager.RestartScene);

            GameObject gate = GameObject.Find("Enter Wall");
            EnterGame enterGame = gate.AddComponent<EnterGame>();
            enterGame.Configure(gameMenu, manager, airplane);

            CreateEventSystem();
            gameMenu.enabled = false;
            EditorSceneManager.SaveScene(scene, DemoScene);
        }

        private static void CreateCompleteScene(TMP_FontAsset font, Material ground, Material cyan, Material amber)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Night;
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.AddComponent<AudioListener>();

            Canvas canvas = CreateCanvas("Completion Canvas");
            CreateImage(canvas.transform, "Backdrop", new Color(0.01f, 0.035f, 0.065f, 1f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            CreateImage(canvas.transform, "Signal Line", Cyan, new Vector2(0f, 0.76f), new Vector2(1f, 0.76f), Vector2.zero, new Vector2(0f, 4f));

            CreateText(canvas.transform, "Eyebrow", "TRAINING SECTOR 08  •  ARRIVAL CONFIRMED", font, 24f, Cyan, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 190f), new Vector2(1200f, 50f));
            CreateText(canvas.transform, "Title", "LAB 3.3\nCOMPLETE", font, 82f, Color.white, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 55f), new Vector2(1100f, 210f));
            CreateText(canvas.transform, "Message", "The Continue button loaded this second scene successfully.", font, 27f, Mist, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -85f), new Vector2(1000f, 70f));

            CompletionScreen controller = cameraObject.AddComponent<CompletionScreen>();
            Button replay = CreateButton(canvas.transform, "Replay Button", "REPLAY DEMO", font, new Vector2(0f, -205f), new Vector2(360f, 76f), Amber, Night);
            UnityEventTools.AddPersistentListener(replay.onClick, controller.ReplayDemo);

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, CompleteScene);
        }

        private static void CreateEnvironment(Material runway, Material ground, Material amber, Material cyan)
        {
            GameObject terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
            terrain.name = "Airfield Ground";
            terrain.transform.position = new Vector3(0f, -0.15f, 13f);
            terrain.transform.localScale = new Vector3(8f, 1f, 7f);
            terrain.GetComponent<Renderer>().sharedMaterial = ground;

            GameObject runwayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            runwayObject.name = "Runway 07";
            runwayObject.transform.position = new Vector3(0f, 0f, 13f);
            runwayObject.transform.localScale = new Vector3(7f, 0.20f, 42f);
            runwayObject.GetComponent<Renderer>().sharedMaterial = runway;

            for (int z = -6; z <= 31; z += 4)
            {
                GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.name = "Centerline " + z;
                stripe.transform.position = new Vector3(0f, 0.12f, z);
                stripe.transform.localScale = new Vector3(0.18f, 0.025f, 1.7f);
                stripe.GetComponent<Renderer>().sharedMaterial = amber;
                RemoveCollider(stripe);
            }

            for (int z = -7; z <= 32; z += 2)
            {
                CreateRunwayLight(new Vector3(-3.15f, 0.18f, z), cyan);
                CreateRunwayLight(new Vector3(3.15f, 0.18f, z), cyan);
            }

            GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gate.name = "Enter Wall";
            gate.transform.position = new Vector3(0f, 3.2f, 17f);
            gate.transform.localScale = new Vector3(8.2f, 6.2f, 0.35f);
            gate.GetComponent<Renderer>().sharedMaterial = cyan;
            BoxCollider gateCollider = gate.GetComponent<BoxCollider>();
            gateCollider.isTrigger = true;

            GameObject gateLabel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gateLabel.name = "Gate Signal";
            gateLabel.transform.position = new Vector3(0f, 6.45f, 17f);
            gateLabel.transform.localScale = new Vector3(8.5f, 0.16f, 0.5f);
            gateLabel.GetComponent<Renderer>().sharedMaterial = amber;
            RemoveCollider(gateLabel);

            GameObject sun = new("Directional Light");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(0.74f, 0.86f, 1f);
            sun.transform.rotation = Quaternion.Euler(38f, -28f, 0f);

            GameObject runwayGlow = new("Runway Glow");
            Light point = runwayGlow.AddComponent<Light>();
            point.type = LightType.Point;
            point.range = 24f;
            point.intensity = 5f;
            point.color = new Color(0.16f, 0.68f, 0.82f);
            runwayGlow.transform.position = new Vector3(0f, 4f, 12f);
        }

        private static void CreateRunwayLight(Vector3 position, Material material)
        {
            GameObject light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            light.name = "Runway Edge Light";
            light.transform.position = position;
            light.transform.localScale = Vector3.one * 0.13f;
            light.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(light);
        }

        private static AirplaneTakeoffController CreateAirplane(Material white, Material navy, Material amber)
        {
            GameObject root = new("Airplane");
            root.tag = "Player";
            root.transform.position = new Vector3(0f, 0.58f, -5f);
            Rigidbody body = root.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            BoxCollider collider = root.AddComponent<BoxCollider>();
            collider.size = new Vector3(4.8f, 1.2f, 4.4f);
            AirplaneTakeoffController controller = root.AddComponent<AirplaneTakeoffController>();

            CreatePart("Fuselage", root.transform, PrimitiveType.Capsule, new Vector3(0f, 0f, 0f), new Vector3(0.72f, 1.55f, 0.72f), white, new Vector3(90f, 0f, 0f));
            CreatePart("Cockpit", root.transform, PrimitiveType.Sphere, new Vector3(0f, 0.34f, 0.55f), new Vector3(0.55f, 0.32f, 0.68f), navy);
            CreatePart("Nose", root.transform, PrimitiveType.Sphere, new Vector3(0f, 0f, 1.42f), new Vector3(0.50f, 0.45f, 0.72f), amber);
            CreatePart("Main Wing", root.transform, PrimitiveType.Cube, new Vector3(0f, 0f, 0.05f), new Vector3(4.8f, 0.13f, 1.0f), white);
            CreatePart("Wing Signal", root.transform, PrimitiveType.Cube, new Vector3(0f, 0.09f, 0.08f), new Vector3(3.0f, 0.04f, 0.16f), amber);
            CreatePart("Tail Wing", root.transform, PrimitiveType.Cube, new Vector3(0f, 0.16f, -1.25f), new Vector3(2.1f, 0.10f, 0.48f), white);
            CreatePart("Tail Fin", root.transform, PrimitiveType.Cube, new Vector3(0f, 0.58f, -1.25f), new Vector3(0.12f, 0.80f, 0.58f), navy, new Vector3(15f, 0f, 0f));
            return controller;
        }

        private static Canvas CreatePopupCanvas(TMP_FontAsset font, out TMP_Text title, out TMP_Text message, out Button continueButton, out Button restartButton)
        {
            Canvas canvas = CreateCanvas("GameMenu");
            CreateImage(canvas.transform, "Dimmer", new Color(0.005f, 0.02f, 0.04f, 0.78f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject panel = CreateImage(canvas.transform, "Panel", Panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(920f, 560f));
            GameObject edge = CreateImage(panel.transform, "Frame", PanelEdge, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-18f, -18f));
            Image edgeImage = edge.GetComponent<Image>();
            edgeImage.color = new Color(0.08f, 0.24f, 0.34f, 1f);

            CreateImage(panel.transform, "Runway Signal", Amber, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -4f), new Vector2(0f, 8f));
            CreateText(panel.transform, "Eyebrow", "FLIGHT CONTROL  •  DEPARTURE GATE 07", font, 20f, Cyan, TextAlignmentOptions.Center,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(800f, 40f));
            title = CreateText(panel.transform, "Title", "Title", font, 64f, Color.white, TextAlignmentOptions.Center,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(820f, 90f));
            message = CreateText(panel.transform, "Message", "Message", font, 26f, Mist, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 18f), new Vector2(760f, 120f));

            GameObject divider = CreateImage(panel.transform, "Divider", new Color(Cyan.r, Cyan.g, Cyan.b, 0.35f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -78f), new Vector2(760f, 2f));
            divider.transform.SetAsLastSibling();

            GameObject actions = new("Actions", typeof(RectTransform));
            actions.transform.SetParent(panel.transform, false);
            RectTransform actionsRect = actions.GetComponent<RectTransform>();
            actionsRect.anchorMin = actionsRect.anchorMax = new Vector2(0.5f, 0f);
            actionsRect.anchoredPosition = new Vector2(0f, 94f);
            actionsRect.sizeDelta = new Vector2(760f, 84f);

            restartButton = CreateButton(actions.transform, "Restart", "Restart", font, new Vector2(-205f, 0f), new Vector2(330f, 72f), PanelEdge, Mist);
            continueButton = CreateButton(actions.transform, "Continue", "Enter", font, new Vector2(205f, 0f), new Vector2(330f, 72f), Amber, Night);

            CreateText(panel.transform, "Footnote", "Popup appears only after takeoff + Enter Wall collision", font, 17f, new Color(0.50f, 0.67f, 0.75f), TextAlignmentOptions.Center,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(760f, 32f));
            return canvas;
        }

        private static Canvas CreateCanvas(string name)
        {
            GameObject canvasObject = new(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static GameObject CreateImage(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            GameObject imageObject = new(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            imageObject.GetComponent<Image>().color = color;
            return imageObject;
        }

        private static TextMeshProUGUI CreateText(Transform parent, string name, string value, TMP_FontAsset font, float fontSize, Color color,
            TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            GameObject textObject = new(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label, TMP_FontAsset font, Vector2 position, Vector2 size, Color baseColor, Color labelColor)
        {
            GameObject buttonObject = new(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = baseColor;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = Color.Lerp(baseColor, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(baseColor, Color.black, 0.25f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.35f);
            colors.fadeDuration = 0.12f;
            button.colors = colors;

            CreateText(buttonObject.transform, "Label", label, font, 23f, labelColor, TextAlignmentOptions.Center,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        private static void CreateEventSystem()
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static void EnsureTmpSettings()
        {
            const string tmpRoot = "Assets/TextMesh Pro";
            const string tmpResources = tmpRoot + "/Resources";
            const string settingsPath = tmpResources + "/TMP Settings.asset";

            string mobileShaderPath = Path.Combine(
                Application.dataPath,
                "TextMesh Pro/Shaders/TMP_SDF-Mobile.shader");
            if (!File.Exists(mobileShaderPath))
            {
                UnityEditor.PackageManager.PackageInfo package =
                    UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
                string essentialResources = Path.Combine(
                    package.resolvedPath,
                    "Package Resources/TMP Essential Resources.unitypackage");
                AssetDatabase.ImportPackage(essentialResources, false);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Debug.Log("[Lab 3.3] Imported TMP Essential Resources from " + essentialResources);
            }

            EnsureFolder(tmpRoot);
            EnsureFolder(tmpResources);

            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(settingsPath) == null)
            {
                TMP_Settings settings = ScriptableObject.CreateInstance<TMP_Settings>();
                settings.name = "TMP Settings";
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            TMP_Settings.LoadDefaultSettings();
        }

        private static TMP_FontAsset CreateFontAsset()
        {
            string path = Fonts + "/Lab33FlightControlFont.asset";
            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (existing != null) AssetDatabase.DeleteAsset(path);

            string fontPath = Fonts + "/RobotoMono-Bold.ttf";
            if (!File.Exists(fontPath))
            {
                string editorFont = Path.Combine(EditorApplication.applicationContentsPath, "Resources/Fonts/RobotoMono-Bold.ttf");
                File.Copy(editorFont, fontPath);
                AssetDatabase.ImportAsset(fontPath, ImportAssetOptions.ForceSynchronousImport);
            }

            Font source = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (source == null)
            {
                throw new FileNotFoundException("Unable to load the Unity editor font used by Lab 3.3.", fontPath);
            }

            TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(source);
            font.name = "Lab33 Flight Control Font";
            AssetDatabase.CreateAsset(font, path);
            font.atlasTextures[0].name = "Lab33 Flight Control Atlas";
            font.material.name = "Lab33 Flight Control Material";
            AssetDatabase.AddObjectToAsset(font.atlasTextures[0], font);
            AssetDatabase.AddObjectToAsset(font.material, font);
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
            return font;
        }

        private static Material CreateMaterial(string name, Color color, float metallic, float smoothness, bool transparent = false)
        {
            string path = Materials + "/" + name + ".mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) AssetDatabase.DeleteAsset(path);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            Material material = new(shader) { name = name, color = color };
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);

            if (transparent && material.HasProperty("_Mode"))
            {
                color.a = 0.22f;
                material.color = color;
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static GameObject CreatePart(string name, Transform parent, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Material material, Vector3? localEuler = null)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            if (localEuler.HasValue) part.transform.localEulerAngles = localEuler.Value;
            part.GetComponent<Renderer>().sharedMaterial = material;
            RemoveCollider(part);
            return part;
        }

        private static void RemoveCollider(GameObject gameObject)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent ?? "Assets", name);
        }
    }
}
#endif
