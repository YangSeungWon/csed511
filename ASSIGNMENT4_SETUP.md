# Assignment 4: Fall Guys VR Game - Unity Setup Guide

## ✅ Phase 1 완료! (스크립트 생성)

모든 핵심 스크립트가 생성되었습니다:
- ✅ VRCharacterController.cs (점프)
- ✅ TimerManager.cs (타이머 + 저장/로드)
- ✅ ObstacleCollisionHandler.cs (충돌 처리)
- ✅ DroneCamera.cs (플라이오버)
- ✅ 3가지 장애물 스크립트
- ✅ 게임 매니저 + 트리거 시스템

**커밋**: `9b17d02`
**푸시 완료**: GitHub 업로드됨

---

## 🎯 다음 단계: Unity에서 씬 설정

### Phase 7: 씬 생성 및 코스 디자인

#### 1. 새 씬 생성

**Unity에서:**

1. **File → New Scene** (또는 Ctrl+N)
2. **Basic (Built-in)** 템플릿 선택
3. **File → Save As...**
4. 저장 위치: `Assets/Scenes/FallGuysScene.unity`

---

#### 2. XR Origin 설정

**SampleScene에서 XR Origin 복사:**

1. `Assets/Scenes/SampleScene.unity` 열기
2. **Hierarchy**에서 **XR Origin (XR Rig)** 선택
3. **Ctrl+D** (복사)
4. `FallGuysScene.unity`로 전환
5. **Ctrl+V** (붙여넣기)

**또는 새로 만들기:**

1. **GameObject → XR → XR Origin (VR)**
2. **XR Origin** 선택 → Inspector
3. **Add Component** → **VR Character Controller**
4. **Add Component** → **Obstacle Collision Handler**

---

#### 3. VR Character Controller 설정

**XR Origin에 컴포넌트 추가:**

1. XR Origin 선택
2. **Add Component** → **VRCharacterController**
3. **설정값:**
   - Jump Force: `5`
   - Gravity: `-9.81`
   - Ground Layer: `Default` (또는 Ground 레이어 생성)
   - Ground Check Distance: `0.2`
   - Right Controller: `XR Origin/Camera Offset/Right Controller`
   - Jump Action: Input Actions → Jump (B 버튼)

4. **Add Component** → **Character Controller**
   - Center: `(0, 1, 0)`
   - Radius: `0.3`
   - Height: `1.8`

---

#### 4. Obstacle Collision Handler 설정

**XR Origin에 컴포넌트 추가:**

1. XR Origin 선택
2. **Add Component** → **ObstacleCollisionHandler**
3. **설정값:**
   - Knockback Force: `10`
   - Knockback Duration: `0.3`
   - Haptic Intensity: `0.8`
   - Haptic Duration: `0.2`
   - Left Controller: `XR Origin/Camera Offset/Left Controller`
   - Right Controller: `XR Origin/Camera Offset/Right Controller`
   - Collision Sounds: `Assets/ShootingSound/cannon_01.wav` 등 추가

---

#### 5. Timer 시스템 설정

**빈 GameObject 생성:**

1. **GameObject → Create Empty**
2. 이름: `TimerManager`
3. **Add Component** → **TimerManager**

**World-space Canvas UI 생성:**

1. **GameObject → UI → Canvas**
2. Canvas 설정:
   - Render Mode: `World Space`
   - Position: `(0, 3, 5)` (플레이어 앞)
   - Scale: `(0.01, 0.01, 0.01)`
   - Width: `400`, Height: `200`

3. **Canvas 아래에 TextMeshPro 생성:**
   - **UI → Text - TextMeshPro** (3개)
   - 이름: `CurrentTimeText`, `BestTimeText`, `NewRecordText`

4. **TimerManager Inspector에서 연결:**
   - Current Time Text: `CurrentTimeText`
   - Best Time Text: `BestTimeText`
   - New Record Text: `NewRecordText`

---

#### 6. 게임 매니저 설정

**빈 GameObject 생성:**

1. **GameObject → Create Empty**
2. 이름: `GameManager`
3. **Add Component** → **FallGuysGameManager**
4. **설정:**
   - Player: `XR Origin` 드래그
   - Start Zone: (나중에 생성 후 연결)
   - Finish Zone: (나중에 생성 후 연결)

---

#### 7. 코스 디자인 (기본 프리미티브 사용)

