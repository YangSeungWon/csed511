#!/bin/bash

# CSED511 과제 제출 파일 생성 스크립트
# Usage: ./create_submission.sh [assignment_number]
# Example: ./create_submission.sh 1

# 색상 코드
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 인자 확인
if [ $# -eq 0 ]; then
    echo -e "${RED}Error: Assignment number required${NC}"
    echo "Usage: $0 <assignment_number>"
    echo "Example: $0 1"
    exit 1
fi

ASSIGNMENT_NUM=$1
SUBMISSION_NAME="Assignment ${ASSIGNMENT_NUM}"
PACKAGE_NAME="Assignment${ASSIGNMENT_NUM}.unitypackage"

echo -e "${GREEN}📦 Creating submission package for Assignment ${ASSIGNMENT_NUM}...${NC}"

# 임시 디렉토리 생성
TEMP_DIR="submission_temp_${ASSIGNMENT_NUM}"
rm -rf "$TEMP_DIR"
mkdir -p "$TEMP_DIR"

# 프로젝트 폴더명 추출 (현재 디렉토리 이름)
PROJECT_NAME=$(basename "$PWD")

# Unity 프로젝트 필수 파일만 복사
echo -e "${YELLOW}📋 Copying Unity project files...${NC}"
rsync -av --progress \
  --include='Assets/***' \
  --include='Packages/***' \
  --include='ProjectSettings/***' \
  --exclude='*.md' \
  --exclude='*.txt' \
  --exclude='*.sh' \
  --exclude='.git/' \
  --exclude='.gitignore' \
  --exclude='Library/' \
  --exclude='Temp/' \
  --exclude='Logs/' \
  --exclude='Build/' \
  --exclude='Builds/' \
  --exclude='UserSettings/' \
  --exclude='*' \
  ./ "$TEMP_DIR/$PROJECT_NAME/"

# .meta 파일 정리 (불필요한 meta 제거)
find "$TEMP_DIR" -name "*.meta" -size 0 -delete 2>/dev/null


# Package 파일 존재 여부 확인 - 필수!
if [ ! -f "$PACKAGE_NAME" ]; then
    echo ""
    echo -e "${RED}═══════════════════════════════════════════════════${NC}"
    echo -e "${RED}❌ ERROR: $PACKAGE_NAME NOT FOUND!${NC}"
    echo -e "${RED}═══════════════════════════════════════════════════${NC}"
    echo ""
    echo -e "${YELLOW}Unity Package는 과제 제출 필수 요구사항입니다!${NC}"
    echo ""
    echo "📝 Unity Package 생성 방법:"
    echo "   1. Unity Editor 열기"
    echo "   2. Assets → Export Package"
    echo "   3. 다음 항목 선택:"
    echo "      ✅ Scripts/"
    echo "      ✅ Scenes/"
    echo "      ✅ Settings/"
    echo "      ✅ InputSystem_Actions.inputactions"
    echo "   4. Export 버튼 클릭"
    echo "   5. 파일명: ${GREEN}$PACKAGE_NAME${NC}"
    echo ""
    echo -e "${RED}Unity Package를 먼저 생성한 후 다시 실행하세요!${NC}"
    rm -rf "$TEMP_DIR"
    exit 1
fi

# Package 파일 타임스탬프 확인
echo -e "${GREEN}✅ Unity Package found: $PACKAGE_NAME${NC}"

# 최신 스크립트 파일 시간 가져오기
LATEST_SCRIPT_TIME=$(find Assets/Scripts -name "*.cs" -type f -exec stat -f "%m" {} \; 2>/dev/null | sort -n | tail -1)
PACKAGE_TIME=$(stat -f "%m" "$PACKAGE_NAME" 2>/dev/null)

if [ -z "$LATEST_SCRIPT_TIME" ]; then
    echo -e "${YELLOW}⚠️  Warning: 스크립트 파일을 찾을 수 없습니다${NC}"
elif [ -z "$PACKAGE_TIME" ]; then
    echo -e "${YELLOW}⚠️  Warning: Package 타임스탬프를 읽을 수 없습니다${NC}"
elif [ "$PACKAGE_TIME" -lt "$LATEST_SCRIPT_TIME" ]; then
    echo ""
    echo -e "${YELLOW}═══════════════════════════════════════════════════${NC}"
    echo -e "${YELLOW}⚠️  WARNING: Unity Package가 오래된 버전입니다!${NC}"
    echo -e "${YELLOW}═══════════════════════════════════════════════════${NC}"
    echo ""
    LATEST_SCRIPT=$(find Assets/Scripts -name "*.cs" -type f -exec stat -f "%m %N" {} \; 2>/dev/null | sort -n | tail -1 | cut -d' ' -f2-)
    echo -e "최근 수정된 스크립트: ${YELLOW}$LATEST_SCRIPT${NC}"
    echo -e "수정 시간: ${YELLOW}$(date -r $LATEST_SCRIPT_TIME '+%Y-%m-%d %H:%M:%S')${NC}"
    echo ""
    echo -e "Unity Package 생성 시간: ${RED}$(date -r $PACKAGE_TIME '+%Y-%m-%d %H:%M:%S')${NC}"
    echo ""
    echo -e "${RED}스크립트가 Unity Package보다 최신입니다!${NC}"
    echo -e "${YELLOW}Unity Editor에서 Package를 다시 Export 해주세요.${NC}"
    echo ""
    read -p "이대로 계속하시겠습니까? (위험!) (y/N): " -n 1 -r
    echo ""
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}Unity Package를 다시 생성 후 실행하세요.${NC}"
        rm -rf "$TEMP_DIR"
        exit 1
    fi
    echo -e "${YELLOW}⚠️  경고: 오래된 Unity Package로 계속합니다...${NC}"
