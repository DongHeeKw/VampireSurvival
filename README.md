# 네모큐브를 키워서 살아남자!

구르는 큐브로 진화하며 살아남는 서바이벌 게임

## 🎮 게임 소개

- 구르는 큐브를 조작하여 생존하는 게임
- 다양한 속성의 큐브를 합치고 진화시키며 성장
- 시간이 지날수록 강해지는 적들과의 전투

## 🛠 개발 환경

- Unity 2022.3 LTS
- C#
- 권장 해상도: 1920 x 1080 (16:9)

## 🌍 기능 소개

### 핵심 시스템
- 큐브 구르기 시스템
- 큐브 합성 및 진화
- 속성 시스템
- 레벨업 시스템

### 적 시스템
- 시간에 따른 난이도 상승
- 다양한 패턴의 적
- 보스 시스템

## 🔧 설치 방법

1. 저장소 클론
```bash
git clone https://github.com/[username]/cube-survivor.git
```

2. Unity Hub에서 프로젝트 열기
3. Unity 2022.3 LTS 버전으로 실행

## 📝 Commit Convention

- feat: 새로운 기능 추가
- fix: 버그 수정
- docs: 문서 수정
- style: 코드 포맷팅, 세미콜론 누락, 코드 변경이 없는 경우
- refactor: 코드 리팩토링
- test: 테스트 코드
- chore: 빌드 업무 수정, 패키지 매니저 수정

## 🌿 Branch 전략

- main: 제품 출시 브랜치
- develop: 개발 브랜치
- feature/*: 기능 개발 브랜치
  - feature/cube-movement
  - feature/evolution-system
  - feature/attribute-system
- hotfix/*: 버그 수정 브랜치
- release/*: 출시 준비 브랜치

## 📁 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── PoolManager.cs
│   │   └── DataManager.cs
│   ├── Cube/
│   │   ├── CubeController.cs
│   │   ├── CubeEvolution.cs
│   │   └── CubeAttribute.cs
│   ├── Enemy/
│   │   ├── EnemySpawner.cs
│   │   └── EnemyBehavior.cs
│   └── UI/
│       ├── UIManager.cs
│       └── HUD.cs
├── Prefabs/
├── Scenes/
├── Art/
└── Resources/
```

## 👥 Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'feat: Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details