**Start Platform (시작 지점):**

1. **GameObject → 3D Object → Plane**
2. 이름: `StartPlatform`
3. Position: `(0, 0, 0)`
4. Scale: `(2, 1, 2)`
5. Material: Green (Create → Material → 초록색)

**Start Trigger Zone:**

1. **GameObject → 3D Object → Cube**
2. 이름: `StartTrigger`
3. Position: `(0, 1, 5)`
4. Scale: `(5, 3, 1)`
5. **Add Component** → **StartFinishTrigger**
6. Trigger Type: `Start`
7. **Box Collider** → Is Trigger: `체크`
8. **MeshRenderer** → 체크 해제 (안 보이게)

---

**Section 1: Breakable Walls Zone:**

1. **GameObject → 3D Object → Plane** (바닥)
   - Position: `(0, 0, 10)`
   - Scale: `(3, 1, 3)`

2. **GameObject → 3D Object → Cube** (벽 x5)
   - 3개는 부서지는 벽, 2개는 단단한 벽
   - Position: `(랜덤 배치)`
   - Scale: `(2, 3, 0.2)`

3. **각 벽에 컴포넌트 추가:**
   - **Add Component** → **Rigidbody** → Is Kinematic: `체크`
   - **Add Component** → **BreakableWall**
   - **부서지는 벽:** Is Breakable: `체크`, Break Threshold: `10`
   - **단단한 벽:** Is Breakable: `체크 해제`
   - Tag: `Obstacle`

---

**Section 2: Rotating Ground Zone:**

1. **GameObject → 3D Object → Cube** (회전 플랫폼 x3)
   - Position: `(0, 1, 20)`, `(5, 1, 22)`, `(-5, 1, 22)`
   - Scale: `(3, 0.5, 3)`

2. **각 플랫폼에 컴포넌트 추가:**
   - **Add Component** → **RotatingGround**
   - Enable Rotation: `체크`
   - Rotation Axis: `(0, 1, 0)` (Y축)
   - Rotation Speed: `30` (각각 다르게)
   - Tag: `Obstacle`

3. **Slippery Material 생성:**
   - **Assets → Create → Physics Material**
   - 이름: `Slippery`
   - Dynamic Friction: `0.1`, Static Friction: `0.1`
   - Bounciness: `0.2`
   - 플랫폼의 Collider에 할당

---

**Section 3: Rolling Boulder Zone:**

1. **경사로 생성:**
   - **GameObject → 3D Object → Plane**
   - Position: `(0, 5, 35)`
   - Rotation: `(20, 0, 0)` (앞으로 기울임)
   - Scale: `(3, 1, 5)`

2. **Waypoints 생성:**
   - **GameObject → Create Empty** (x3)
   - 이름: `BoulderWaypoint1`, `2`, `3`
   - Position: 경사로 위→중간→아래

3. **Boulder 생성:**
   - **GameObject → 3D Object → Sphere**
   - 이름: `RollingBoulder`
   - Position: 경사로 맨 위
   - Scale: `(2, 2, 2)`

4. **Boulder 컴포넌트:**
   - **Add Component** → **Rigidbody**
   - **Add Component** → **RollingBoulder**
   - Waypoints: Waypoint 1, 2, 3 드래그
   - Move Speed: `5`
   - Loop: `체크`
   - Auto Respawn: `체크`
   - Use Physics: `체크 해제` (Transform movement)
   - Tag: `Obstacle`

---

**Finish Platform (종료 지점):**

1. **GameObject → 3D Object → Plane**
2. 이름: `FinishPlatform`
3. Position: `(0, 0, 50)`
4. Scale: `(3, 1, 3)`
5. Material: Red (빨간색)

**Finish Trigger Zone:**

1. **GameObject → 3D Object → Cube**
2. 이름: `FinishTrigger`
3. Position: `(0, 1, 50)`
4. Scale: `(5, 3, 1)`
5. **Add Component** → **StartFinishTrigger**
6. Trigger Type: `Finish`
7. **Box Collider** → Is Trigger: `체크`
8. **MeshRenderer** → 체크 해제

---

#### 8. Drone Camera 설정 (Optional Timeline)

**간단한 방법 (코드 기반):**

1. **GameObject → Camera**
2. 이름: `DroneCamera`
3. Position: `(0, 10, -10)` (위에서 내려다보기)
4. Rotation: `(30, 0, 0)`

