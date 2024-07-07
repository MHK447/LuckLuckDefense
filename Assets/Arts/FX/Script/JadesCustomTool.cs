using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class JadesCustomTool : EditorWindow
{
    class ParticleSystemAnalysisResult
    {
        public int ShaderComplexityScore;
        public int ParticleCount;
        public float OverdrawEstimate;
        public long ResourceMemoryUsage;
    }

    private List<Material> selectedMaterials = new List<Material>();
    private List<Material> usedMaterials = new List<Material>();
    private List<Material> unusedMaterials = new List<Material>();
    private Vector2 scrollPos;
    private Vector2 scrollPosUsed;
    private Vector2 scrollPosUnused;
    private Vector2 scrollPosition;
    private bool showEditParticleSection = true;
    private bool showEtcSection = false;  // Etc 섹션의 foldout 상태
    private bool showOptimizeSection = false;  // Optimize Section의 접힘 상태

    private Dictionary<ParticleSystem, ParticleSystemAnalysisResult> analysisResults = new Dictionary<ParticleSystem, ParticleSystemAnalysisResult>();


    private float hueSlider = 0.0f;
    private float sizeMultiplier = 1.0f;
    private float delayAddition = 0.0f;
    private float sortingFudge;

    private bool editCurrentOnly = true;
    private bool editChildren = false;

    private GUIStyle sectionTitleStyle;
    private bool styleInitialized = false;

    private bool isAnalyzeModeActive = false;

    private List<UnityEngine.Object> namingObjects = new List<UnityEngine.Object>();
    private List<string> previewNames = new List<string>();
    private string prefix = "";
    private bool showNamingToolSection = false; // Naming Tool 섹션의 foldout 상태
    private Camera sceneCamera;




    [MenuItem("Window/Jade FX Tool")]
    public static void ShowWindow()
    {
        GetWindow<JadesCustomTool>("Jade FX Tool");
    }

    private void InitializeStyle()
    {
        if (!styleInitialized)
        {
            sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            styleInitialized = true;
        }
    }

    private void OnGUI()
    {
        InitializeStyle();

        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.Space(10);
        GUILayout.Label("Jade FX Tool v0.17", sectionTitleStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        showEditParticleSection = EditorGUILayout.Foldout(showEditParticleSection, "Particle System Tool");
        if (showEditParticleSection)
        {
            DrawEditParticleSection();
        }

        showEtcSection = EditorGUILayout.Foldout(showEtcSection, "Check Material Tool");
        if (showEtcSection)
        {
            DrawEtcSection();
        }

        showOptimizeSection = EditorGUILayout.Foldout(showOptimizeSection, "Optimize Tool");
        if (showOptimizeSection)
        {
            DrawOptimizeSection();
        }

        showNamingToolSection = EditorGUILayout.Foldout(showNamingToolSection, "Naming Tool");
if (showNamingToolSection)
{
    DrawNamingToolSection();
}

        GUILayout.EndScrollView();
    }

    private void DrawEditParticleSection()
    {

        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.Space(10);
        editCurrentOnly = EditorGUILayout.Toggle("Current Particle Only", editCurrentOnly);
        if (editCurrentOnly)
        {
            editChildren = false;
        }

        editChildren = EditorGUILayout.Toggle("Current and Its Children", editChildren);
        if (editChildren)
        {
            editCurrentOnly = false;
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        
        if (GUILayout.Button("상위 파티클 루트 추가"))
        {
            CreateEmptyRootWithParticleSystem();
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);
        // Resize


        EditorGUILayout.BeginVertical("helpbox");

        

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        sizeMultiplier = EditorGUILayout.FloatField("Size Multiplier", sizeMultiplier);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("파티클 스케일 적용"))
        {
            ApplyResize();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        delayAddition = EditorGUILayout.FloatField("Delay Amount", delayAddition);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("파티클 딜레이 적용" ))
        {
            ApplyDelayAdjustment();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical("helpbox");

        // Sorting Fudge 조정 섹션
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        sortingFudge = EditorGUILayout.FloatField("Sorting Fudge Amount", sortingFudge);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Sorting Fudge Amount 일괄 더하기"))
        {
            ApplySortingFudgeAdjustment();
        }

        if (GUILayout.Button("Sorting Fudge 0으로 초기화"))
        {
            ResetSortingFudge();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);



        EditorGUILayout.BeginVertical("helpbox");
        if (GUILayout.Button("파티클 정리"))
    {
        if (editCurrentOnly)
        {
            if (Selection.activeGameObject != null)
            {
                var ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
                if (ps != null) ClearParticle(ps);
            }
        }
        else if (editChildren)
        {
            if (Selection.activeGameObject != null)
            {
                var particles = Selection.activeGameObject.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in particles)
                {
                    ClearParticle(ps);
                }
            }
        }
    }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }

    private void ApplyColorAdjustment()
    {
        ApplyEffectBasedOnSelection(EffectType.Color);
    }

    private void ApplyResize()
    {
        ApplyEffectBasedOnSelection(EffectType.Resize);
    }

    private void ApplyDelayAdjustment()
    {
        ApplyEffectBasedOnSelection(EffectType.Delay);
    }
    private void ApplyEffectToParticles(GameObject obj, EffectType effectType)
    {
        ParticleSystem particleSystem = obj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {

            Undo.RecordObject(particleSystem, "Particle System Edit");
            var mainModule = particleSystem.main;

            switch (effectType)
            {
                case EffectType.Color:
                    // 색상 조정 로직...
                    break;
                case EffectType.Resize:
                    ApplyResizeEffects(mainModule, particleSystem);
                    break;
                case EffectType.Delay:
                    ApplyDelayEffect(ref mainModule);
                    break;
            }

            particleSystem.Play(); // 변경 사항 적용을 위해 파티클 시스템 재시작
        }
    }

    private void ApplyResizeEffects(ParticleSystem.MainModule mainModule, ParticleSystem particleSystem)
    {
        if (mainModule.startSize3D)
        {
            mainModule.startSizeX = ResizeParticleParameter(mainModule.startSizeX);
            mainModule.startSizeY = ResizeParticleParameter(mainModule.startSizeY);
            mainModule.startSizeZ = ResizeParticleParameter(mainModule.startSizeZ);
        }
        else
        {
            mainModule.startSize = ResizeParticleParameter(mainModule.startSize);
        }

        mainModule.startSpeed = ResizeParticleParameter(mainModule.startSpeed);
        var shapeModule = particleSystem.shape;
        shapeModule.scale *= sizeMultiplier;
    }

    private ParticleSystem.MinMaxCurve ResizeParticleParameter(ParticleSystem.MinMaxCurve parameter)
    {
        switch (parameter.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return new ParticleSystem.MinMaxCurve(parameter.constant * sizeMultiplier);
            case ParticleSystemCurveMode.TwoConstants:
                return new ParticleSystem.MinMaxCurve(parameter.constantMin * sizeMultiplier, parameter.constantMax * sizeMultiplier);
            default:
                return parameter; // Curve 모드와 Two Curves 모드 처리 필요
        }
    }


    private void ApplyDelayEffect(ref ParticleSystem.MainModule mainModule)
    {
        mainModule.startDelay = AdjustDelayParameter(mainModule.startDelay);
    }

    private ParticleSystem.MinMaxCurve AdjustDelayParameter(ParticleSystem.MinMaxCurve parameter)
    {
        switch (parameter.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return new ParticleSystem.MinMaxCurve(parameter.constant + delayAddition);
            case ParticleSystemCurveMode.TwoConstants:
                return new ParticleSystem.MinMaxCurve(parameter.constantMin + delayAddition, parameter.constantMax + delayAddition);
            default:
                return parameter; // Curve 모드와 Two Curves 모드 처리 필요
        }
    }
    

  private void ClearParticle(ParticleSystem ps)
{
    var main = ps.main;
    var renderer = ps.GetComponent<ParticleSystemRenderer>();
    var emission = ps.emission;

    // Main 모듈 설정 변경
    main.useUnscaledTime = false;
    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
    main.playOnAwake = true;

    // Renderer 모듈 설정 변경
    if (renderer != null)
    {
        renderer.sortingLayerID = SortingLayer.NameToID("Default");
    }

    // Renderer와 Emission 모듈 상태에 따른 추가 처리
    if (renderer != null)
    {
        if (!renderer.enabled && emission.enabled)
        {
            // Renderer가 꺼져 있고 Emission이 켜져 있으면, Emission을 끄고 Renderer 초기화
            emission.enabled = false;
            ResetRenderer(renderer);
        }
        else if (renderer.enabled && !emission.enabled)
        {
            // Renderer가 켜져 있고 Emission이 꺼져 있으면, Renderer를 끄고 초기화
            renderer.enabled = false;
            ResetRenderer(renderer);
        }
    }
    if (!renderer.enabled && !emission.enabled)
    {
        ResetRenderer(renderer);
    }
}

private void ResetRenderer(ParticleSystemRenderer renderer)
{
    renderer.material = null;
    renderer.trailMaterial = null;
    renderer.mesh = null;
    renderer.renderMode = ParticleSystemRenderMode.Billboard;
}


    private void CreateEmptyRootWithParticleSystem()
    {
        List<GameObject> createdParents = new List<GameObject>();
        foreach (var selectedObject in Selection.gameObjects)
        {
            // 새로운 부모 오브젝트 생성
            GameObject newParent = new GameObject("pos");

            // 새로운 부모 오브젝트의 월드 트랜스폼을 선택된 오브젝트의 월드 트랜스폼에 맞춤
            newParent.transform.position = selectedObject.transform.position;
            newParent.transform.rotation = selectedObject.transform.rotation;
            newParent.transform.localScale = selectedObject.transform.lossyScale;

            // 선택된 오브젝트의 현재 부모를 임시로 저장
            Transform originalParent = selectedObject.transform.parent;

            // 선택된 오브젝트를 새로운 부모의 자식으로 설정
            selectedObject.transform.SetParent(newParent.transform, false);

            // 새로운 부모 오브젝트를 원래의 부모 아래로 이동
            newParent.transform.SetParent(originalParent, true);

            // 새로운 부모 오브젝트의 로컬 트랜스폼 초기화
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localRotation = Quaternion.identity;
            newParent.transform.localScale = Vector3.one;

            // 새로운 부모에 파티클 시스템 추가 및 설정
            ParticleSystem particleSystem = newParent.AddComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.duration = 5;
            main.loop = false;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.enabled = false;

            DisableAllParticleSystemModules(particleSystem);
            createdParents.Add(newParent);
        }
        Selection.objects = createdParents.ToArray();
    }
    private void DisableAllParticleSystemModules(ParticleSystem particleSystem)
    {
        var emission = particleSystem.emission;
        emission.enabled = false;

        var shape = particleSystem.shape;
        shape.enabled = false;


    }


    private void ApplyEffectBasedOnSelection(EffectType effectType)
    {
        if (editCurrentOnly)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                ApplyEffectToParticles(obj, effectType);
            }
        }
        else if (editChildren)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                ApplyEffectToAllChildren(obj, effectType);
            }
        }
    }
    private void ApplyEffectToAllChildren(GameObject obj, EffectType effectType)
    {
        ApplyEffectToParticles(obj, effectType);

        foreach (Transform child in obj.transform)
        {
            ApplyEffectToAllChildren(child.gameObject, effectType);
        }
    }


    private enum EffectType
    {
        Color,
        Resize,
        Delay
    }


    private void ApplySortingFudgeAdjustment()
    {
        if (Selection.activeGameObject != null)
        {
            var ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
            if (ps != null) AddToSortingFudge(ps, sortingFudge);

            if (editChildren)
            {
                foreach (Transform child in Selection.activeGameObject.transform)
                {
                    var childPs = child.GetComponent<ParticleSystem>();
                    if (childPs != null) AddToSortingFudge(childPs, sortingFudge);
                }
            }
        }
    }


    private void AddToSortingFudge(ParticleSystem ps, float addValue)
    {
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingFudge += addValue;
        }
    }

    private void SetSortingFudge(ParticleSystem ps, float fudgeValue)
    {
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingFudge = fudgeValue;
        }
    }

    private void ResetSortingFudge()
    {
        if (Selection.activeGameObject != null)
        {
            var ps = Selection.activeGameObject.GetComponent<ParticleSystem>();
            if (ps != null) SetSortingFudge(ps, 0);

            if (editChildren)
            {
                foreach (Transform child in Selection.activeGameObject.transform)
                {
                    var childPs = child.GetComponent<ParticleSystem>();
                    if (childPs != null) SetSortingFudge(childPs, 0);
                }
            }
        }
    }



    private void DrawEtcSection()
    {

        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.Space(10);
        if (GUILayout.Button("머테리얼 선택"))
        {
            SelectMaterials();
        }

        if (GUILayout.Button("선택 목록 초기화"))
        {
            selectedMaterials.Clear();
            unusedMaterials.Clear();
        }

        if (selectedMaterials.Count > 0 && GUILayout.Button("현재 씬 사용여부 검색"))
        {
            CheckMaterialUsageInScene();
        }

        if (selectedMaterials.Count > 0)
        {
            GUILayout.Label("사용된 머테리얼 목록:");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
            foreach (var material in selectedMaterials)
            {
                EditorGUILayout.ObjectField(material, typeof(Material), false);
            }
            EditorGUILayout.EndScrollView();
        }

        if (selectedMaterials.Count > 0)
        {
            GUILayout.Label("사용되지 않는 머테리얼 목록:");
            scrollPosUnused = EditorGUILayout.BeginScrollView(scrollPosUnused, GUILayout.Height(100));
            foreach (var material in unusedMaterials)
            {
                EditorGUILayout.ObjectField(material, typeof(Material), false);
            }
            EditorGUILayout.EndScrollView();

        }

        if (selectedMaterials.Count > 0 && GUILayout.Button("사용된 머테리얼 선택"))
        {
            SelectMaterialsInProject(usedMaterials);
        }


        if (unusedMaterials.Count > 0 && GUILayout.Button("사용되지 않는 머테리얼 선택"))
        {
            SelectMaterialsInProject(unusedMaterials);
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }


    private void SelectMaterials()
    {
        selectedMaterials.Clear();
        usedMaterials.Clear();
        unusedMaterials.Clear();

        string[] guids = Selection.assetGUIDs;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            Debug.Log($"GUID: {guid}, Asset Path: {assetPath}");

            if (material != null)
            {
                selectedMaterials.Add(material);
                Debug.Log($"Selected Material: {material.name}");
            }
            else
            {
                Debug.Log($"No material found at path: {assetPath}");
            }
        }

        if (selectedMaterials.Count == 0)
        {
            Debug.Log("No materials selected in the project view.");
        }

        Repaint(); // 에디터 윈도우 강제 새로고침
    }

    private void CheckMaterialUsageInScene()
    {
        usedMaterials.Clear();
        unusedMaterials.Clear();

        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (var material in selectedMaterials)
        {
            bool isUsed = false;
            foreach (var renderer in renderers)
            {
                if (Array.IndexOf(renderer.sharedMaterials, material) >= 0)
                {
                    usedMaterials.Add(material);
                    isUsed = true;
                    break;
                }
            }
            if (!isUsed)
            {
                unusedMaterials.Add(material);
            }
        }
    }

    private void SelectMaterialsInProject(List<Material> materials)
    {
        UnityEngine.Object[] objects = new UnityEngine.Object[materials.Count];
        for (int i = 0; i < materials.Count; i++)
        {
            objects[i] = materials[i];
        }
        Selection.objects = objects;
    }

    void OnEnable()
    {
       EditorApplication.update += UpdateAnalysis;
    }

    void OnDisable()
    {
       EditorApplication.update -= UpdateAnalysis;
    }


    private void DrawOptimizeSection()
    {
        
        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.Space(10);

        if (GUILayout.Button("분석 모드 활성화"))
        {
        isAnalyzeModeActive = true;
        EditorApplication.update += UpdateAnalysis;
         }


        if (GUILayout.Button("분석 모드 중지"))
         {
        isAnalyzeModeActive = false;
        EditorApplication.update -= UpdateAnalysis;
        analysisResults.Clear();
        Repaint();
          }


        if (isAnalyzeModeActive)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.Space(5);
            foreach (var entry in analysisResults)
             {
             GUILayout.Label($"{entry.Key.gameObject.name}: 쉐이더 복잡도: {entry.Value.ShaderComplexityScore}, 파티클 수: {entry.Value.ParticleCount}, 오버드로우 추정치: {entry.Value.OverdrawEstimate:F2}, 리소스 사용량: {entry.Value.ResourceMemoryUsage} 바이트");
              }

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        if (isAnalyzeModeActive)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("helpbox");
            // 합계 계산
            int totalShaderComplexity = 0;
        int totalParticleCount = 0;
        float totalOverdrawEstimate = 0f;
        long totalResourceMemoryUsage =0;

        foreach (var entry in analysisResults)
        {
            totalShaderComplexity += entry.Value.ShaderComplexityScore;
            totalParticleCount += entry.Value.ParticleCount;
            totalOverdrawEstimate += entry.Value.OverdrawEstimate;
            totalResourceMemoryUsage += entry.Value.ResourceMemoryUsage;
        }

        // 합계 표시
        GUIStyle boldCenterStyle = new GUIStyle(GUI.skin.label) 
        { 
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.Space(10);

        GUILayout.Label($"총 쉐이더 복잡도: {totalShaderComplexity}, 총 파티클 수: {totalParticleCount}, 총 오버드로우 추정치: {totalOverdrawEstimate:F2}, 총 리소스 사용량: {FormatBytes(totalResourceMemoryUsage)}", boldCenterStyle);

            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }

    private void Update()
    {
        sceneCamera = SceneView.lastActiveSceneView.camera;
    }

    void UpdateAnalysis()
{
    if (isAnalyzeModeActive && Selection.activeGameObject != null)
    {
        AnalyzeParticleSystemWithChildren(Selection.activeGameObject);
    }
}

    // 바이트를 적절한 단위로 변환하여 문자열로 반환하는 메서드
    private string FormatBytes(long bytes)
    {
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    double len = bytes;
    int order = 0;
    while (len >= 1024 && order < sizes.Length - 1)
    {
        order++;
        len = len / 1024;
    }
    return $"{len:0.##} {sizes[order]}";
}

    private float GetScreenSizeOfParticle(ParticleSystem ps, float distanceToCamera)
    {
        float particleStartSize = Mathf.Max(ps.main.startSize.constantMax, 0.01f);
        // 카메라의 FOV와 거리를 고려하여 화면상의 파티클 크기 계산
        float screenSize = (particleStartSize / distanceToCamera) * Mathf.Rad2Deg * sceneCamera.fieldOfView;
        return screenSize;
    }

    private float EstimateOverdraw(ParticleSystem ps)
    {
        if (ps == null || Camera.main == null) return 0;

        var camera = Camera.main;
        var main = ps.main;
        var shape = ps.shape;
        var sizeOverLifetime = ps.sizeOverLifetime;
        var particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticles = ps.GetParticles(particles);

        float totalOverdraw = 0f;

        // 파티클 속도 및 형태 정보를 반영한 보정 계수
        float speedFactor = CalculateSpeedFactor(main.startSpeed);
        float shapeFactor = CalculateShapeFactor(shape); // 형태에 따른 보정

        for (int i = 0; i < numParticles; i++)
        {
            var particle = particles[i];
            float normalizedLifetime = particle.remainingLifetime / particle.startLifetime;
            float sizeMultiplier = sizeOverLifetime.enabled ? sizeOverLifetime.size.Evaluate(normalizedLifetime) : 1f;
            float particleScale = main.startSize.constant * sizeMultiplier;
            Vector3 particleSize = CalculateParticleSize(particleScale, ps);

            float distanceToCamera = Vector3.Distance(camera.transform.position, particle.position);
            float particleScreenArea = GetParticleScreenArea(particleSize, distanceToCamera, camera);
            float transparencyFactor = Mathf.Max(particle.GetCurrentColor(ps).a, 0.01f);

            float sizeFactor = Mathf.Sqrt(particleScreenArea);
            totalOverdraw += particleScreenArea * transparencyFactor;
        }

        float particleCountImpact = Mathf.Log10(numParticles + 1);
        return totalOverdraw * particleCountImpact * speedFactor * shapeFactor;
    }
    private float CalculateSpeedFactor(ParticleSystem.MinMaxCurve startSpeed)
{
    float speed = Mathf.Max(startSpeed.constant, 0.001f); // 속도가 0에 가깝지 않도록 보장

    // 로그 함수를 사용하여 속도에 따른 보정 계수 계산
    float rawFactor = Mathf.Log10(1 / speed + 1);

    // speedFactor의 영향을 완화하기 위한 스케일링 적용
    // 예: 결과값을 0.5 ~ 1.5 범위 내로 조정
    float scaledFactor = Mathf.Lerp(0.5f, 1.5f, rawFactor / 10f);

    return scaledFactor;
}

    private float CalculateShapeFactor(ParticleSystem.ShapeModule shape)
    {
        float shapeFactor = 1f; // 기본 보정 계수

        switch (shape.shapeType)
        {
            case ParticleSystemShapeType.Sphere:
                shapeFactor = 1 / Mathf.Max(shape.radius, 0.1f); // 구형의 경우, 반지름을 고려
                break;
            case ParticleSystemShapeType.Cone:
                shapeFactor = 1 / Mathf.Max(shape.radius, 0.1f); // 원뿔의 경우, 기저 반지름을 고려
                break;
            case ParticleSystemShapeType.Box:
                Vector3 boxSize = shape.scale;
                float boxVolume = boxSize.x * boxSize.y * boxSize.z;
                shapeFactor = 1 / Mathf.Max(boxVolume, 0.1f); // 박스의 경우, 부피를 고려
                break;
                // 추가적인 형태에 대한 처리가 필요한 경우 여기에 추가
        }

        return Mathf.Clamp(shapeFactor, 0.1f, 10f); // 보정 계수를 합리적인 범위 내로 제한
    }

    private Vector3 CalculateParticleSize(float particleScale, ParticleSystem ps)
    {
        Vector3 particleSize = Vector3.one * particleScale;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            if (renderer.renderMode == ParticleSystemRenderMode.Mesh)
            {
                Mesh mesh = renderer.mesh;
                if (mesh != null)
                {
                    particleSize = mesh.bounds.size * particleScale; // 메시의 원본 크기에 스케일 적용
                }
            }
            else if (renderer.renderMode == ParticleSystemRenderMode.Stretch)
            {
                float stretchFactor = renderer.velocityScale; // 스트레치드 빌보드의 경우 velocityScale을 고려
                particleSize *= stretchFactor;
            }
            // 빌보드 모드의 경우 이미 위에서 계산된 스케일을 사용
        }

        return particleSize;
    }

    private float GetParticleScreenArea(Vector3 particleSize, float distanceToCamera, Camera camera)
    {
        // 카메라의 FOV와 거리를 고려하여 화면상의 파티클 면적 계산
        float screenSize = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distanceToCamera;
        Vector3 screenParticleSize = particleSize / screenSize;
        return screenParticleSize.x * screenParticleSize.y; // 화면상의 면적 반환
    }

    private void AnalyzeParticleSystems(bool currentOnly, bool includeChildren, bool clearResults = true)
    {
        if (clearResults)
        {
            analysisResults.Clear();
        }

        if (currentOnly)
        {
            foreach (var selectedObject in Selection.gameObjects)
            {
                AnalyzeParticleSystem(selectedObject);
            }
        }
        else if (includeChildren)
        {
            foreach (var selectedObject in Selection.gameObjects)
            {
                AnalyzeParticleSystemWithChildren(selectedObject);
            }
        }
    }


   

    private void AnalyzeParticleSystemWithChildren(GameObject obj, bool clearResults = true)
    {
        if (clearResults)
        {
            analysisResults.Clear();
        }

        // 자식 오브젝트의 파티클 시스템 분석 로직...
        foreach (Transform child in obj.transform)
        {
            AnalyzeParticleSystemWithChildren(child.gameObject, false); // 자식에 대해 재귀 호출, 결과 지우지 않음
        }
        AnalyzeParticleSystem(obj, false); // 현재 오브젝트 분석, 결과 지우지 않음
    }

    private void AddToShaderComplexityScores(ParticleSystem ps)
    {
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        var analysisResult = new ParticleSystemAnalysisResult();

        if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.shader != null)
        {
            analysisResult.ShaderComplexityScore = AnalyzeShaderComplexity(renderer.sharedMaterial.shader);
        }

        var mainModule = ps.main;
        analysisResult.ParticleCount = mainModule.maxParticles;

        analysisResults.Add(ps, analysisResult);
    }


    private int AnalyzeShaderComplexity(Shader shader)
    {
        int score = 0;

        // 셰이더 프로퍼티 수에 따른 점수 계산
        int propertyCount = ShaderUtil.GetPropertyCount(shader);
        score += propertyCount * 1;

        // 셰이더에서 사용하는 텍스처 수 계산
        int textureCount = 0;
        for (int i = 0; i < propertyCount; i++)
        {
            if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                textureCount++;
            }
        }
        score += textureCount * 2;

        // 점수를 100점 만점으로 조정
        score = Mathf.Clamp(score, 0, 100);

        return score;
    }


    private long CalculateTextureMemoryUsage(Texture texture)
{
    if (texture is Texture2D texture2D)
    {
        float bitsPerPixel = TextureFormatToBitsPerPixel(texture2D.format);
        long bytes = (long)((texture2D.width * texture2D.height * bitsPerPixel) / 8f);
        return bytes;
    }
    // 다른 텍스처 타입에 대한 처리가 필요하면 여기에 추가
    // 예: if (texture is RenderTexture renderTexture) { ... }
    return 0;
}


    private long CalculateMeshMemoryUsage(Mesh mesh)
    {
        if (mesh == null) return 0;

        // 정점당 메모리 사용량: 위치(Vector3), 노멀(Vector3), UV(Vector2)
        long vertexSize = 3 * 4 + 3 * 4 + 2 * 4; // 각 Vector3는 12바이트, Vector2는 8바이트
        long verticesMemory = mesh.vertexCount * vertexSize;

        // 인덱스당 메모리 사용량: 각 인덱스는 4바이트 (int)
        long indexSize = 4; // 인덱스 하나당 4바이트
        long indicesMemory = mesh.triangles.Length * indexSize;

        return verticesMemory + indicesMemory;
    }

    private float TextureFormatToBitsPerPixel(TextureFormat format)
{
    switch (format)
    {
        case TextureFormat.Alpha8: return 8f;
        case TextureFormat.ARGB4444: return 16f;
        case TextureFormat.RGB24: return 24f;
        case TextureFormat.RGBA32: return 32f;
        case TextureFormat.ARGB32: return 32f;
        case TextureFormat.RGB565: return 16f;
        case TextureFormat.R16: return 16f;
        case TextureFormat.DXT1: return 4f; // DXT1 압축 포맷은 블록 압축이므로 픽셀 당 비트가 아니라 블록 당 비트입니다.
        case TextureFormat.DXT5: return 8f; // DXT5 압축 포맷도 마찬가지입니다.
        case TextureFormat.ASTC_4x4: return 8f;
        case TextureFormat.ASTC_5x5: return 5.12f;
        case TextureFormat.ASTC_6x6: return 3.56f;
        case TextureFormat.ASTC_8x8: return 2f;   
        case TextureFormat.ASTC_10x10: return 1.28f;
        case TextureFormat.ASTC_12x12: return 0.89f;
            
        default: return 0; // 알 수 없거나 처리되지 않은 포맷
     }
}

