# docs/project/development-plan.md

# 개발 계획서

## 1. 개발자 별 담당 영역

### 개발자 A (플레이어 & 전투 시스템)
#### 담당 영역
- 플레이어 이동/조작 시스템
- 전투 시스템
- 플레이어 데이터 구조
- 레벨업 UI 연동

#### 담당 파일
```
Assets/Scripts/
├── Core/
│   └── DataManager.cs        # 플레이어 데이터 관리
├── Cube/
│   ├── CubeController.cs     # 플레이어 입력 및 이동
│   ├── CubeStats.cs         # 플레이어 스탯
│   └── CubeEvolution.cs     # 진화 시스템
└── Weapon/
    ├── WeaponBase.cs        # 무기 기본 클래스
    ├── WeaponSystem.cs      # 무기 관리
    └── Weapons/             # 구체적 무기 구현
```

### 개발자 B (적 & 게임 진행)
#### 담당 영역
- 적 생성 및 AI
- 레벨업 시스템
- 게임 진행 관리
- UI 시스템

#### 담당 파일
```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs       # 게임 진행 관리
│   └── PoolManager.cs       # 오브젝트 풀링
├── Enemy/
│   ├── EnemyBehavior.cs     # 적 AI
│   └── EnemySpawner.cs      # 적 생성
└── UI/
    ├── HUD.cs               # 게임 내 UI
    ├── LevelUpUI.cs         # 레벨업 메뉴
    └── UIManager.cs         # UI 총괄
```

## 2. 주차 별 개발 계획

### Week 1-2: 기획 및 초기 세팅
개발자 A:
- [ ] 프로젝트 초기 설정
- [ ] 플레이어 이동 메커니즘 기획
- [ ] 무기 시스템 설계

개발자 B:
- [ ] 적 생성 시스템 기획
- [ ] UI 플로우 설계
- [ ] 게임 진행 로직 설계

### Week 3-4: 프로토타입 개발
개발자 A:
- [ ] 큐브 구르기 구현
- [ ] 기본 무기 시스템 구현
- [ ] 플레이어 데이터 구조 구현

개발자 B:
- [ ] 기본 적 AI 구현
- [ ] 적 생성 시스템 구현
- [ ] 기본 UI 프레임워크 구현

### Week 5-6: 핵심 시스템 개발
개발자 A:
- [ ] 무기 진화 시스템
- [ ] 충돌 처리 시스템
- [ ] 플레이어 속성 시스템

개발자 B:
- [ ] 레벨업 시스템
- [ ] 게임 진행 매니저
- [ ] 레벨업 UI 구현

### Week 7-8: 마무리 및 출시 준비
개발자 A:
- [ ] 전투 밸런싱
- [ ] 플레이어 피드백 개선
- [ ] 버그 수정

개발자 B:
- [ ] 난이도 밸런싱
- [ ] UI 최적화
- [ ] 최종 테스트

## 3. 작업 관리 방법

### 3.1 브랜치 관리
- 개발자 A: feature/player-* 브랜치 사용
- 개발자 B: feature/enemy-* 브랜치 사용

### 3.2 주간 마일스톤
매주 금요일까지:
1. 해당 주차 작업 완료
2. PR 생성 및 코드 리뷰
3. develop 브랜치에 머지

### 3.3 데일리 스크럼
매일 오전 10시:
1. 전날 진행한 작업
2. 오늘 진행할 작업
3. 차단 사항 공유

## 4. 의존성 관리

### 4.1 공유 데이터
- GameManager: 게임 전반의 상태 관리
- DataManager: 플레이어 데이터 관리
- PoolManager: 공유 오브젝트 풀 관리

### 4.2 이벤트 시스템
```csharp
// 레벨업 이벤트
public static event Action OnLevelUp;

// 게임 오버 이벤트
public static event Action OnGameOver;

// 경험치 획득 이벤트
public static event Action<float> OnExperienceGained;
```

## 5. 주간 체크리스트

### 5.1 Week 1
개발자 A:
- [ ] 프로젝트 초기 설정 완료
- [ ] CubeController 기본 구조 구현
- [ ] WeaponBase 클래스 설계

개발자 B:
- [ ] GameManager 기본 구조 구현
- [ ] EnemySpawner 프로토타입 구현
- [ ] UI 기본 구조 설정

(이후 주차도 동일한 형식으로 체크리스트 작성)