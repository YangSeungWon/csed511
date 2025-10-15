# Quest 3 VR 컨트롤러 설정 가이드

## 사전 준비
- Unity 에디터에서 csed511 프로젝트 열기
- Assets/Scenes/SampleScene.unity 열기

---

## Step 1: VR 컨트롤러 추가 (LeftHand Controller)

### 1-1. LeftHand Controller GameObject 생성
1. Hierarchy에서 **XR Origin (XR Rig)** 찾기
2. 그 안에 **Camera Offset** GameObject 찾기
3. **Camera Offset** 우클릭 → **XR → XR Controller (Action-based)**
4. 생성된 GameObject 이름을 **"LeftHand Controller"**로 변경

### 1-2. LeftHand Controller 설정
1. **LeftHand Controller** 선택
2. Inspector에서 **XR Controller (Action-based)** 컴포넌트 찾기
3. 다음 설정 변경:
   ```
   Position Action: XRI LeftHand/Position
   Rotation Action: XRI LeftHand/Rotation
   Select Action: XRI LeftHand/Select
   Activate Action: XRI LeftHand/Activate
   UI Press Action: XRI LeftHand/UI Press
   ```

### 1-3. Controller Model 추가 (선택사항, 시각적 표현)
1. **LeftHand Controller** 선택
2. **Add Component** → **XR Controller Recorder** 또는 단순히 Cube로 시각화
3. 또는 LeftHand Controller 하위에 3D Object → Cube 추가
   - Scale: (0.05, 0.05, 0.1) 정도로 작게

---

## Step 2: VR 컨트롤러 추가 (RightHand Controller)

### 2-1. RightHand Controller GameObject 생성
1. **Camera Offset** 우클릭 → **XR → XR Controller (Action-based)**
2. 생성된 GameObject 이름을 **"RightHand Controller"**로 변경

### 2-2. RightHand Controller 설정
1. **RightHand Controller** 선택
2. Inspector에서 **XR Controller (Action-based)** 컴포넌트 찾기
3. 다음 설정 변경:
   ```
   Position Action: XRI RightHand/Position
   Rotation Action: XRI RightHand/Rotation
   Select Action: XRI RightHand/Select
   Activate Action: XRI RightHand/Activate
   UI Press Action: XRI RightHand/UI Press
   ```

### 2-3. Controller Model 추가 (선택사항)
1. RightHand Controller 하위에 3D Object → Cube 추가
2. Scale: (0.05, 0.05, 0.1)

---

## Step 3: Continuous Move Provider 추가 (조이스틱 이동)

### 3-1. Component 추가
1. Hierarchy에서 **XR Origin (XR Rig)** 선택
2. Inspector 하단에서 **Add Component** 클릭
3. 검색창에 **"Action Based Continuous Move"** 입력
4. **Action Based Continuous Move Provider** 선택

### 3-2. Continuous Move Provider 설정
1. **Action Based Continuous Move Provider** 컴포넌트에서:
   ```
   System: XR Interaction Manager (자동으로 찾아짐, 없으면 드래그)
   Move Speed: 2.0 (걷기 속도)
   Enable Strafe: ✓ (체크)
   Use Gravity: ✓ (체크)
   Gravity Application Mode: Immediate
   Forward Source: Camera Offset (Hierarchy에서 드래그)
   ```

2. **Left Hand Move Action** 설정:
   - Use Reference: ✗ (체크 해제)
   - Action 펼치기
   ```
   Name: Left Hand Move
   Action Type: Value
   Control Type: Vector2
   ```
   - Bindings 추가 (+ 버튼):
     ```
     Path: <XRController>{LeftHand}/Primary2DAxis
     ```

3. **Right Hand Move Action** 설정:
   - Use Reference: ✗ (체크 해제)
   - Action 펼치기
   ```
   Name: Right Hand Move
   Action Type: Value
   Control Type: Vector2
   ```
   - Bindings 추가 (+ 버튼):
     ```
     Path: <XRController>{RightHand}/Primary2DAxis
     ```

---

## Step 4: Teleportation System 추가

### 4-1. Teleportation Provider 추가
1. **XR Origin (XR Rig)** 선택
2. **Add Component** → **Teleportation Provider**

### 4-2. Teleportation Provider 설정
```
System: XR Interaction Manager (자동)
```