else
    PACKAGE_DATE=$(date -r $PACKAGE_TIME '+%Y-%m-%d %H:%M:%S')
    echo -e "   📅 Package 생성 시간: ${GREEN}$PACKAGE_DATE${NC}"
    echo -e "   ✅ Package가 최신 상태입니다"
fi

cp "$PACKAGE_NAME" "$TEMP_DIR/"

# 최종 zip 생성
echo -e "${YELLOW}🗜️  Creating final zip...${NC}"
cd "$TEMP_DIR" || exit
zip -r "../${SUBMISSION_NAME}.zip" . \
  -x "*.DS_Store" \
  -x "*/.DS_Store" \
  -x "*/Thumbs.db" \
  -x "*/.gitkeep"
cd ..

# 파일 크기 확인
ZIP_SIZE=$(du -h "${SUBMISSION_NAME}.zip" | cut -f1)

# 임시 디렉토리 삭제
rm -rf "$TEMP_DIR"

# 완료 메시지
echo ""
echo -e "${GREEN}═══════════════════════════════════════════════════${NC}"
echo -e "${GREEN}✅ 제출 파일 생성 완료!${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════${NC}"
echo ""
echo -e "📁 File: ${GREEN}${SUBMISSION_NAME}.zip${NC} (${ZIP_SIZE})"
echo ""
echo "📦 포함된 내용:"
echo "  - Assets/ (모든 게임 에셋)"
echo "  - ProjectSettings/ (프로젝트 설정)"
echo "  - Packages/ (패키지 의존성)"
if [ -f "$PACKAGE_NAME" ]; then
    echo "  - ${PACKAGE_NAME}"
fi
echo ""
echo "❌ 제외된 내용:"
echo "  - 모든 .md 파일 (README, 가이드 등)"
echo "  - 모든 .txt 파일"
echo "  - 모든 .sh 스크립트"
echo "  - Library/ (Unity 캐시)"
echo "  - Temp/, Logs/, Build/"
echo "  - Git 관련 파일"
echo "  - Hidden files (.DS_Store 등)"
echo ""
echo -e "${YELLOW}📧 제출 안내:${NC}"
echo "  To: dgkim94@postech.ac.kr"
echo "  Subject: CSED511 Assignment ${ASSIGNMENT_NUM}"
echo "  Deadline: Check syllabus"
echo ""
echo -e "${GREEN}Good luck with your submission!${NC}"