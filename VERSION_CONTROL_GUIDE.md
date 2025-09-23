# Unity 버전 관리 가이드

## 1. Git 설정 (가장 일반적)

### 초기 설정
```bash
git init
git add .
git commit -m "Initial commit"
```

### .gitignore 필수 항목
- `/Library/` - Unity가 자동 생성하는 캐시
- `/Temp/` - 임시 파일
- `/Obj/` - 빌드 중간 파일
- `/Build/` - 빌드 결과물
- `*.csproj`, `*.sln` - IDE 프로젝트 파일

### 버전 관리에 포함해야 할 것
✅ **반드시 포함:**
- `Assets/` - 모든 게임 에셋
- `ProjectSettings/` - 프로젝트 설정
- `Packages/` - 패키지 의존성

❌ **제외해야 할 것:**
- `Library/` - Unity가 재생성 가능
- `Temp/`, `Obj/`, `Build/`
- `.vs/` - Visual Studio 캐시

## 2. Unity 프로젝트 설정

### Version Control 설정
1. **Edit → Project Settings → Editor**
2. **Version Control → Mode:** Visible Meta Files
3. **Asset Serialization → Mode:** Force Text

이렇게 하면:
- `.meta` 파일이 보임 (GUID 추적)
- Scene/Prefab이 텍스트로 저장 (병합 가능)

### 대용량 파일 처리 (Git LFS)
```bash
# Git LFS 설치
git lfs install

# 대용량 파일 추적
git lfs track "*.psd"
git lfs track "*.fbx"
git lfs track "*.wav"
git lfs track "*.mp3"
git add .gitattributes
```

## 3. Unity Collaborate (Unity 내장)

### 장점
- Unity Editor에서 직접 사용
- 자동 충돌 해결
- Cloud 저장소 제공

### 설정 방법
1. Window → General → Services
2. Collaborate 활성화
3. Start Now 클릭

## 4. Plastic SCM (Unity 추천)

### 장점
- 대용량 파일 최적화
- Artist 친화적 UI
- Unity와 긴밀한 통합

### 설정
1. Unity Hub → 프로젝트 → Version Control 설정
2. Plastic SCM 선택

## 5. 팀 작업시 주의사항

### Scene 병합 충돌 방지
- **한 번에 한 사람만 Scene 수정**
- 또는 Scene을 여러 개로 분할
- Prefab 활용으로 충돌 최소화

### 메타 파일 관리
- `.meta` 파일 절대 삭제 금지
- 파일 이동시 Unity Editor에서 이동
- 외부에서 파일 이동하면 참조 깨짐

## 6. 백업 전략

### 로컬 백업
```bash
# 전체 프로젝트 백업
cp -r CSED511-assn1 CSED511-assn1-backup-$(date +%Y%m%d)
```

### 필수 백업 파일
- `Assets/` 폴더 전체
- `ProjectSettings/` 폴더
- `Packages/manifest.json`

### Unity Package 백업
1. Assets → Export Package
2. 모든 에셋 선택
3. `.unitypackage` 파일로 저장

## 7. 과제 제출용 압축

### 필요한 것만 포함
```bash
# 제출용 압축 (Library 제외)
zip -r "Assignment 1.zip" . -x "Library/*" -x "Temp/*" -x "Obj/*" -x "Build/*" -x ".git/*"
```

### 또는 Unity Package 사용
1. 모든 Scripts, Scenes, Materials 선택
2. Export Package
3. 프로젝트 폴더 + .unitypackage 함께 제출

## 현재 프로젝트 상태

- Unity Version: 6000.0.34f1
- `.gitignore` 파일 생성됨
- Text 기반 serialization 권장
- Scene 파일은 텍스트 모드로 저장 권장