5. **빈 GameObject 생성:**
   - 이름: `DroneCameraController`
   - **Add Component** → **DroneCamera**
   - Drone Camera: `DroneCamera`
   - VR Camera: `XR Origin/Camera Offset/Main Camera`
   - XR Origin: `XR Origin`
   - Flyover Duration: `4`
   - Move Provider: `XR Origin → ActionBasedContinuousMoveProvider`
   - Turn Provider: `XR Origin → ActionBasedSnapTurnProvider`
   - VR Character Controller: `XR Origin → VRCharacterController`

**Timeline 방법 (고급):**

1. **Window → Sequencing → Timeline**
2. Create Timeline Asset
3. 드론 카메라 경로 애니메이션 생성
4. DroneCameraController → Timeline 연결

---

#### 9. Input System 설정 (B 버튼 점프)

**InputSystem_Actions 수정:**

1. `Assets/InputSystem_Actions.inputactions` 더블클릭
2. **+ (Add Action)** 클릭
3. 이름: `Jump`
4. Action Type: `Button`
5. **+ (Add Binding)**
6. Path: `XR Controller → Optional Controls → Primary Button` (B 버튼)
7. **Save Asset**

8. **VRCharacterController Inspector:**
   - Jump Action: `InputSystem_Actions → Jump`

---

#### 10. 레이어 및 태그 설정

**Tags:**

1. **Edit → Project Settings → Tags and Layers**
2. **Tags** 섹션에 추가:
   - `Obstacle`
   - `Player` (XR Origin에 적용)

**Layers:**

1. **Layers** 섹션에 추가:
   - `Ground` (모든 바닥/플랫폼에 적용)

2. **VRCharacterController Inspector:**
   - Ground Layer: `Ground`

---

## 🧪 테스트

### 1. Play Mode 테스트 (Unity Editor)

1. **Play 버튼** 클릭
2. XR Device Simulator 사용:
   - WASD: 이동
   - 마우스: 시점
   - Space: 점프 (B 버튼 대신)

### 2. 확인 사항

- ✅ 드론 카메라 플라이오버 작동
- ✅ 타이머 시작 (Start Zone 진입)
- ✅ 점프 작동
- ✅ 부서지는 벽 충돌 시 파괴
- ✅ 회전 바닥 회전
- ✅ 바위 굴러옴
- ✅ 충돌 시 넉백 + 햅틱
- ✅ 타이머 정지 (Finish Zone 진입)
- ✅ 최고 기록 저장/로드

### 3. Quest 빌드 테스트

1. **File → Build Settings**
2. **Platform: Android**
3. **Build and Run**
4. Quest에서 실제 B 버튼 점프, 햅틱 피드백 테스트

---

## 📋 체크리스트

### 필수 구현 항목

- [ ] 새 씬 생성 (FallGuysScene.unity)
- [ ] XR Origin + VRCharacterController
- [ ] B 버튼 점프 작동
- [ ] 드론 카메라 플라이오버 (3-6초)
- [ ] 맵 타이틀: "Only Up - [이름]"
- [ ] Obstacle 1: Breakable Walls (부서지는/단단한 벽)
- [ ] Obstacle 2: Rotating Ground (회전 바닥)
- [ ] Obstacle 3: Rolling Boulder (굴러오는 바위)
- [ ] 충돌 시: 넉백 + 햅틱 + 오디오
- [ ] 타이머 UI (현재 시간, 최고 기록)
- [ ] 시작/종료 트리거
- [ ] 최고 기록 저장/로드 (파일)
- [ ] "New Record!" 표시

---

## 💡 팁

### 디버깅

- **Console 로그 확인**: 모든 이벤트가 로그 출력됨
- **Gizmos**: Scene 뷰에서 트리거/경로 시각화
- **Scene/Game 뷰 전환**: Play 중 Scene 뷰로 전환해서 확인

### 최적화

- 타겟 FPS: 72 fps (Quest 2)
- 너무 많은 물리 오브젝트 피하기
- Occlusion Culling 고려

### 추가 기능 (선택)

- 체크포인트 시스템
- 파티클 이펙트 (충돌, 파괴)
- 사운드 다양화
- 난이도 조절

---

**다음: Unity에서 씬 설정 시작하세요!** 🚀

질문 있으면 언제든 물어보세요!