### 4-3. LeftHand에 Ray Interactor 추가
1. **LeftHand Controller** 선택
2. **Add Component** → **XR Ray Interactor**
3. 설정:
   ```
   Interaction Manager: XR Interaction Manager
   Line Type: Projectile Curve
   Velocity: 8
   Max Raycast Distance: 20
   Enable Interaction with UI GameObjects: ✓
   ```

4. **Add Component** → **XR Interactor Line Visual**
5. 설정:
   ```
   Line Width: 0.02
   Valid Color Gradient: 초록색 그라데이션
   Invalid Color Gradient: 빨간색 그라데이션
   ```

### 4-4. RightHand에 Ray Interactor 추가
1. **RightHand Controller** 선택
2. 위의 4-3과 동일하게 **XR Ray Interactor** 추가
3. **XR Interactor Line Visual** 추가

### 4-5. Teleportation Area 설정 (바닥)
1. Hierarchy에서 **Ground** (바닥 Plane) 선택
2. **Add Component** → **Teleportation Area**
3. 설정:
   ```
   Interaction Manager: XR Interaction Manager
   Teleportation Provider: XR Origin의 Teleportation Provider 드래그
   Match Orientation: None
   ```

---

## Step 5: VR Interaction System (물체 집기/던지기)

### 5-1. LeftHand에 Direct Interactor 추가
1. **LeftHand Controller** 선택
2. **Add Component** → **XR Direct Interactor**
3. 설정:
   ```
   Interaction Manager: XR Interaction Manager
   Attach Transform: LeftHand Controller의 Transform
   ```

4. **Add Component** → **Sphere Collider** (손 닿는 범위)
   ```
   Is Trigger: ✓
   Radius: 0.1
   ```

### 5-2. RightHand에 Direct Interactor 추가
1. **RightHand Controller** 선택
2. 위와 동일하게 **XR Direct Interactor** 추가
3. **Sphere Collider** 추가 (Is Trigger ✓, Radius: 0.1)

### 5-3. Pickupable 오브젝트 설정
씬에 있는 모든 "Pickupable" 태그 오브젝트에:

1. 오브젝트 선택 (예: Light Cube, Heavy Cube 등)
2. **Add Component** → **XR Grab Interactable**
3. 설정:
   ```
   Interaction Manager: XR Interaction Manager
   Movement Type: Kinematic
   Throw on Detach: ✓
   Throw Smoothing Duration: 0.1
   Throw Velocity Scale: 1.5
   Throw Angular Velocity Scale: 1.0
   ```

---

## Step 6: VR Movement Speed Controller 연결

1. **XR Origin (XR Rig)** 선택
2. **Add Component** → **VR Movement Speed Controller** (이미 만들어진 스크립트)
3. 설정:
   ```
   Walk Speed: 2
   Run Speed: 4
   Dash Speed: 6
   Move Provider: Action Based Continuous Move Provider (드래그)
   ```

---

## Step 7: 테스트

1. Unity 에디터 **Play 버튼** 클릭
2. Quest 3 헤드셋 착용
3. 테스트:
   - 왼쪽 조이스틱: 이동
   - 컨트롤러로 바닥 가리키기: 텔레포트 라인 표시
   - 트리거: 텔레포트 실행
   - 물체에 손 가까이: 트리거로 집기
   - 집은 상태에서 트리거 놓기: 던지기

---

## 문제 해결

### 이동이 안될 때:
- Continuous Move Provider의 Forward Source가 Camera Offset인지 확인
- Left/Right Hand Move Action의 Bindings가 올바른지 확인

### 텔레포트가 안될 때:
- Ground에 Teleportation Area 컴포넌트 있는지 확인
- Ray Interactor의 Line Type이 Projectile Curve인지 확인

### 물체를 못 집을 때:
- 오브젝트에 XR Grab Interactable 있는지 확인
- Controller에 Sphere Collider (Is Trigger) 있는지 확인
- Direct Interactor 컴포넌트 있는지 확인

---

## 다음 단계

현재는 기본 VR 기능이 작동합니다. Assignment 요구사항에 맞게:
- UI 추가 (VRUIManager 활용)
- Speed 변경 버튼 추가
- Object Spawner 추가

이 부분은 다음에 진행하시겠습니까?
