# Unity Editor Scene 구성 가이드

## 1. Player 설정

### Player GameObject 생성
1. Hierarchy에서 우클릭 → Create Empty
2. 이름을 "Player"로 변경
3. Position: (0, 1, 0)

### Player Components 추가
Player GameObject 선택 후 Inspector에서:
1. **Add Component → Character Controller**
   - Height: 2
   - Radius: 0.5

2. **Add Component → Scripts → PlayerMovementController**
   - Walk Speed: 3
   - Run Speed: 6
   - Dash Speed: 10
   - Mouse Sensitivity: 2

3. **Add Component → Scripts → TeleportationController**
   - Max Teleport Distance: 20
   - Valid Teleport Color: Green
   - Invalid Teleport Color: Red

4. **Add Component → Scripts → InteractionController**
   - Interaction Range: 3
   - Hold Distance: 2
   - Throw Force: 10

### Camera 설정
1. Player 하위에 Camera 생성 (우클릭 → Camera)
2. 이름: "Player Camera"
3. Transform:
   - Position: (0, 1.6, 0)
   - Rotation: (0, 0, 0)
4. Tag: MainCamera

## 2. Environment 구성

### Ground (바닥)
1. Hierarchy → 3D Object → Plane
2. 이름: "Ground"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (5, 1, 5)
4. Material: 회색 또는 원하는 텍스처

### Walls (벽) - 4개
각각 다음과 같이 생성:

**North Wall:**
- 3D Object → Cube
- Position: (0, 2.5, 25)
- Scale: (50, 5, 1)

**South Wall:**
- Position: (0, 2.5, -25)
- Scale: (50, 5, 1)

**East Wall:**
- Position: (25, 2.5, 0)
- Scale: (1, 5, 50)

**West Wall:**
- Position: (-25, 2.5, 0)
- Scale: (1, 5, 50)

### Obstacles (장애물) - 최소 3개
예시:
1. **Obstacle 1** (Cube)
   - Position: (5, 1, 5)
   - Scale: (2, 2, 2)

2. **Obstacle 2** (Cube)
   - Position: (-5, 1, -5)
   - Scale: (3, 2, 1)

3. **Ramp** (경사로)
   - 3D Object → Cube
   - Position: (-10, 0.5, 0)
   - Rotation: (0, 0, 15)
   - Scale: (5, 0.2, 3)

4. **Platform** (플랫폼)
   - Position: (0, 1.5, 10)
   - Scale: (4, 0.3, 4)

## 3. Pickupable Objects 설정 (최소 2개, 다른 물리 속성)

### Light Cube (가벼운 큐브)
1. 3D Object → Cube
2. 이름: "Light Cube"
3. Position: (3, 1, 3)
4. Scale: (0.5, 0.5, 0.5)
5. **Tag: "Pickupable"** (Inspector → Tag → Add Tag로 생성 필요)
6. Add Component → Rigidbody
   - Mass: 0.5
   - Drag: 0.5
   - Angular Drag: 0.5
7. Material/Color: 빨간색

### Heavy Cube (무거운 큐브)
1. 3D Object → Cube
2. 이름: "Heavy Cube"
3. Position: (-3, 1, 3)
4. Scale: (0.7, 0.7, 0.7)
5. **Tag: "Pickupable"**
6. Add Component → Rigidbody
   - Mass: 5
   - Drag: 0.5
   - Angular Drag: 0.5
7. Material/Color: 파란색

### Bouncy Sphere (튀는 공)
1. 3D Object → Sphere
2. 이름: "Bouncy Sphere"
3. Position: (0, 1, -3)
4. Scale: (0.6, 0.6, 0.6)
5. **Tag: "Pickupable"**
6. Add Component → Rigidbody
   - Mass: 1
7. Physic Material 생성 및 적용:
   - Project → Create → Physic Material
   - 이름: "Bouncy"
   - Bounciness: 0.8
   - Sphere의 Collider → Material에 적용

### 추가 오브젝트 (선택사항)
- Cylinder, Capsule 등 다양한 형태
- 각각 다른 Mass 값 설정
- 모두 "Pickupable" 태그 필요

## 4. Lighting 설정

1. Hierarchy → Light → Directional Light
2. Rotation: (45, -30, 0)
3. Intensity: 1

## 5. 태그 생성 방법

1. 아무 GameObject 선택
2. Inspector → Tag 드롭다운 → Add Tag...
3. Tags 리스트에서 + 버튼
4. "Pickupable" 입력
5. 각 픽업 가능한 오브젝트에 이 태그 적용

## 6. 테스트

1. Play 버튼 클릭
2. 컨트롤 테스트:
   - WASD: 이동
   - Shift: 달리기
   - Ctrl: 대시
   - 마우스: 시점 회전
   - 좌클릭: 텔레포트 (조준선이 보일 때)
   - E: 물체 집기/놓기
   - 우클릭: 물체 던지기
   - ESC: 마우스 커서 해제

## 주의사항

- 모든 오브젝트는 기본적으로 Collider가 있음 (Primitive 생성시 자동)
- Pickupable 오브젝트는 반드시 Rigidbody 필요
- Tag 설정 잊지 말 것
- Player의 Character Controller가 없으면 이동 안됨

## 문제 해결

**이동이 안될 때:**
- Player에 Character Controller 있는지 확인
- PlayerMovementController 스크립트 추가되었는지 확인

**텔레포트가 안될 때:**
- 바닥과 벽에 Collider 있는지 확인
- TeleportationController의 Layer Mask 설정 확인 (Everything 또는 Default)

**물체를 못 집을 때:**
- 물체에 "Pickupable" 태그 있는지 확인
- 물체에 Rigidbody 있는지 확인
- InteractionController의 Interactable Layer 설정 확인