private void AnalyzeParticleSystem(GameObject obj, bool clearResults = true)
{
    if (clearResults)
    {
        analysisResults.Clear();
    }

    ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>(true);

    foreach (var ps in particleSystems)
    {
        if (!analysisResults.ContainsKey(ps))
        {
            analysisResults[ps] = new ParticleSystemAnalysisResult();
        }

        var analysisResult = analysisResults[ps];
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        if (renderer != null)
        {
            if (renderer != null && renderer.sharedMaterial != null)
             {
            // 모든 텍스처 프로퍼티 이름을 가져옵니다.
            string[] texturePropertyNames = renderer.sharedMaterial.GetTexturePropertyNames();

            foreach (string propertyName in texturePropertyNames)
            {
                // 각 프로퍼티 이름에 해당하는 텍스처를 가져옵니다.
                Texture texture = renderer.sharedMaterial.GetTexture(propertyName);
                if (texture != null)
                {
                    // 텍스처 메모리 사용량을 계산하고 총 리소스 메모리 사용량에 추가합니다.
                    long textureMemoryUsage = CalculateTextureMemoryUsage(texture);
                    analysisResult.ResourceMemoryUsage += textureMemoryUsage;
                }
                }
            }

            if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.shader != null)
            {
                analysisResult.ShaderComplexityScore = AnalyzeShaderComplexity(renderer.sharedMaterial.shader);
            }

            // 메쉬 메모리 사용량 계산
            if (renderer.mesh != null)
            {
                analysisResult.ResourceMemoryUsage += CalculateMeshMemoryUsage(renderer.mesh);
            }
        }

        analysisResult.ParticleCount = ps.particleCount;
        analysisResult.OverdrawEstimate = EstimateOverdraw(ps);
    }

    Repaint();
}

    // Naming Tool 섹션을 그리는 메서드
