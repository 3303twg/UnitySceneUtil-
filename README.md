# Scene Menu (Unity Editor)

프로젝트 내 씬을 Unity 상단 메뉴 `Scenes/`에서 바로 여는 에디터 전용 도구입니다.

---

## 폴더 구성

`SceneMenu/` 폴더 하나에 스크립트를 모아 둡니다.

```
SceneMenu/
├── SceneMenuBootstrap.cs
├── SceneMenuGenerator.cs
├── SceneMenuSettings.cs
├── SceneMenuSettingsWindow.cs
├── SceneMenuWatcher.cs
└── GeneratedSceneMenu.cs   ← 자동 생성 (Git 제외)
```

`GeneratedSceneMenu.cs`는 `SceneMenuGenerator.cs`와 **같은 폴더**에 생성됩니다.  
폴더 위치를 옮겨도 Generator가 자기 경로를 기준으로 출력합니다.

---

## 포함 파일

| 파일 | 역할 |
|------|------|
| `SceneMenuGenerator.cs` | 씬 목록을 읽어 메뉴 코드 생성 |
| `SceneMenuSettings.cs` | 씬별 표시 여부 저장 (`EditorPrefs`) |
| `SceneMenuSettingsWindow.cs` | 표시 설정 UI |
| `SceneMenuBootstrap.cs` | 에디터 시작 시 메뉴 자동 생성 |
| `SceneMenuWatcher.cs` | 씬 에셋 추가·삭제·이동 시 자동 재생성 |

아래 파일은 **자동 생성**되며 Git에 올리지 않습니다.

| 파일 | 역할 |
|------|------|
| `GeneratedSceneMenu.cs` | `[MenuItem("Scenes/...")]` 메뉴 코드 (로컬 생성) |

---

## 다른 프로젝트에 이식

1. Release에서 .unitypackage 다운 → Unity에서 Import
(또는 더블클릭)
2. .gitignore에 아래 내용 추가
**/SceneMenu/GeneratedSceneMenu.cs
**/SceneMenu/GeneratedSceneMenu.cs.meta
3. Unity 에디터를 엽니다. `GeneratedSceneMenu.cs`가 없으면 즉시 생성되고, 이후 씬 목록으로 갱신됩니다.

외부 패키지 의존성 없음. `UnityEditor` API만 사용합니다.

---

## Git 설정

생성 파일은 개발자마다 씬 구성·표시 설정이 달라지므로 커밋하지 않습니다.

프로젝트 루트 `.gitignore`에 추가:

```gitignore
# Scene Menu (auto-generated)
**/SceneMenu/GeneratedSceneMenu.cs
**/SceneMenu/GeneratedSceneMenu.cs.meta
```

이미 Git에 추적 중인 경우, 한 번만 실행:

```bash
git rm --cached Assets/Script/Editor/SceneMenu/GeneratedSceneMenu.cs
git rm --cached Assets/Script/Editor/SceneMenu/GeneratedSceneMenu.cs.meta
```

---

## 사용법

### 씬 열기

메뉴 `Scenes` → 씬 이름 클릭.

열기 전에 수정된 씬이 있으면 저장 여부를 묻습니다.

### 표시 설정

메뉴 `Scenes` → `Settings...`

- 체크박스: 해당 씬을 메뉴에 표시할지 설정
- 드래그: 여러 씬을 한 번에 on/off
- **Show All / Hide All**: 전체 표시·숨김
- **Apply**: 설정을 메뉴에 반영 (Apply를 눌러야 메뉴가 갱신됨)
- **Close**: 창 닫기

표시 설정은 `EditorPrefs`에 저장되며 **PC·계정별로 개인 설정**입니다.

---

## 동작 요약

```
에디터 시작 / 씬(.unity) 변경
        ↓
SceneMenuGenerator.Generate()
        ↓
프로젝트 전체 씬 검색 (AssetDatabase.FindAssets)
        ↓
SceneMenuSettings 표시 필터 적용
        ↓
SceneMenu/GeneratedSceneMenu.cs 생성
  (내용 동일하면 스킵 → 불필요한 재컴파일 방지)
        ↓
Unity 재컴파일 → Scenes/ 메뉴 반영
```

- 씬 목록: 프로젝트 내 **모든** `.unity` 에셋 (이름순 정렬)
- 메뉴 구조: 플랫 리스트 (`Scenes/씬이름`)
- 설정 변경 후 메뉴 반영: **Apply 버튼** (자동 반영 아님)

---

## 알려진 특성

- 씬 파일을 이동·이름 변경하면 이전 경로의 `EditorPrefs` 설정은 남을 수 있습니다. (동작에는 영향 없음)
- `GeneratedSceneMenu.cs`가 없으면 에디터 로드 시 최소 스텁을 만들고, 곧바로 전체 씬 목록으로 갱신합니다.
- 에디터를 처음 연 직후, 첫 컴파일이 끝나기 전까지 `Scenes/` 메뉴가 잠깐 `Settings...`만 보일 수 있습니다.