private void DrawNamingToolSection()
{
    EditorGUILayout.BeginVertical("helpbox");
    EditorGUILayout.Space(10);

    if (GUILayout.Button("선택 목록 등록"))
    {
        RegisterNamingSelection();
    }

    if (GUILayout.Button("선택 목록 초기화"))
    {
        namingObjects.Clear();
        previewNames.Clear();
    }

    
    if (namingObjects.Count > 0)
    {
        EditorGUILayout.BeginVertical("helpbox");
        EditorGUILayout.Space(10);
        GUILayout.Label("Preview of Changes:");
        for (int i = 0; i < namingObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(namingObjects[i], typeof(UnityEngine.Object), false);
            GUILayout.Label(" => " + previewNames[i]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
    


   
    if (namingObjects.Count > 0)
    {
        EditorGUILayout.BeginVertical("helpbox");
        if (GUILayout.Button("네이밍 초기화"))
        {
        ClearNames();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
    }
    

    
    if (namingObjects.Count > 0)
    {
        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        prefix = EditorGUILayout.TextField("접두어", prefix, GUILayout.Width(400f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("일괄 접두어 적용"))
         {
             ApplyPrefix();
          }
         GUILayout.EndVertical();
         EditorGUILayout.Space(5);
    }

    
    
    if (namingObjects.Count > 0)
    {
        EditorGUILayout.BeginVertical("helpbox");
        if (GUILayout.Button("번호 붙이기 적용"))
        {
            AddSequentialNumbers();
        }
        EditorGUILayout.EndVertical();
    }

    EditorGUILayout.Space(10);
    
    
    if (namingObjects.Count > 0)
    {
        EditorGUILayout.BeginVertical("helpbox");
        if(GUILayout.Button("네이밍 적용"))
        {
            ApplyNamingChanges();
        }
        EditorGUILayout.EndVertical();
    }

    EditorGUILayout.EndVertical();
    EditorGUILayout.Space(5);

}

// 선택한 파일들을 등록하는 메서드
private void RegisterNamingSelection()
{
    namingObjects = new List<UnityEngine.Object>(Selection.objects);
    previewNames = namingObjects.Select(obj => obj.name).ToList();
}

// 선택한 파일들의 이름을 빈칸으로 만드는 메서드
private void ClearNames()
{
    previewNames = namingObjects.Select(obj => string.Empty).ToList();
}

// 접두어를 적용하는 메서드
private void ApplyPrefix()
{
    for (int i = 0; i < namingObjects.Count; i++)
    {
        previewNames[i] = prefix + previewNames[i];
    }
}

// 순차적 번호를 추가하는 메서드
private void AddSequentialNumbers()
{
    int numberLength = namingObjects.Count.ToString().Length; // 숫자의 길이를 계산하여 앞에 0을 붙일 개수를 결정합니다.
    for (int i = 0; i < namingObjects.Count; i++)
    {
        string number = (i + 1).ToString().PadLeft(numberLength, '0');
        previewNames[i] = previewNames[i] + "_" + number;
    }
}

// 네이밍 변경을 적용하는 메서드
private void ApplyNamingChanges()
{
    for (int i = 0; i < namingObjects.Count; i++)
    {
        if (namingObjects[i] is GameObject) // 계층 구조 뷰의 게임 오브젝트인 경우
        {
            Undo.RecordObject(namingObjects[i], "Rename GameObject");
            namingObjects[i].name = previewNames[i];
        }
        else // 프로젝트 뷰의 에셋인 경우
        {
            string assetPath = AssetDatabase.GetAssetPath(namingObjects[i]);
            AssetDatabase.RenameAsset(assetPath, previewNames[i]);
        }
    }
    AssetDatabase.SaveAssets(); // 에셋 변경 사항 저장
    EditorSceneManager.MarkAllScenesDirty(); // 씬 변경 사항 표시
}


}
